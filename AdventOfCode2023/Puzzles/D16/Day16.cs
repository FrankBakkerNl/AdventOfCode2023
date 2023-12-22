using System.Diagnostics;

namespace AdventOfCode2023.Puzzles.Day16;

/// <summary>
/// https://adventofcode.com/2023/day/16
/// </summary>
public class Day16
{
    [Result(8249)]
    [TestCase(result: 46)]
    public static int GetAnswer1(string[] input)
    {
        var seenBeams = new HashSet<Beam>();
        var backlog = new Stack<Beam>();
        var startBeam = new Beam(0, 0, Direction.East);
        (startBeam,  _) = Reflect(startBeam, input);
        
        backlog.Push(startBeam);
        seenBeams.Add(startBeam);
        
        while (backlog.TryPop(out var currentBeam))
        {
            currentBeam = currentBeam.Step();
            while (IsOnMap(currentBeam, input) && seenBeams.Add(currentBeam))
            {
                (currentBeam, var other) = Reflect(currentBeam, input);
                
                if (other.HasValue) backlog.Push(other.Value);
                currentBeam = currentBeam.Step();
            }
        }

        return seenBeams.DistinctBy(b => (b.X, b.Y)).Count();
    }

    private static bool IsOnMap(Beam currentBeam, string[] input) =>
        0 <= currentBeam.X && currentBeam.X < input[1].Length && 
        0 <= currentBeam.Y && currentBeam.Y < input.Length;

    static (Beam, Beam?) Reflect(Beam beam, string[] input)
    {
        return input[beam.Y][beam.X] switch
        {
            '|' when beam.Direction is Direction.East or Direction.West
                => (beam with { Direction = Direction.North }, 
                    beam with { Direction = Direction.South }),

            '-' when beam.Direction is  Direction.North or Direction.South
                => (beam with { Direction = Direction.East }, 
                    beam with { Direction = Direction.West }),

            '/' => (beam with
            {
                Direction = beam.Direction switch
                {
                    Direction.North => Direction.East,
                    Direction.East => Direction.North,
                    Direction.South => Direction.West,
                    Direction.West => Direction.South,
                    _ => throw new UnreachableException()
                }
            }, null),

            '\\' => (beam with
            {
                Direction = beam.Direction switch
                {
                    Direction.North => Direction.West,
                    Direction.East => Direction.South,
                    Direction.South => Direction.East,
                    Direction.West => Direction.North,
                    _ => throw new UnreachableException()
                }
            }, null),
            
            _ => (beam, null)
        };
    }

    record struct Beam(int X, int Y, Direction Direction)
    {
        private static readonly (int X, int Y)[] Moves = { (0, -1), (1, 0), (0, 1), (-1, 0) };
        
        public Beam Step()
        {
            var delta = Moves[(int)Direction];
            return new (X + delta.X, Y + delta.Y, Direction);
        }
    };
    
    enum Direction { North = 0, East = 1, South = 2, West = 3 }


    //[Result(0)]
    [TestCase(result: 0)]
    [Focus]
    public static long GetAnswer2(string[] input)
    {
        return 0;
    }
}