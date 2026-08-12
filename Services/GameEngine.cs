using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AdventureGame.Models;

namespace AdventureGame.Services;

public class GameEngine
{
    private readonly Canvas _canvas;
    private readonly GameState _gameState;
    private readonly Player _player;
    private readonly List<Enemy> _enemies;
    private readonly List<Coin> _coins;
    private readonly List<Platform> _platforms;
    private readonly List<PowerUp> _powerUps;
    private readonly Dictionary<InputAction, bool> _inputStates;
    
    private DispatcherTimer? _gameLoop;
    private LevelData? _currentLevel;
    private bool _isPaused;
    private double _cameraX;
    private int _frameCount;
    
    public event EventHandler<GameState>? OnGameStateChanged;
    public event EventHandler<GameState>? OnGameOver;
    public event EventHandler<GameState>? OnLevelComplete;

    public GameState State => _gameState;
    public Player Player => _player;
    public IReadOnlyList<Enemy> Enemies => _enemies;
    public IReadOnlyList<Coin> Coins => _coins;
    public IReadOnlyList<Platform> Platforms => _platforms;
    public IReadOnlyList<PowerUp> PowerUps => _powerUps;
    public double CameraX => _cameraX;
    public LevelData CurrentLevel => _currentLevel!;

    public GameEngine(Canvas canvas)
    {
        _canvas = canvas;
        _gameState = new GameState();
        _player = new Player();
        _enemies = new List<Enemy>();
        _coins = new List<Coin>();
        _platforms = new List<Platform>();
        _powerUps = new List<PowerUp>();
        _inputStates = new Dictionary<InputAction, bool>();
        _cameraX = 0;
        _frameCount = 0;
    }

    public void Start()
    {
        _isPaused = false;
        LoadLevel(1);
        
        _gameLoop = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _gameLoop.Tick += GameLoop_Tick;
        _gameLoop.Start();
        
        System.Diagnostics.Debug.WriteLine("GameEngine Started");
    }

    public void Stop()
    {
        _gameLoop?.Stop();
        System.Diagnostics.Debug.WriteLine("GameEngine Stopped");
    }

    public void Reset()
    {
        _isPaused = false;
        _gameState.Score = 0;
        _gameState.Lives = 3;
        _gameState.Coins = 0;
        _gameState.Level = 1;
        _gameState.State = GameStateType.Playing;
        
        LoadLevel(_gameState.Level);
        NotifyStateChanged();
    }

    public void SetInput(InputAction action, bool isPressed)
    {
        _inputStates[action] = isPressed;
        System.Diagnostics.Debug.WriteLine($"SetInput: {action} = {isPressed}");
    }

    public void TogglePause()
    {
        if (_gameState.State == GameStateType.Playing || _gameState.State == GameStateType.Paused)
        {
            _isPaused = !_isPaused;
            _gameState.State = _isPaused ? GameStateType.Paused : GameStateType.Playing;
            NotifyStateChanged();
        }
    }

    private void LoadLevel(int levelNumber)
    {
        _currentLevel = LevelLoader.GetLevel(levelNumber);
        _platforms.Clear();
        _coins.Clear();
        _enemies.Clear();
        _powerUps.Clear();
        
        foreach (var platform in _currentLevel.Platforms)
        {
            _platforms.Add(new Platform
            {
                Bounds = new GameRect(platform.Bounds.X, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height),
                Type = platform.Type,
                IsBreakable = platform.IsBreakable
            });
        }
        
        foreach (var coin in _currentLevel.Coins)
        {
            _coins.Add(new Coin
            {
                Bounds = new GameRect(coin.Bounds.X, coin.Bounds.Y, coin.Bounds.Width, coin.Bounds.Height),
                Value = coin.Value,
                FloatOffset = Random.Shared.NextDouble() * Math.PI * 2
            });
        }
        
        foreach (var spawn in _currentLevel.Enemies)
        {
            _enemies.Add(new Enemy
            {
                Bounds = new GameRect(spawn.X, spawn.Y, Enemy.Width, Enemy.Height),
                Type = spawn.Type,
                PatrolStart = (int)spawn.X - spawn.PatrolRange,
                PatrolEnd = (int)spawn.X + spawn.PatrolRange,
                VelocityX = -1.5
            });
        }
        
        _player.Bounds = new GameRect(
            _currentLevel.PlayerStart.X,
            _currentLevel.PlayerStart.Y,
            Player.Width,
            Player.Height
        );
        _player.VelocityX = 0;
        _player.VelocityY = 0;
        _player.IsOnGround = false;
        _player.Facing = Direction.Right;
        
        _cameraX = 0;
        System.Diagnostics.Debug.WriteLine($"Level {levelNumber} Loaded, Player at ({_player.Bounds.X}, {_player.Bounds.Y})");
    }

    private void GameLoop_Tick(object? sender, EventArgs e)
    {
        _frameCount++;
        
        if (!_isPaused && _gameState.State == GameStateType.Playing)
        {
            UpdatePlayer();
            UpdateEnemies();
            UpdateCoins();
            UpdatePowerUps();
            CheckCollisions();
            UpdateCamera();
            CheckLevelComplete();
            
            // 每60帧输出一次玩家位置（调试用）
            if (_frameCount % 60 == 0)
            {
                System.Diagnostics.Debug.WriteLine($"Frame: {_frameCount}, Player: ({_player.Bounds.X:F1}, {_player.Bounds.Y:F1}), State: {_player.State}, Input: Left={_inputStates.GetValueOrDefault(InputAction.MoveLeft)}, Right={_inputStates.GetValueOrDefault(InputAction.MoveRight)}");
            }
        }
    }

    private void UpdatePlayer()
    {
        if (_player.IsInvincible)
        {
            _player.InvincibleTimer--;
            if (_player.InvincibleTimer <= 0)
                _player.IsInvincible = false;
        }

        if (_player.IsAttacking)
        {
            _player.AttackTimer--;
            if (_player.AttackTimer <= 0)
                _player.IsAttacking = false;
        }

        bool movingLeft = _inputStates.GetValueOrDefault(InputAction.MoveLeft);
        bool movingRight = _inputStates.GetValueOrDefault(InputAction.MoveRight);
        bool jumpRequested = _inputStates.GetValueOrDefault(InputAction.Jump);
        bool attackRequested = _inputStates.GetValueOrDefault(InputAction.Attack);

        if (attackRequested && !_player.IsAttacking)
        {
            _player.IsAttacking = true;
            _player.AttackTimer = Player.AttackDuration;
        }

        if (movingLeft)
        {
            _player.VelocityX = -Player.MoveSpeed;
            _player.Facing = Direction.Left;
        }
        else if (movingRight)
        {
            _player.VelocityX = Player.MoveSpeed;
            _player.Facing = Direction.Right;
        }
        else
        {
            _player.VelocityX *= 0.8;
            if (Math.Abs(_player.VelocityX) < 0.1) _player.VelocityX = 0;
        }

        if (jumpRequested && _player.IsOnGround)
        {
            _player.VelocityY = Player.JumpForce;
            _player.IsOnGround = false;
            _player.IsJumping = true;
        }

        _player.VelocityY += Player.Gravity;
        if (_player.VelocityY > Player.MaxFallSpeed)
            _player.VelocityY = Player.MaxFallSpeed;

        MovePlayerX();
        MovePlayerY();

        UpdatePlayerState(movingLeft || movingRight);
        _player.AnimationFrame++;
    }

    private void MovePlayerX()
    {
        var newX = _player.Bounds.X + _player.VelocityX;
        var newBounds = new GameRect(newX, _player.Bounds.Y, Player.Width, Player.Height);
        
        foreach (var platform in _platforms)
        {
            if (platform.IsBroken) continue;
            
            if (newBounds.Intersects(platform.Bounds))
            {
                if (_player.VelocityX > 0)
                {
                    newX = platform.Bounds.X - Player.Width;
                }
                else if (_player.VelocityX < 0)
                {
                    newX = platform.Bounds.Right;
                }
                _player.VelocityX = 0;
                newBounds.X = newX;
            }
        }
        
        _player.Bounds = newBounds;
        
        if (_player.Bounds.X < 0) _player.Bounds.X = 0;
        if (_player.Bounds.Right > _currentLevel!.LevelWidth)
            _player.Bounds.X = _currentLevel.LevelWidth - Player.Width;
    }

    private void MovePlayerY()
    {
        var newY = _player.Bounds.Y + _player.VelocityY;
        var newBounds = new GameRect(_player.Bounds.X, newY, Player.Width, Player.Height);
        _player.IsOnGround = false;
        
        foreach (var platform in _platforms)
        {
            if (platform.IsBroken) continue;
            
            if (newBounds.Intersects(platform.Bounds))
            {
                if (_player.VelocityY > 0)
                {
                    newY = platform.Bounds.Y - Player.Height;
                    _player.VelocityY = 0;
                    _player.IsOnGround = true;
                    _player.IsJumping = false;
                }
                else if (_player.VelocityY < 0)
                {
                    newY = platform.Bounds.Bottom;
                    _player.VelocityY = 0;
                    
                    if (platform.Type == PlatformType.QuestionBlock && !platform.IsBroken)
                    {
                        OnHitQuestionBlock(platform);
                    }
                    else if (platform.IsBreakable)
                    {
                        platform.BreakTimer = 30;
                    }
                }
                newBounds.Y = newY;
            }
        }
        
        _player.Bounds = newBounds;
        
        if (_player.Bounds.Y > _currentLevel!.LevelHeight + 100)
        {
            PlayerDie();
        }
    }

    private void UpdatePlayerState(bool isMoving)
    {
        if (!_player.IsOnGround)
        {
            _player.State = _player.VelocityY < 0 ? PlayerState.Jumping : PlayerState.Falling;
        }
        else if (_player.IsAttacking)
        {
            _player.State = PlayerState.Attacking;
        }
        else if (!_player.IsInvincible && _player.InvincibleTimer > 50)
        {
            _player.State = PlayerState.Hurt;
        }
        else if (isMoving)
        {
            _player.State = PlayerState.Running;
        }
        else
        {
            _player.State = PlayerState.Idle;
        }
    }

    private void UpdateEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.IsAlive) continue;
            
            enemy.AnimationFrame++;
            
            switch (enemy.Type)
            {
                case EnemyType.Goomba:
                case EnemyType.Koopa:
                    UpdateWalkingEnemy(enemy);
                    break;
                case EnemyType.FlyingEnemy:
                    UpdateFlyingEnemy(enemy);
                    break;
            }
        }
    }

    private void UpdateWalkingEnemy(Enemy enemy)
    {
        enemy.VelocityY += Player.Gravity;
        if (enemy.VelocityY > Player.MaxFallSpeed)
            enemy.VelocityY = Player.MaxFallSpeed;
        
        if (enemy.StompBounce > 0)
        {
            enemy.StompBounce--;
            enemy.VelocityY = -8;
        }

        var newX = enemy.Bounds.X + enemy.VelocityX;
        var newBounds = new GameRect(newX, enemy.Bounds.Y, Enemy.Width, Enemy.Height);
        
        bool blocked = false;
        foreach (var platform in _platforms)
        {
            if (platform.IsBroken) continue;
            if (newBounds.Intersects(platform.Bounds))
            {
                blocked = true;
                break;
            }
        }
        
        if (blocked || newX < enemy.PatrolStart || newX > enemy.PatrolEnd)
        {
            enemy.VelocityX = -enemy.VelocityX;
            enemy.Facing = enemy.VelocityX > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            enemy.Bounds = newBounds;
        }

        var newY = enemy.Bounds.Y + enemy.VelocityY;
        var yBounds = new GameRect(enemy.Bounds.X, newY, Enemy.Width, Enemy.Height);
        enemy.IsOnGround = false;
        
        foreach (var platform in _platforms)
        {
            if (platform.IsBroken) continue;
            if (yBounds.Intersects(platform.Bounds))
            {
                if (enemy.VelocityY > 0)
                {
                    newY = platform.Bounds.Y - Enemy.Height;
                    enemy.VelocityY = 0;
                    enemy.IsOnGround = true;
                }
                else if (enemy.VelocityY < 0)
                {
                    newY = platform.Bounds.Bottom;
                    enemy.VelocityY = 0;
                }
                yBounds.Y = newY;
            }
        }
        
        enemy.Bounds = yBounds;
        
        if (enemy.Bounds.Y > _currentLevel!.LevelHeight + 100)
        {
            enemy.IsAlive = false;
        }
    }

    private void UpdateFlyingEnemy(Enemy enemy)
    {
        enemy.AnimationFrame++;
        var bobOffset = Math.Sin(enemy.AnimationFrame * 0.05) * 0.5;
        enemy.Bounds = new GameRect(
            enemy.Bounds.X + enemy.VelocityX,
            enemy.Bounds.Y + bobOffset,
            Enemy.Width,
            Enemy.Height
        );
        
        if (enemy.Bounds.X < enemy.PatrolStart || enemy.Bounds.X > enemy.PatrolEnd)
        {
            enemy.VelocityX = -enemy.VelocityX;
            enemy.Facing = enemy.VelocityX > 0 ? Direction.Right : Direction.Left;
        }
    }

    private void UpdateCoins()
    {
        foreach (var coin in _coins)
        {
            if (!coin.IsCollected)
            {
                coin.AnimationFrame++;
                coin.FloatOffset += 0.05;
            }
        }
    }

    private void UpdatePowerUps()
    {
        for (int i = _powerUps.Count - 1; i >= 0; i--)
        {
            var powerUp = _powerUps[i];
            powerUp.AnimationFrame++;
            powerUp.VelocityY += Player.Gravity * 0.5;
            if (powerUp.VelocityY > 5) powerUp.VelocityY = 5;
            
            var newY = powerUp.Bounds.Y + powerUp.VelocityY;
            var newBounds = new GameRect(powerUp.Bounds.X, newY, powerUp.Bounds.Width, powerUp.Bounds.Height);
            
            foreach (var platform in _platforms)
            {
                if (platform.IsBroken) continue;
                if (newBounds.Intersects(platform.Bounds))
                {
                    if (powerUp.VelocityY > 0)
                    {
                        newY = platform.Bounds.Y - powerUp.Bounds.Height;
                        powerUp.VelocityY = 0;
                    }
                }
            }
            
            powerUp.Bounds = new GameRect(powerUp.Bounds.X, newY, powerUp.Bounds.Width, powerUp.Bounds.Height);
            
            if (_currentLevel != null && powerUp.Bounds.Y > _currentLevel.LevelHeight + 50)
            {
                _powerUps.RemoveAt(i);
            }
        }
    }

    private void CheckCollisions()
    {
        var playerBounds = _player.Bounds;
        
        foreach (var coin in _coins)
        {
            if (!coin.IsCollected && playerBounds.Intersects(coin.Bounds))
            {
                coin.IsCollected = true;
                _gameState.Coins++;
                _gameState.Score += coin.Value;
                NotifyStateChanged();
            }
        }
        
        foreach (var powerUp in _powerUps)
        {
            if (!powerUp.IsCollected && playerBounds.Intersects(powerUp.Bounds))
            {
                powerUp.IsCollected = true;
                _gameState.Score += 200;
                _gameState.Lives++;
                NotifyStateChanged();
                _powerUps.Remove(powerUp);
                break;
            }
        }
        
        foreach (var enemy in _enemies)
        {
            if (!enemy.IsAlive) continue;
            
            if (playerBounds.Intersects(enemy.Bounds))
            {
                bool playerAbove = _player.VelocityY > 0 && 
                                   playerBounds.Bottom - enemy.Bounds.Y < 15;
                
                if (playerAbove && enemy.Type != EnemyType.Spike)
                {
                    enemy.IsAlive = false;
                    _player.VelocityY = Player.JumpForce * 0.6;
                    _gameState.Score += 100;
                    NotifyStateChanged();
                }
                else if (_player.IsAttacking && 
                         IsAttackHit(enemy.Bounds))
                {
                    enemy.IsAlive = false;
                    _gameState.Score += 150;
                    NotifyStateChanged();
                }
                else if (!_player.IsInvincible)
                {
                    PlayerHit();
                }
            }
        }
    }

    private bool IsAttackHit(GameRect enemyBounds)
    {
        var attackBounds = new GameRect(
            _player.Facing == Direction.Right 
                ? _player.Bounds.Right 
                : _player.Bounds.X - 20,
            _player.Bounds.Y + 10,
            20,
            30
        );
        return attackBounds.Intersects(enemyBounds);
    }

    private void UpdateCamera()
    {
        double viewportWidth = GetViewportWidth();
        double maxCameraX = Math.Max(0, _currentLevel!.LevelWidth - viewportWidth);
        double targetX = Math.Clamp(_player.Bounds.CenterX - viewportWidth / 2, 0, maxCameraX);

        if (!double.IsFinite(_cameraX))
        {
            _cameraX = targetX;
            return;
        }

        _cameraX = Math.Clamp(_cameraX + (targetX - _cameraX) * 0.1, 0, maxCameraX);
    }

    private double GetViewportWidth()
    {
        if (double.IsFinite(_canvas.ActualWidth) && _canvas.ActualWidth > 0)
            return _canvas.ActualWidth;

        if (double.IsFinite(_canvas.RenderSize.Width) && _canvas.RenderSize.Width > 0)
            return _canvas.RenderSize.Width;

        if (double.IsFinite(_canvas.Width) && _canvas.Width > 0)
            return _canvas.Width;

        return 900;
    }

    private void CheckLevelComplete()
    {
        if (_currentLevel!.Goal.Intersects(_player.Bounds))
        {
            if (_gameState.Level < 3)
            {
                _gameState.Level++;
                LoadLevel(_gameState.Level);
                NotifyStateChanged();
            }
            else
            {
                _gameState.State = GameStateType.Victory;
                OnLevelComplete?.Invoke(this, _gameState);
            }
        }
    }

    private void PlayerHit()
    {
        _gameState.Lives--;
        _player.IsInvincible = true;
        _player.InvincibleTimer = Player.InvincibleDuration;
        _player.VelocityY = -8;
        _player.VelocityX = _player.Facing == Direction.Right ? -5 : 5;
        NotifyStateChanged();
        
        if (_gameState.Lives <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        _gameState.Lives = 0;
        _gameState.State = GameStateType.GameOver;
        OnGameOver?.Invoke(this, _gameState);
    }

    private void OnHitQuestionBlock(Platform platform)
    {
        platform.IsBroken = true;
        _gameState.Coins++;
        _gameState.Score += 100;
        NotifyStateChanged();
        
        var coin = new Coin
        {
            Bounds = new GameRect(
                platform.Bounds.X + 10,
                platform.Bounds.Y - 30,
                20,
                20
            ),
            Value = 50,
            FloatOffset = 0
        };
        _coins.Add(coin);
    }

    private void NotifyStateChanged()
    {
        OnGameStateChanged?.Invoke(this, _gameState);
    }
}
