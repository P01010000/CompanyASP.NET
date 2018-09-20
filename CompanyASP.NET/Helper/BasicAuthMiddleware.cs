using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CompanyASP.NET.Models;

namespace CompanyASP.NET.Helper
{
    public class BasicAuthMiddleware
    {
        private RequestDelegate _next;
        private IDictionary<string, string> userList = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            
        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
            userList.Add("heiner", "123");
            userList.Add("postman", "password");
        }

        public async Task Invoke(HttpContext context)
        {
            string auth = context.Request.Headers["Authorization"];
            if(auth != null && auth.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
            {
                string base64 = auth.Substring(6).Trim();
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

                int index = decoded.IndexOf(":");

                string username = decoded.Substring(0, index);
                string password = decoded.Substring(index + 1);

                try
                {
                    if (userList[username] == password)
                    {
                        await _next.Invoke(context);
                    }
                    else
                    {
                        Reject(context);
                    }
                } catch (Exception)
                {
                    Reject(context);
                }
            } else if(auth !=null && auth.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
            {
                string content = auth.Substring(7).Trim().Split('.')[1];
                // Add Padding if base64 string is not well formed
                if (content.Length % 4 == 2) content += "==";
                if (content.Length % 4 == 3) content += "=";

                string payload = Encoding.UTF8.GetString(Convert.FromBase64String(content));
                var tokenPayload = JsonConvert.DeserializeObject<ChaynsTokenPayload>(payload);
                // Only Allow Access if Token is valid for my locationId and has not expired yet
                if (tokenPayload.LocationId == 157669 && tokenPayload.IsAdmin) {
                    if (DateTime.UtcNow < tokenPayload.exp)
                    {
                        await _next.Invoke(context);
                    } else
                    {
                        Reject(context, "Token expired");
                    }
                } else
                {
                    Reject(context, "Token is not valid for LocationId 157669");
                }

                // check hash against server?
            } else
            {
                Reject(context);
            }

            return;
        }

        private async void Reject(HttpContext context, string message = "Not Allowed")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(message));
        }
    }
}
