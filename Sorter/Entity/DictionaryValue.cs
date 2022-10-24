namespace Sorter.Entity
{
    internal struct DictionaryValue
    {
        internal StreamWriter streamWriter;
        internal int count = 0;
        internal ValueTask disposeTask = new ValueTask();

        public DictionaryValue(StreamWriter sw)
        {
            this.streamWriter = sw;
        }

        internal void DisposeWriter()
        {
            disposeTask = streamWriter.DisposeAsync();
        }

        internal void WriteLine(string? v)
        {
            streamWriter.WriteLine(v);
            count++;
        }
    }
}
