using Microsoft.AspNetCore.Identity;
using System;

namespace PVIMS.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Active { get; set; }
    }
}
