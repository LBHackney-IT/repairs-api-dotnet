using JWT;
using JWT.Builder;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RepairsApi.V2.Services
{
#nullable enable
    public class CurrentUserService : ICurrentUserLoader, ICurrentUserService
    {
        private const string EmailClaim = "email";
        private const string NameClaim = "name";
        private readonly ILogger<CurrentUserService> _logger;
        private User? _user = null;

        public CurrentUserService(ILogger<CurrentUserService> logger)
        {
            _logger = logger;
        }

        public void LoadUser(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return;
            try
            {
                var values = new JwtBuilder()
                        .Decode<IDictionary<string, object>>(jwt);

                _user = new User();
                _user.Email = values[EmailClaim].ToString();
                _user.Name = values[NameClaim].ToString();
            } catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public bool IsUserPresent()
        {
            return _user != null;
        }

        public User? GetUser()
        {
            return _user;
        }
    }
}
