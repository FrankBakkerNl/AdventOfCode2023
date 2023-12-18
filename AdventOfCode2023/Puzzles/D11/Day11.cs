namespace AdventOfCode2023.Puzzles.Day11;

/// <summary>
/// https://adventofcode.com/2023/day/11
/// </summary>
public class Day11
{
    [Result(9556896)]
    [TestCase(result: 374)]
    public static long GetAnswer1(string[] input)
    {
        return CalculateTotalDistances(input, 1);
    }
    
    [Result(685038186836)]
    //[TestCase(result: 8410)] testcase requires expansionFactor to be changed
    public static long GetAnswer2(string[] input)
    {
        return CalculateTotalDistances(input, 999_999);
    }

    private static long CalculateTotalDistances(string[] input, int expansionFactor)
    {
        var (galaxies, galaxyInRow, galaxyInColumn) = ScanInput(input);
        long total = 0;
        for (var i = 0; i + 1 < galaxies.Length; i++)
        {
            for (var j = i+1; j < galaxies.Length; j++)
            {
                total += (galaxies[i] - galaxies[j]).Distance;
                total += expansionFactor * Expansion(galaxyInRow, galaxies[i].Y, galaxies[j].Y);
                total += expansionFactor * Expansion(galaxyInColumn, galaxies[i].X, galaxies[j].X);
            }
        }
        return total;
    }

    static int Expansion(bool[] occupationMap, int start, int end)
    {
        // Swap if needed
        if (start > end) (end, start) = (start, end);

        var count = 0;
        for (int i = start +1; i < end; i++)
        {
            if (!occupationMap[i]) count++;
        }

        return count;
    }

    static (Vector[] galaxies, bool[] galaxyInRow, bool[] galaxyInColumn) ScanInput(string[] input)
    {
        List<Vector> galaxies = new List<Vector>();
        var galaxyInColumn = new bool[input[0].Length];
        var galaxyInRow = new bool[input.Length];
        
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[i].Length; j++)
            {
                if (input[i][j] == '#')
                {
                    galaxies.Add(new(i,j));
                    galaxyInColumn[j] = true;
                    galaxyInRow[i] = true;
                }
            }
        }

        return (galaxies.ToArray(), galaxyInRow, galaxyInColumn);
    }
    
    public readonly record struct Vector(int Y, int X)
    {
        public static Vector operator-(Vector a, Vector b) => new(b.Y - a.Y, b.X - a.X);
        public int Distance => Y + Abs(X);
    }
}