namespace AdventOfCode2023.Puzzles.Day07;

/// <summary>
/// https://adventofcode.com/2023/day/6
/// </summary>
public class Day07
{
    [Result(253910319)]
    [TestCase(result: 6440)]
    [Focus]
    public static int GetAnswer1(string[] input)
    {
        var gameResults = ParseResults(input);
        var ordered = OrderCard(gameResults);

        // The weakest hand gets rank 1, the second-weakest hand gets rank 2
        return ordered.Select((g,i)=>g.bid * (i + 1)).Sum();
    }

    private static IEnumerable<(string hand, int bid)> OrderCard((string hand, int bid)[] gameResults) =>
        // Hands are primarily ordered based on type; 
        gameResults
            .OrderBy(t => GetTypeValue(t.hand))
            .ThenByDescending(t => GetHandKey(t.hand));

    static string GetHandKey(string hand) => new(hand.Select((c, i) => GetCardValue(c)).ToArray());

    static char GetCardValue(char c) => (char)('A'+ "AKQJT98765432".IndexOf(c)); // Assign a letter to each value so we can sort on the whole string

    static int GetTypeValue(string hand)
    {
        var groupSizes = hand
            .GroupBy(c => c)
            .Select(g => g.Count())
            .OrderDescending()
            .Take(2).ToArray();
        
        return groupSizes[0] switch
        {
            5 => 7,                     // Five of a kind
            4 => 6,                     // Four of a kind
            3 when groupSizes[1] == 2 => 5, // Full house
            3 => 4,                     // Three of a kind
            2 when groupSizes[1] == 2 => 3, // Two pair 
            2 => 2,                     // One pair
            _ => 1,
        };
    }
    
    const string CardsRule2 = "AKQT98765432J";
    
    //[Result(28545089L)] // 253718982
    [TestCase(result: 5905)]
    [Focus]
    public static int GetAnswer2(string[] input)
    {
        var gameResults = ParseResults(input);
        var ordered = OrderCardJokerRule(gameResults);

        // the weakest hand gets rank 1, the second-weakest hand gets rank 2
        return ordered.Select((g,i)=>g.bid * (i + 1)).Sum();
    }

    private static IEnumerable<(string hand, int bid)> OrderCardJokerRule((string hand, int bid)[] gameResults) =>
        // Hands are primarily ordered based on type; 
        gameResults
            .OrderBy(t => GetTypeValueJokerRule(t.hand))
            .ThenByDescending(t => GetHandKeyJokerRule(t.hand));
    
    static string GetHandKeyJokerRule(string hand) => new(hand.Select((c, i) => GetCardValueJokerRule(c)).ToArray());
    
    static char GetCardValueJokerRule(char c) => (char)('A'+ CardsRule2.IndexOf(c));
    static int GetTypeValueJokerRule(string hand)
    {
        var charLookup = hand.ToLookup(c => c);
        var numJokers = charLookup['J'].Count();
        
        var groupSizes = charLookup
            .Where(c=>c.Key != 'J')
            .Select(g => g.Count())
            .OrderDescending()
            .Take(2).ToArray();
        
        // Add the number of jokers to the largest group size
        var firstGroupSize = groupSizes.FirstOrDefault() + numJokers;

        return firstGroupSize switch
        {
            5 => 7,                     // Five of a kind
            4 => 6,                     // Four of a kind
            3 when groupSizes[1] == 2 => 5, // Full house
            3 => 4,                     // Three of a kind
            2 when groupSizes[1] == 2 => 3, // Two pair 
            2 => 2,                     // One pair
            _ => 1,
        };
    }
    
    
    private static (string hand, int bid)[] ParseResults(string[] input) => input.Select(ParseLine).ToArray();

    private static (string hand, int bid) ParseLine(string arg)
    {
        var split = arg.Split(' ');
        return (split[0], int.Parse(split[1]));
    }
}