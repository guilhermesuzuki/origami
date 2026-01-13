using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class EmailStatusRepository : IEmailStatusRepository
    {
        public Result? Status { get; set; } = null;
    }
}
