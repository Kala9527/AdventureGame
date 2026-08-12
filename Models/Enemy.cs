namespace AdventureGame.Models;

public class Enemy
{
    public GameRect Bounds { get; set; } = null!;
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public EnemyType Type { get; set; }
    public Direction Facing { get; set; } = Direction.Left;
    public int Health { get; set; }
    public bool IsAlive { get; set; } = true;
    public bool IsOnGround { get; set; }
    public int AnimationFrame { get; set; }
    public int PatrolStart { get; set; }
    public int PatrolEnd { get; set; }
    public double StompBounce { get; set; }

    public const double Width = 30;
    public const double Height = 30;
}

public enum EnemyType
{
    Goomba,
    Koopa,
    Spike,
    FlyingEnemy
}
