namespace AdventureGame.Models;

public class PowerUp
{
    public GameRect Bounds { get; set; } = null!;
    public PowerUpType Type { get; set; }
    public bool IsCollected { get; set; }
    public double VelocityY { get; set; }
    public int AnimationFrame { get; set; }
}

public enum PowerUpType
{
    Mushroom,
    Star,
    Flower
}
