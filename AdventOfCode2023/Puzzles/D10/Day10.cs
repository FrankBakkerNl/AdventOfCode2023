using System.Diagnostics;
using static AdventOfCode2023.Puzzles.Day10.Day10.Direction;

namespace AdventOfCode2023.Puzzles.Day10;

/// <summary>
/// https://adventofcode.com/2023/day/10
/// </summary>
public class Day10
{
    [Result(7145)]
    [TestCase(result: 8)]
    public static int GetAnswer1(string[] input)
    {
        var beastLocation = GetBeastPosition(input);
        var map = new Map(input);
        var (w1, w2) = GetStartWalkers(map, beastLocation);

        var distance = 0;
        while (w1.Vector != w2.Vector || distance == 0)
        {
            w1 = map.Step(w1);
            if (w1.Vector == w2.Vector) return distance;
            w2 = map.Step(w2);
            distance++;
        }
        
        return distance;
    }

    static Vector GetBeastPosition(string[] input)
    {
        for (var y = 0; y < input.Length; y++)
        {
            var x = input[y].IndexOf('S');
            if (x >= 0) return new (y, x);
        }

        throw new UnreachableException();
    }
    
    static (Walker, Walker) GetStartWalkers(Map map, Vector start)
    {
        var starts = Enum.GetValues<Direction>().Select(direction =>
        {
            var vector = GetMoveOffset(direction);
            var pipe = map.TryGetPipe(start + vector);
            if (pipe?.CanEnter(Opposite(direction)) ?? false)
            {
                return (Walker?)new Walker(start, direction);
            }

            return null;
        }).OfType<Walker>().ToArray();
        return (starts[0], starts[1]);
    }

    static Direction Opposite(Direction direction) => (Direction)(((int)direction + 2) % 4);
    
    static Vector GetMoveOffset(Direction direction) =>
        direction switch
        {
            North => new (-1,  0),
            East =>  new ( 0,  1),
            South => new ( 1,  0),
            West =>  new ( 0, -1),
            _ => throw new UnreachableException()
        };

    public enum Direction { North, East, South, West }
    
    class Map(string[] input)
    {
        static Dictionary<char, Pipe> _pipeTypes = new ()
        {
            ['|'] = new (North, South),
            ['-'] = new (East,  West),
            ['L'] = new (North, East),
            ['J'] = new (West, North),
            ['7'] = new (South, West),
            ['F'] = new (East, South),
        };

        public Pipe? TryGetPipe(Vector v) => _pipeTypes.GetValueOrDefault(input[v.Y][v.X]);
        public Pipe GetPipe(Vector v) => _pipeTypes[input[v.Y][v.X]];

        public Walker Step(Walker walker)
        {
            walker = walker.Step();
            return GetPipe(walker.Vector).Turn(walker);
        }
    }

    readonly record struct Walker(Vector Vector, Direction Direction)
    {
        public Walker Step() => this with { Vector = Vector + GetMoveOffset(Direction) };
    };
    

    public record struct Vector(int Y, int X)
    {
        public static Vector operator+(Vector a, Vector b) => new(a.Y + b.Y, a.X + b.X);
    };

    readonly record struct Pipe(Direction Opening1, Direction Opening2)
    {
        public bool CanEnter(Direction side) => Opening1 == side || Opening2 == side;
        
        public Walker Turn(Walker incoming) => incoming with { Direction = Opposite(incoming.Direction) == Opening1 ? Opening2 : Opening1};
    }
}