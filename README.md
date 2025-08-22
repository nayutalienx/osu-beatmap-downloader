# osu! Beatmap Downloader

A command-line tool for downloading osu! beatmaps by their IDs. This application uses the official osu! API to authenticate and download beatmapsets in `.osz` format.

## Features

- **Bulk Beatmap Downloads**: Download multiple beatmaps at once using IDs
- **File Input Support**: Load beatmap IDs from JSON files
- **ID Conversion**: Convert individual beatmap IDs to beatmapset IDs via osu! API
- **Smart Caching**: Avoid re-downloading already processed beatmaps
- **Progress Tracking**: Real-time download progress with animated progress bars
- **OAuth Authentication**: Secure authentication using osu! credentials
- **No Video Downloads**: Downloads exclude video files for faster transfers

## Prerequisites

- [.NET 6.0 Runtime](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- Valid osu! account credentials
- Internet connection

## Installation

1. **Download or Clone the Repository**
   ```bash
   git clone https://github.com/nayutalienx/osu-beatmap-downloader.git
   cd osu-beatmap-downloader
   ```

2. **Build the Application**
   ```bash
   dotnet build --configuration Release
   ```

3. **Run the Application**
   ```bash
   dotnet run -- [options]
   ```
   
   Or use the built executable:
   ```bash
   ./bin/Release/net6.0/OsuBeatmapDownloader [options]
   ```

## Usage

### Basic Syntax
```bash
OsuBeatmapDownloader.exe -login=username -password=password [options]
```

### Command-Line Options

| Option | Description | Required | Example |
|--------|-------------|----------|---------|
| `-login=` | Your osu! username | ✅ | `-login=myusername` |
| `-password=` | Your osu! password | ✅ | `-password=mypassword` |
| `-ids=` | Comma-separated beatmap/beatmapset IDs | * | `-ids=1234,5678,9012` |
| `-input=` | Path to JSON file containing beatmap IDs | * | `-input=beatmaps.json` |
| `-convert` | Convert beatmap IDs to beatmapset IDs | ❌ | `-convert` |
| `-help` | Display help information | ❌ | `-help` |

\* Either `-ids` or `-input` is required

### Examples

#### Download specific beatmapsets by ID
```bash
OsuBeatmapDownloader.exe -login=myuser -password=mypass -ids=123456,789012,345678
```

#### Download beatmaps from a JSON file
```bash
OsuBeatmapDownloader.exe -login=myuser -password=mypass -input=my_beatmaps.json
```

#### Convert beatmap IDs to beatmapset IDs and download
```bash
OsuBeatmapDownloader.exe -login=myuser -password=mypass -convert -ids=1234567,2345678
```

#### Display help information
```bash
OsuBeatmapDownloader.exe -help
```

## Input File Format

When using the `-input` option, provide a JSON file containing an array of beatmap ID strings:

```json
[
  "123456",
  "789012", 
  "345678",
  "901234"
]
```

## Output Structure

Downloaded beatmaps are saved in the following structure:
```
output/
├── 123456.osz
├── 789012.osz
└── 345678.osz

archive/
├── archive_beatmapset.json    # Cache of downloaded beatmapsets
└── archive_convert.json       # Cache of ID conversions
```

## Technical Details

### Architecture
- **Program.cs**: Main entry point and command-line argument processing
- **OAuth.cs**: Handles osu! API authentication using OAuth 2.0
- **Downloader.cs**: Manages beatmap downloads with progress tracking
- **Converter.cs**: Converts beatmap IDs to beatmapset IDs via API
- **FileStore.cs**: Generic JSON-based caching system
- **ProgressBar.cs**: Animated console progress bar implementation
- **FileUtils.cs**: File and directory utilities

### Caching System
The application implements intelligent caching to avoid unnecessary API calls and re-downloads:
- **Download Cache**: Tracks successfully downloaded beatmapsets
- **Conversion Cache**: Stores beatmap ID to beatmapset ID mappings
- **Batch Processing**: API requests are batched for efficiency

### API Integration
- Uses osu! API v2 endpoints
- Authenticates via OAuth 2.0 with username/password flow
- Respects API rate limits through batching
- Downloads exclude video content (`noVideo=1` parameter)

## Configuration

The application automatically creates necessary directories:
- `output/` - Contains downloaded `.osz` files
- `archive/` - Contains cache files for tracking downloads and conversions

## Troubleshooting

### Common Issues

**Authentication Failed**
- Verify your osu! username and password are correct
- Ensure your account has API access enabled

**Download Failures**
- Check your internet connection
- Verify the beatmap IDs exist and are accessible
- Some beatmaps may be restricted or deleted

**Permission Errors**
- Ensure the application has write permissions for the output directory
- Run with appropriate permissions if needed

### Error Messages

| Error | Solution |
|-------|----------|
| "You need pass credentials. Try -help" | Provide both `-login` and `-password` options |
| "You need pass beatmaps ids. Try -help" | Provide either `-ids` or `-input` option |
| "Error occurred when beatmap downloaded" | Check network connection and beatmap availability |

## Development

### Building from Source
```bash
# Clone the repository
git clone https://github.com/nayutalienx/osu-beatmap-downloader.git
cd osu-beatmap-downloader

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests (if available)
dotnet test
```

### Dependencies
- **Newtonsoft.Json** (13.0.1): JSON serialization and deserialization
- **.NET 6.0**: Target framework

## License

This project is provided as-is for educational and personal use. Please respect osu! terms of service and API usage guidelines.

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## Disclaimer

This tool is not officially affiliated with osu! or ppy Pty Ltd. Use responsibly and in accordance with osu! terms of service.
