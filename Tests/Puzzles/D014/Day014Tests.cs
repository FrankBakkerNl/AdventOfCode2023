using AdventOfCode2023.Puzzles.Day14;
using FluentAssertions;

namespace Tests.Puzzles.D014;

public class Day014Tests
{
    [Fact]
    public void TestFindCorrespondingIteration()
    {
        Day14.FindCorrespondingIteration(0, 10, 100).Should().Be(0);
        Day14.FindCorrespondingIteration(1, 11, 100).Should().Be(10);
        Day14.FindCorrespondingIteration(3, 10, 1_000_000_000).Should().Be(6);
    }
}