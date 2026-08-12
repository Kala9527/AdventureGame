using AdventureGame.Models;

namespace AdventureGame.Services;

public static class LevelLoader
{
    // 地面基础Y坐标（适配Canvas实际高度）
    private const int GroundY = 400;
    private const int GroundHeight = 40;
    
    public static LevelData GetLevel(int levelNumber)
    {
        return levelNumber switch
        {
            1 => CreateLevel1(),
            2 => CreateLevel2(),
            3 => CreateLevel3(),
            _ => CreateLevel1()
        };
    }

    private static LevelData CreateLevel1()
    {
        var level = new LevelData
        {
            LevelNumber = 1,
            BackgroundColor = "#0f3460",
            GroundColor = "#4a9c5d",
            LevelWidth = 3600,
            LevelHeight = 500
        };

        level.Platforms.AddRange(new[]
        {
            CreateGround(0, GroundY, 400, GroundHeight),
            CreateGround(450, GroundY, 300, GroundHeight),
            CreateGround(800, GroundY, 400, GroundHeight),
            CreateGround(1300, GroundY, 500, GroundHeight),
            CreateGround(1900, GroundY, 400, GroundHeight),
            CreateGround(2400, GroundY, 600, GroundHeight),
            CreateGround(3100, GroundY, 500, GroundHeight),
            CreateBrick(200, 280),
            CreateQuestion(320, 280),
            CreateBrick(360, 280),
            CreateFloatingPlatform(550, 280, 80),
            CreateBrick(700, 280),
            CreateQuestion(740, 280),
            CreateBrick(780, 280),
            CreateFloatingPlatform(900, 250, 60),
            CreateFloatingPlatform(1100, 200, 80),
            CreateBrick(1400, 280),
            CreateQuestion(1440, 280),
            CreateBrick(1480, 280),
            CreateFloatingPlatform(1600, 220, 100),
            CreateFloatingPlatform(1800, 180, 80),
            CreateBrick(2000, 280),
            CreateBrick(2040, 280),
            CreateBrick(2080, 280),
            CreateFloatingPlatform(2200, 250, 120),
            CreateQuestion(2250, 250),
            CreateFloatingPlatform(2500, 280, 100),
            CreateBrick(2700, 220),
            CreateQuestion(2740, 220),
            CreateBrick(2780, 220),
            CreateFloatingPlatform(2900, 180, 120),
            CreateBrick(3200, 280),
            CreateBrick(3240, 280),
            CreateQuestion(3280, 280),
            CreateBrick(3320, 280),
        });

        level.Coins.AddRange(new[]
        {
            CreateCoin(150, 230),
            CreateCoin(250, 230),
            CreateCoin(570, 230),
            CreateCoin(720, 230),
            CreateCoin(1120, 150),
            CreateCoin(1620, 170),
            CreateCoin(1650, 170),
            CreateCoin(1820, 130),
            CreateCoin(1850, 130),
            CreateCoin(2220, 200),
            CreateCoin(2250, 200),
            CreateCoin(2280, 200),
            CreateCoin(2520, 230),
            CreateCoin(2720, 170),
            CreateCoin(2920, 130),
            CreateCoin(2950, 130),
            CreateCoin(2980, 130),
            CreateCoin(3220, 230),
            CreateCoin(3260, 230),
            CreateCoin(3300, 230),
        });

        level.Enemies.AddRange(new[]
        {
            new EnemySpawnPoint { X = 500, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 900, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 150 },
            new EnemySpawnPoint { X = 1400, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 200 },
            new EnemySpawnPoint { X = 2000, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 150 },
            new EnemySpawnPoint { X = 2500, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 200 },
            new EnemySpawnPoint { X = 3200, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
        });

        level.PlayerStart = new GameRect(50, GroundY - 48, 32, 48);
        level.Goal = new GameRect(3500, GroundY - 80, 40, 80);

        return level;
    }

    private static LevelData CreateLevel2()
    {
        var level = new LevelData
        {
            LevelNumber = 2,
            BackgroundColor = "#2d1b69",
            GroundColor = "#8b5cf6",
            LevelWidth = 4000,
            LevelHeight = 500
        };

        level.Platforms.AddRange(new[]
        {
            CreateGround(0, GroundY, 350, GroundHeight),
            CreateGround(420, GroundY, 200, GroundHeight),
            CreateGround(700, GroundY, 350, GroundHeight),
            CreateGround(1150, GroundY, 400, GroundHeight),
            CreateGround(1650, GroundY, 300, GroundHeight),
            CreateGround(2050, GroundY, 500, GroundHeight),
            CreateGround(2650, GroundY, 300, GroundHeight),
            CreateGround(3050, GroundY, 500, GroundHeight),
            CreateGround(3650, GroundY, 350, GroundHeight),
            CreateFloatingPlatform(100, 280, 80),
            CreateFloatingPlatform(250, 220, 80),
            CreateFloatingPlatform(450, 250, 60),
            CreateFloatingPlatform(600, 200, 100),
            CreateBrick(800, 280),
            CreateBrick(840, 280),
            CreateQuestion(880, 280),
            CreateBrick(920, 280),
            CreateFloatingPlatform(1100, 250, 80),
            CreateFloatingPlatform(1250, 200, 100),
            CreateFloatingPlatform(1400, 150, 80),
            CreateBrick(1750, 280),
            CreateQuestion(1790, 280),
            CreateBrick(1830, 280),
            CreateFloatingPlatform(1950, 220, 100),
            CreateFloatingPlatform(2100, 180, 120),
            CreateBrick(2200, 280),
            CreateBrick(2240, 280),
            CreateQuestion(2280, 280),
            CreateBrick(2320, 280),
            CreateFloatingPlatform(2500, 250, 80),
            CreateFloatingPlatform(2700, 200, 100),
            CreateFloatingPlatform(2900, 150, 120),
            CreateBrick(3200, 280),
            CreateBrick(3240, 280),
            CreateBrick(3280, 280),
            CreateQuestion(3320, 280),
            CreateBrick(3360, 280),
            CreateFloatingPlatform(3500, 220, 100),
        });

        level.Coins.AddRange(new[]
        {
            CreateCoin(120, 200),
            CreateCoin(270, 140),
            CreateCoin(470, 170),
            CreateCoin(620, 120),
            CreateCoin(660, 120),
            CreateCoin(820, 220),
            CreateCoin(860, 220),
            CreateCoin(1120, 170),
            CreateCoin(1270, 120),
            CreateCoin(1420, 70),
            CreateCoin(1770, 220),
            CreateCoin(1810, 220),
            CreateCoin(1970, 160),
            CreateCoin(2120, 120),
            CreateCoin(2160, 120),
            CreateCoin(2220, 220),
            CreateCoin(2260, 220),
            CreateCoin(2520, 190),
            CreateCoin(2720, 140),
            CreateCoin(2920, 90),
            CreateCoin(2960, 90),
            CreateCoin(3220, 230),
            CreateCoin(3260, 230),
            CreateCoin(3300, 230),
            CreateCoin(3520, 160),
        });

        level.Enemies.AddRange(new[]
        {
            new EnemySpawnPoint { X = 450, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 800, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 120 },
            new EnemySpawnPoint { X = 1200, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 150 },
            new EnemySpawnPoint { X = 1700, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 100 },
            new EnemySpawnPoint { X = 2100, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 180 },
            new EnemySpawnPoint { X = 2300, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 2700, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 150 },
            new EnemySpawnPoint { X = 3100, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 3300, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 100 },
            new EnemySpawnPoint { X = 2600, Y = 100, Type = EnemyType.FlyingEnemy, PatrolRange = 200 },
        });

        level.PlayerStart = new GameRect(50, GroundY - 48, 32, 48);
        level.Goal = new GameRect(3900, GroundY - 80, 40, 80);

        return level;
    }

    private static LevelData CreateLevel3()
    {
        var level = new LevelData
        {
            LevelNumber = 3,
            BackgroundColor = "#1a472a",
            GroundColor = "#2d5a3d",
            LevelWidth = 4500,
            LevelHeight = 500
        };

        level.Platforms.AddRange(new[]
        {
            CreateGround(0, GroundY, 300, GroundHeight),
            CreateGround(380, GroundY, 250, GroundHeight),
            CreateGround(720, GroundY, 300, GroundHeight),
            CreateGround(1100, GroundY, 250, GroundHeight),
            CreateGround(1450, GroundY, 300, GroundHeight),
            CreateGround(1850, GroundY, 400, GroundHeight),
            CreateGround(2350, GroundY, 300, GroundHeight),
            CreateGround(2750, GroundY, 250, GroundHeight),
            CreateGround(3100, GroundY, 350, GroundHeight),
            CreateGround(3550, GroundY, 300, GroundHeight),
            CreateGround(3950, GroundY, 550, GroundHeight),
            CreateFloatingPlatform(80, 280, 60),
            CreateFloatingPlatform(180, 230, 60),
            CreateFloatingPlatform(280, 180, 80),
            CreateFloatingPlatform(450, 260, 60),
            CreateFloatingPlatform(550, 210, 80),
            CreateFloatingPlatform(750, 260, 60),
            CreateFloatingPlatform(850, 200, 80),
            CreateFloatingPlatform(950, 140, 100),
            CreateBrick(1150, 260),
            CreateQuestion(1190, 260),
            CreateBrick(1230, 260),
            CreateFloatingPlatform(1300, 200, 100),
            CreateFloatingPlatform(1500, 260, 60),
            CreateFloatingPlatform(1600, 200, 80),
            CreateFloatingPlatform(1700, 140, 100),
            CreateBrick(1900, 260),
            CreateBrick(1940, 260),
            CreateQuestion(1980, 260),
            CreateBrick(2020, 260),
            CreateBrick(2060, 260),
            CreateFloatingPlatform(2150, 210, 100),
            CreateFloatingPlatform(2250, 160, 120),
            CreateFloatingPlatform(2400, 230, 80),
            CreateFloatingPlatform(2500, 180, 80),
            CreateBrick(2800, 260),
            CreateQuestion(2840, 260),
            CreateBrick(2880, 260),
            CreateFloatingPlatform(2950, 200, 80),
            CreateFloatingPlatform(3050, 140, 100),
            CreateBrick(3150, 260),
            CreateBrick(3190, 260),
            CreateBrick(3230, 260),
            CreateQuestion(3270, 260),
            CreateBrick(3310, 260),
            CreateFloatingPlatform(3400, 200, 100),
            CreateFloatingPlatform(3600, 230, 60),
            CreateFloatingPlatform(3700, 160, 100),
            CreateFloatingPlatform(3800, 100, 120),
            CreateBrick(4000, 260),
            CreateBrick(4040, 260),
            CreateQuestion(4080, 260),
            CreateBrick(4120, 260),
            CreateBrick(4160, 260),
        });

        level.Coins.AddRange(new[]
        {
            CreateCoin(100, 200),
            CreateCoin(200, 150),
            CreateCoin(300, 100),
            CreateCoin(470, 180),
            CreateCoin(570, 130),
            CreateCoin(770, 180),
            CreateCoin(870, 120),
            CreateCoin(970, 60),
            CreateCoin(1000, 60),
            CreateCoin(1170, 220),
            CreateCoin(1210, 220),
            CreateCoin(1320, 160),
            CreateCoin(1520, 220),
            CreateCoin(1620, 160),
            CreateCoin(1720, 100),
            CreateCoin(1750, 100),
            CreateCoin(1920, 220),
            CreateCoin(1960, 220),
            CreateCoin(2000, 220),
            CreateCoin(2170, 170),
            CreateCoin(2270, 120),
            CreateCoin(2300, 120),
            CreateCoin(2420, 190),
            CreateCoin(2520, 140),
            CreateCoin(2820, 220),
            CreateCoin(2860, 220),
            CreateCoin(2970, 160),
            CreateCoin(3070, 100),
            CreateCoin(3170, 220),
            CreateCoin(3210, 220),
            CreateCoin(3250, 220),
            CreateCoin(3420, 160),
            CreateCoin(3620, 190),
            CreateCoin(3720, 120),
            CreateCoin(3820, 60),
            CreateCoin(3850, 60),
            CreateCoin(4020, 220),
            CreateCoin(4060, 220),
            CreateCoin(4100, 220),
        });

        level.Enemies.AddRange(new[]
        {
            new EnemySpawnPoint { X = 400, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 150 },
            new EnemySpawnPoint { X = 800, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 150 },
            new EnemySpawnPoint { X = 1150, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 1500, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 150 },
            new EnemySpawnPoint { X = 1900, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 2050, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 2400, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 150 },
            new EnemySpawnPoint { X = 2800, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 3150, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 150 },
            new EnemySpawnPoint { X = 3600, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 100 },
            new EnemySpawnPoint { X = 4000, Y = GroundY - 30, Type = EnemyType.Koopa, PatrolRange = 200 },
            new EnemySpawnPoint { X = 4200, Y = GroundY - 30, Type = EnemyType.Goomba, PatrolRange = 150 },
            new EnemySpawnPoint { X = 1800, Y = 100, Type = EnemyType.FlyingEnemy, PatrolRange = 200 },
            new EnemySpawnPoint { X = 3300, Y = 80, Type = EnemyType.FlyingEnemy, PatrolRange = 250 },
        });

        level.PlayerStart = new GameRect(50, GroundY - 48, 32, 48);
        level.Goal = new GameRect(4400, GroundY - 80, 40, 80);

        return level;
    }

    private static Platform CreateGround(double x, double y, double width, double height)
    {
        return new Platform
        {
            Bounds = new GameRect(x, y, width, height),
            Type = PlatformType.Ground
        };
    }

    private static Platform CreateBrick(double x, double y)
    {
        return new Platform
        {
            Bounds = new GameRect(x, y, 40, 40),
            Type = PlatformType.Brick,
            IsBreakable = true
        };
    }

    private static Platform CreateQuestion(double x, double y)
    {
        return new Platform
        {
            Bounds = new GameRect(x, y, 40, 40),
            Type = PlatformType.QuestionBlock,
            IsBreakable = true
        };
    }

    private static Platform CreateFloatingPlatform(double x, double y, double width)
    {
        return new Platform
        {
            Bounds = new GameRect(x, y, width, 20),
            Type = PlatformType.FloatingPlatform
        };
    }

    private static Coin CreateCoin(double x, double y)
    {
        return new Coin
        {
            Bounds = new GameRect(x, y, 20, 20),
            Value = 100
        };
    }
}