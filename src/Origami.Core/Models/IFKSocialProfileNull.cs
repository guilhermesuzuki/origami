namespace Origami.Core.Models
{
    public interface IFKSocialProfileNull : ISocialProfileIdNull
    {
        /// <summary>
        /// Social Profile (FK)
        /// </summary>
        OrigamiSocialProfile? SocialProfile { get; set; }
    }
}
