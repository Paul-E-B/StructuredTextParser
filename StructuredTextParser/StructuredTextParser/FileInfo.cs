namespace StructuredTextParser
{
    /// <summary>
    /// A class for various generating various kinds of file info from input
    /// </summary>
    internal class FileInfo:IFile
    {
        public string Path { get; set; }

        public string Extension { get; set; }

        public string Name { get; set; }
        public char Delimiter { get; set; }


        //Used to hold general file info
        public FileInfo(string path, string extension, string name)
        {
            Path = path;
            Extension = extension;
            Name = name;
        }


        //Used to hold file info specifically for a delimiter type of data
        public FileInfo(string path, string extension, string name, char delimiter)
        {
            Path = path;
            Extension = extension;
            Name = name;
            Delimiter = delimiter;
        }

    }
}
