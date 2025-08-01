using System.Net;

namespace FinTrack.Shared.Exceptions.Household;

public class UserHasHouseholdException : BaseException
{
    public UserHasHouseholdException(Guid userId)
    {
        this.ErrorMessage = $"User with id: {userId} has a household";
        this.StatusCode = (int)HttpStatusCode.Conflict;
    }
}