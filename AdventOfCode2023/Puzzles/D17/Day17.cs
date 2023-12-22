using System.Diagnostics;

namespace AdventOfCode2023.Puzzles.Day17;

/// <summary>
/// https://adventofcode.com/2023/day/17
/// </summary>
public class Day17
{
    [Result(1155)]
    [TestCase(result: 102)]
    [Focus]
    public static int GetAnswer1(string[] input)
    {
        var field = GetField(input);
        return FindPath(field, new Walker(new Vector(0, 0), new Vector(0, 0), 0, 0));
    }

    static int[,] GetField(string[] input)
    {
        var result = new int[input[1].Length,input.Length];
        for (var y = 0; y < input.Length; y++)
        {
            for (var x = 0; x < input[y].Length; x++)
            {
                result[x, y] = input[y][x] - '0';
            }
        }

        return result;
    }

    static int FindPath(int[,] field, Walker start)
    {
        var fieldSize = field.GetLength(0);
        var endPoint = new Vector(field.GetUpperBound(1), field.GetUpperBound(0));
        var visitHistory = new Dictionary<(Vector, int, Vector), int>();
        
        var queue = new PriorityQueue<Walker, int>();
        var best = int.MaxValue;
        
        queue.Enqueue(start, 0);

        Span<Walker> sharedBuffer = new Walker[3];

        while (queue.TryDequeue(out var current, out _))
        {
            if (current.Position == endPoint)
            {
                best = Min(best, current.HeatLoss);
                continue;
            }
            
            var newWalkers = AllowedSteps(current, fieldSize, field, sharedBuffer);
            foreach (var newWalker in newWalkers)
            {
                // do not continue on a path that looses more heat than the best found yet
                if (newWalker.HeatLoss >= best) continue;
                
                if (visitHistory.TryGetValue((newWalker.Position, newWalker.StepsInDirection, newWalker.Direction), out var previousVisit) 
                    && previousVisit <= newWalker.HeatLoss) continue;
                
                visitHistory[(newWalker.Position, newWalker.StepsInDirection, newWalker.Direction)]  = newWalker.HeatLoss;
                
                var taxiDistanceTravelled = newWalker.Position.X + newWalker.Position.Y;
                    
                queue.Enqueue(newWalker, newWalker.HeatLoss - (5* taxiDistanceTravelled));
            }
        }

        return best;
    }

    private static bool IsOnMap(Vector walker, int size) =>
        0 <= walker.X && walker.X < size && 
        0 <= walker.Y && walker.Y < size;

    static Span<Walker> AllowedSteps(Walker walker, int size, int[,] field, Span<Walker> output)
    {
        var count = 0;
        foreach (var move in Moves)
        {
            var newPosition = walker.Position + move;
            if (!IsOnMap(newPosition, size)) continue;

            if (move == walker.Direction)
            {
                // only continue in current direction if steps left
                if (walker.StepsInDirection < 3)
                {
                    output[count++] = walker with
                    {
                        Position = newPosition, StepsInDirection = walker.StepsInDirection + 1, HeatLoss = walker.HeatLoss + field[newPosition.X, newPosition.Y]
                    };
                }
            }
            else if (move != walker.Direction.GetOpposite())
            {
                output[count++] = new Walker(Position: newPosition, Direction: move, StepsInDirection: 1, HeatLoss: walker.HeatLoss + field[newPosition.X, newPosition.Y]);
            }
        }

        return output[..count];
    }

    static readonly Vector[] Moves = { new(0, -1), new (1, 0), new(0, 1), new(-1, 0) };
    
    public record struct Vector(int Y, int X)
    {
        public Vector GetOpposite() => new (-Y, -X);
        public static Vector operator+(Vector a, Vector b) => new(a.Y + b.Y, a.X + b.X);
        public static Vector operator-(Vector a, Vector b) => new(b.Y - a.Y, b.X - a.X);
    }
    
    readonly record struct Walker(Vector Position, Vector Direction, int StepsInDirection, int HeatLoss);

    //[Result(0)]
    [TestCase(result: 0)]
    public static long GetAnswer2(string[] input)
    {
        return 0;
    }
}