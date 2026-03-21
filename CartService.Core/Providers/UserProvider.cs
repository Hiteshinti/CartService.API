using CartService.Core.IProviders;
using CartService.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CartService.Core.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProvider> _logger;

        public UserProvider(
            HttpClient httpClient,
            IOptions<ApiSettingsOptions> options,
            ILogger<UserProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var userApi = options.Value.UserApi;
            if (string.IsNullOrWhiteSpace(userApi))
            {
                throw new InvalidOperationException("ApiSettings:UserApi is not configured.");
            }

            _httpClient.BaseAddress = new Uri(userApi, UriKind.Absolute);
        }

        public async Task<Guid?> ValidateUser(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var bearerToken = token.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            using var request = new HttpRequestMessage(HttpMethod.Get, "ValidateUser");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("User validation request failed with status code {StatusCode}.", response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return Guid.TryParse(responseContent, out var userId) ? userId : null;

        }
    }
}
