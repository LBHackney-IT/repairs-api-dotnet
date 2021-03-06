using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Strategies;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Notify.Client;
using Notify.Interfaces;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Email;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.DRS.BackgroundService;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.MiddleWare;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using RepairsApi.V2.Services.DRS.BackgroundService;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using RepairsApi.Versioning;
using Serilog;
using SoapCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using V2_Generated_DRS;

namespace RepairsApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            AWSXRayRecorder.InitializeInstance(configuration);
            AWSSDKHandler.RegisterXRayForAllServices();
            AWSXRayRecorder.Instance.ContextMissingStrategy = ContextMissingStrategy.LOG_ERROR;
        }

        public IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> ApiVersions { get; set; }
        private const string ApiName = "Repairs API";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    }
                ) // Required for the generated json attributes on hact models function as model validation
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(2, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RaiseSpendLimit", policy =>
                    policy.Requirements.Add(new RaiseLimitRequirement()));
                options.AddPolicy("VarySpendLimit", policy =>
                    policy.Requirements.Add(new VaryLimitRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, RaiseSpendLimitAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, VarySpendLimitAuthorizationHandler>();

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney API Key",
                        Name = "X-Api-Key",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityDefinition("UserHeader",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Hackney User JWT",
                        Name = "X-Hackney-User",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Token"
                            }
                        },
                        new List<string>()
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "UserHeader"
                            }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in ApiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);

                c.OperationFilter<DeprecateRepairsFilter>();
            });
            ConfigureDbContext(services);

            AddHttpClients(services);
            services.Configure<GatewayOptions>(Configuration.GetSection(nameof(GatewayOptions)));
            services.Configure<DrsOptions>(Configuration.GetSection(nameof(DrsOptions)));
            services.Configure<FilterConfiguration>(Configuration.GetSection(nameof(FilterConfiguration)));
            services.Configure<NotifyOptions>(Configuration.GetSection(nameof(NotifyOptions)));
            services.Configure<EmailOptions>(Configuration.GetSection(nameof(EmailOptions)));

            RegisterGateways(services);
            RegisterUseCases(services);
            services.AddTransient<IJobStatusUpdateStrategyFactory, JobStatusUpdateStrategyFactory>();
            services.AddTransient(typeof(IActivatorWrapper<>), typeof(ActivatorWrapper<>));
            services.AddHttpContextAccessor();
            services.AddScoped<CurrentUserService>();
            services.AddScoped<IDrsService, DrsService>();
            services.AddScoped<IDrsMapping, DrsMapping>();
            services.AddScoped<SOAP>(sp => new SOAPClient(sp.GetRequiredService<IOptions<DrsOptions>>()));
            services.AddScoped<ICurrentUserService>(sp => sp.GetService<CurrentUserService>());
            services.AddScoped<ICurrentUserLoader>(sp => sp.GetService<CurrentUserService>());
            services.AddTransient<ITransactionManager, TransactionManager>();
            services.AddSingleton<IAuthenticationService, ChallengeOnlyAuthenticationService>();
            AddEmailService(services);
            services.AddFeatureManagement()
                .AddFeatureFilter<GroupFeatureFilter>();
            services.AddFilteringConfig();
            ConfigureDRSSoap(services);
            services.AddTransient(typeof(Lazy<>), typeof(LazyWrapper<>));

            AddNotificationHandlers(services);
        }

        private void AddEmailService(IServiceCollection services)
        {
            NotifyOptions options = new NotifyOptions();
            Configuration.Bind(nameof(NotifyOptions), options);

            services.AddTransient<IEmailService, GovUKNotifyService>();
            services.AddTransient<IAsyncNotificationClient>(sp => new NotificationClient(options.ApiKey));
        }

        private static void ConfigureDRSSoap(IServiceCollection services)
        {
            services.AddSoapCore();
            services.AddScoped<IDrsBackgroundService, DrsBackgroundService>();
            services.AddSoapExceptionTransformer((ex) =>
            {
                Log.Logger.Error("Error handling SOAP request {ERROR}", ex.Message);
                return ex.Message;
            });

        }

        private static void AddNotificationHandlers(IServiceCollection services)
        {
            services.AddTransient<INotifier, Notifier>();

            services.AddTransients(typeof(Notifier), typeof(INotificationHandler<>));
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddTransient<IApiGateway, ApiGateway>();
            services.AddTransient<IPropertyGateway, PropertyGateway>();
            services.AddTransient<IAlertsGateway, AlertsGateway>();
            services.AddTransient<ITenancyGateway, TenancyGateway>();
            services.AddTransient<IResidentContactGateway, ResidentContactGateway>();
            services.AddTransient<IRepairsGateway, RepairsGateway>();
            services.AddTransient<IWorkOrderCompletionGateway, WorkOrderCompletionGateway>();
            services.AddTransient<IScheduleOfRatesGateway, ScheduleOfRatesGateway>();
            services.AddTransient<IJobStatusUpdateGateway, JobStatusUpdateGateway>();
            services.AddTransient<ISorPriorityGateway, SorPriorityGateway>();
            services.AddTransient<IAppointmentsGateway, AppointmentGateway>();
            services.AddTransient<IGroupsGateway, GroupsGateway>();
            services.AddTransient<IOperativesGateway, OperativesGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddTransient<IListAlertsUseCase, ListAlertsUseCase>();
            services.AddTransient<IListPropertiesUseCase, ListPropertiesUseCase>();
            services.AddTransient<IGetPropertyUseCase, GetPropertyUseCase>();
            services.AddTransient<ICreateWorkOrderUseCase, CreateWorkOrderUseCase>();
            services.AddScoped<IListScheduleOfRatesUseCase, ListScheduleOfRatesUseCase>();
            services.AddTransient<IListWorkOrdersUseCase, ListWorkOrdersUseCase>();
            services.AddTransient<ICompleteWorkOrderUseCase, CompleteWorkOrderUseCase>();
            services.AddTransient<IUpdateJobStatusUseCase, UpdateJobStatusUseCase>();
            services.AddTransient<IGetWorkOrderUseCase, GetWorkOrderUseCase>();
            services.AddTransient<IListWorkOrderTasksUseCase, ListWorkOrderTasksUseCase>();
            services.AddTransient<IListSorTradesUseCase, ListSorTradesUseCase>();
            services.AddTransient<IListWorkOrderNotesUseCase, ListWorkOrderNotesUseCase>();
            services.AddTransient<IUpdateSorCodesUseCase, UpdateSorCodesUseCase>();
            services.AddTransient<IListAppointmentsUseCase, ListAppointmentsUseCase>();
            services.AddTransient<ICreateAppointmentUseCase, CreateAppointmentUseCase>();
            services.AddTransient<IListVariationTasksUseCase, ListVariationTasksUseCase>();
            services.AddTransient<IGetFilterUseCase, GetFilterUseCase>();
            services.AddTransient<IListOperativesUseCase, ListOperativesUseCase>();
            services.AddTransient<IGetOperativeUseCase, GetOperativeUseCase>();
            services.AddTransient<IDeleteOperativeUseCase, DeleteOperativeUseCase>();
        }

        private void AddHttpClients(IServiceCollection services)
        {
            GatewayOptions options = new GatewayOptions();
            Configuration.Bind(nameof(GatewayOptions), options);

            AddClient(services, HttpClientNames.Properties, options.PropertiesAPI, options.PropertiesAPIKey);
            AddClient(services, HttpClientNames.Alerts, options.AlertsApi, options.AlertsAPIKey);
            AddClient(services, HttpClientNames.Tenancy, options.TenancyApi, options.TenancyApiKey);
            AddClient(services, HttpClientNames.Contacts, options.HousingResidentInformationApi, options.HousingResidentInformationApiKey);
        }

        private static void AddClient(IServiceCollection services, string clientName, Uri uri, string key)
        {
            services.AddHttpClient(clientName, c =>
            {
                c.BaseAddress = uri;
                c.DefaultRequestHeaders.Add("Authorization", key);
            });
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                ?? Configuration.GetValue<string>("DatabaseConnectionString");

            services.AddDbContext<RepairsContext>(
                opt => opt
                    .UseLazyLoadingProxies()
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .AddXRayInterceptor(true)
            );
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseXRay("repairs-api");
                app.UseHsts();
            }


            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            ApiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in ApiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseMiddleware<InitialiseUserMiddleware>();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<DrsBackgroundServiceLogger>();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseSoapEndpoint<IDrsBackgroundService>("/Service.asmx", new BasicHttpsBinding(), SoapSerializer.XmlSerializer);
        }
    }
}
