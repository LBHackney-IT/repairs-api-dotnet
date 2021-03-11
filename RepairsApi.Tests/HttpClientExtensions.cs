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
                .AddClaim("sub", user.Sub)
                .Encode();
            client.DefaultRequestHeaders.Remove("X-Hackney-User");
            client.DefaultRequestHeaders.Add("X-Hackney-User", jwt);
        }

        public static void SetAgent(this HttpClient client)
        {
            client.SetUser(new RepairsApi.V2.Domain.User
            {
                Sub = TestUserInformation.SUB,
                Name = TestUserInformation.NAME,
                Email = TestUserInformation.EMAIL,
                Groups = new List<string>()
                {
                    "agent",
                    "raise50",
                    "vary50"
                }
            });
        }

        public static void SetAgent(this HttpClient client, string raiseLimit, string varyLimit)
        {
            client.SetUser(new RepairsApi.V2.Domain.User
            {
                Sub = TestUserInformation.SUB,
                Name = TestUserInformation.NAME,
                Email = TestUserInformation.EMAIL,
                Groups = new List<string>()
                {
                    "agent",
                    raiseLimit,
                    varyLimit
                }
            });
        }

        public static void SetGroup(this HttpClient client, string group)
        {
            client.SetUser(new RepairsApi.V2.Domain.User
            {
                Sub = TestUserInformation.SUB,
                Name = TestUserInformation.NAME,
                Email = TestUserInformation.EMAIL,
                Groups = new List<string>()
                {
                    group,
                    "raise50",
                    "vary50"
                }
            });
        }
    }
}
