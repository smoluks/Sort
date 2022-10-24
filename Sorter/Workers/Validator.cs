using Sorter.Entity;

namespace Sorter.Workers
{
    internal static class Validator
    {
        internal static void ValidateSorting(string file)
        {
            Record? previous = null;
            Record current;
            long count = 0;
            var comparer = new RecordComparer();

            Console.WriteLine($"Starting result validation...");

            using (var sr = new StreamReader(file))
            {
                do
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        break;

                    if (previous.HasValue)
                    {
                        current = new Record(line);
                        if (comparer.Compare(current, previous.Value) < 0)
                            throw new Exception($"{current} is lower than {previous.Value}, but upper in file");

                        previous = current;
                    }
                    else
                    {
                        previous = new Record(line);
                    }

                    if (++count % 1000000 == 0)
                    {
                        Console.WriteLine($"{count} validated");
                    }
                } while (true);
            }

            Console.WriteLine($"Validated {count} records OK");
        }
    }
}
