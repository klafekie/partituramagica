# CLAUDE.md

Guidance for working in `UnitTests/` (see root `CLAUDE.md`).

NUnit 3.12 + Moq + FluentAssertions, targeting `Lib`. Windows/.NET Framework only (project name is `Test.csproj`, output `Test.dll`).

- No UI is started: `Fake*` classes (`FakeFileSystem`, `FakeDrawingPanel`, `FakeImage`, `FakeEditingContext`) stand in for the interfaces `Lib` defines. Extend these fakes rather than mocking the same interface in several places.
- `TestUtils.cs` holds shared builders; check it before writing setup code.
- Test files group by area: `EditingTests` (commands), `PartituraTests` (domain), `SnippetManagerTests`, `SystemFinderTests`, `BoxFinderTests`.

Run a subset: `nunit3-console UnitTests/bin/Debug/Test.dll --where "class == PartituraMagica.Test.EditingTests"`.
