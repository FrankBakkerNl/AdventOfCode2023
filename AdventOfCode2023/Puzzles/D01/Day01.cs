namespace AdventOfCode2023.Puzzles.D01;

public class Day01
{
    private static readonly char[] DigitChars = "1234567890".ToCharArray();

    [Result(54561)]
    [TestCase(result:142)]
    public static int GetAnswer1(string[] input) => input.Sum(GetCorrection);

    private static int GetCorrection(string line)
    {
        var firstindex = line.IndexOfAny(DigitChars);
        var secondIndex = line.LastIndexOfAny(DigitChars);
        return int.Parse($"{line[firstindex]}{line[secondIndex]}");
    }

    [Result(54076)]
    [TestCase(result:281)]
    public static int GetAnswer2(string[] input) => input.Sum(GetCorrection2);

    private static int GetCorrection2(string line)
    {
        var first = FindFirst(line);
        var second = FindLast(line);
        return first * 10 + second;
    }

    private static readonly (string match, int value)[] Map = BuildMap();
        
    private static int FindFirst(string line) =>
        Map
            .Select(w => (Index: line.IndexOf(w.match, StringComparison.Ordinal), w.value))
            .Where(t=>t.Index >=0)
            .MinBy(t => t.Index).value;

    private static int FindLast(string line) =>
        Map
            .Select(w => (Index: line.LastIndexOf(w.match, StringComparison.Ordinal), w.value))
            .Where(t=>t.Index >=0)
            .MaxBy(t => t.Index).value;

    private static (string match, int value)[] BuildMap()
    {
        var wordsDigits = new[] {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var charDgits = "0123456789";
            
        var wordMap = wordsDigits.Select((w, i) => (w, i + 1));
        var charMap = charDgits.Select((c, i) => (c.ToString(), i));
        return charMap.Union(wordMap).ToArray();
    }
}