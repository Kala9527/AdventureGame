namespace AdventureGame.Models;

public class Platform
{
    public GameRect Bounds { get; set; } = null!;
    public PlatformType Type { get; set; }
    public bool IsBreakable { get; set; }
    public int BreakTimer { get; set; }
    public bool IsBroken { get; set; }

    public const double BlockSize = 40;
}

public enum PlatformType
{
    Ground,
    Brick,
    QuestionBlock,
    Pipe,
    StairBlock,
    FloatingPlatform
}
