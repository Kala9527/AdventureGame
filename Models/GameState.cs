namespace AdventureGame.Models;

public enum GameStateType
{
    Playing,
    Paused,
    GameOver,
    LevelComplete,
    Victory
}

public class GameState
{
    public int Score { get; set; }
    public int Lives { get; set; } = 3;
    public int Coins { get; set; }
    public int Level { get; set; } = 1;
    public GameStateType State { get; set; } = GameStateType.Playing;
}
