namespace Identity.Core.App.Common.Results;

public record AuthenticationResult(string Token, string RefreshToken);