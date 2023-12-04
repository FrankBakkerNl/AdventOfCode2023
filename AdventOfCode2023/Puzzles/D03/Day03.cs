namespace AdventOfCode2023.Puzzles.D03;

public class Day03
{
    [Focus]
    [Result(531561)]
    [TestCase(result: 4361)]
    public static long GetAnswer1(string[] input)
    {
        var total = 0l;
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
                    if (currentNumber > 0 && FindSymbol(input, i, start-1, j+1))
                    {
                        total += currentNumber;
                    }
                    currentNumber = 0;
                }
            }
            if (currentNumber > 0 && FindSymbol(input, i, start-1, j+1))
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



    // [Result(2829354)]
    // [TestCase(result: 230)]
    // public static int GetAnswer2(string[] input)
    // {
    //     
    // }
}