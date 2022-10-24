namespace Sorter.Entity
{
    public struct Record
    {
        public string text;
        public string number;
        public int parcedNumber = -1;

        internal Record(string row)
        {
            var parts = row.Split(". ");
            if(parts.Count() != 2)
            {
                throw new Exception($"Bad file row: {row}");
            }

            text = parts[1];
            number = parts[0];

            if (!int.TryParse(number, out parcedNumber))
            {
                throw new Exception($"Bad number: {number}");
            }
        }

        public override string? ToString()
        {
            return $"{number}. {text}";
        }
    }

    public class RecordComparer : IComparer<Record>
    {
        public int Compare(Record first, Record second)
        {
            var textComparsion = first.text.CompareTo(second.text);
            if (textComparsion != 0)
                return textComparsion;

            return first.parcedNumber.CompareTo(second.parcedNumber);
        }
    }
}
