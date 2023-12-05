using System.Collections.Generic;

namespace AdventOfCode2020.Puzzles.Day04;

public class Day04
{
    [Result(21158)]
    [TestCase(result: 13)]
    public static int GetAnswer1(string[] input) => input.Sum(GetPoints);

    private static int GetPoints(string line)
    {
        var matchCount = GetMatchCount(line);
        return matchCount == 0 ? 0 : 1 << matchCount - 1;
    }

    private static int GetMatchCount(string line)
    {
        var (set0, set1) = ParseLine(line);
        return set0.Intersect(set1).Count();
    }

    private static readonly char[] SplitseSeparator = ":|".ToArray();

    private static (IEnumerable<int>, IEnumerable<int>) ParseLine(string line)
    {
        // parse `Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53`
        var split = line.Split(SplitseSeparator);
        var set0 = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        var set1 = split[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        return (set0, set1);
    }

    [Result(6050769)]
    [TestCase(result: 30)]
    public static int GetAnswer2(string[] input)
    {
        var total = 0;
        var copyCount = new int[input.Length]; // keep track of copies, initialized to 0, so we need to compensate later 
        
        for (int i = 0; i < input.Length; i++)
        {
            var numberOfCopiesCurrentCard = copyCount[i] + 1; // As we only count added copies, add 1 for the original card
            var numberOfCardsToCopy = GetMatchCount(input[i]);
            total += numberOfCopiesCurrentCard;

            for (int j = i + 1; j <= i + numberOfCardsToCopy && j < input.Length; j++)
            {
                copyCount[j] += numberOfCopiesCurrentCard;
            }
        }

        return total;
    }
}