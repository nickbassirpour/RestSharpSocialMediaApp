using RestSharpSocialMediaPosts.Reddit.Services.Interfaces;
using RestSharpSocialMediaPosts.Tumblr.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Token
{
    public class TokenRefreshService : BackgroundService, ITokenRefreshService
    {
        private readonly ILogger<TokenRefreshService> _logger;
        private readonly IRedditService _redditService;
        private readonly ITumblrService _tumblrService;
        private static string? _redditRefreshToken;
        private string? _tumblrRefreshToken;

        public TokenRefreshService(ILogger<TokenRefreshService> logger, IRedditService redditService, ITumblrService tumblrService)
        {
            _logger = logger;
            _redditService = redditService;
            _tumblrService = tumblrService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token refresh service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!string.IsNullOrEmpty(_redditRefreshToken))
                {
                    _logger.LogInformation("Reddit refresh token found. Starting refresh process...");
                    await StartRedditTokenRefresh(stoppingToken);
                }

                if (!string.IsNullOrEmpty(_tumblrRefreshToken))
                {
                    _logger.LogInformation("Tumblr refresh token found. Starting refresh process...");
                    await StartTumblrTokenRefresh(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Token refresh service is stopping.");
        }

        public void SetRedditRefreshToken(string refreshToken)
        {
            _redditRefreshToken = refreshToken;
        }

        public void SetTumblrRefreshToken(string refreshToken)
        {
            _tumblrRefreshToken = refreshToken;
        }

        private async Task StartRedditTokenRefresh(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                if (!string.IsNullOrEmpty(_redditRefreshToken))
                {
                    _logger.LogInformation("Refreshing reddit token...");
                    await _redditService.RefreshToken();
                }
            }
        }

        private async Task StartTumblrTokenRefresh(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Token expires after 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                if (!string.IsNullOrEmpty(_tumblrRefreshToken))
                {
                    _logger.LogInformation("Refreshing tumblr token...");
                    await _tumblrService.RefreshToken();
                }
            }
        }
    }
}
