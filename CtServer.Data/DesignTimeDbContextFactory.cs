using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace CtServer.Data;
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CtDbContext>
    {
        public CtDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CtDbContext>()
                .UseNpgsql(Environment.GetEnvironmentVariable("DB_STRING")!);

            return new CtDbContext(optionsBuilder.Options);
        }
    }
