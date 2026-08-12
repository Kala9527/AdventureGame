# Rendering Troubleshooting Notes

## Issue: Player and Parts of the UI Did Not Render

Date: 2026-08-06

### Symptoms

- The game window opened successfully.
- The HUD and some ground/platform elements were visible.
- The player, coins, enemies, and parts of the UI scene were missing or stopped rendering.
- The `error.log` file contained runtime WPF rendering exceptions similar to:

```text
System.ArgumentException: "NaN" is not a valid value for property "X1".
```

### Root Cause

`GameEngine.UpdateCamera()` used `Canvas.Width` to calculate the camera position:

```csharp
double targetX = _player.Bounds.CenterX - _canvas.Width / 2;
```

In WPF, a `Canvas` whose width is controlled by layout often has `Width = NaN`. The real rendered size is available through `ActualWidth` or `RenderSize.Width`.

Once `Canvas.Width` produced `NaN`, the camera position also became `NaN`. That corrupted downstream screen coordinates such as:

```csharp
double screenX = worldX - camX;
```

When those invalid coordinates were passed into WPF shape properties such as `Canvas.SetLeft`, `Line.X1`, or `Line.X2`, rendering failed and later objects were not drawn.

### Fix

The camera calculation now uses the actual viewport width instead of `Canvas.Width`.

Implementation:

- Prefer `_canvas.ActualWidth`.
- Fall back to `_canvas.RenderSize.Width`.
- Fall back to `_canvas.Width` only when it is finite and positive.
- Use a final default width of `900`.
- Clamp the camera value to the valid level range.
- Reset the camera if it ever becomes non-finite.

The render service now also protects drawing code from invalid values:

- Canvas size updates use `ActualWidth/ActualHeight` and `RenderSize`.
- Camera `NaN` is treated as `0` during rendering.
- Platforms, coins, enemies, player, and goal are skipped if their computed draw rectangle contains `NaN`, `Infinity`, or invalid dimensions.

### Related Fix

While testing, a separate pause-state issue was found:

- Pressing `P` paused the game.
- Pressing `P` again did not resume because pause toggling only ran while the state was `Playing`.
- Restarting could also keep the internal `_isPaused` flag set.

Fix:

- `TogglePause()` now handles both `Playing` and `Paused`.
- `Start()` and `Reset()` clear `_isPaused`.

### Verification

Commands run:

```powershell
dotnet build
```

Result:

- Build succeeded.
- 0 warnings.
- 0 errors.

Runtime checks:

- Started `bin\Debug\net10.0-windows\AdventureGame.exe`.
- Captured the running game window.
- Confirmed HUD, bottom controls, player, coins, platforms, enemies, and background rendered correctly.
- Simulated right movement.
- Confirmed gameplay continued and falling into a gap triggered the game-over overlay.
- Confirmed `error.log` did not grow during the verification run.

### Prevention Notes

- Avoid using `FrameworkElement.Width` or `Height` for layout-controlled WPF elements unless explicitly set.
- Prefer `ActualWidth`, `ActualHeight`, or `RenderSize` for runtime viewport calculations.
- Guard rendering code against `NaN` and `Infinity` before assigning values to WPF shape or Canvas properties.
- When a visual bug is suspected, run the actual app and capture the window instead of relying only on successful compilation.
