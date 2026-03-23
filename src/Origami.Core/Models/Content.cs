using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Origami.Core.Models
{
    /// <summary>
    /// Simple content class with Id and Type
    /// </summary>
    public class Content : IId
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// OrigamiPage, OrigamiPost, OrigamiVideo, etc.
        /// </summary>
        [StringLength(64)]
        public string Type { get; set; } = string.Empty;
    }
}
