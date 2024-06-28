using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Entities;

public class AppUser : IdentityUser
{
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    
}