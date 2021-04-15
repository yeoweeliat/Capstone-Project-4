using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grocery.WebApp.Data.Enums;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Identity;


namespace Grocery.WebApp.Data
{
    public static class ApplicationDbContextSeed // public static class = becomes singleton class
    {
        //public static void SeedRoles(RoleManager<MyIdentityRole> roleManager)
        //{

        //}

        //need to call seedroles in startup.cs
        public static async Task SeedRolesAsync(RoleManager<MyIdentityRole> roleManager)
        {

            //MyIdentityRole role = new MyIdentityRole
            //{,

            //    Name = "Administrator",
            //    Description = "The admin for the application..."

            //};
            //await roleManager.CreateAsync(role);


            foreach( MyAppRoleTypes role in Enum.GetValues(typeof(MyAppRoleTypes)) ) {

                MyIdentityRole roleObj = new MyIdentityRole()
                {
                    Name = role.ToString(),
                    Description = $"The {role} for the Application"

                };
            await roleManager.CreateAsync(roleObj);
            }
        }
    }
}
