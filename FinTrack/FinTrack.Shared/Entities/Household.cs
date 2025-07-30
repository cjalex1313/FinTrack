namespace FinTrack.Shared.Entities;

public class Household
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid OwnerId { get; set; }
}