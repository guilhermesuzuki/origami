namespace Origami.Core.Models.FileSystem
{
    public class DirectoryContext :
        IId,
        IName
    {
        public DirectoryContext(OrigamiSystemDirectory directory) : base()
        {
            Directory = directory;
        }

        /// <summary>
        /// More context information about the directory
        /// </summary>
        public object? Context { get; set; }
        public OrigamiSystemDirectory Directory { get; }
        public Guid Id { get; set; }

        public string Name
        {
            get
            {
                if (Context != null)
                {
                    return Context switch
                    {
                        OrigamiBlog blog => blog.Name,
                        OrigamiPage page => page.Title,
                        OrigamiPost post => post.Title,
                        OrigamiVideo video => video.Title,
                        OrigamiCategory category => category.Name,
                        OrigamiUser user => user.Username,
                        _ => string.Empty,
                    };
                }
                return string.Empty;
            }
            set => throw new NotImplementedException();
        }
    }
}
