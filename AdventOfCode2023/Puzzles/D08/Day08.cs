namespace AdventOfCode2023.Puzzles.Day08;

/// <summary>
/// https://adventofcode.com/2023/day/8
/// </summary>
public class Day08
{
    [Result(14893)]
    [TestCase(result: 6)]
    public static int GetAnswer1(string[] input)
    {
        var (pattern, current, zIndex, leftMap, rightMap) = ParseInput(input);

        int steps;
        for (steps = 0; current != zIndex; steps++)
        {
            // We need to wrap around the pattern when at the end, so we can modulo the steps to get the correct index 
            current = pattern[steps % pattern.Length] == 'L' ? leftMap[current] : rightMap[current];
        }
        
        return steps;
    }

    static (string pattern, int aIndex, int zIndex, int[] leftMap, int[] rightMap) ParseInput(string[] input)
    {
        // Map the strings to integers so we can do fast array indexing
        var indexFromCode = new Dictionary<string, int>();

        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            indexFromCode.Add(line[..3], i);
        }

        // Now build two arrays for left and right
        var leftMap = new int [input.Length - 2]; 
        var rightMap = new int [input.Length - 2]; 

        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            leftMap[i] = indexFromCode[line[7..10]];
            rightMap[i] = indexFromCode[line[12..15]];
        }

        return (input[0], indexFromCode["AAA"], indexFromCode["ZZZ"], leftMap, rightMap);
    }
    
    [Result(10_241_191_004_509)]
    public static long GetAnswer2(string[] input)
    {
        var (pattern, aIndexes, zIndex, leftMap, rightMap) = ParseInput2(input);

        // Simulating all ghosts step by step simultaneously until they meet at a Z at the same time takes forever
        // so we analyse each ghosts pattern separately so we can later on calculate where these patterns meet
        var cycles = aIndexes.Select(start => FindCyclePattern(start, zIndex, pattern, leftMap, rightMap)).ToArray();

        
        // After manually inspecting the cycle patterns, it turns out for each ghost,
        // * only a single Z value was ever found during a cycle
        // * this Z value was found at exactly the same step as the cycle length
        // this means a Z value will be found exactly at every position that is a multiple of the cycle time
        
        // It feels a bit like cheating but this greatly simplifies the solution because but we can now ignore
        // the start of the cycle and simply just find the first value where all cycles meet, AKA Least Common Multiple 
        // that was exactly what we also did here:
        // https://github.com/FrankBakkerNl/AdventOfCode2019/blob/Develop/AdventOfCode2019/Day12.cs#L63

        return cycles.Select(c => (long)c.CycleTime).Aggregate(Lcm);
    }

    record CyclePattern(int PreCycle, int CycleTime, int[] zVisist);
   
    /// <summary>
    ///  Each ghost has his own path that will eventually have a cycle we want to know
    /// - the length of the cycle
    /// - the number of steps before the first cycle starts 
    /// - where in the cycle the Z value is found
    /// </summary>
    static CyclePattern FindCyclePattern(int current, HashSet<int> zIndexes, string pattern, int[] leftMap, int[] rightMap)
    {
        Span<int> visits = stackalloc int[pattern.Length * leftMap.Length];
        var zVisits = new HashSet<int>();
        
        for (int step = 0; ; step++)
        {
            var patternIndex = step % pattern.Length;
            if (zIndexes.Contains(current)) zVisits.Add(step);

            // we are in a cycle if we are on the same position in the map AND at the same position in the instruction list
            // so we keep track of each such combination and when we visited it
            // as this lookup is the hotspot of this algorithm we combine both into a single integer and use an array
            var positionKey = current * pattern.Length + patternIndex;
            if (visits[positionKey] > 0)
            {
                var previousVisitStep = visits[positionKey] -1;
                // we have been here before so we found our cycle time
                return new CyclePattern(PreCycle: previousVisitStep, CycleTime: step - previousVisitStep, zVisist: zVisits.ToArray());
            }

            visits[positionKey] = step + 1; // we want 0 to mean 'not visited' so we add 1 to the stepcount and substract later

            // We need to wrap around the pattern when at the end, so we can modulo the steps to get the correct index 
            current = pattern[patternIndex] == 'L' ? leftMap[current] : rightMap[current];
        }
    }
    
    static long Gcd(long a, long b) => a % b == 0 ? b : Gcd(b, a % b);
    static long Lcm(long a, long b) => a * b / Gcd(a, b);

    static (string pattern, int[] aIndex, HashSet<int> zIndexes, int[] leftMap, int[] rightMap) ParseInput2(string[] input)
    {
        // Map the strings to integers so we can do fast array indexing
        var indexFromCode = new Dictionary<string, int>();

        HashSet<int> aIndexes = new HashSet<int>();
        HashSet<int> zIndexes = new HashSet<int>();
        
        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            indexFromCode.Add(line[..3], i);
            if (line[2] == 'A') aIndexes.Add(i);
            else if (line[2] == 'Z') zIndexes.Add(i);
        }
        
        // Now build two arrays for left and right
        var leftMap = new int [input.Length - 2]; 
        var rightMap = new int [input.Length - 2]; 

        for (var i = 0; i < input.Length - 2; i++)
        {
            var line = input[i + 2];
            leftMap[i] = indexFromCode[line[7..10]];
            rightMap[i] = indexFromCode[line[12..15]];
        }

        return (input[0], aIndexes.ToArray(), zIndexes, leftMap, rightMap);
    }
}