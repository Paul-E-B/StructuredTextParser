namespace StructuredTextParser
{
    /// <summary>
    /// An interface used by an type of file info class
    /// </summary>
    internal interface IFile
    {
        public string Path { get; set; }

        public string Extension { get; set; }

        public string Name { get; set; }

        public char Delimiter { get; set; }

        public string ParseType { get; set; }


    }
}
