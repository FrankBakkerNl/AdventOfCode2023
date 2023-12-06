namespace AdventOfCode2023.Puzzles.Day05;

/// <summary>
/// https://adventofcode.com/2023/day/6
/// </summary>
public class Day06
{
    [Result(6209190L)]
    [TestCase(result: 288L)]
    public static long GetAnswer1(string[] input)
    {
        var races = GetRaces(input);
        var options = races.Select(r => GetNumberOfOptions(r.time, r.distance));

        return options.Aggregate((x, y) => x * y);
    }

    [Result(28545089L)]
    [TestCase(result: 71503L)]
    public static long GetAnswer2(string[] input)
    {
        var race = GetSingleRace(input);
        var options = GetNumberOfOptions(race.time, race.recordDistance);

        return options;
    }    
    
    static long GetNumberOfOptions(long time, long recordDistance)
    {
        var (min, max) = GetSoltions(time, recordDistance);
        // we need to get the number of integers > min and < max (not counting min and max itself) 
        return  double.IsNaN(max) ? 0 : (long)Ceiling(max - 1) - (long)Floor(min + 1) + 1;
    }
    
    static (double min, double max) GetSoltions(long gameTime, long recordDistance)
    {
        // we need to know for which holdDuration travelDistance > recordDistance

        // Some algebra:
        // travelDistance = travelTime * speed
        // travelTime = gameTime - holdDuration
        // speed = holdDuration
        // travelDistance = (gameTime - holdDuration) * holdDuration
        // TravelDistance > recordDistance
        // (gameTime - HoldDuration) * holdDuration > recordDistance

        // now shuffle this around to format y = ax^2 + bx + c  

        // holdDuration * gameTime - holdDuration^2 > recordDistance
        // holdDuration * gameTime - holdDuration^2 -recordDistance > 0
        // - holdDuration^2 + holdDuration * gameTime  -recordDistance > 0
        // a =-1                            b=gameTime c=-recordDistance
        // Solve for holdDuration using quadratic formula 

        return SolveQuadratic(-1, gameTime, -recordDistance);
    }
    
    static (double r1, double r2) SolveQuadratic(int a, in long b, long c)
    {
        var d = b * b - 4 * a * c;
        
        var root = Sqrt(d);
        var r1 = (-b + root) / (2 * a);
        var r2 = (-b - root) / (2 * a);
        return (r1, r2);
    }

    static (int time, int distance)[] GetRaces(string[] input)
    {
        var times = GetNumbers(input[0].Split(":")[1]);
        var distances = GetNumbers(input[1].Split(":")[1]);
        return times.Zip(distances).Select(t => (t.First, t.Second)).ToArray();
    }
    
    static int[] GetNumbers(string line) => line
        .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).ToArray();

    private static (long time, long recordDistance) GetSingleRace(string[] input)
    {
        var time = long.Parse(input[0].Split(":")[1].Replace(" ", ""));
        var distance = long.Parse(input[1].Split(":")[1].Replace(" ", ""));
        return (time, distance);
    }
}