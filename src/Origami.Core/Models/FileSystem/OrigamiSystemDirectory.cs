using System.ComponentModel;

namespace Origami.Core.Models.FileSystem
{
    public partial class OrigamiSystemDirectory :
        IId,
        IChanged,
        ILocalPath,
        IWebPath,
        IFKParentNull<OrigamiSystemDirectory>,
        IChecked
    {
        protected bool _checked;

        /// <summary>
        /// directory full path
        /// </summary>
        protected string _localPath = string.Empty;

        /// <summary>
        /// directory name
        /// </summary>
        protected string _name = string.Empty;
        /// <summary>
        /// web path for this directory
        /// </summary>
        protected string _webPath = string.Empty;

        /// <summary>
        /// Unique identifier for this directory
        /// </summary>
        protected Guid _id = Guid.NewGuid();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSystemDirectory(string localPath) : base()
        {
            if (new DirectoryInfo(localPath).Exists == false)
            {
                throw new DirectoryNotFoundException($"Directory {localPath} does NOT exist");
            }

            LocalPath = localPath;
            Name = Path.GetFileName(localPath) ?? string.Empty;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSystemDirectory(string localPath, string webPath) : this(localPath)
        {
            WebPath = webPath;
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public bool Checked
        {
            get => _checked;
            set => this.Set(ref _checked, value, Changed);
        }

        /// <summary>
        /// gets directories contained within this directory
        /// </summary>
        public IEnumerable<OrigamiSystemDirectory> Directories
        {
            get
            {
                var directories = new DirectoryInfo(LocalPath).EnumerateDirectories();

                foreach (var directory in directories)
                {
                    yield return new OrigamiSystemDirectory(directory.FullName);
                }
            }
        }

        /// <summary>
        /// gets the Files in this directory
        /// </summary>
        public IEnumerable<OrigamiSystemFile> Files
        {
            get
            {
                var files = new DirectoryInfo(LocalPath).EnumerateFiles();

                foreach (var file in files)
                {
                    yield return this.WebPath.Has() ?
                        new(file.FullName, $"{WebPath.TrimEnd('/')}/{file.Name}") :
                        new(file.FullName);
                }
            }
        }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// gets the full path to the directory.
        /// </summary>
        public string LocalPath
        {
            get => _localPath;
            private set => this.Set(ref _localPath, value, Changed);
        }

        /// <summary>
        /// gets the directory name
        /// </summary>
        public string Name
        {
            get => _name;
            private set => this.Set(ref _name, value, Changed);
        }
        /// <summary>
        /// Parent directory (if any)
        /// </summary>
        public OrigamiSystemDirectory? Parent
        {
            get
            {
                var parent = new DirectoryInfo(LocalPath).Parent;
                return parent != null ? new OrigamiSystemDirectory(parent.FullName) : null;
            }

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// DO NOT USE THIS, it doesn't do anything. 
        /// It only exists to fulfill the interface requirements.
        /// </summary>
        public Guid? ParentId { get; set; }

        public string WebPath
        {
            get => _webPath;
            private set => this.Set(ref _webPath, value, Changed);
        }
    }
}
