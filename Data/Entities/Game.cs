namespace PokerPlanning.Data.Entities;

public class Game
{
    public int Id { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsActive { get; set; } = true;
    public string EstimationUnit { get; set; } = "Hours";
    
    // Navigation properties
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
