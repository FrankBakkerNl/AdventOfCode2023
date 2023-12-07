using CommandLine;

namespace AdventOfCode2023.Puzzles.Day07;

/// <summary>
/// https://adventofcode.com/2023/day/6
/// </summary>
public class Day07
{
    const string Cards = "AKQJT98765432";

    [Result(253910319)]
    [TestCase(result: 6440)]
    [Focus]
    public static int GetAnswer1(string[] input)
    {
        var gameResults = ParseResults(input);
        var ordered = OrderCard(gameResults);

        // the weakest hand gets rank 1, the second-weakest hand gets rank 2
        return ordered.Select((g,i)=>g.bid * (i + 1)).Sum();
    }

    private static IEnumerable<(string hand, int bid)> OrderCard((string hand, int bid)[] gameResults) =>
        // Hands are primarily ordered based on type; 
        gameResults
            .OrderBy(t => GetTypeValue(t.hand))
            .ThenByDescending(t => GetHandKey(t.hand));

    static string GetHandKey(string hand) => new(hand.Select((c, i) => GetCardValue(c)).ToArray());

    static char GetCardValue(char c) => (char)('A'+ Cards.IndexOf(c));

    static int GetTypeValue(string hand)
    {
        var groupSizes = hand.GroupBy(c => c).Select(g => g.Count()).OrderDescending().ToArray();
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
    
    private static (string hand, int bid)[] ParseResults(string[] input) => input.Select(ParseLine).ToArray();

    private static (string hand, int bid) ParseLine(string arg)
    {
        var split = arg.Split(' ');
        return (split[0], int.Parse(split[1]));
    }


    [Result(28545089L)]
    [TestCase(result: 71503L)]
    public static long GetAnswer2(string[] input)
    {
        return 0;
    }    
}