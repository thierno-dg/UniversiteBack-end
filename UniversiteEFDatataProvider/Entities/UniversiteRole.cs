using Microsoft.AspNetCore.Identity;

namespace UniversiteEFDataProvider.Entities;

public class UniversiteRole : IdentityRole
{
    public UniversiteRole() { }

    public UniversiteRole(string role) : base(role) { }
}