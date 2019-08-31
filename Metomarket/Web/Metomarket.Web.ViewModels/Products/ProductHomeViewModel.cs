using AutoMapper;

using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductHomeViewModel : IMapFrom<Product>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, ProductHomeViewModel>().ForMember(
                m => m.Type,
                opt => opt.MapFrom(p => p.Type.Name));
        }
    }
}