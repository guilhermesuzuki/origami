using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public interface ITOTPSecret
    {
        Guid TOTPSecret { get; set; }

        string TOTPRecoveryCodes { get; set; }
    }
}
