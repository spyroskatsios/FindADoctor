namespace Identity.Contracts.Identity;

public record RegisterDoctorRequest(string UserName, string Email, string Password);
public record RegisterPatientRequest(string UserName, string Email, string Password);

public record LoginRequest(string UserName, string Password);

public record RefreshTokenRequest(string Token, string RefreshToken);