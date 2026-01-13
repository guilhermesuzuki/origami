using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IEmailStatusRepository
    {
        /// <summary>
        /// Status of the email service
        /// </summary>
        Result? Status { get; set; }
    }
}
