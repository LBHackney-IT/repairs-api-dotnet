using JWT;
using JWT.Builder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepairsApi.V2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RepairsApi.V2.Services
{
#nullable enable
    public class CurrentUserService : ICurrentUserLoader, ICurrentUserService
    {
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
                        .Decode<User>(jwt);

                _user = values;
            }
            catch (Exception e)
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
