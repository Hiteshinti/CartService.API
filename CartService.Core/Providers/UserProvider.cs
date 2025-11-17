using CartService.Core.IProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure;

namespace CartService.Core.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public UserProvider(HttpClient httpClient, IConfiguration configuration) 
        { 
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:UserApi"]??"");
        }
        public async Task<string> ValidateUser(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "ValidateUser");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token.Replace("Bearer",""));

            var response = await _httpClient.SendAsync(request);
            if (response == null || !response.IsSuccessStatusCode)
                return "";
            else
                return await response.Content.ReadAsStringAsync(); 

        }
    }
}
