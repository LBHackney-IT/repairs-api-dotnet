using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace RepairsApi.Tests
{
    public static class HttpClientExtensions
    {
        public static void SetUser(this HttpClient client, User user)
        {
            var jwt = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret("topsecret")
                .AddClaim("email", user.Email)
                .AddClaim("name", user.Name)
                .AddClaim("groups", user.Groups)
                .Encode();
            client.DefaultRequestHeaders.Remove("X-Hackney-User");
            client.DefaultRequestHeaders.Add("X-Hackney-User", jwt);
        }

        public static void SetAgent(this HttpClient client)
        {
            client.SetUser(new RepairsApi.V2.Domain.User
            {
                Name = TestUserInformation.NAME,
                Email = TestUserInformation.EMAIL,
                Groups = new List<string>()
                {
                    "repairs-hub-frontend-staging"
                }
            });
        }

        public static void SetContractor(this HttpClient client, string contractorReference)
        {
            var group = Groups.SecurityGroups.Where(kv => kv.Value.ContractorReference == contractorReference).Select(kv => kv.Key).Single();

            client.SetUser(new RepairsApi.V2.Domain.User
            {
                Name = TestUserInformation.NAME,
                Email = TestUserInformation.EMAIL,
                Groups = new List<string>()
                {
                    group
                }
            });
        }
    }
}
