namespace PokerPlanning.Data.Entities;

public class Vote
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int ParticipantId { get; set; }
    public string EstimateValue { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    
    // Navigation properties
    public Game Game { get; set; } = null!;
    public Participant Participant { get; set; } = null!;
}
