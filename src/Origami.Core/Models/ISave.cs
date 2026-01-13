namespace Origami.Core.Models
{
    public interface ISave
    {
        void Save();
        void UndoChanges();
    }
}
