using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        public BaseValidator(Text text, IWebRootPath webRootPath) : base()
        {
            WebRootPath = webRootPath;
            Text = text;
        }
        public IWebRootPath WebRootPath { get; }
        public Text Text { get; }
    }
}
