using Metomarket.Web.Controllers;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class HomeRouteTests
    {
        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap("/Home/Index")
                .To<HomeController>(c => c.Index());

        [Fact]
        public void SlashShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap("/")
                .To<HomeController>(c => c.Index());
    }
}