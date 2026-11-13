# CLAUDE.md

Guidance for working in `PartituraMagica/` (WPF editor app, namespace `PartituraMagica.App`; see root `CLAUDE.md`).

Windows-only (WPF, SlimDX Direct3D). Implements the UI interfaces declared in `Lib/`.

- `BasicRegistry.cs` - Lamar bindings from `Lib` interfaces to the WPF/D3D implementations. New UI-side services get registered here.
- `EditingController.cs` - implements `ICommandParameters`; holds the active partitura and drawing panel that commands operate on.
- `DrawingPanel.cs`, `ClippingD3D.cs`, `TextureCache.cs`, `GreyImage.cs` - page rendering and image loading.
- `Player/` - Direct3D player view (`PlayerViewD3D`), keyboard mapping, scrolling panel.
- `MainWindow.xaml(.cs)` - maps menu/toolbar actions to commands; keep logic in `Lib` commands, not code-behind.
