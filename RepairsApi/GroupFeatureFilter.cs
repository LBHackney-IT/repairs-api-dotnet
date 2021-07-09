using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi
{
    public class GroupFilterSettings
    {
        public IList<string> AllowedGroups { get; set; } = new List<string>();
    }

    [FilterAlias("Group")]
    public class GroupFeatureFilter : IFeatureFilter
    {
        private readonly ICurrentUserService _currentUserService;

        public GroupFeatureFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<GroupFilterSettings>();

            var user = _currentUserService.GetUser();

            return Task.FromResult(settings.AllowedGroups.Intersect(user.Groups()).Any());
        }
    }
}
