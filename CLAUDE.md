# StoryForge - Development Guidelines

## Build & Run Commands
- Build: `dotnet build StoryForge.sln` 
- Run: `dotnet run --project StoryForge.csproj`
- Build specific architecture: `dotnet build -r win-x64` (or win-x86, win-arm64)
- Publish: `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained`

## Code Style Guidelines
- **Nullable Reference Types**: Enable with `#nullable enable`
- **Namespaces**: Match folder structure (`StoryForge.Models`, `StoryForge.Services`)
- **Naming**: PascalCase for public members, _camelCase with underscore for private fields
- **Error Handling**: Use try/catch with specific exception types and proper logging
- **XAML**: Use element properties for simple settings, attached properties when needed
- **Async Pattern**: Always use async/await with Task, never .Result or .Wait()
- **Services**: Implement singleton pattern for services (see SettingsService)
- **Models**: Use primary constructor syntax when appropriate
- **UI Logic**: Keep code-behind minimal, use MVVM pattern where possible

## Architecture
- **Services**: For API communication, settings, and other non-UI functionality
- **Models**: For data structures and business objects
- **Pages**: For UI components with minimal code-behind logic