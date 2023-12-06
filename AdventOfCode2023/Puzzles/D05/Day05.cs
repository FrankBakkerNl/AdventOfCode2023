using static System.Math;

namespace AdventOfCode2023.Puzzles.Day04;

/// <summary>
/// https://adventofcode.com/2023/day/5
/// </summary>
public class Day05
{
    [Result(318728750L)]
    [TestCase(result: 35L)]
    public static long GetAnswer1(string[] input)
    {
        var requiredSeeds = ParseRequiredSeeds(input[0]);
        var maps = ParseMaps(input);
        var result = maps.Aggregate(requiredSeeds, (halfProduct, map) => halfProduct.Select(map.GetTarget));

        return result.Min();
    }
    
    [TestCase(result: 46L)]
    [Result(37384986L)]
    public static long GetAnswer2(string[] input)
    {
        var requiredSeeds = ParseRequiredSeedRanges(input[0]);
        var maps = ParseMaps(input);
        var result = maps.Aggregate(requiredSeeds, (halfProduct, map) => map.GetTargetRanges(halfProduct));
        
        return result.Select(r=>r.Start).Min();
    }

    static IEnumerable<long> ParseRequiredSeeds(string line)
    {
        // get the ints after the :
        var numberPart = line[line.IndexOf(' ')..];
        return GetNumbers(numberPart);
    }

    static IEnumerable<Range> ParseRequiredSeedRanges(string line)
    {
        // get the ints after the:
        var numberPart = line[line.IndexOf(' ')..];
        var numbers = GetNumbers(numberPart);
        return numbers.Select((n, i) => (n, i / 2)).GroupBy(t => t.Item2)
            .Select(t => new Range(t.First().n, t.First().n + t.ElementAt(1).n - 1));
    }
    
    static List<Map> ParseMaps(string[] input)
    {
        var maps = new List<Map>();
        int index = 2;
        while (index < input.Length)
        {
            maps.Add(ParseMap(input, ref index));
        }

        return maps;
    }

    static Map ParseMap(string[] input, ref int index)
    {
        var ranges = new List<RangeShift>();
        index++; // skip first line
        string line;
        while (index< input.Length && (line = input[index++]) != "")
        {
            var ints = GetNumbers(line);
            ranges.Add(new RangeShift(ints[0], ints[1], ints[2]));
        }
        return new Map(ranges);
    }

    static long[] GetNumbers(string line) => line
        .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(long.Parse).ToArray(); 
    
    
    record Map(List<RangeShift> Ranges)
    {
        public List<RangeShift> Ranges { get; } = Ranges.OrderBy(r=>r.SourceRange.Start).ToList();

        public long GetTarget(long source)
        {
            var range = Ranges.FirstOrDefault(r => r.Matches(source));
            return range?.MapValue(source) ?? source;
        }

        public IEnumerable<Range> GetTargetRanges(IEnumerable<Range> input) => input.SelectMany(GetTargetRanges);

        private IEnumerable<Range> GetTargetRanges(Range input)
        {
            var output = new List<Range>();
            
            // One input range can overlap with multiple output ranges so we need to split it up with all overlaps
            var overlapping = Ranges.Where(r => r.SourceRange.Overlaps(input)).ToList();
            if (overlapping.Count == 0) return new[] { input };
            
            // Check if there is a part before the first overlap
            if (input.Start < overlapping[0].SourceRange.Start)
            {
                output.Add(input with { End = overlapping[0].SourceRange.Start - 1 });
            }

            // Add all intersections and apply their shift
            output.AddRange(overlapping.Select(
                other => input.IntersectWith(other.SourceRange).Shift(other.Shift)));

            // Check if there is a part after the last overlap
            if (input.End > overlapping.Last().SourceRange.End)
            {
                output.Add(input with { Start = overlapping.Last().SourceRange.End + 1 });
            }

            return output;
        }
    }

    record Range(long Start, long End)
    {
        public bool Before(Range other) => End < other.Start;

        public bool After(Range other) => Start > other.End;
        
        public bool Overlaps(Range other) => !Before(other) && !After(other);

        public Range IntersectWith(Range other) => new(Max(Start, other.Start), Min(End, other.End));

        public Range Shift(long distance) => new (Start + distance, End + distance);

    };
    
    record RangeShift(Range SourceRange, long Shift)
    {
        public RangeShift(long destinationStart, long sourceRangeStart, long rangeLength) 
            : this(new Range(sourceRangeStart, sourceRangeStart + rangeLength -1), destinationStart - sourceRangeStart)
        { }

        public bool Matches(long source) => SourceRange.Start <= source && source <= SourceRange.End;
        public long MapValue(long source) => source + Shift;
    }
}