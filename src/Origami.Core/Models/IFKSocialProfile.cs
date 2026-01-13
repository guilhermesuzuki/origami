namespace Origami.Core.Models
{
    public interface IFKSocialProfile : ISocialProfileId
    {
        /// <summary>
        /// Social Profile (FK)
        /// </summary>
        OrigamiSocialProfile? SocialProfile { get; set; }
    }
}
