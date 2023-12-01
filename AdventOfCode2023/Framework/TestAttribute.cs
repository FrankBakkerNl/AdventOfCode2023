namespace AdventOfCode2023.Framework;

class FocusAttribute : Attribute
{
}
class SkipAttribute : Attribute
{
}
    
class TestCase : Attribute
{
    public string? Filename { get; }
    public object Result { get; }

    public TestCase(object result)
    {
        Result = result;
    }

        
    public TestCase(object result, string filename)
    {
        Filename = filename;
        Result = result;
    }
}