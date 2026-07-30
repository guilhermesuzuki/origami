using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class ContentCommentReactionTests : CustomClassFixture
    {
        protected readonly ContentCommentTests _contentCommentTests = new();

        public ContentCommentReactionTests()
        {
            
        }

        [Theory]
        [InlineData(true)]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _contentCommentTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);


        }
    }
}
