namespace PokerPlanning.Models;

public class VoteResult
{
    public int ParticipantId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string EstimateValue { get; set; } = string.Empty;
}
