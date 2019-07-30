using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.Infrastructure.ComponentViewModels.CreditCompanies
{
    public class CreditCompanyOptionViewModel : IMapFrom<CreditCompany>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}