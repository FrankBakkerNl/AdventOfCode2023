namespace AdventOfCode2023.Puzzles.Day14;

/// <summary>
/// https://adventofcode.com/2023/day/xx
/// </summary>
public class Day14
{
    //[Result(0)]
    [TestCase(result: 136)]
    [Focus]
    public static int GetAnswer1(string[] input)
    {
        var score = 0;
        Span<int> firstFreeCell = stackalloc int[input[0].Length];
        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] is '#')
                {
                    firstFreeCell[x] = y + 1;
                }
                else if (line[x] is 'O')
                {
                    score += input.Length - firstFreeCell[x];
                    firstFreeCell[x]++;
                }
            }
        }

        return score;
    }

    //[Result(0)]
    [TestCase(result: 0)]
    public static long GetAnswer2(string[] input)
    {
        return 0;
    }
    
    
}