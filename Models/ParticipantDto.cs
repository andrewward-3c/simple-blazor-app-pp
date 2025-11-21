namespace PokerPlanning.Models;

public class ParticipantDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool HasVoted { get; set; }
    public bool IsCreator { get; set; }
    public bool IsSpectator { get; set; }
}
