using Metomarket.Web.ViewModels.Orders;
using Metomarket.Web.ViewModels.Products;
using Metomarket.Web.ViewModels.ShoppingCart;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    [Authorize]
    public class ShoppingCartController : MarketController
    {
        public IActionResult Index()
        {
            ShoppingCartViewModel model = new ShoppingCartViewModel
            {
                Orders = new OrderShoppingCartViewModel[]
                {
                    new OrderShoppingCartViewModel
                    {
                        Id = "1",
                        Quantity = 2,
                        Product = new ProductShoppingCartViewModel
                        {
                            Name = "laptop",
                            Price = 999,
                            ImageUrl = "https://www.lenovo.com/medias/lenovo-laptop-thinkpad-x1-extreme-hero.png?context=bWFzdGVyfHJvb3R8NzkyMDF8aW1hZ2UvcG5nfGg2NC9oZDUvOTk4NjE1MTg0MTgyMi5wbmd8NDNhZGJlZTg2MjAwMmYyYTcyMDQ0NzIxNDIwODRiOWIxODliODY5ZWQ5NmZiOWQ0MTQ5MzM0YjIxMDJhZTFlMQ",
                        },
                    },
                    new OrderShoppingCartViewModel
                    {
                        Id = "2",
                        Quantity = 4,
                        Product = new ProductShoppingCartViewModel
                        {
                            Name = "laptop",
                            Price = 888,
                            ImageUrl = "https://zdnet1.cbsistatic.com/hub/i/r/2019/04/17/1f68c3a6-495e-4325-bc16-cc531812f0ec/thumbnail/770x433/84ff4194826e8303efb771cd377a854f/chuwi-herobook-header.jpg",
                        },
                    },
                },
            };

            return this.View(model);
        }

        public IActionResult DeleteOrder(string id)
        {
            return this.Content(id);
        }

        [HttpPost]
        public IActionResult ConfirmOrders()
        {
            throw new System.Exception();
        }

        [HttpPost]
        public IActionResult EmptyCart()
        {
            throw new System.Exception();
        }
    }
}