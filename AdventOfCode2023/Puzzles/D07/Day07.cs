using System.Globalization;

namespace AdventOfCode2023.Puzzles.Day07;

/// <summary>
/// https://adventofcode.com/2023/day/6
/// </summary>
public class Day07
{
    [Result(253910319)]
    [TestCase(result: 6440)]
    public static int GetAnswer1(string[] input)
    {
        var gameResults = ParseResults(input);
        var ordered = OrderCard(gameResults);

        // The weakest hand gets rank 1, the second-weakest hand gets rank 2
        return ordered.Select((g,i)=>g.bid * (i + 1)).Sum();
    }

    private static IEnumerable<(string hand, int bid)> OrderCard(IEnumerable<(string hand, int bid)> gameResults) =>
        // Hands are primarily ordered based on type; 
        gameResults
            .OrderBy(t => GetTypeValue(t.hand))
            .ThenBy(t => GetHandKey(t.hand));

    static string GetHandKey(string hand) => new(hand.Select(GetCardHexValue).ToArray());

    static char GetCardHexValue(char c) => 
        char.IsAsciiDigit(c) ? c : 
        (char)('A'+ "TJQKA".IndexOf(c)); // Convert to a Hex string

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

    [Result(254083736)]
    [TestCase(result: 5905)]
    public static int GetAnswer2(string[] input)
    {
        var gameResults = ParseResults(input);
        var ordered = OrderCardJokerRule(gameResults);

        // the weakest hand gets rank 1, the second-weakest hand gets rank 2
        return ordered.Select((g,i)=>g.bid * (i + 1)).Sum();
    }

    private static IEnumerable<(string hand, int bid)> OrderCardJokerRule(IEnumerable<(string hand, int bid)> gameResults) =>
        // Hands are primarily ordered based on type; 
        gameResults
            .OrderBy(t => GetTypeValueJokerRule(t.hand))
            .ThenBy(t => GetHandKeyJokerRule(t.hand));
    
    static string GetHandKeyJokerRule(string hand) => new(hand.Select(GetCardHexValueJokerRule).ToArray());
    
    static char GetCardHexValueJokerRule(char c) => 
        char.IsAsciiDigit(c) ? c : 
        c == 'J' ? '1' : 
        (char)('A'+ "TJQKA".IndexOf(c)); // Convert to a Hex string

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
    
    private static IEnumerable<(string hand, int bid)> ParseResults(string[] input) => input.Select(ParseLine);

    private static (string hand, int bid) ParseLine(string arg) => (arg[..5], int.Parse(arg[6..], NumberStyles.None));
}