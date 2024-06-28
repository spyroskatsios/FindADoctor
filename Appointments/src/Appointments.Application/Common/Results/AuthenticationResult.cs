namespace Appointments.Application.Common.Results;

public record AuthenticationResult(string Token, string RefreshToken);