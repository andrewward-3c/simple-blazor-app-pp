using Microsoft.AspNetCore.SignalR;
using PokerPlanning.Services;
using PokerPlanning.Models;

namespace PokerPlanning.Hubs;

public class GameHub : Hub
{
    private readonly IGameService _gameService;

    public GameHub(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task<bool> JoinGame(string roomCode, string displayName, bool isSpectator = false)
    {
        try
        {
            if (!await _gameService.GameExistsAsync(roomCode))
                return false;

            var participantId = await _gameService.JoinGameAsync(roomCode, displayName, Context.ConnectionId, isSpectator);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            
            var gameState = await _gameService.GetGameStateAsync(roomCode);
            
            // Notify all participants in the room
            await Clients.Group(roomCode).SendAsync("OnParticipantJoined", gameState);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SubmitVote(string roomCode, int participantId, string estimateValue)
    {
        var success = await _gameService.SubmitVoteAsync(roomCode, participantId, estimateValue);
        
        if (success)
        {
            var gameState = await _gameService.GetGameStateAsync(roomCode);
            await Clients.Group(roomCode).SendAsync("OnVoteSubmitted", gameState);
        }
    }

    public async Task RevealVotes(string roomCode)
    {
        var gameState = await _gameService.RevealVotesAsync(roomCode);
        await Clients.Group(roomCode).SendAsync("OnVotesRevealed", gameState);
    }

    public async Task ResetVotes(string roomCode)
    {
        await _gameService.ResetVotesAsync(roomCode);
        var gameState = await _gameService.GetGameStateAsync(roomCode);
        await Clients.Group(roomCode).SendAsync("OnVotesReset", gameState);
    }

    public async Task StartNewVote(string roomCode)
    {
        await _gameService.StartNewVoteAsync(roomCode);
        var gameState = await _gameService.GetGameStateAsync(roomCode);
        await Clients.Group(roomCode).SendAsync("OnNewVoteStarted", gameState);
    }

    public async Task ChangeEstimationUnit(string roomCode, string unit)
    {
        await _gameService.ChangeEstimationUnitAsync(roomCode, unit);
        var gameState = await _gameService.GetGameStateAsync(roomCode);
        await Clients.Group(roomCode).SendAsync("OnEstimationUnitChanged", gameState);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _gameService.RemoveParticipantAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
