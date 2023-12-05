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
        return (int)Math.Pow(2, matchCount -1);
    }

    private static int GetMatchCount(string line)
    {
        var (set0, set1) = ParseLine(line);
        return set0.Intersect(set1).Count();
    }

    private static (IEnumerable<int>, IEnumerable<int>) ParseLine(string line)
    {
        // parse `Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53`
        var data = line.Split(':')[1];
        var split = data.Split('|');
        var set0 = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        var set1 = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        return (set0, set1);
    }

    [Result(6050769)]
    [TestCase(result: 30)]
    public static int GetAnswer2(string[] input)
    {
        var total = 0;
        var matchCounts = input.Select(GetMatchCount).ToArray();
        var copyCount = Enumerable.Repeat(1, input.Length).ToArray();
        
        for (int i = 0; i < input.Length; i++)
        {
            var numberOfCopiesCurrentCard = copyCount[i];
            var numberOfCardsToCopy = matchCounts[i];
            total += numberOfCopiesCurrentCard;

            for (int j = i + 1; j <= i + numberOfCardsToCopy && j < input.Length; j++)
            {
                copyCount[j] += numberOfCopiesCurrentCard;
            }
        }

        return total;
    }
}