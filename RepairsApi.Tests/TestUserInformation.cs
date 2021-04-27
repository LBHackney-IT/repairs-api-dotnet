using System.Collections.Generic;

namespace RepairsApi.Tests
{
    // Info built from jwt.io
    public static class TestUserInformation
    {
        public const string Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMDA1MTg4ODg3NDY5MjIxMTY2NDciLCJlbWFpbCI6ImhhY2tuZXkudXNlckB0ZXN0LmhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJIYWNrbmV5IFVzZXIiLCJncm91cHMiOlsiZ3JvdXAgMSIsImdyb3VwIDIiXSwiaWF0IjoxNTcwNDYyNzMyfQ.BxBlEHWHGU6GkPO5DZoshJp3VQcrm2umaMkQ7Q7zxw8";
        public const string Email = "hackney.user@test.hackney.gov.uk";
        public const string Name = "Hackney User";
        public const string Sub = "100518888746922116647";
        public static readonly List<string> Groups = new List<string> { "group 1", "group 2" };
    }
}
