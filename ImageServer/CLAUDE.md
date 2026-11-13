# CLAUDE.md

Guidance for working in `ImageServer/` (see root `CLAUDE.md`).

Small Express + TypeScript server (single file `src/index.ts`, port 8080). Serves a directory of partitura files: `/open` is a directory index filtered to `.partitura2` files and extension-less folders, `/partitura` serves the files statically, `/image/:path/:x.:y` is an unfinished stub.

```bash
npm start   # prebuild runs tslint --fix, then tsc into dist/, then node .
```

- `partituraRoot` is hardcoded to a local Windows path; make it configurable before running elsewhere.
- `dist/` is committed; rebuilding changes it.
- No tests (`npm test` is the npm placeholder).
