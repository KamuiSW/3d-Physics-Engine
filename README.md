# 3D Physics Engine

A C#-based 3D game engine with realistic physics and Unity-like editor features.

## Project Structure

- `src/GameEngine.Core` - Core engine functionality
- `src/GameEngine.Editor` - Editor application with UI
- `src/GameEngine.ProjectManager` - Project management tool

## Requirements

- .NET 7.0 SDK
- Visual Studio 2022 or JetBrains Rider (recommended)
- OpenGL 4.5+ capable graphics card

## Building

1. Clone the repository
2. Open `3d-Physics-Engine.sln` in your IDE
3. Build the solution

```bash
# Or build from command line
dotnet build
```

## Running

1. Set `GameEngine.ProjectManager` as the startup project
2. Run the application

```bash
# Or run from command line
dotnet run --project src/GameEngine.ProjectManager/GameEngine.ProjectManager.csproj
```

## Development

The project follows the roadmap defined in `roadmap.md`. Current phase: Phase 1 - Core Infrastructure

### Key Features (In Progress)

- Project Management System
- Dockable Window Framework
- Scene Management
- 3D Viewport
- Asset Management

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
This project is licensed under the MIT License - see the LICENSE file for details.
