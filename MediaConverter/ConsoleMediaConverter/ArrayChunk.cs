namespace ConsoleMediaConverter
{
    /// <summary>
    /// Chunk start and end indexes.
    /// </summary>
    internal struct ArraySlice
    {
        public int Start { get; set; }
        public int End { get; set; }

        public ArraySlice(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
