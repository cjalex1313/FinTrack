using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;

namespace FinTrack.Shared.Mappers;

public static class HouseholdMapper
{
    public static HouseholdDTO MapToDTO(this Household household)
    {
        return new HouseholdDTO()
        {
            Id = household.Id,
            Name = household.Name
        };
    }

    public static HouseholdMemberDTO MapToDTO(this HouseholdMember member)
    {
        return new HouseholdMemberDTO()
        {
            HouseholdId = member.HouseholdId,
            HouseholdName = member.Household.Name,
            UserEmail = member.User.Email!,
            UserFirstName = member.User.FirstName,
            UserLastName = member.User.LastName,
            Role = member.Role,
            Status = member.Status,
            CreatedAt = member.CreatedAt
        };
    }
}