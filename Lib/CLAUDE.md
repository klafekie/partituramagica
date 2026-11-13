# CLAUDE.md

Guidance for working in `Lib/` (see root `CLAUDE.md` for the overall picture).

UI-agnostic core: domain model, commands and player logic. Must not reference WPF/WinForms/SlimDX; reach the UI only through the interfaces in `Domain/` (`IMainWindow`, `IDrawingPanel`, `IPlayerView`, `IFileSystem`, `IImageLoader`, ...).

- `Domain/` - `Partitura`/`Page`/`Box`/`Part`/`Rect`, mementos (binary `*Memento`, JSON `*JsonMemento`), `CommandFactory`, `EditedPartitura`.
- `PageEditing/` - commands that edit boxes on the current page, plus `BoxLayouter` and `SystemFinder` (auto-detects staff systems for `AutoFillCommand`).
- `PartituraCommands/` - app-level commands (open, save, new, page navigation, startup/close).
- `Playing/` - player state: `PlayerController` coordinates `PartNavigator`, `SnippetManager`, `SpeedManager`, `ZoomManager`; `PartPositionCalculator` maps parts to scroll positions.
- `LibServiceRegistry.cs` - Lamar registrations for the logic singletons.

Adding a command: create `FooCommand : CommandBase` (or `ICommand`), keep the `Command` suffix (the scan in `PartituraMagica/BasicRegistry.cs` registers it under the name `Foo`), and create it via `ICommandFactory.Create<FooCommand>()`. Add a test using the fakes in `UnitTests/`.

Changing a `*JsonMemento` property changes the `.partitura2` file format consumed by `WebPlayer/`; update `WebPlayer/src/Partitura.tsx` accordingly.

.NET Framework 4.8, tabs for indentation, usings inside `#region usings`.
