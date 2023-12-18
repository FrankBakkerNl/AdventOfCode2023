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
        var walker = new Walker(beastLocation, GetFirstStartDirection(map, beastLocation));

        var distance = 0;
        do
        {
            walker = map.Step(walker);
            distance++;
        } while (walker.Vector != beastLocation);

        return distance /2;
    }
    
    [Result(445)]
    [TestCase(result: 10)]
    public static long GetAnswer2(string[] input)
    {
        var beastLocation = GetBeastPosition(input);
        ReplaceBeast(input, beastLocation);
        var mainLoopMap = FindMainLoop(input, beastLocation);

        return input.Select((s, i) => CountTilesInLine(s, mainLoopMap, i)).Sum();
    }

    private static void ReplaceBeast(string[] input, Vector beastLocation)
    {
        var map = new Map(input);
        var openings = GetStartDirections(map, beastLocation).ToArray();

        // Find a type of pipe that will fit here
        var replaceChar = _pipeTypes
            .Single(p => openings.Contains(p.Value.Opening1) && openings.Contains(p.Value.Opening2)).Key;
        input[beastLocation.Y] = input[beastLocation.Y].Replace('S', replaceChar);
    }
    
    static bool[,] FindMainLoop(string[] input, Vector beastLocation)
    {
        var result = new bool[input.Length, input[0].Length];
        var map = new Map(input);
        var walker = new Walker(beastLocation, GetFirstStartDirection(map, beastLocation));
        
        do
        {
            walker = map.Step(walker);
            result[walker.Vector.Y, walker.Vector.X] = true;
        } while (walker.Vector != beastLocation);

        return result;
    }

    static int CountTilesInLine(string line, bool[,] mainLoopMap, int y)
    {
        int numInsideCells = 0;
        bool inside = false;
        char startHlineChar = (char)0;
        
        // scan a horizontal line, on the left we will be outside the main loop. Every time we cross the main loop
        // we will change between inside and outside the loop. Only inside the loop any non-loop tile will count

        for (var x = 0; x < line.Length; x++)
        {
            var c = line[x];
            if (!mainLoopMap[y, x])
            {
                if (inside) numInsideCells++;
            }

            else if (c is '|') inside = !inside;

            else if (c is 'L' or 'F') startHlineChar = c;

            else if ((startHlineChar, c) is ('L', '7') or ('F', 'J'))
            {
                // we crossed a horizontal loop segment that looked like 'L--7'  or 'F---J' meaning we changed sides
                inside = !inside;
            }
        }

        return numInsideCells;
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
    
    private static Direction GetFirstStartDirection(Map map, Vector start) => GetStartDirections(map, start).First();

    private static IEnumerable<Direction> GetStartDirections(Map map, Vector start)
    {
        return Enum.GetValues<Direction>().Where(direction =>
        {
            var vector = GetMoveOffset(direction);
            var pipe = map.TryGetPipe(start + vector);
            return pipe?.CanEnter(Opposite(direction)) ?? false;
        });
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

    static Dictionary<char, Pipe> _pipeTypes = new ()
    {
        ['|'] = new (North, South),
        ['-'] = new (East,  West),
        ['L'] = new (North, East),
        ['J'] = new (West, North),
        ['7'] = new (South, West),
        ['F'] = new (East, South),
        ['S'] = new (0, 0), // use a dummy for the start
    };
    
    class Map(string[] input)
    {
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
    }
    
    public record struct Vector(int Y, int X)
    {
        public static Vector operator+(Vector a, Vector b) => new(a.Y + b.Y, a.X + b.X);
    }

    readonly record struct Pipe(Direction Opening1, Direction Opening2)
    {
        public bool CanEnter(Direction side) => Opening1 == side || Opening2 == side;
        
        public Walker Turn(Walker incoming) => incoming with { Direction = Opposite(incoming.Direction) == Opening1 ? Opening2 : Opening1};
    }
}