namespace Origami.Core.Models;

public interface IVideo
{
    OrigamiFile MediaFile { get; }
    OrigamiFile? Subtitle1 { get; set; }
    OrigamiFile? Subtitle2 { get; set; }
    OrigamiFile? Subtitle3 { get; set; }
}
