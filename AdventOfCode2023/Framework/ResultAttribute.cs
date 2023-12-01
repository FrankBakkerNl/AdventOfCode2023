namespace AdventOfCode2023.Framework;

public class ResultAttribute(object result) : Attribute
{
    public object Result { get; } = result;
}