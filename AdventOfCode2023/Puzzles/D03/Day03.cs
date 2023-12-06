namespace AdventOfCode2023.Puzzles.D03;

public class Day03
{
    [Result(531561)]
    [TestCase(result: 4361)]
    public static int GetAnswer1(string[] input)
    {
        var total = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var currentNumber = 0;
            var start = -1;
            int j;
            for (j = 0; j < input[i].Length; j++)
            {
                var c = input[i][j];
                var currentHasSymbol = HasSymbol(input, i, j);
                
                if (char.IsAsciiDigit(c))
                {
                    if (currentNumber == 0) start = j;
                    currentNumber *= 10;
                    currentNumber += c- '0';
                }
                else
                {
                    if (currentNumber > 0 && FindSymbol(input, i, start - 1, j + 1))
                    {
                        total += currentNumber;
                    }
                    currentNumber = 0;
                }
            }
            if (currentNumber > 0 && FindSymbol(input, i, start - 1, j + 1))
            {
                total += currentNumber;
            }
        }

        return total;
    }

    static bool FindSymbol(string[] input, int i, int start, int end)
    {
        for (int j = start == -1 ? 0 : start; j<input[i].Length && j < end; j++)
        {
            if (HasSymbol(input, i, j)) return true;
        }

        return false;
    }
    
    static bool HasSymbol(string[] input, int i, int j)
    {
        var c = input[i][j];
        var cAbove = i>0 ? input[i - 1][j] : '.';
        var cBelow = i + 1 < input.Length ? input[i + 1][j] : '.';
        return IsSymbol(c) || IsSymbol(cAbove) || IsSymbol(cBelow);
    }

    static bool IsSymbol(char c) => !char.IsAsciiDigit(c) && c != '.';

    
    [Result(83279367)]
    [TestCase(result: 467835)]
    public static int GetAnswer2(string[] input)
    {
        var sum = 0;
        var schematic = new Schematic(input);
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[i].Length; j++)
            {
                if (schematic[i, j] == '*')
                {
                    sum += GetRatio(schematic, i, j);
                }
            }
        }

        return sum;
    }

    private static int GetRatio(Schematic input, int i, int j)
    {
        var found = EnumerateSurroundingNumbers(input, i, j);

        // Stop searching after 3 numbers are found 
        var matches = found.OfType<int>().Take(3).ToList(); 
        if (matches.Count == 2)
        {
            return matches[0] * matches[1];
        }

        return 0;
    }

    private static IEnumerable<int?> EnumerateSurroundingNumbers(Schematic input, int i, int j)
    {
        yield return FindNumberAtPosition(input, i, j - 1);
        yield return FindNumberAtPosition(input, i, j + 1);
        var aboveNumber = FindNumberAtPosition(input, i - 1, j);
        var belowNumber = FindNumberAtPosition(input, i + 1, j);
        yield return aboveNumber;
        yield return belowNumber;

        if (belowNumber == null)
        {
            // only if there is no number directly below this * we look diagonally below
            yield return FindNumberAtPosition(input, i + 1, j - 1);
            yield return FindNumberAtPosition(input, i + 1, j + 1);
        }

        if (aboveNumber == null)
        {
            // only if there is no number directly below this * we look diagonally above
            yield return FindNumberAtPosition(input, i - 1, j + 1);
            yield return FindNumberAtPosition(input, i - 1, j - 1);
        }
    }    
    
    public static int? FindNumberAtPosition(Schematic input, int i, int j)
    {
        // check left of current *
        var scanPosition = j;
        var c = input[i, scanPosition--];
        if (!char.IsAsciiDigit(c)) return null;
        var factor = 1;
        int number = 0;
        while (char.IsAsciiDigit(c))
        {
            // while reading right to left each next digit is a factor 10 higher
            number += (c - '0') * factor;
            factor *= 10;
            c = input[i, scanPosition--];
        }
        
        scanPosition = j + 1;

        c = input[i, scanPosition++];
        while (char.IsAsciiDigit(c))
        {
            // while reading left to right each next digit means we need to multiply all previous read by 10  
            number *= 10;
            number += c - '0';
            c = input[i, scanPosition++];
        }        

        return number;
    }

    public class Schematic(string[] input)
    {
        public char this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= input.Length || j < 0 || j >= input[i].Length) return '.';
                return input[i][j];
            }
        }
    }
}