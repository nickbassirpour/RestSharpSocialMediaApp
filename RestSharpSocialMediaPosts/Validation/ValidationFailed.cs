using RestSharp;

namespace RestSharpSocialMediaPosts.Validation;

public record ValidationFailed(string ErrorMessage, int? StatusCode)
{
    public ValidationFailed(RestResponse response) : this(response.ErrorMessage ?? "Unknown error", (int?)response.StatusCode)
    {
    }
}
