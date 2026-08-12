namespace AdventureGame.Models;

public class Player
{
    public GameRect Bounds { get; set; } = null!;
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public Direction Facing { get; set; } = Direction.Right;
    public bool IsOnGround { get; set; }
    public bool IsJumping { get; set; }
    public bool IsAttacking { get; set; }
    public int AttackTimer { get; set; }
    public bool IsInvincible { get; set; }
    public int InvincibleTimer { get; set; }
    public int AnimationFrame { get; set; }
    public PlayerState State { get; set; } = PlayerState.Idle;

    public const double MoveSpeed = 4.0;
    public const double JumpForce = -15.0;
    public const double Gravity = 0.5;
    public const double MaxFallSpeed = 12.0;
    public const int AttackDuration = 15;
    public const int InvincibleDuration = 60;
    public const double Width = 32;
    public const double Height = 48;
}

public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Attacking,
    Hurt
}
