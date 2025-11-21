# Poker Planning App

A real-time collaborative poker planning application built with ASP.NET Core Blazor Server and SignalR.

## Features

- **Create Games**: Generate a unique room code and shareable URL
- **Real-time Collaboration**: See participants join and vote in real-time
- **Anonymous Participation**: No user accounts required - just enter a display name
- **Voting System**: Submit numeric estimates for items being discussed
- **Reveal Votes**: Creator can reveal all votes when everyone has voted
- **Average Calculation**: Automatically calculates the average of all numeric votes
- **Reset Functionality**: Clear all votes to start a new round
- **Persistent Storage**: Games and votes are stored in SQLite database

## Technology Stack

- **Backend**: ASP.NET Core 8.0 (Blazor Server)
- **Real-time Communication**: SignalR
- **Data Access**: Entity Framework Core 8.0
- **Database**: SQLite
- **Frontend**: Blazor components with C#

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd c:\temp\pokerplanning
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to `http://localhost:5012`

## How to Use

### Creating a Game

1. Click **"Create New Game"** on the home page
2. A unique 6-character room code will be generated
3. Share the URL with participants (they can join via the link or by entering the room code)
4. Click **"Join Game"** to enter your own game

### Joining a Game

1. Navigate to the shared URL or enter the room code
2. Enter your display name
3. Click **"Join Game"**

### Voting

1. Enter your numeric estimate in the input field
2. Click **"Submit Vote"**
3. You'll see checkmarks appear next to participants who have voted
4. Wait for all participants to vote

### Revealing Results

1. The game creator can click **"Reveal Votes"** (enabled when all participants have voted)
2. All votes are displayed with participant names
3. The average of all numeric votes is shown prominently

### Starting a New Round

1. After votes are revealed, the creator can click **"Start New Vote"**
2. All previous votes are cleared
3. Participants can submit new estimates

### Resetting Votes

1. The creator can click **"Reset Votes"** at any time to clear all votes and start over

## Project Structure

```
PokerPlanning/
├── Data/
│   ├── Entities/          # Database entities (Game, Participant, Vote)
│   └── ApplicationDbContext.cs
├── Services/
│   ├── IGameService.cs    # Service interface
│   └── GameService.cs     # Business logic for games and voting
├── Hubs/
│   └── GameHub.cs         # SignalR hub for real-time communication
├── Models/
│   ├── ParticipantDto.cs
│   ├── VoteResult.cs
│   └── GameState.cs
├── Pages/
│   ├── Index.razor        # Home page - create game
│   └── Game.razor         # Game room - voting interface
└── wwwroot/
    └── css/
        └── site.css       # Custom styling
```

## Database

The application uses SQLite for data persistence. The database file (`pokerplanning.db`) is created automatically on first run.

### Schema

- **Games**: Stores game sessions with room codes and state
- **Participants**: Tracks who has joined each game
- **Votes**: Stores individual votes for each participant

## Features in Detail

### Real-time Updates

- Participant list updates when someone joins
- Vote status indicators update when someone submits a vote
- Results are broadcast to all participants when revealed
- Automatic cleanup when participants disconnect

### Creator Privileges

The first participant to join a game becomes the creator and has special privileges:
- Reveal votes
- Reset votes
- Start new voting rounds

### Vote Average Calculation

Only numeric votes are included in the average calculation. Non-numeric values (like "?", "∞", etc.) are displayed but excluded from the average.

## Future Enhancements

Potential features to add:
- Predefined voting scales (Fibonacci, T-shirt sizes)
- Vote history and session tracking
- Export results to CSV
- Timer for voting rounds
- Observer mode (watch without voting)
- Moderator controls separate from creator
- Session cleanup for inactive games

## License

This project is provided as-is for demonstration purposes.
