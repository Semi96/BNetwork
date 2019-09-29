using BnetworkManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Data
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Administrator";
        private const string adminEmail = "";
        private const string adminPassword = "";
        private const string adminRole = "Admin";
        public static async void EnsurePopulated(IApplicationBuilder app)
        {

            IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser user = await userManager.FindByEmailAsync(adminEmail);
                if (user == null)
                {
                    user = new ApplicationUser();
                    user.Email = adminEmail;
                    user.UserName = adminEmail;

                    await userManager.CreateAsync(user, adminPassword);
                }
                //UserManager<IdentityUser> userManager = app.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();
                //IdentityUser user = await userManager.FindByIdAsync(adminEmail);
                //if (user == null)
                //{
                //    user = new IdentityUser(adminEmail);

                //    await userManager.CreateAsync(user, adminPassword);
                //}
                IdentityResult roleResult;


                var roleExist = await roleManager.RoleExistsAsync(adminRole);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));

                }
                await userManager.AddToRoleAsync(user, adminRole);
            }

        }

    }

}
