using System.Text.RegularExpressions;

namespace AdventOfCode2023.Puzzles.Day15;

/// <summary>
/// https://adventofcode.com/2023/day/15
/// </summary>
public partial class Day15
{
    [Result(511343)]
    [TestCase(result: 1320)]
    public static int GetAnswer1(string input) => input.Split(',').Select(GetHash).Sum();

    private static int GetHash(string input)
    {
        var result = 0;
        foreach (var c in input)
        {
            result += c;
            result *= 17;
            result %= 256;
        }

        return result;
    }

    [Result(294474)]
    [TestCase(result: 145)]
    [Focus]
    public static long GetAnswer2(string input)
    {
        var boxes = new Box[256];
        boxes.Initialize();
        var matches = ParseInstructions().Matches(input);
        var instructions = matches.Select(m => new Instruction(m));
        
        foreach (var instruction in instructions)
        {
            boxes[instruction.BoxNumber].Process(instruction);
        }

        return boxes.Select((b, i) => b.LensValues * (i + 1)).Sum();
    }

    struct Box()
    {
        private readonly List<(string? label, int focus)> _lenses = new();

        public void Process(Instruction instruction)
        {
            var existing = _lenses.FindIndex(l => l.label == instruction.Label);

            if (instruction.Operation == '-' && existing >=0)
            {
                _lenses.RemoveAt(existing);
            }

            else if (instruction.Operation == '=')
            {
                if (existing >= 0)
                {
                    _lenses[existing] = (instruction.Label, instruction.LensFocalLength);
                }
                else
                {
                    _lenses.Add((instruction.Label, instruction.LensFocalLength));
                }
            }
        }

        public int LensValues => _lenses.Select((l, i) => l.focus * (i + 1)).Sum();
    }
    
    [GeneratedRegex("([a-z]+)([=-])([0-9])?")]
    private static partial Regex ParseInstructions();

    record Instruction
    {
        public int BoxNumber;
        public char Operation;
        public int LensFocalLength;
        public string Label;

        public Instruction(Match Match)
        {
            Label = Match.Groups[1].Value;
            BoxNumber = GetHash(Label);
            Operation = Match.Groups[2].Value[0];
            LensFocalLength = Match.Groups[3].Value.FirstOrDefault() - '0';
        }
    }
    
}