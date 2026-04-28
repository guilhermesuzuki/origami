using Microsoft.Extensions.DependencyInjection;
using Origami.Core.Data;

namespace Origami.UI.Admin.IntegrationTests
{
    public class BlogRepository: CustomClassFixture
    {
        public BlogRepository(CustomWebApplicationFactory factory) : base(factory)
        {

        }

        [Fact]
        public void Test1()
        {
            var scope = _factory.Services.CreateScope();
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>();
        }
    }
}
