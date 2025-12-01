// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IoTCenter.Data
{
    public class GWDbContextFactory : IDesignTimeDbContextFactory<GWDbContext>
    {
        public GWDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GWDbContext>();

            optionsBuilder.UseSqlite($"Filename=.\\opengwiotcenter.db");

            var gwDbContext = new GWDbContext(optionsBuilder.Options);



            return gwDbContext;
        }
    }
}
