using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public interface ISocialProfileIdNull
    {
        /// <summary>
        /// Social Profile Id (FK)
        /// </summary>
        Guid? SocialProfileId { get; set; }
    }
}
