using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

using Grocery.WebApp.Data.Enums;
using Grocery.WebApp.Models;

namespace Grocery.WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyIdentityUser, MyIdentityRole, Guid> // tell use custom context
    {

        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // add any model related customization
            // which cannot be done by DataAnnotations are handled here (using FluentAPI)

            builder.Entity<MyIdentityUser>().Property(e => e.IsAdminUser).HasDefaultValue(false); // you want to make it compatible to all databases, it should work with any database anywhere int he world, DA only work on sql server 

            builder.Entity<MyIdentityUser>().Property(e => e.Gender).HasDefaultValue<MyAppGenderTypes>(MyAppGenderTypes.Male);

            builder.Entity<Product>().Property(e => e.Quantity).HasDefaultValue(0);

            builder.Entity<Product>().Property(e => e.SellingPricePerUnit).HasDefaultValue(0.00);

            builder.Entity<Product>().Property(e => e.LastUpdatedOn).HasDefaultValueSql("getdate()"); //hasdefaultvaluesql allows you to write func inside, getdate() = gen current date time, is a built in func in sql server

            // define foreign key policies for addressing cascade update & delete
            // Note: (p) - parent entity, (c) - child entity
            // we only have to worry about cascade delete

            builder.Entity<Product>() // select child table
                .HasOne<MyIdentityUser>(c => c.CreatedByUser) // define object of parent in child
                .WithMany(p => p.ProductsCreatedByUser) // define collection of children in parent
                .HasForeignKey(c => c.CreatedByUserId)    //map to column of child which provides the fk (on which the fk is established)
                .OnDelete(DeleteBehavior.Restrict); // define cascade delete behaviour


            builder.Entity<Product>()
               .HasOne<MyIdentityUser>(c => c.UpdatedByUser)
               .WithMany(p => p.ProductsUpdatedByUser)
               .HasForeignKey(c => c.UpdatedByUserId)  
               .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(builder);
        }
    }
}
