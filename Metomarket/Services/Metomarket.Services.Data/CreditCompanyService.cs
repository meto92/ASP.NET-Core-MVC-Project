using System;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;

namespace Metomarket.Services.Data
{
    public class CreditCompanyService : ICreditCompanyService
    {
        private readonly IRepository<CreditCompany> creditCompanyRepository;

        public CreditCompanyService(IRepository<CreditCompany> creditCompanyRepository)
        {
            this.creditCompanyRepository = creditCompanyRepository;
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
    }
}