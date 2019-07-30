using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Services.Data
{
    public class CreditCompanyService : ICreditCompanyService
    {
        private readonly IRepository<CreditCompany> creditCompanyRepository;

        public CreditCompanyService(IRepository<CreditCompany> creditCompanyRepository)
        {
            this.creditCompanyRepository = creditCompanyRepository;
        }

        public IEnumerable<TModel> All<TModel>()
        {
            IEnumerable<TModel> models = this.creditCompanyRepository.AllAsNoTracking()
                .To<TModel>()
                .ToArray();

            return models;
        }

        public async Task<bool> CreateAsync(string name, DateTime activeSince)
        {
            CreditCompany creditCompany = new CreditCompany
            {
                Name = name,
                ActiveSince = activeSince,
            };

            await this.creditCompanyRepository.AddAsync(creditCompany);
            await this.creditCompanyRepository.SaveChangesAsync();

            return true;
        }

        public bool Exists(string id)
        {
            bool creditCompanyExists = this.creditCompanyRepository.All()
                .Where(creditCompany => creditCompany.Id == id)
                .FirstOrDefault() != null;

            return creditCompanyExists;
        }
    }
}