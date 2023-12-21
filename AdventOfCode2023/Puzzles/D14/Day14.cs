namespace AdventOfCode2023.Puzzles.Day14;

/// <summary>
/// https://adventofcode.com/2023/day/xx
/// </summary>
public class Day14
{
    [Result(108614)]
    [TestCase(result: 136)]
    public static int GetAnswer1(string[] input)
    {
        var score = 0;
        for (var x = 0; x < input[0].Length; x++)
        {
            var firstFree = 0;
            for (var y = 0; y < input.Length; y++)
            {
                switch (input[y][x])
                {
                    case '#':
                        firstFree = y + 1;
                        break;
                    case 'O':
                        score += input.Length - firstFree;
                        firstFree++;
                        break;
                }
            }
        }

        return score;
    }

    [Result(96447)]
    [TestCase(result: 64)]
    public static long GetAnswer2(string[] input)
    {
        int targetIteration = 1_000_000_000;
        var array = BuildArray(input);

        // Keep track of each configuration we have seen before at which cycle
        var cycleNumberByConfiguration = new Dictionary<char[,], int>(new ArrayComparer());
        var weights = new List<char[,]>();

        var tempBuffer = new char[array.GetLength(0), array.GetLength(1)];

        for (int i = 0; i < targetIteration; i++)
        {
            weights.Add(array);
            if (!cycleNumberByConfiguration.TryAdd(array, i))
            {
                // We have seen this exact configuration before so we found a cycle of the pattern 

                var startCycle = cycleNumberByConfiguration[array];
                var correspondingIteration = FindCorrespondingIteration(startCycle, i, targetIteration);
                return GetWeight(weights[correspondingIteration]);
            }

            array = Cycle(array, tempBuffer);
        }

        return GetWeight(array);
    }

    private static char[,] BuildArray(string[] input)
    {
        var array = new char[input[0].Length, input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[i].Length; j++)
            {
                array[j, i] = input[i][j];
            }
        }

        return array;
    }

    public static int FindCorrespondingIteration(int startCycle, int endCycle, int targetIteration)
    {
        var cycleLength = endCycle - startCycle;
        var offset = (targetIteration - startCycle) % cycleLength;
        return startCycle + offset;
    }

    static int GetWeight(char[,] input)
    {
        var score = 0;
        for (var i = 0; i < input.GetLength(0); i++)
        {
            for (var j = 0; j < input.GetLength(1); j++)
            {
                if (input[j, i] == 'O') score += input.GetLength(0) - i;
            }
        }

        return score;
    }

    static char[,] Cycle(char[,] input, char[,] tempBuffer)
    {
        // We create one new buffer for the result because we do not want to overwrite the input buffer.
        // For the intermediate results we do re-use the provided temp buffer
        var newBuffer = new char[input.GetLength(0), input.GetLength(1)];
        TiltAndTurn(input, tempBuffer);
        TiltAndTurn(tempBuffer, newBuffer);
        TiltAndTurn(newBuffer, tempBuffer);
        TiltAndTurn(tempBuffer, newBuffer);
        return newBuffer;
    }

    static void TiltAndTurn(char[,] input, char[,] output)
    {
        for (var x = 0; x < input.GetLength(0); x++)
        {
            var firstFreeY = 0;
            for (var y = 0; y < input.GetLength(1); y++)
            {
                switch (input[x, y])
                {
                    case '#':
                        SetTransposed(x, y, '#');
                        firstFreeY = y + 1;
                        break;
                    case 'O':
                        SetTransposed(x, y, '.');
                        SetTransposed(x, firstFreeY, 'O');
                        firstFreeY++;
                        break;
                    default:
                        SetTransposed(x, y, '.');
                        break;
                }
            }
        }

        // While filling the output array we also turn it at the same time to avoid
        void SetTransposed(int x, int y, char c) => output[output.GetLength(0) - y - 1, x] = c;
    }

    class ArrayComparer : IEqualityComparer<char[,]>
    {
        public bool Equals(char[,]? x, char[,]? y)
        {
            for (var i = 0; i < x!.GetLength(0); i++)
            {
                for (var j = 0; j < x.GetLength(1); j++)
                {
                    if (x[i, j] != y![i, j]) return false;
                }
            }

            return true;
        }

        public int GetHashCode(char[,] obj) => GetWeight(obj);
    }
}