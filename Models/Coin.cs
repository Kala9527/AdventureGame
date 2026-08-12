namespace AdventureGame.Models;

public class Coin
{
    public GameRect Bounds { get; set; } = null!;
    public int Value { get; set; } = 100;
    public bool IsCollected { get; set; }
    public int AnimationFrame { get; set; }
    public double FloatOffset { get; set; }
}
