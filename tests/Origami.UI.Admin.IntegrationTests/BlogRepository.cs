using Microsoft.Extensions.DependencyInjection;
using Origami.Core.Data;

namespace Origami.UI.Admin.IntegrationTests
{
    public class BlogRepository: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public BlogRepository(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Test1()
        {
            var scope = _factory.Services.CreateScope();
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>();
        }
    }
}
