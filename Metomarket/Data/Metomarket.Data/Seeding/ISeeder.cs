﻿using System;
using System.Threading.Tasks;

namespace Metomarket.Data.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}