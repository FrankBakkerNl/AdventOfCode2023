namespace AdventOfCode2023.Puzzles.Day09;

/// <summary>
/// https://adventofcode.com/2023/day/09
/// </summary>
public class Day09
{
    [Result(1992273652)]
    [TestCase(result: 114)]
    public static int GetAnswer1(string[] input) => input.Select(NextValueForLine).Sum(v => v.next);

    [Result(1012)]
    [TestCase(result: 2)]
    public static int GetAnswer2(string[] input) => input.Select(NextValueForLine).Sum(v => v.preceding);

    static (int preceding, int next) NextValueForLine(string line)
    {
        var values = ParseLine(line);
        var lines = new int[values.Length][];
        lines[0] = values;
        var lineCount = 1;
        
        while (values.Any(v => v != 0))
        {
            values = GetDiffs(values);
            lines[lineCount++]=values;
        }

        var preceding = 0;
        var next = 0;
        while (lineCount-- > 0)
        {
            next += lines[lineCount][^1];
            preceding = lines[lineCount][0] - preceding;
        }

        return (preceding, next);
    }
    
    static int[] GetDiffs(int[] input)
    {
        var result = new int[input.Length - 1];
        var previous = input[0];
        for (var i = 1; i < input.Length; i++)
        {
            var current = input[i];
            result[i - 1] = current - previous;
            previous = current;
        }

        return result;
    }

    static int[] ParseLine(string line) 
        => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
}