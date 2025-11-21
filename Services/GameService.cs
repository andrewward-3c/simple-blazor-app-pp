using Microsoft.EntityFrameworkCore;
using PokerPlanning.Data;
using PokerPlanning.Data.Entities;
using PokerPlanning.Models;

namespace PokerPlanning.Services;

public class GameService : IGameService
{
    private readonly ApplicationDbContext _context;

    public GameService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateGameAsync(string estimationUnit = "Hours")
    {
        var roomCode = Guid.NewGuid().ToString("N"); // 32 character hex string without dashes

        var game = new Game
        {
            RoomCode = roomCode,
            CreatedAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            IsRevealed = false,
            IsActive = true,
            EstimationUnit = estimationUnit
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return roomCode;
    }

    public async Task<bool> GameExistsAsync(string roomCode)
    {
        return await _context.Games.AnyAsync(g => g.RoomCode == roomCode && g.IsActive);
    }

    public async Task<int> JoinGameAsync(string roomCode, string displayName, string connectionId, bool isSpectator = false)
    {
        var game = await _context.Games
            .Include(g => g.Participants)
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        // Check if participant already exists with this connection
        var existingParticipant = await _context.Participants
            .FirstOrDefaultAsync(p => p.GameId == game.Id && p.ConnectionId == connectionId);

        if (existingParticipant != null)
            return existingParticipant.Id;

        var participant = new Participant
        {
            GameId = game.Id,
            DisplayName = displayName,
            ConnectionId = connectionId,
            JoinedAt = DateTime.UtcNow,
            IsCreator = !game.Participants.Any(p => !p.IsSpectator), // First non-spectator is creator
            IsSpectator = isSpectator
        };

        _context.Participants.Add(participant);
        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return participant.Id;
    }

    public async Task<bool> SubmitVoteAsync(string roomCode, int participantId, string estimateValue)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null || game.IsRevealed)
            return false;

        // Remove existing vote for this participant if any
        var existingVote = await _context.Votes
            .FirstOrDefaultAsync(v => v.GameId == game.Id && v.ParticipantId == participantId);

        if (existingVote != null)
        {
            existingVote.EstimateValue = estimateValue;
            existingVote.SubmittedAt = DateTime.UtcNow;
        }
        else
        {
            var vote = new Vote
            {
                GameId = game.Id,
                ParticipantId = participantId,
                EstimateValue = estimateValue,
                SubmittedAt = DateTime.UtcNow
            };
            _context.Votes.Add(vote);
        }

        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<GameState> GetGameStateAsync(string roomCode)
    {
        var game = await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.Votes)
                .ThenInclude(v => v.Participant)
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        var participants = game.Participants.Select(p => new ParticipantDto
        {
            Id = p.Id,
            DisplayName = p.DisplayName,
            HasVoted = game.Votes.Any(v => v.ParticipantId == p.Id),
            IsCreator = p.IsCreator,
            IsSpectator = p.IsSpectator
        }).ToList();

        var gameState = new GameState
        {
            RoomCode = game.RoomCode,
            IsRevealed = game.IsRevealed,
            EstimationUnit = game.EstimationUnit,
            Participants = participants
        };

        if (game.IsRevealed)
        {
            gameState.Votes = game.Votes.Select(v => new VoteResult
            {
                ParticipantId = v.ParticipantId,
                DisplayName = v.Participant.DisplayName,
                EstimateValue = v.EstimateValue
            }).ToList();

            gameState.Average = CalculateAverage(game.Votes);
        }

        return gameState;
    }

    public async Task<GameState> RevealVotesAsync(string roomCode)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        game.IsRevealed = true;
        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetGameStateAsync(roomCode);
    }

    public async Task ResetVotesAsync(string roomCode)
    {
        var game = await _context.Games
            .Include(g => g.Votes)
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        _context.Votes.RemoveRange(game.Votes);
        game.IsRevealed = false;
        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task StartNewVoteAsync(string roomCode)
    {
        await ResetVotesAsync(roomCode);
    }

    public async Task RemoveParticipantAsync(string connectionId)
    {
        var participant = await _context.Participants
            .Include(p => p.Game)
            .FirstOrDefaultAsync(p => p.ConnectionId == connectionId);

        if (participant == null)
            return;

        var game = participant.Game;
        
        // Remove participant's votes
        var votes = await _context.Votes
            .Where(v => v.ParticipantId == participant.Id)
            .ToListAsync();
        _context.Votes.RemoveRange(votes);

        // Remove participant
        _context.Participants.Remove(participant);

        // Check if game has no more participants
        var remainingParticipants = await _context.Participants
            .CountAsync(p => p.GameId == game.Id && p.Id != participant.Id);

        if (remainingParticipants == 0)
        {
            game.IsActive = false;
        }

        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateParticipantConnectionAsync(int participantId, string newConnectionId)
    {
        var participant = await _context.Participants.FindAsync(participantId);
        if (participant != null)
        {
            participant.ConnectionId = newConnectionId;
            await _context.SaveChangesAsync();
        }
    }

    private static double? CalculateAverage(IEnumerable<Vote> votes)
    {
        var numericVotes = votes
            .Select(v => v.EstimateValue)
            .Where(v => double.TryParse(v, out _))
            .Select(double.Parse)
            .ToList();

        if (!numericVotes.Any())
            return null;

        return Math.Round(numericVotes.Average(), 2);
    }

    public async Task<bool> ChangeEstimationUnitAsync(string roomCode, string unit)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode && g.IsActive);

        if (game == null)
            return false;

        game.EstimationUnit = unit;
        game.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
