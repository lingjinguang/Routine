using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Data
{
    public class RoutineDbContext:DbContext
    {
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options)
            :base(options) 
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Introduction).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<Company>().Property(x => x.Country).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Industry).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Product).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Employee>().Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(x => x.LastName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Employee>()
                .HasOne(navigationExpression: x => x.Company)
                .WithMany(navigationExpression: x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);//Restrict 不允许company删除时被删除，Cascade是允许
            modelBuilder.Entity<Company>().HasData(
                new Company { 
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    Name= "Microsoft",
                    Introduction = "Great Company",
                    Country = "USA",
                    Industry = "SoftWare",
                    Product = "SoftWare"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    Name = "Google",
                    Introduction = "Don't be evil",
                    Country = "USA",
                    Industry = "Internet",
                    Product = "SoftWare"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    Name = "Alipapa",
                    Introduction = "Fubao Company",
                    Country = "Chain",
                    Industry = "SoftWare",
                    Product = "SoftWare"
                }
            );
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("078563f337ec6d6fedf131ddc857db19"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(year: 1986, month: 11, day: 4),
                    EmployeeNo = "G003",
                    FirstName = "Mary",
                    LastName = "King",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("7692dcdc19e41e66c6ae2de54a696b25"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(year: 1996, month: 11, day: 11),
                    EmployeeNo = "G097",
                    FirstName = "Kevin",
                    LastName = "Richardson",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("0f3e84acb19dff22f695f31dbe3e972a"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(year: 1976, month: 1, day: 2),
                    EmployeeNo = "MSFT231",
                    FirstName = "NICK",
                    LastName = "Carter",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("268e27056a3e52cf3755d193cbeb0594"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(year: 1996, month: 11, day: 11),
                    EmployeeNo = "MSFT245",
                    FirstName = "Vince",
                    LastName = "Chart",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("00c66aaf5f2c3f49946f15c1ad2ea0d3"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    DateOfBirth = new DateTime(year: 1986, month: 11, day: 4),
                    EmployeeNo = "MSFT246",
                    FirstName = "Mary",
                    LastName = "Harot",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("e10adc3949ba59abbe56e057f20f883e"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    DateOfBirth = new DateTime(year: 1955, month: 2, day: 24),
                    EmployeeNo = "MSFT247",
                    FirstName = "Steve",
                    LastName = "Jobs",
                    Gender = Gender.男
                }
            );
        }
    }
}
