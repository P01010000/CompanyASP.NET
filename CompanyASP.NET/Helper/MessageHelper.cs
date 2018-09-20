using Chayns.Backend.Api.Credentials;
using Chayns.Backend.Api.Models.Data;
using Chayns.Backend.Api.Repositories;
using CompanyASP.NET.Interfaces;
using CompanyASP.NET.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Helper
{
    public class MessageHelper : IMessageHelper
    {
        private ChaynsApiSettings _settings;
        public MessageHelper(IOptions<ChaynsApiSettings> options)
        {
            _settings = options.Value;
        }

        public bool SendIntercom(string message)
        {
            var secret = new SecretCredentials(_settings.Secret, 430015);

            //var userRepository = new UserRepository(secret);
            //var user = userRepository.GetUser(1948119, new LocationIdentifier(157669));
            
            var intercomRepository = new IntercomRepository(secret);
            var intercomData = new IntercomData(157669)
            {
                Message = message,
                UserIds = new List<int>
                {
                    1948119
                }
            };
            var result = intercomRepository.SendIntercomMessage(intercomData);

            return result.Status.Success;
        }
    }
}
