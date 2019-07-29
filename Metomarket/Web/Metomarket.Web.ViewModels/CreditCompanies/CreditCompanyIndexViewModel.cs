using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.CreditCompanies
{
    public class CreditCompanyIndexViewModel : IMapFrom<CreditCompany>
    {
        public string Name { get; set; }

        public int ContractsCount { get; set; }
    }
}