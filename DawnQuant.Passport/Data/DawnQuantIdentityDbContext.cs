using DawnQuant.Passport.Constants;
using DawnQuant.Passport.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DawnQuant.Passport.Data
{
    public class DawnQuantIdentityDbContext : IdentityDbContext<
        DawnQuantIdentityUser, DawnQuantIdentityRole, long, DawnQuantIdentityUserClaim,
        DawnQuantIdentityUserRole, DawnQuantIdentityUserLogin
        , DawnQuantIdentityRoleClaim, DawnQuantIdentityUserToken>
    {
        public DawnQuantIdentityDbContext(DbContextOptions<DawnQuantIdentityDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureIdentityContext(builder);
        }

        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<DawnQuantIdentityRole>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityRoles);
            builder.Entity<DawnQuantIdentityRoleClaim>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityRoleClaims);
            builder.Entity<DawnQuantIdentityUserRole>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityUserRoles);

            builder.Entity<DawnQuantIdentityUser>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityUsers);
            builder.Entity<DawnQuantIdentityUserLogin>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityUserLogins);
            builder.Entity<DawnQuantIdentityUserClaim>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityUserClaims);
            builder.Entity<DawnQuantIdentityUserToken>().ToTable(DawnQuantIdentityConsts.TableNames.IdentityUserTokens);
        }
    }
}
