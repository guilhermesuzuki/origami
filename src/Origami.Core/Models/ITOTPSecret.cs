using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public interface ITOTPSecret
    {
        /// <summary>
        /// TOTP URI secret
        /// </summary>
        string TOTPSecret { get; set; }

        /// <summary>
        /// TOTP recovery codes, hashed to SHA256, separated by comma
        /// </summary>
        string TOTPRecoveryCodes { get; set; }
    }
}
