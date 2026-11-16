# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

PartituraMagica turns scanned sheet-music pages into a "playable" score: the user draws boxes around staff systems on page images, labels them as parts, and a player then scrolls/navigates through the boxes (e.g. for a musician reading from a screen). The repo holds a legacy Windows desktop app plus a newer web player that is replacing its playback half.

## Components

| Folder | What | Stack |
|---|---|---|
| `Lib/` | Core domain + commands + player logic, UI-agnostic | C# / netstandard2.0 |
| `PartituraMagica/` | WPF editor app (`PartituraMagica.App`), Direct3D player view | C#, WPF, SlimDX |
| `UnitTests/` | NUnit + Moq + FluentAssertions tests for `Lib` | C# |
| `PartituraFileConverter/` | Converts old binary `.partitura` to the JSON `.partitura2` format | C# |
| `PlayerView/`, `PlayerViewDX/`, `Direct3D/`, `FormApplication/`, `Contract/` | Older/experimental player front ends and shared contracts | C# WinForms |
| `WebPlayer/` | Browser player (the active development area) | React 17, TypeScript, PixiJS, Create React App |
| `ImageServer/` | Express server that lists/serves partitura files and images | Node, TypeScript |
| `nextjs/partitura-magica/` | Next.js scaffold, currently the stock template | Next 12 |
| `Examples/`, `TestPartituras/` | Sample score files and page images | data |

Each of the active folders has its own `CLAUDE.md` with details.

## Commands

`Lib/` and `UnitTests/` are SDK-style projects (`netstandard2.0` / `net8.0`). The rest of `PartituraMagica.sln` is old-style .csproj with `packages/` restore and **only builds on Windows** (needs .NET Framework 4.8, WPF, SlimDX).

```bash
# Lib (netstandard2.0) and UnitTests (net8.0) are SDK-style and build/test anywhere
dotnet test UnitTests/Test.csproj
dotnet test UnitTests/Test.csproj --filter "FullyQualifiedName~SystemFinderTests"   # subset

# Windows only: the full solution (WPF app etc.)
msbuild PartituraMagica.sln /restore
```

Web player (`WebPlayer/`, uses yarn, but a package-lock.json also exists):
```bash
yarn start        # dev server on :3000
yarn build
yarn test                                   # watch mode
yarn test --watchAll=false App              # single run, filtered by name
```

Image server (`ImageServer/`):
```bash
npm start         # runs tslint --fix, tsc, then node . on :8080
```

## Architecture (big picture)

**Command pattern + DI is the spine of the desktop app.** Every user action is an `ICommand` (`Lib/Domain/ICommand.cs`) in `Lib/PageEditing/` (box editing) or `Lib/PartituraCommands/` (open/save/navigate pages). Commands are auto-registered by an assembly scan in `PartituraMagica/BasicRegistry.cs` (`AddAllTypesOf<ICommand>().NameBy(...)`, name = class name minus `Command`) and resolved via `CommandFactory` (`AddBoxCommand` -> `"AddBox"`). A new command class only needs to live in a scanned assembly and keep the `Command` suffix. Commands get their context via `ICommandParameters` (implemented by `EditingController` in the WPF project).

**Lib is UI-free; the UI implements its interfaces.** `Lib` defines `IMainWindow`, `IDrawingPanel`, `IPlayerView`, `IFileSystem`, `IImageLoader`, etc. `Lib/LibServiceRegistry.cs` registers the logic singletons (`PlayerController`, `SnippetManager`, ...); `PartituraMagica/BasicRegistry.cs` binds the UI-side implementations. Tests substitute `Fake*` classes in `UnitTests/`.

**Domain model** (`Lib/Domain/`): `Partitura` -> `Page`s -> `Box`es (a rectangle in page coordinates plus `Splitters` that cut a box into columns and `LabelIds` assigning it to parts). `EditedPartitura` is the mutable editing wrapper. Persistence goes through memento classes (`PartituraMemento`, `PageMemento`, ...).

**Two persisted formats, don't confuse them:**
- Legacy `.partitura` / `.partitura.runtime`: binary .NET serialization (`PartituraMemento`), only readable by the C# app.
- `.partitura2` / `.runtime2`: JSON via `*JsonMemento` classes (PascalCase keys). The web player reads these. `Examples/` and `WebPlayer/public/` contain samples. Changing a `*JsonMemento` property changes the contract with `WebPlayer/src/Partitura.tsx` (which camel-cases the keys on load).

**Web player data flow:** `App.tsx` nests React context providers in a fixed order: `Configuration` (loads `/AppConfiguration.json`) -> `Partitura` (fetches the `.partitura2`) -> `PlayablePartitura` -> `Textures` -> `Player` -> input handlers (`KeyboardHandler`, `GestureHandler`) -> `PixiStage` (renders only; all logic was deliberately moved out of it). Tuning constants (speeds, zoom limits, gesture thresholds) live in `public/AppConfiguration.json`, not in code.

## Known rough edges

- `WebPlayer/src/Partitura.tsx` hardcodes the score to load (`/La Catedral.partitura2`) and a screen-coordinate factor; `ImageServer/src/index.ts` hardcodes a local Windows path as the score root and its `/image` route is a stub. Treat these as work in progress.
- `ImageServer/dist/` is committed build output.
- Code uses tabs for indentation throughout (C# and TS).
