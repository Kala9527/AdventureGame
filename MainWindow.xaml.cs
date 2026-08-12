using System.Windows;
using System.Windows.Input;
using AdventureGame.Services;
using AdventureGame.Models;

namespace AdventureGame;

public partial class MainWindow : Window
{
    private readonly GameEngine _gameEngine;
    private readonly RenderService _renderService;
    private bool _leftKeyDown;
    private bool _rightKeyDown;
    private bool _jumpKeyDown;
    private bool _attackKeyDown;

    public MainWindow()
    {
        InitializeComponent();
        
        _gameEngine = new GameEngine(GameCanvas);
        _renderService = new RenderService(GameCanvas);
        _renderService.SetGameEngine(_gameEngine);
        
        _gameEngine.OnGameStateChanged += UpdateUI;
        _gameEngine.OnGameOver += ShowGameOver;
        _gameEngine.OnLevelComplete += ShowLevelComplete;
        
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 等待布局完成后再启动
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _gameEngine.Start();
            _renderService.Start();
            
            // 确保Window获得键盘焦点
            Focus();
            Keyboard.Focus(this);
            
            // 调试：输出Canvas实际尺寸
            System.Diagnostics.Debug.WriteLine($"Canvas ActualWidth={GameCanvas.ActualWidth}, ActualHeight={GameCanvas.ActualHeight}");
            System.Diagnostics.Debug.WriteLine($"Canvas Width={GameCanvas.Width}, Height={GameCanvas.Height}");
            System.Diagnostics.Debug.WriteLine($"Window ActualWidth={ActualWidth}, ActualHeight={ActualHeight}");
            System.Diagnostics.Debug.WriteLine("Game Started - Window Focused");
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        HandleKeyDown(e.Key);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        HandleKeyUp(e.Key);
    }

    private void HandleKeyDown(Key key)
    {
        switch (key)
        {
            case Key.Left:
            case Key.A:
                if (!_leftKeyDown)
                {
                    _leftKeyDown = true;
                    _gameEngine.SetInput(InputAction.MoveLeft, true);
                }
                break;
            case Key.Right:
            case Key.D:
                if (!_rightKeyDown)
                {
                    _rightKeyDown = true;
                    _gameEngine.SetInput(InputAction.MoveRight, true);
                }
                break;
            case Key.Up:
            case Key.W:
            case Key.Space:
                if (!_jumpKeyDown)
                {
                    _jumpKeyDown = true;
                    _gameEngine.SetInput(InputAction.Jump, true);
                }
                break;
            case Key.LeftShift:
            case Key.RightShift:
                if (!_attackKeyDown)
                {
                    _attackKeyDown = true;
                    _gameEngine.SetInput(InputAction.Attack, true);
                }
                break;
            case Key.P:
                _gameEngine.TogglePause();
                break;
            case Key.R:
                RestartGame();
                break;
        }
    }

    private void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Left:
            case Key.A:
                _leftKeyDown = false;
                _gameEngine.SetInput(InputAction.MoveLeft, false);
                break;
            case Key.Right:
            case Key.D:
                _rightKeyDown = false;
                _gameEngine.SetInput(InputAction.MoveRight, false);
                break;
            case Key.Up:
            case Key.W:
            case Key.Space:
                _jumpKeyDown = false;
                _gameEngine.SetInput(InputAction.Jump, false);
                break;
            case Key.LeftShift:
            case Key.RightShift:
                _attackKeyDown = false;
                _gameEngine.SetInput(InputAction.Attack, false);
                break;
        }
    }

    private void UpdateUI(object? sender, GameState state)
    {
        TextScore.Text = $"分数: {state.Score}";
        TextLives.Text = $"生命: {new string('♥', state.Lives)}";
        TextCoins.Text = $"金币: {state.Coins}";
        TextLevel.Text = $"关卡: {state.Level}";
    }

    private void ShowGameOver(object? sender, GameState state)
    {
        GameOverOverlay.Visibility = Visibility.Visible;
    }

    private void ShowLevelComplete(object? sender, GameState state)
    {
        WinOverlay.Visibility = Visibility.Visible;
        TextFinalScore.Text = $"最终得分: {state.Score}";
    }

    private void BtnRestart_Click(object sender, RoutedEventArgs e)
    {
        RestartGame();
        Keyboard.Focus(this);
    }

    private void RestartGame()
    {
        GameOverOverlay.Visibility = Visibility.Collapsed;
        WinOverlay.Visibility = Visibility.Collapsed;
        _gameEngine.Reset();
    }

    protected override void OnClosed(EventArgs e)
    {
        _gameEngine.Stop();
        _renderService.Stop();
        base.OnClosed(e);
    }
}
