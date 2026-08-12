namespace AdventureGame.Models;

public class LevelData
{
    public int LevelNumber { get; set; }
    public string BackgroundColor { get; set; } = "#0f3460";
    public string GroundColor { get; set; } = "#4a9c5d";
    public List<Platform> Platforms { get; set; } = new();
    public List<Coin> Coins { get; set; } = new();
    public List<EnemySpawnPoint> Enemies { get; set; } = new();
    public GameRect PlayerStart { get; set; } = null!;
    public GameRect Goal { get; set; } = null!;
    public double LevelWidth { get; set; }
    public double LevelHeight { get; set; }
}

public class EnemySpawnPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public EnemyType Type { get; set; }
    public int PatrolRange { get; set; } = 100;
}
