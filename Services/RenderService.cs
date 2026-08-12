using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AdventureGame.Models;

namespace AdventureGame.Services;

public class RenderService
{
    private readonly Canvas _canvas;
    private GameEngine _gameEngine;
    private readonly DispatcherTimer _renderLoop;
    private double _canvasWidth;
    private double _canvasHeight;
    
    public RenderService(Canvas canvas)
    {
        _canvas = canvas;
        _gameEngine = null!;
        _renderLoop = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _renderLoop.Tick += RenderLoop_Tick;
        
        _canvasWidth = 900;  // 默认值
        _canvasHeight = 500; // 默认值
    }

    public void SetGameEngine(GameEngine engine)
    {
        _gameEngine = engine;
    }

    public void Start()
    {
        _renderLoop.Start();
        System.Diagnostics.Debug.WriteLine("RenderService Started");
    }

    public void Stop()
    {
        _renderLoop.Stop();
    }

    private void RenderLoop_Tick(object? sender, EventArgs e)
    {
        if (_gameEngine == null) return;
        if (_gameEngine.State.State == GameStateType.Paused) return;
        
        // 更新画布尺寸
        UpdateCanvasSize();
        
        Render();
    }
    
    private void UpdateCanvasSize()
    {
        // 使用ActualWidth/ActualHeight获取真实尺寸
        if (double.IsFinite(_canvas.ActualWidth) && _canvas.ActualWidth > 0)
            _canvasWidth = _canvas.ActualWidth;
        else if (double.IsFinite(_canvas.RenderSize.Width) && _canvas.RenderSize.Width > 0)
            _canvasWidth = _canvas.RenderSize.Width;
        else if (double.IsFinite(_canvas.Width) && _canvas.Width > 0)
            _canvasWidth = _canvas.Width;
            
        if (double.IsFinite(_canvas.ActualHeight) && _canvas.ActualHeight > 0)
            _canvasHeight = _canvas.ActualHeight;
        else if (double.IsFinite(_canvas.RenderSize.Height) && _canvas.RenderSize.Height > 0)
            _canvasHeight = _canvas.RenderSize.Height;
        else if (double.IsFinite(_canvas.Height) && _canvas.Height > 0)
            _canvasHeight = _canvas.Height;
    }

    private void Render()
    {
        var level = _gameEngine.CurrentLevel;
        if (level == null) return;
        
        _canvas.Children.Clear();
        
        double camX = _gameEngine.CameraX;
        if (!double.IsFinite(camX))
            camX = 0;
        
        DrawBackground(level);
        DrawPlatforms(level, camX);
        DrawCoins(camX);
        DrawEnemies(camX);
        DrawPlayer(camX);
        DrawGoal(level, camX);
        
        // 调试：每60帧输出画布尺寸
        if (_gameEngine.Player.AnimationFrame % 60 == 0)
        {
            System.Diagnostics.Debug.WriteLine($"Canvas Size: {_canvasWidth}x{_canvasHeight}, Player: ({_gameEngine.Player.Bounds.X:F1}, {_gameEngine.Player.Bounds.Y:F1}), Enemies: {_gameEngine.Enemies.Count}");
        }
    }

    private void DrawBackground(LevelData level)
    {
        var bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(level.BackgroundColor));
        var bgShape = new Rectangle
        {
            Fill = bgBrush,
            Width = _canvasWidth,
            Height = _canvasHeight
        };
        Canvas.SetLeft(bgShape, 0);
        Canvas.SetTop(bgShape, 0);
        _canvas.Children.Add(bgShape);
        
        DrawStars();
    }

    private void DrawStars()
    {
        var starBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        double offset = _gameEngine.CameraX * 0.1;
        
        for (int i = 0; i < 50; i++)
        {
            double x = ((i * 137 - offset) % 1200 + 1200) % 1200;
            double y = (i * 73) % 250 + 20;
            
            var star = new Ellipse
            {
                Fill = starBrush,
                Width = 2,
                Height = 2
            };
            Canvas.SetLeft(star, x);
            Canvas.SetTop(star, y);
            _canvas.Children.Add(star);
        }
    }

    private void DrawPlatforms(LevelData level, double camX)
    {
        foreach (var platform in _gameEngine.Platforms)
        {
            if (platform.IsBroken) continue;
            
            double screenX = platform.Bounds.X - camX;
            // 视图裁剪检查
            if (!IsFiniteRect(screenX, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height)) continue;
            if (screenX + platform.Bounds.Width < 0 || screenX > _canvasWidth) continue;
            
            switch (platform.Type)
            {
                case PlatformType.Ground:
                    DrawGroundBlock(screenX, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height, level.GroundColor);
                    break;
                case PlatformType.Brick:
                    DrawBrickBlock(screenX, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height);
                    break;
                case PlatformType.QuestionBlock:
                    DrawQuestionBlock(screenX, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height);
                    break;
                case PlatformType.FloatingPlatform:
                    DrawFloatingPlatform(screenX, platform.Bounds.Y, platform.Bounds.Width, platform.Bounds.Height);
                    break;
            }
        }
    }

    private void DrawGroundBlock(double x, double y, double width, double height, string color)
    {
        var mainBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        
        var baseColor = (Color)ColorConverter.ConvertFromString(color);
        var topColor = Color.FromRgb(
            (byte)Math.Min(255, baseColor.R + 30),
            (byte)Math.Min(255, baseColor.G + 30),
            (byte)Math.Min(255, baseColor.B + 30));
        var topBrush = new SolidColorBrush(topColor);
        
        var darkColor = Color.FromRgb(
            (byte)(baseColor.R / 2),
            (byte)(baseColor.G / 2),
            (byte)(baseColor.B / 2));
        var darkBrush = new SolidColorBrush(darkColor);
        
        var mainRect = new Rectangle
        {
            Fill = mainBrush,
            Width = width,
            Height = height,
            Stroke = darkBrush,
            StrokeThickness = 1
        };
        Canvas.SetLeft(mainRect, x);
        Canvas.SetTop(mainRect, y);
        _canvas.Children.Add(mainRect);
        
        var topRect = new Rectangle
        {
            Fill = topBrush,
            Width = width,
            Height = 6
        };
        Canvas.SetLeft(topRect, x);
        Canvas.SetTop(topRect, y);
        _canvas.Children.Add(topRect);
    }

    private void DrawBrickBlock(double x, double y, double width, double height)
    {
        var brickBrush = new SolidColorBrush(Color.FromRgb(180, 100, 50));
        var mortarBrush = new SolidColorBrush(Color.FromRgb(100, 60, 30));
        
        var mainRect = new Rectangle
        {
            Fill = brickBrush,
            Width = width,
            Height = height,
            Stroke = mortarBrush,
            StrokeThickness = 1
        };
        Canvas.SetLeft(mainRect, x);
        Canvas.SetTop(mainRect, y);
        _canvas.Children.Add(mainRect);
        
        int rows = (int)(height / 10);
        for (int row = 0; row < rows; row++)
        {
            double rowY = y + row * 10;
            var rowLine = new Line
            {
                Stroke = mortarBrush,
                X1 = x,
                Y1 = rowY,
                X2 = x + width,
                Y2 = rowY
            };
            _canvas.Children.Add(rowLine);
        }
        
        var highlightRect = new Rectangle
        {
            Fill = new SolidColorBrush(Color.FromRgb(220, 140, 80)),
            Width = width - 4,
            Height = 3
        };
        Canvas.SetLeft(highlightRect, x + 2);
        Canvas.SetTop(highlightRect, y + 2);
        _canvas.Children.Add(highlightRect);
    }

    private void DrawQuestionBlock(double x, double y, double width, double height)
    {
        var orangeBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0));
        var darkBrush = new SolidColorBrush(Color.FromRgb(200, 120, 0));
        var innerBrush = new SolidColorBrush(Color.FromRgb(255, 200, 50));
        
        var mainRect = new Rectangle
        {
            Fill = orangeBrush,
            Width = width,
            Height = height,
            Stroke = darkBrush,
            StrokeThickness = 2
        };
        Canvas.SetLeft(mainRect, x);
        Canvas.SetTop(mainRect, y);
        _canvas.Children.Add(mainRect);
        
        var innerRect = new Rectangle
        {
            Fill = innerBrush,
            Width = width - 8,
            Height = height - 8
        };
        Canvas.SetLeft(innerRect, x + 4);
        Canvas.SetTop(innerRect, y + 4);
        _canvas.Children.Add(innerRect);
        
        var textBlock = new TextBlock
        {
            Text = "?",
            Foreground = new SolidColorBrush(Colors.White),
            FontSize = height * 0.5,
            FontWeight = FontWeights.Bold
        };
        Canvas.SetLeft(textBlock, x + width * 0.35);
        Canvas.SetTop(textBlock, y + height * 0.25);
        _canvas.Children.Add(textBlock);
    }

    private void DrawFloatingPlatform(double x, double y, double width, double height)
    {
        var mainBrush = new SolidColorBrush(Color.FromRgb(139, 90, 43));
        var topBrush = new SolidColorBrush(Color.FromRgb(160, 120, 70));
        var borderBrush = new SolidColorBrush(Color.FromRgb(80, 50, 20));
        
        var mainRect = new Rectangle
        {
            Fill = mainBrush,
            Width = width,
            Height = height,
            Stroke = borderBrush,
            StrokeThickness = 1
        };
        Canvas.SetLeft(mainRect, x);
        Canvas.SetTop(mainRect, y);
        _canvas.Children.Add(mainRect);
        
        var topRect = new Rectangle
        {
            Fill = topBrush,
            Width = width,
            Height = Math.Min(4, height / 2)
        };
        Canvas.SetLeft(topRect, x);
        Canvas.SetTop(topRect, y);
        _canvas.Children.Add(topRect);
    }

    private void DrawCoins(double camX)
    {
        foreach (var coin in _gameEngine.Coins)
        {
            if (coin.IsCollected) continue;
            
            double screenX = coin.Bounds.X - camX;
            if (!IsFiniteRect(screenX, coin.Bounds.Y, coin.Bounds.Width, coin.Bounds.Height)) continue;
            if (screenX + coin.Bounds.Width < 0 || screenX > _canvasWidth) continue;
            
            double bobY = Math.Sin(coin.FloatOffset) * 3;
            double coinY = coin.Bounds.Y + bobY;
            
            double squeeze = Math.Abs(Math.Sin(coin.AnimationFrame * 0.1));
            double coinWidth = coin.Bounds.Width * (0.6 + 0.4 * squeeze);
            
            var coinBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0));
            var darkBrush = new SolidColorBrush(Color.FromRgb(200, 170, 0));
            var highlightBrush = new SolidColorBrush(Color.FromRgb(255, 255, 150));
            
            var coinEllipse = new Ellipse
            {
                Fill = coinBrush,
                Width = coinWidth,
                Height = coin.Bounds.Height
            };
            Canvas.SetLeft(coinEllipse, screenX + (coin.Bounds.Width - coinWidth) / 2);
            Canvas.SetTop(coinEllipse, coinY);
            _canvas.Children.Add(coinEllipse);
            
            var innerEllipse = new Ellipse
            {
                Fill = darkBrush,
                Width = coinWidth * 0.5,
                Height = coin.Bounds.Height * 0.6
            };
            Canvas.SetLeft(innerEllipse, screenX + coinWidth * 0.25 + (coin.Bounds.Width - coinWidth) / 2);
            Canvas.SetTop(innerEllipse, coinY + coin.Bounds.Height * 0.2);
            _canvas.Children.Add(innerEllipse);
        }
    }

    private void DrawEnemies(double camX)
    {
        foreach (var enemy in _gameEngine.Enemies)
        {
            if (!enemy.IsAlive) continue;
            
            double screenX = enemy.Bounds.X - camX;
            if (!IsFiniteRect(screenX, enemy.Bounds.Y, enemy.Bounds.Width, enemy.Bounds.Height)) continue;
            if (screenX + enemy.Bounds.Width < 0 || screenX > _canvasWidth) continue;
            
            switch (enemy.Type)
            {
                case EnemyType.Goomba:
                    DrawGoomba(screenX, enemy.Bounds.Y, enemy.Bounds.Width, enemy.Bounds.Height, enemy.Facing);
                    break;
                case EnemyType.Koopa:
                    DrawKoopa(screenX, enemy.Bounds.Y, enemy.Bounds.Width, enemy.Bounds.Height, enemy.Facing);
                    break;
                case EnemyType.Spike:
                    DrawSpike(screenX, enemy.Bounds.Y, enemy.Bounds.Width, enemy.Bounds.Height);
                    break;
                case EnemyType.FlyingEnemy:
                    DrawFlyingEnemy(screenX, enemy.Bounds.Y, enemy.Bounds.Width, enemy.Bounds.Height, enemy);
                    break;
            }
        }
    }

    private void DrawGoomba(double x, double y, double width, double height, Direction facing)
    {
        var bodyBrush = new SolidColorBrush(Color.FromRgb(139, 69, 19));
        var feetBrush = new SolidColorBrush(Color.FromRgb(80, 40, 10));
        var eyeBrush = new SolidColorBrush(Colors.White);
        var pupilBrush = new SolidColorBrush(Colors.Black);
        
        var bodyEllipse = new Ellipse
        {
            Fill = bodyBrush,
            Width = width - 4,
            Height = height - 8
        };
        Canvas.SetLeft(bodyEllipse, x + 2);
        Canvas.SetTop(bodyEllipse, y + 2);
        _canvas.Children.Add(bodyEllipse);
        
        var leftFoot = new Rectangle
        {
            Fill = feetBrush,
            Width = 10,
            Height = 6
        };
        Canvas.SetLeft(leftFoot, x + 4);
        Canvas.SetTop(leftFoot, y + height - 6);
        _canvas.Children.Add(leftFoot);
        
        var rightFoot = new Rectangle
        {
            Fill = feetBrush,
            Width = 10,
            Height = 6
        };
        Canvas.SetLeft(rightFoot, x + width - 14);
        Canvas.SetTop(rightFoot, y + height - 6);
        _canvas.Children.Add(rightFoot);
        
        double eyeY = y + height * 0.35;
        double eyeSpacing = width * 0.25;
        
        var leftEye = new Ellipse
        {
            Fill = eyeBrush,
            Width = 10,
            Height = 14
        };
        Canvas.SetLeft(leftEye, x + width / 2 - eyeSpacing - 5);
        Canvas.SetTop(leftEye, eyeY);
        _canvas.Children.Add(leftEye);
        
        var rightEye = new Ellipse
        {
            Fill = eyeBrush,
            Width = 10,
            Height = 14
        };
        Canvas.SetLeft(rightEye, x + width / 2 + eyeSpacing - 5);
        Canvas.SetTop(rightEye, eyeY);
        _canvas.Children.Add(rightEye);
        
        double pupilOffset = facing == Direction.Left ? -2 : 2;
        
        var leftPupil = new Ellipse
        {
            Fill = pupilBrush,
            Width = 4,
            Height = 6
        };
        Canvas.SetLeft(leftPupil, x + width / 2 - eyeSpacing - 2 + pupilOffset);
        Canvas.SetTop(leftPupil, eyeY + 2);
        _canvas.Children.Add(leftPupil);
        
        var rightPupil = new Ellipse
        {
            Fill = pupilBrush,
            Width = 4,
            Height = 6
        };
        Canvas.SetLeft(rightPupil, x + width / 2 + eyeSpacing - 2 + pupilOffset);
        Canvas.SetTop(rightPupil, eyeY + 2);
        _canvas.Children.Add(rightPupil);
    }

    private void DrawKoopa(double x, double y, double width, double height, Direction facing)
    {
        var shellBrush = new SolidColorBrush(Color.FromRgb(0, 150, 0));
        var shellDarkBrush = new SolidColorBrush(Color.FromRgb(0, 100, 0));
        var bodyBrush = new SolidColorBrush(Color.FromRgb(255, 255, 100));
        var feetBrush = new SolidColorBrush(Color.FromRgb(255, 200, 0));
        
        double shellOffset = facing == Direction.Left ? 0 : -5;
        
        var shellEllipse = new Ellipse
        {
            Fill = shellBrush,
            Width = width - shellOffset,
            Height = height - 14
        };
        Canvas.SetLeft(shellEllipse, x + shellOffset);
        Canvas.SetTop(shellEllipse, y + 8);
        _canvas.Children.Add(shellEllipse);
        
        var shellDarkEllipse = new Ellipse
        {
            Fill = shellDarkBrush,
            Width = (width - shellOffset) - 6,
            Height = (height - 14) / 2
        };
        Canvas.SetLeft(shellDarkEllipse, x + shellOffset + 3);
        Canvas.SetTop(shellDarkEllipse, y + 11);
        _canvas.Children.Add(shellDarkEllipse);
        
        var headEllipse = new Ellipse
        {
            Fill = bodyBrush,
            Width = 15,
            Height = 12
        };
        Canvas.SetLeft(headEllipse, x + shellOffset);
        Canvas.SetTop(headEllipse, y);
        _canvas.Children.Add(headEllipse);
        
        double eyeX = facing == Direction.Left ? x + shellOffset + 4 : x + shellOffset + 11;
        var eye = new Ellipse
        {
            Fill = new SolidColorBrush(Colors.White),
            Width = 6,
            Height = 8
        };
        Canvas.SetLeft(eye, eyeX);
        Canvas.SetTop(eye, y + 3);
        _canvas.Children.Add(eye);
        
        var pupil = new Ellipse
        {
            Fill = new SolidColorBrush(Colors.Black),
            Width = 3,
            Height = 4
        };
        Canvas.SetLeft(pupil, eyeX + (facing == Direction.Left ? -1 : 2));
        Canvas.SetTop(pupil, y + 5);
        _canvas.Children.Add(pupil);
        
        var leftFoot = new Rectangle
        {
            Fill = feetBrush,
            Width = 8,
            Height = 4
        };
        Canvas.SetLeft(leftFoot, x + 6);
        Canvas.SetTop(leftFoot, y + height - 4);
        _canvas.Children.Add(leftFoot);
        
        var rightFoot = new Rectangle
        {
            Fill = feetBrush,
            Width = 8,
            Height = 4
        };
        Canvas.SetLeft(rightFoot, x + width - 14);
        Canvas.SetTop(rightFoot, y + height - 4);
        _canvas.Children.Add(rightFoot);
    }

    private void DrawSpike(double x, double y, double width, double height)
    {
        var spikeBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        var darkBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        
        int spikes = 3;
        double spikeWidth = width / spikes;
        
        for (int i = 0; i < spikes; i++)
        {
            double spikeX = x + i * spikeWidth;
            
            var triangle = new Polygon
            {
                Fill = spikeBrush,
                Stroke = darkBrush,
                StrokeThickness = 1,
                Points = new PointCollection
                {
                    new Point(spikeX, y + height),
                    new Point(spikeX + spikeWidth / 2, y),
                    new Point(spikeX + spikeWidth, y + height)
                }
            };
            _canvas.Children.Add(triangle);
        }
    }

    private void DrawFlyingEnemy(double x, double y, double width, double height, Enemy enemy)
    {
        var bodyBrush = new SolidColorBrush(Color.FromRgb(150, 50, 150));
        var wingBrush = new SolidColorBrush(Color.FromRgb(200, 100, 200));
        var eyeBrush = new SolidColorBrush(Colors.Red);
        var pupilBrush = new SolidColorBrush(Colors.White);
        
        double wingFlap = Math.Sin(enemy.AnimationFrame * 0.3) * 8;
        
        var leftWing = new Polygon
        {
            Fill = wingBrush,
            Points = new PointCollection
            {
                new Point(x + 5, y + 5),
                new Point(x - 10, y - 10 + wingFlap),
                new Point(x - 5, y + 10 + wingFlap),
                new Point(x + 5, y + height / 2)
            }
        };
        _canvas.Children.Add(leftWing);
        
        var rightWing = new Polygon
        {
            Fill = wingBrush,
            Points = new PointCollection
            {
                new Point(x + width - 5, y + 5),
                new Point(x + width + 10, y - 10 + wingFlap),
                new Point(x + width + 5, y + 10 + wingFlap),
                new Point(x + width - 5, y + height / 2)
            }
        };
        _canvas.Children.Add(rightWing);
        
        var bodyEllipse = new Ellipse
        {
            Fill = bodyBrush,
            Width = width - 16,
            Height = height - 10
        };
        Canvas.SetLeft(bodyEllipse, x + 8);
        Canvas.SetTop(bodyEllipse, y + 5);
        _canvas.Children.Add(bodyEllipse);
        
        double eyeY = y + height * 0.35;
        var leftEye = new Ellipse
        {
            Fill = eyeBrush,
            Width = 8,
            Height = 10
        };
        Canvas.SetLeft(leftEye, x + width / 2 - 10);
        Canvas.SetTop(leftEye, eyeY);
        _canvas.Children.Add(leftEye);
        
        var rightEye = new Ellipse
        {
            Fill = eyeBrush,
            Width = 8,
            Height = 10
        };
        Canvas.SetLeft(rightEye, x + width / 2 + 2);
        Canvas.SetTop(rightEye, eyeY);
        _canvas.Children.Add(rightEye);
        
        var leftPupil = new Ellipse
        {
            Fill = pupilBrush,
            Width = 3,
            Height = 4
        };
        Canvas.SetLeft(leftPupil, x + width / 2 - 8);
        Canvas.SetTop(leftPupil, eyeY + 2);
        _canvas.Children.Add(leftPupil);
        
        var rightPupil = new Ellipse
        {
            Fill = pupilBrush,
            Width = 3,
            Height = 4
        };
        Canvas.SetLeft(rightPupil, x + width / 2 + 4);
        Canvas.SetTop(rightPupil, eyeY + 2);
        _canvas.Children.Add(rightPupil);
    }

    private void DrawPlayer(double camX)
    {
        var player = _gameEngine.Player;
        double screenX = player.Bounds.X - camX;
        if (!IsFiniteRect(screenX, player.Bounds.Y, player.Bounds.Width, player.Bounds.Height)) return;
        
        // 调试：输出玩家位置
        if (player.AnimationFrame % 60 == 0)
        {
            System.Diagnostics.Debug.WriteLine($"DrawPlayer: WorldX={player.Bounds.X:F1}, ScreenX={screenX:F1}, Y={player.Bounds.Y:F1}, CamX={camX:F1}, Bounds=({player.Bounds.X:F1},{player.Bounds.Y:F1},{player.Bounds.Width},{player.Bounds.Height})");
        }
        
        if (player.IsInvincible && (player.InvincibleTimer % 8 < 4))
            return;
        
        DrawPixelPlayer(screenX, player.Bounds.Y, player.Bounds.Width, player.Bounds.Height, player);
    }

    private void DrawPixelPlayer(double x, double y, double width, double height, Player player)
    {
        var hatBrush = new SolidColorBrush(Color.FromRgb(200, 50, 50));
        var shirtBrush = new SolidColorBrush(Color.FromRgb(200, 50, 50));
        var pantsBrush = new SolidColorBrush(Color.FromRgb(50, 50, 200));
        var skinBrush = new SolidColorBrush(Color.FromRgb(255, 200, 150));
        var eyeBrush = new SolidColorBrush(Colors.Black);
        var shoeBrush = new SolidColorBrush(Color.FromRgb(100, 50, 0));
        var buttonBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        
        double scaleX = player.Facing == Direction.Left ? -1 : 1;
        
        var hatRect = new Rectangle
        {
            Fill = hatBrush,
            Width = width - 8,
            Height = 10
        };
        Canvas.SetLeft(hatRect, x + 4);
        Canvas.SetTop(hatRect, y);
        _canvas.Children.Add(hatRect);
        
        var hatBrimRect = new Rectangle
        {
            Fill = hatBrush,
            Width = width - 4,
            Height = 4
        };
        Canvas.SetLeft(hatBrimRect, x + 2);
        Canvas.SetTop(hatBrimRect, y + 8);
        _canvas.Children.Add(hatBrimRect);
        
        double hatLogoX = scaleX > 0 ? x + width * 0.6 : x + width * 0.2;
        var hatLogoRect = new Rectangle
        {
            Fill = buttonBrush,
            Width = 6,
            Height = 5
        };
        Canvas.SetLeft(hatLogoRect, hatLogoX);
        Canvas.SetTop(hatLogoRect, y + 3);
        _canvas.Children.Add(hatLogoRect);
        
        var faceRect = new Rectangle
        {
            Fill = skinBrush,
            Width = width - 12,
            Height = 14
        };
        Canvas.SetLeft(faceRect, x + 6);
        Canvas.SetTop(faceRect, y + 12);
        _canvas.Children.Add(faceRect);
        
        double eyeX = scaleX > 0 ? x + width * 0.55 : x + width * 0.25;
        var eyeRect = new Rectangle
        {
            Fill = eyeBrush,
            Width = 4,
            Height = 5
        };
        Canvas.SetLeft(eyeRect, eyeX);
        Canvas.SetTop(eyeRect, y + 16);
        _canvas.Children.Add(eyeRect);
        
        var mustacheRect = new Rectangle
        {
            Fill = eyeBrush,
            Width = width - 16,
            Height = 3
        };
        Canvas.SetLeft(mustacheRect, x + 8);
        Canvas.SetTop(mustacheRect, y + 22);
        _canvas.Children.Add(mustacheRect);
        
        var shirtY = y + 26;
        var shirtRect = new Rectangle
        {
            Fill = shirtBrush,
            Width = width - 8,
            Height = 10
        };
        Canvas.SetLeft(shirtRect, x + 4);
        Canvas.SetTop(shirtRect, shirtY);
        _canvas.Children.Add(shirtRect);
        
        double buttonX = x + width * 0.45;
        for (int i = 0; i < 2; i++)
        {
            var buttonRect = new Rectangle
            {
                Fill = buttonBrush,
                Width = 3,
                Height = 3
            };
            Canvas.SetLeft(buttonRect, buttonX);
            Canvas.SetTop(buttonRect, shirtY + 2 + i * 5);
            _canvas.Children.Add(buttonRect);
        }
        
        if (player.IsAttacking)
        {
            double armX = scaleX > 0 ? x + width : x - 12;
            var armRect = new Rectangle
            {
                Fill = shirtBrush,
                Width = 12,
                Height = 6
            };
            Canvas.SetLeft(armRect, armX);
            Canvas.SetTop(armRect, y + 28);
            _canvas.Children.Add(armRect);
            
            var fistRect = new Rectangle
            {
                Fill = skinBrush,
                Width = 8,
                Height = 10
            };
            double fistX = scaleX > 0 ? x + width + 8 : x - 16;
            Canvas.SetLeft(fistRect, fistX);
            Canvas.SetTop(fistRect, y + 26);
            _canvas.Children.Add(fistRect);
        }
        else
        {
            var leftArmRect = new Rectangle
            {
                Fill = shirtBrush,
                Width = 4,
                Height = 8
            };
            Canvas.SetLeft(leftArmRect, x);
            Canvas.SetTop(leftArmRect, y + 28);
            _canvas.Children.Add(leftArmRect);
            
            var rightArmRect = new Rectangle
            {
                Fill = shirtBrush,
                Width = 4,
                Height = 8
            };
            Canvas.SetLeft(rightArmRect, x + width - 4);
            Canvas.SetTop(rightArmRect, y + 28);
            _canvas.Children.Add(rightArmRect);
        }
        
        var pantsY = y + 36;
        var pantsRect = new Rectangle
        {
            Fill = pantsBrush,
            Width = width - 8,
            Height = 8
        };
        Canvas.SetLeft(pantsRect, x + 4);
        Canvas.SetTop(pantsRect, pantsY);
        _canvas.Children.Add(pantsRect);
        
        double legOffset = 0;
        if (player.State == PlayerState.Running)
        {
            legOffset = Math.Sin(player.AnimationFrame * 0.3) * 3;
        }
        else if (player.State == PlayerState.Jumping)
        {
            legOffset = -3;
        }
        else if (player.State == PlayerState.Falling)
        {
            legOffset = 3;
        }
        
        double legY = pantsY + 8 + legOffset;
        
        var leftLegRect = new Rectangle
        {
            Fill = pantsBrush,
            Width = 8,
            Height = 6
        };
        Canvas.SetLeft(leftLegRect, x + 6);
        Canvas.SetTop(leftLegRect, legY);
        _canvas.Children.Add(leftLegRect);
        
        var rightLegRect = new Rectangle
        {
            Fill = pantsBrush,
            Width = 8,
            Height = 6
        };
        Canvas.SetLeft(rightLegRect, x + width - 14);
        Canvas.SetTop(rightLegRect, legY);
        _canvas.Children.Add(rightLegRect);
        
        var leftShoeRect = new Rectangle
        {
            Fill = shoeBrush,
            Width = 12,
            Height = 6
        };
        Canvas.SetLeft(leftShoeRect, x + 4);
        Canvas.SetTop(leftShoeRect, y + height - 6 + legOffset);
        _canvas.Children.Add(leftShoeRect);
        
        var rightShoeRect = new Rectangle
        {
            Fill = shoeBrush,
            Width = 12,
            Height = 6
        };
        Canvas.SetLeft(rightShoeRect, x + width - 16);
        Canvas.SetTop(rightShoeRect, y + height - 6 + legOffset);
        _canvas.Children.Add(rightShoeRect);
    }

    private void DrawGoal(LevelData level, double camX)
    {
        double screenX = level.Goal.X - camX;
        if (!IsFiniteRect(screenX, level.Goal.Y, level.Goal.Width, level.Goal.Height)) return;
        if (screenX + level.Goal.Width < 0 || screenX > _canvasWidth) return;
        
        var poleBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        var flagBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        var baseBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        
        double poleX = screenX + level.Goal.Width / 2;
        
        var poleRect = new Rectangle
        {
            Fill = poleBrush,
            Width = 6,
            Height = level.Goal.Height
        };
        Canvas.SetLeft(poleRect, poleX - 3);
        Canvas.SetTop(poleRect, level.Goal.Y);
        _canvas.Children.Add(poleRect);
        
        int flagWave = (int)(DateTime.Now.Millisecond / 100) % 2;
        var flagPoints = new PointCollection
        {
            new Point(poleX, level.Goal.Y + 10),
            new Point(poleX + 25 - flagWave * 3, level.Goal.Y + 18),
            new Point(poleX, level.Goal.Y + 26)
        };
        var flag = new Polygon
        {
            Fill = flagBrush,
            Points = flagPoints
        };
        _canvas.Children.Add(flag);
        
        var baseRect = new Rectangle
        {
            Fill = baseBrush,
            Width = level.Goal.Width + 10,
            Height = 8
        };
        Canvas.SetLeft(baseRect, screenX - 5);
        Canvas.SetTop(baseRect, level.Goal.Bottom - 8);
        _canvas.Children.Add(baseRect);
    }

    private static bool IsFiniteRect(double x, double y, double width, double height)
    {
        return double.IsFinite(x) &&
               double.IsFinite(y) &&
               double.IsFinite(width) &&
               double.IsFinite(height) &&
               width > 0 &&
               height > 0;
    }
}
