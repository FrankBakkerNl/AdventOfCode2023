namespace AdventOfCode2023.Puzzles.Day08;

/// <summary>
/// https://adventofcode.com/2023/day/8
/// </summary>
public class Day08
{
    [Result(14893)]
    [TestCase(result: 6)]
    public static int GetAnswer1(string[] input)
    {
        var (pattern, current, zIndex, leftMap, rightMap) = ParseInput(input);

        int steps;
        for (steps = 0; current != zIndex; steps++)
        {
            // We need to wrap around the pattern when at the end, so we can modulo the steps to get the correct index 
            current = pattern[steps % pattern.Length] == 'L' ? leftMap[current] : rightMap[current];
        }
        
        return steps;
    }

    static (string pattern, int aIndex, int zIndex, int[] leftMap, int[] rightMap) ParseInput(string[] input)
    {
        // Map the strings to integers so we can do fast array indexing
        var indexFromCode = new Dictionary<string, int>();

        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            indexFromCode.Add(line[..3], i);
        }

        // Now build two arrays for left and right
        var leftMap = new int [input.Length - 2]; 
        var rightMap = new int [input.Length - 2]; 

        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            leftMap[i] = indexFromCode[line[7..10]];
            rightMap[i] = indexFromCode[line[12..15]];
        }

        return (input[0], indexFromCode["AAA"], indexFromCode["ZZZ"], leftMap, rightMap);
    }

}