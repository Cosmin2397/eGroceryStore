﻿using Microsoft.AspNetCore.Identity;

namespace eGroceryStore.Areas.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; } = String.Empty;
    }

    public class ApplicationRole : IdentityRole
    {

    }
}
