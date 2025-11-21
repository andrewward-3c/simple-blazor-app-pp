using PokerPlanning.Models;

namespace PokerPlanning.Services;

public interface IGameService
{
    Task<string> CreateGameAsync(string estimationUnit = "Hours");
    Task<bool> GameExistsAsync(string roomCode);
    Task<int> JoinGameAsync(string roomCode, string displayName, string connectionId, bool isSpectator = false);
    Task<bool> SubmitVoteAsync(string roomCode, int participantId, string estimateValue);
    Task<GameState> GetGameStateAsync(string roomCode);
    Task<GameState> RevealVotesAsync(string roomCode);
    Task ResetVotesAsync(string roomCode);
    Task StartNewVoteAsync(string roomCode);
    Task RemoveParticipantAsync(string connectionId);
    Task UpdateParticipantConnectionAsync(int participantId, string newConnectionId);
    Task<bool> ChangeEstimationUnitAsync(string roomCode, string unit);
}
