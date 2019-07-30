using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;

namespace Metomarket.Services.Data
{
    public class ContractService : IContractService
    {
        private const string CreditCompanyNotFoundMessage = "Credit company not found.";
        private const string InvalidPeriodMessage = "Invali period.";

        private readonly IRepository<Contract> contractRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly ICreditCompanyService creditCompanyService;
        private readonly IUserService userService;

        public ContractService(
            IRepository<Contract> contractRepository,
            IRepository<Order> orderRepository,
            ICreditCompanyService creditCompanyService,
            IUserService userService)
        {
            this.contractRepository = contractRepository;
            this.orderRepository = orderRepository;
            this.creditCompanyService = creditCompanyService;
            this.userService = userService;
        }

        public async Task<bool> CreateAsync(
            string userId,
            string creditCompanyId,
            IEnumerable<string> orderIds,
            decimal total,
            string creditCardNumber,
            int periodInMonths)
        {
            await this.ValidateAsync(userId, creditCompanyId, periodInMonths);

            decimal pricePerMonth = total / periodInMonths;
            DateTime activeUntil = DateTime.UtcNow.AddMonths(periodInMonths);
            List<Order> orders = this.orderRepository.All()
                .Where(order => orderIds.Contains(order.Id))
                .ToList();

            Contract contract = new Contract
            {
                CustomerId = userId,
                CompanyId = creditCompanyId,
                Orders = orders,
                CreditCardNumber = creditCardNumber,
                PricePerMonth = pricePerMonth,
                ActiveUntil = activeUntil,
            };

            await this.contractRepository.AddAsync(contract);
            await this.contractRepository.SaveChangesAsync();

            return true;
        }

        private async Task ValidateAsync(string userId, string creditCompanyId, int periodInMonths)
        {
            if (periodInMonths <= 0)
            {
                throw new ServiceException(InvalidPeriodMessage);
            }

            bool userExists = await this.userService.ExistsAsync(userId);

            if (!userExists)
            {
                throw new ServiceException();
            }

            bool creditCompanyExists = this.creditCompanyService.Exists(creditCompanyId);

            if (!creditCompanyExists)
            {
                throw new ServiceException(CreditCompanyNotFoundMessage);
            }
        }
    }
}