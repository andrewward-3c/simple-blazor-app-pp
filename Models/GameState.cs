namespace PokerPlanning.Models;

public class GameState
{
    public string RoomCode { get; set; } = string.Empty;
    public bool IsRevealed { get; set; }
    public string EstimationUnit { get; set; } = "Hours";
    public List<ParticipantDto> Participants { get; set; } = new();
    public List<VoteResult> Votes { get; set; } = new();
    public double? Average { get; set; }
}
