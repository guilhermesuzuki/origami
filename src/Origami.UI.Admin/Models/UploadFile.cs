using Microsoft.AspNetCore.Components.Forms;
using Origami.Core.Models;

namespace Origami.UI.Admin.Models
{
    public class UploadFile :
        IId
    {
        public UploadFile(IBrowserFile file) : base()
        {
            File = file;
        }

        public IBrowserFile File { get; init; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Exception? Exception { get; set; }
    }
}
