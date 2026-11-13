# CLAUDE.md

Guidance for working in `WebPlayer/` (see root `CLAUDE.md`).

Create React App (react-scripts 4) + TypeScript + PixiJS via `@inlet/react-pixi`. Tabs for indentation.

```bash
yarn start
yarn build
yarn test --watchAll=false <name-filter>
```

## Structure

`src/App.tsx` composes React context providers in this order, and the order matters because inner ones consume outer ones:
`Configuration` -> `Partitura` -> `PlayablePartitura` -> `Textures` -> `Player` -> (`PlayerControls`, `KeyboardHandler`, `GestureHandler` -> `PixiStage`).

- `Configuration.tsx` - fetches `public/AppConfiguration.json`; all tunables (speeds, zoom limits, gesture/keyboard timings, font) belong there.
- `Partitura.tsx` - fetches the `.partitura2` JSON, camel-cases keys, converts boxes to texture coordinates. Currently hardcodes `/La Catedral.partitura2`.
- `PlayablePartitura.tsx` / `Player.tsx` - playback state and position.
- `PixiStage.tsx` - rendering only; keep logic out of it (earlier refactor removed it on purpose). Input goes through `GestureHandler` (react-use-gesture) and `KeyboardHandler`.
- The Pixi `Application` is created with its ticker stopped (`autoStart: false`); frames are driven by the player, not by Pixi.

The JSON shape in `Partitura.tsx` mirrors the C# `*JsonMemento` classes in `Lib/Domain/`; keep them in sync.
