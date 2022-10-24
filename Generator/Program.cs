const long size = 10L * 1024 * 1024 * 1024;
const string filePath = @"D:\1\gen.txt";

var random = new Random();

using (var sw = new StreamWriter(filePath))
{
    int count = 0;
    long currentSize = 0;
    do
    {
        var row = $"{random.Next(1, 1000)}. {RandomString(4)}";
        sw.WriteLine(row);

        currentSize += row.Length + 2;

        if(++count % 100000 == 0)
        {
            Console.WriteLine($"{count} created, {((float)currentSize * 100 / size).ToString("0.00")} %");
        }

    }
    while (currentSize < size);

    Console.WriteLine($"Finished with {count} records");

    Console.ReadKey();
}

string RandomString(int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}