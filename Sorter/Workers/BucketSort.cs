using System.Diagnostics;
using Sorter.Entity;

namespace Sorter.Workers
{
    internal static class BucketSort
    {
        static string _bucketDirectory = "";
        //static readonly Stopwatch stopWatch = new();
        static readonly SortedDictionary<string, DictionaryValue> buckets = new();

        internal static void Sort(string rawFile, string outputFile)
        {
            _bucketDirectory = Path.GetDirectoryName(outputFile) + "\\temp";
            Directory.CreateDirectory(_bucketDirectory);

            //------sort original file to buckets-----
            int count = 0;                      

            //stopWatch.Start();
            using (var sr = new StreamReader(rawFile, GetFileStreamOptionsForRead()))
            {
                do
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        break;

                    AddRecordToBucket(line);

                    if (++count % 1000000 == 0)
                    {
                        Console.WriteLine($"{count} readed");
                    }
                } while (true);
            }

            //stopWatch.Stop();
            //Console.WriteLine($"Readed {count} records by {stopWatch.ElapsedMilliseconds} ms");

            foreach (var value in buckets.Values)
            {
                value.DisposeWriter();
            }

            //------sort buckets------
            count = 0;

            //stopWatch.Restart();
            foreach (var bucket in buckets)
            {              
                var records = new List<Record>(bucket.Value.count);

                bucket.Value.disposeTask.GetAwaiter().GetResult();

                using (var sr = new StreamReader($"{_bucketDirectory}\\{bucket.Key}", GetFileStreamOptionsForRead()))
                {
                    do
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            break;

                        var record = new Record(line);
                        records.Add(record);
                    } while (true);
                }

                records.Sort(new RecordComparer());

                using (var sw = new StreamWriter($"{_bucketDirectory}\\{bucket.Key}", GetFileStreamOptionsForWrite()))
                {
                    foreach (var record in records)
                    {
                        sw.Write(record.number);
                        sw.Write(". ");
                        sw.WriteLine(record.text);
                    }
                }
                
                Console.WriteLine($"Sorted {++count} buckets of {buckets.Count}");
            }

            //stopWatch.Stop();
            //Console.WriteLine($"Sorting take {stopWatch.ElapsedMilliseconds} ms");

            //------concat all bucket files------
            var resultStream = new StreamWriter($"{outputFile}", GetFileStreamOptionsForWrite());
            count = 0;

            //stopWatch.Restart();
            foreach (var bucketkey in buckets.Keys)
            {
                //stopWatch.Restart();
                using (var sr = new StreamReader($"{_bucketDirectory}\\{bucketkey}", GetFileStreamOptionsForRead()))
                {
                    do
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            break;

                        resultStream.WriteLine(line);
                    } while (true);
                }

                Console.WriteLine($"Merged {++count} buckets of {buckets.Count}");
            }

            //stopWatch.Stop();
            //Console.WriteLine($"Merge take {stopWatch.ElapsedMilliseconds} ms");

            //------clean------
            resultStream.Dispose();

            var di = new DirectoryInfo(_bucketDirectory);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            Directory.Delete(_bucketDirectory);
        }

        private static void AddRecordToBucket(string row)
        {
            var id = GetBucketId(row);

            if (buckets.ContainsKey(id))
            {
                buckets[id].WriteLine(row);
                return;
            }

            var sw = new StreamWriter($"{_bucketDirectory}\\{id}", GetFileStreamOptionsForWrite());
            sw.WriteLine(row);

            buckets.Add(id, new DictionaryValue(sw));
        }

        private static string GetBucketId(string text)
        {
            return text.Substring(text.IndexOf(' ') + 1, 2).ToUpper();
        }

        private static FileStreamOptions GetFileStreamOptionsForWrite()
        {
            return new FileStreamOptions()
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.None,
                //Options = FileOptions.SequentialScan
                //BufferSize = 128
            };
        }

        private static FileStreamOptions GetFileStreamOptionsForRead()
        {
            return new FileStreamOptions()
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.None,
                Options = FileOptions.SequentialScan,
                //BufferSize = 128
            };
        }
    }
}
