namespace AdventOfCode2023.Puzzles.Day13;

/// <summary>
/// https://adventofcode.com/2023/day/D13
/// </summary>
public class Day13
{
    [Result(31265)]
    [TestCase(result: 405)]
    public static int GetAnswer1(string[] input)
    {
        var patterns = ParsePatterns(input);
        return patterns.Sum(v => FindMirrorValue(v, -1));
    }

    [Result(39359)]
    [TestCase(result: 400)]
    public static long GetAnswer2(string[] input)
    {
        var patterns = ParsePatterns(input);
        return patterns.Sum(FindAlternativeMirror);    
    }
    
    private static int FindMirrorValue(Pattern pattern, int ignore)
    {
        var result = FindMirror(pattern.Rows, ignore / 100) * 100;
        return result >0 ? result : FindMirror(pattern.Columns, ignore % 100);
    }
    
    public static int FindMirror(int[] input, int ignore)
    {
        var result = FindMirrorBeforeMidpoint(input, ignore);
        if (result > 0) return result;
        
        // reverse and search again
        result = FindMirrorBeforeMidpoint(input.Reverse().ToArray(), input.Length - ignore);
        return result > 0 ? input.Length - result : -1;
    }
    
    public static int FindMirrorBeforeMidpoint(int[] input, int ignore)
    {
        // only search until we get half way, we will get back later for the other side
        for (var i = 0; i < input.Length / 2 ; i++)
        {
            if (input[i] == input[i + 1])
            {
                // found two equal neighbours, check outwards to see if all others match
                for (int j = 1; j <= i; j++)
                {
                    if (input[i - j] != input[i + j + 1]) goto tryNext;
                }
                if (i +1 != ignore) return i + 1;
            }
            tryNext: ;
        }

        return -1;
    }


    static int FindAlternativeMirror(Pattern pattern)
    {
        int originalPosition = FindMirrorValue(pattern, -1);
        for (int i = 0; i < pattern.Rows.Length; i++)
        {
            for (var j = 0; j < pattern.Columns.Length; j++)
            {
                // flip one bit
                pattern.Columns[j] ^= 1 << i;
                pattern.Rows[i] ^= 1 << j;

                var value = FindMirrorValue(pattern, originalPosition);

                // flip it back
                pattern.Columns[j] ^= 1 << i;
                pattern.Rows[i] ^= 1 << j;
                
                if (value >= 0 && value != originalPosition) return value;
            }
        }

        return -1;
    }

    record struct Pattern(int[] Columns, int[] Rows);
    
    static IEnumerable<Pattern> ParsePatterns(string[] input)
    {
        int start = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var s = input[index];
            if (string.IsNullOrEmpty(s))
            {
                yield return ParsePattern(input[start..index]);
                start = index + 1;
            }
        }

        yield return ParsePattern(input[start..]);
    }

    static Pattern ParsePattern(Span<string> input)
    {
        var result = new Pattern(new int[input[0].Length], new int[input.Length]);
        
        for (var i = 0; i < input.Length; i++)
        {
            var line = input[i];
            for (var j = 0; j < new int[input[0].Length].Length; j++)
            {
                if (line[j] == '#')
                {
                    result.Columns[j] |= 1 << i;
                    result.Rows[i] |= 1 << j;
                }
            }
        }
        
        return result;
    }
}