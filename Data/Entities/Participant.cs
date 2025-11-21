namespace PokerPlanning.Data.Entities;

public class Participant
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsCreator { get; set; }
    public bool IsSpectator { get; set; }
    
    // Navigation property
    public Game Game { get; set; } = null!;
}
