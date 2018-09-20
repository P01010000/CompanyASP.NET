using Chayns.Backend.Api.Credentials;
using Chayns.Backend.Api.Models.Data;
using Chayns.Backend.Api.Models.Result;
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
    public class UacHelper : IUacHelper
    {
        private ChaynsApiSettings _settings;
        public UacHelper(IOptions<ChaynsApiSettings> options)
        {
            _settings = options.Value;
        }

        public IEnumerable<UacGroupResult> GetUacGroups(int locationId, int userId)
        {
            IEnumerable<UacGroupResult> result = new List<UacGroupResult>();

            var secret = new SecretCredentials(_settings.Secret, 430015);
            var repo = new UacRepository(secret);

            var data = new UacGroupDataGet(locationId);
            data.UserId = userId;

            var response = repo.GetUacGroups(data);

            if (response.Status.Success && response.Data !=null)
                result = response.Data.ToList();

            return result;
        }
    }
}
