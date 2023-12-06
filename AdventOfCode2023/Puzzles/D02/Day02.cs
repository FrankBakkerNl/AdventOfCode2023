namespace AdventOfCode2023.Puzzles;

/// <summary> https://adventofcode.com/2023/day/2</summary>
public class Day02
{
    private static readonly Dictionary<colour, int> MaxCountPerColour = new()
    {
        [colour.red] = 12,
        [colour.green] = 13,
        [colour.blue] = 14
    };

    [Result(1931)]
    [TestCase(result: 8)]
    public static int GetAnswer1(string[] input)
    {
        var games = ParseGames(input);
        return games.Where(GamePossible).Sum(g => g.number);
    }

    private static bool GamePossible(Game game)
    {
        // The Elf would first like to know which games would have been possible
        // if the bag contained only 12 red cubes, 13 green cubes, and 14 blue cubes?
        return game.reveals.SelectMany(r => r.sets).All(s => MaxCountPerColour[s.colour] >= s.count);
    }

    [Result(83105)]
    [TestCase(result: 2286)]
    public static int GetAnswer2(string[] input)
    {
        // what is the fewest number of cubes of each color that could have been in the bag to make the game possible?
        var games = ParseGames(input);
        return games.Sum(GameValue);
    }

    private static int GameValue(Game game)
    {
        var maxPerColour = game.reveals.SelectMany(r => r.sets)
            .GroupBy(s => s.colour)
            .Select(g => g.Max(s=>s.count));
        return maxPerColour.Aggregate(1, (a, v) => a * v);
    }
    
    private static List<Game> ParseGames(string[] input) => input.Select(ParseGame).ToList();

    private static Game ParseGame(string line)
    {
        var parts = line.Split(':');
        var number = int.Parse(parts[0][5..]);
        return new Game(number, ParseReveals(parts[1]));
    }

    private static Reveal[] ParseReveals(string part) => part.Split(';').Select(ParseReveal).ToArray();

    private static Reveal ParseReveal(string arg) => new(arg.Split(',').Select(ParseSet).ToArray());

    private static (colour colour, int count) ParseSet(string arg)
    {
        var split = arg.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (Enum.Parse<colour>(split[1]), int.Parse(split[0]));
    }

    record Game(int number, Reveal[] reveals);

    record Reveal((colour colour, int count)[] sets);

    enum colour { red, green, blue };
}
