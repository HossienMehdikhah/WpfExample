using Microsoft.EntityFrameworkCore;
using System;
using WpfExample.Model.Entity;

namespace WpfExample.DataLayer
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Personal>(builder =>
            {
                builder.HasKey(x => x.ID);
                builder.Property(x => x.Name).IsRequired();
                builder.Property(x => x.CompanyName).IsRequired();
                builder.Property(x => x.Tell).IsRequired();
                builder.Property(x => x.Email).IsRequired();

                builder.HasMany<Order>()
               .WithOne(x => x.Personal)
               .HasForeignKey(x => x.PersonalID);

                builder.HasData(new Personal[]
                {
                    new Personal()
                    {
                    CompanyName="microsoft",
                    Email="microsoft@hotmail.com",
                    Tell="09119159244",
                    Name = "Bill Gates"
                    },
                    new Personal()
                    {
                        CompanyName = "Apple",
                        Email = "Apple@hotmail.com",
                        Tell = "09119159244",
                        Name = "Steve"
                    },
                    new Personal()
                    {
                        CompanyName="Google",
                        Email="Google@Gmail.com",
                        Tell="09119159244",
                        Name = "Hasan"
                    }

                });               
            });

            builder.Entity<Produce>(builder =>
            {
                builder.HasKey(x => x.ID);
                builder.Property(x => x.Name).IsRequired();
                builder.Property(x => x.Price).IsRequired();
                builder.Property(x => x.Code).IsRequired();

                builder.HasMany<OrderDetail>()
                .WithOne(x => x.Produce)
                .HasForeignKey(x => x.ProduceID);



                builder.HasData(new Produce[]
                {
                    new Produce()
                    {
                        Code="a1",
                        Name = "pride",
                        Price=10700
                    },
                    new Produce()
                    {
                        Code="a2",
                        Name = "206",
                        Price=15400
                    },
                    new Produce()
                    {
                        Code="a3",
                        Name = "pars",
                        Price=14300
                    },
                    new Produce()
                    {
                        Code="a4",
                        Name = "405",
                        Price=17700
                    }
                });
            });

            builder.Entity<Order>(builder =>
            {
                builder.HasKey(x => x.ID);
                builder.Property(x => x.PersonalID).IsRequired();
                builder.Property(x => x.Number).IsRequired();
                builder.Property(x => x.Date).IsRequired();

                builder.HasMany<OrderDetail>()
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<OrderDetail>(builder =>
            {
                builder.HasKey(x => x.ID);
                builder.Property(x => x.OrderID).IsRequired();
                builder.Property(x => x.Price).IsRequired();
                builder.Property(x => x.ProduceID).IsRequired();
                builder.Property(x => x.SumPrice).IsRequired();
                builder.Property(x => x.Count).IsRequired();

                builder.HasOne(x => x.Produce)
                .WithMany()
                .HasForeignKey(x => x.ProduceID);
            });




        }
    }
}
