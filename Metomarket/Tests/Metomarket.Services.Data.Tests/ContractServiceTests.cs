using System;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data;
using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Metomarket.Services.Data.Tests
{
    public class ContractServiceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CreateAsyncShouldThrownWhenPeriodInMonthsIsLessThenOne(int period)
        {
            IContractService contractService = this.GetBaseContractService();

            Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await contractService.CreateAsync(string.Empty, string.Empty, new string[0], 1, string.Empty, period);
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(24)]
        [InlineData(36)]
        public async Task CreateAsyncShouldNotThrownWhenPeriodInMonthsIsAtLeastOne(int period)
        {
            var contractRepository = Mock.Of<IRepository<Contract>>();
            var orderRepository = Mock.Of<IRepository<Order>>();
            var creditCompanyService = new Mock<ICreditCompanyService>();
            var userService = new Mock<IUserService>();

            creditCompanyService.Setup(ccs => ccs.Exists(string.Empty))
                .Returns(true);
            userService.Setup(us => us.ExistsAsync(string.Empty))
                .Returns(Task.FromResult(true));

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository,
                creditCompanyService.Object,
                userService.Object);

            await contractService.CreateAsync(string.Empty, string.Empty, new string[0], 1, string.Empty, period);
        }

        [Fact]
        public async Task CreateAsyncShouldThrownWhenUserDoesntExist()
        {
            var contractRepository = Mock.Of<IRepository<Contract>>();
            var orderRepository = Mock.Of<IRepository<Order>>();
            var creditCompanyService = new Mock<ICreditCompanyService>();
            var userService = new Mock<IUserService>();

            creditCompanyService.Setup(ccs => ccs.Exists(string.Empty))
                .Returns(true);
            userService.Setup(us => us.ExistsAsync(string.Empty))
                .Returns(Task.FromResult(false));

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository,
                creditCompanyService.Object,
                userService.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await contractService.CreateAsync(string.Empty, string.Empty, new string[0], 1, string.Empty, 1);
            });
        }

        [Fact]
        public async Task CreateAsyncShouldThrownWhenCreditCompanyDoesntExist()
        {
            var contractRepository = Mock.Of<IRepository<Contract>>();
            var orderRepository = Mock.Of<IRepository<Order>>();
            var creditCompanyService = new Mock<ICreditCompanyService>();
            var userService = new Mock<IUserService>();

            creditCompanyService.Setup(ccs => ccs.Exists(string.Empty))
                .Returns(false);
            userService.Setup(uc => uc.ExistsAsync(string.Empty))
                .Returns(Task.FromResult(true));

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository,
                creditCompanyService.Object,
                userService.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await contractService.CreateAsync(string.Empty, string.Empty, new string[0], 1, string.Empty, 1);
            });
        }

        [Fact]
        public async Task CreateAsyncShouldSetPropertiesCorrectly()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            Order order1 = dbContext.Orders.Add(new Order()).Entity;
            Order order2 = dbContext.Orders.Add(new Order()).Entity;

            await dbContext.SaveChangesAsync();

            const string userId = "userId";
            const string creditCompanyId = "ccId";
            string[] orderIds = new string[] { order1.Id, order2.Id };
            const decimal total = 100;
            const string creditCardNumber = "credit card";
            const int months = 10;

            var contractRepository = new EfRepository<Contract>(dbContext);
            var orderRepository = new Mock<IRepository<Order>>();
            var creditCompanyService = new Mock<ICreditCompanyService>();
            var userService = new Mock<IUserService>();

            orderRepository.Setup(or => or.All())
                .Returns(new Order[] { order1, order2 }.AsQueryable);
            creditCompanyService.Setup(ccs => ccs.Exists(creditCompanyId))
                .Returns(true);
            userService.Setup(us => us.ExistsAsync(userId))
                .Returns(Task.FromResult(true));

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository.Object,
                creditCompanyService.Object,
                userService.Object);

            string id = await contractService.CreateAsync(
                userId,
                creditCompanyId,
                orderIds,
                total,
                creditCardNumber,
                months);

            Contract contract = contractRepository.All()
                .Where(c => c.Id == id)
                .Include(c => c.Orders)
                .FirstOrDefault();

            Assert.NotNull(contract);
            Assert.Equal(userId, contract.CustomerId);
            Assert.Equal(creditCompanyId, contract.CompanyId);
            Assert.True(orderIds.SequenceEqual<string>(contract.Orders.Select(o => o.Id)));
            Assert.Equal(creditCardNumber, contract.CreditCardNumber);
            Assert.Equal(total / months, contract.PricePerMonth);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectCount()
        {
            const int count = 3;

            ApplicationDbContext dbContext = this.GetNewDbContext();

            var contractRepository = new EfRepository<Contract>(dbContext);
            var orderRepository = Mock.Of<IRepository<Order>>();
            var creditCompanyService = Mock.Of<ICreditCompanyService>();
            var userService = Mock.Of<IUserService>();

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository,
                creditCompanyService,
                userService);

            for (int i = 0; i < count; i++)
            {
                dbContext.Contracts.Add(new Contract());
            }

            await dbContext.SaveChangesAsync();

            Assert.Equal(count, contractService.GetCount());
        }

        private IContractService GetBaseContractService()
        {
            var contractRepository = Mock.Of<IRepository<Contract>>();
            var orderRepository = Mock.Of<IRepository<Order>>();
            var creditCompanyService = Mock.Of<ICreditCompanyService>();
            var userService = Mock.Of<IUserService>();

            IContractService contractService = new ContractService(
                contractRepository,
                orderRepository,
                creditCompanyService,
                userService);

            return contractService;
        }

        private ApplicationDbContext GetNewDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            ApplicationDbContext dbContext = new ApplicationDbContext(options);

            return dbContext;
        }
    }
}