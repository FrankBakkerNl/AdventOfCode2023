using System.Buffers;

namespace AdventOfCode2023.Puzzles.D01;

public class Day01Optimized
{
    [Result(54561)]
    [TestCase(result:142)]
    public static int GetAnswer1(string[] input) => input.Sum(GetCorrection);

    private static int GetCorrection(string line)
    {
        var first = FirstDigit(line);
        var second = LastDigit(line);
        
        return first * 10 + second;
    }

    private static int FirstDigit(string input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];
            if (char.IsAsciiDigit(currentChar)) return currentChar - '0';
        }

        throw new InvalidOperationException("Not found");
    }    
    
    private static int LastDigit(string input)
    {
        for (var i = input.Length - 1; i >= 0; i--)
        {
            var currentChar = input[i];
            if (char.IsAsciiDigit(currentChar)) return currentChar - '0';
        }

        throw new InvalidOperationException("Not found");
    }    

    [Result(54076)]
    [TestCase(result:281)]
    public static int GetAnswer2(string[] input) => input.Sum(GetCorrection2);

    private static int GetCorrection2(string line)
    {
        var first = FirstMatch(line);
        var second = LastMatch(line);

        return first * 10 + second;
    }

      
    private static int FirstMatch(string input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];
            if (char.IsAsciiDigit(currentChar)) return currentChar - '0';
            if (!InitialChars.Contains(currentChar)) continue;
            
            foreach (var (match, value) in WordMap)
            {
                if (input.Length >= i + match.Length && input.AsSpan(i, match.Length).SequenceEqual(match))
                {
                    return value;
                }
            }
        }

        return 0;
    }
    
    private static int LastMatch(string input)
    {
        for (var i = input.Length - 1; i >= 0; i--)
        {
            var currentChar = input[i];
            if (char.IsAsciiDigit(currentChar)) return currentChar - '0';
            if (!InitialChars.Contains(currentChar)) continue;
            
            foreach (var (match, value) in WordMap)
            {
                if (i + match.Length <= input.Length && input.AsSpan(i, match.Length).SequenceEqual(match))
                {
                    return value;
                }
            }
        }

        return 0;
    }

    private static readonly SearchValues<char> InitialChars = SearchValues.Create("otfsen");
    private static readonly (string w, int)[] WordMap = BuildMap();
    
    private static (string match, int value)[] BuildMap()
    {
        var wordsDigits = new[] {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
           
        return wordsDigits.Select((w, i) => (w, i + 1)).ToArray();
    }
}