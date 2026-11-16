# CLAUDE.md

Guidance for working in `UnitTests/` (see root `CLAUDE.md`).

NUnit 3.14 + Moq + FluentAssertions, targeting `Lib`. SDK-style project targeting `net8.0` (project name is `Test.csproj`, output `Test.dll`).

- No UI is started: `Fake*` classes (`FakeFileSystem`, `FakeDrawingPanel`, `FakeImage`, `FakeEditingContext`) stand in for the interfaces `Lib` defines. Extend these fakes rather than mocking the same interface in several places.
- `TestUtils.cs` holds shared builders; check it before writing setup code.
- Test files group by area: `EditingTests` (commands), `PartituraTests` (domain), `SnippetManagerTests`, `SystemFinderTests`, `BoxFinderTests`.

Run with `dotnet test UnitTests/Test.csproj`; a subset with `dotnet test UnitTests/Test.csproj --filter "FullyQualifiedName~EditingTests"`. Note: `PartituraTests` use Windows-style paths (`c:\\...`) that `System.IO.Path` only handles on Windows.
