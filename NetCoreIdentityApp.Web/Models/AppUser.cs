﻿using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public byte? Gender { get; set; }
    }
}
