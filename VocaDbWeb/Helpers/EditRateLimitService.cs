using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Helpers;

public class EditRateLimitService 
{
	// Initialize request tracking
	private readonly Dictionary<int, List<DateTime>> _userEndpointRequests = new();

	public bool IsAllowed(IUserPermissionContext permissionContext)
	{
		var userId = permissionContext.LoggedUserId;
		if (!_userEndpointRequests.ContainsKey(userId))
		{
			_userEndpointRequests[userId] = new List<DateTime>();
		}

		var requestTimes = _userEndpointRequests[userId];

		// Remove requests older than an hour
		requestTimes.RemoveAll(time => time < DateTime.Now.AddHours(-1));

		var requestLimit = permissionContext.UserGroupId == UserGroupId.Trusted ? 500 : 100;

		return requestTimes.Count < requestLimit;
	}

	public void IncrementCounter(IUserPermissionContext permissionContext)
	{
		var userId = permissionContext.LoggedUserId;
		if (!_userEndpointRequests.ContainsKey(userId))
		{
			_userEndpointRequests[userId] = new List<DateTime>();
		}

		_userEndpointRequests[userId].Add(DateTime.Now);
	}

	public void RegisterEdit(IUserPermissionContext permissionContext)
	{
		if (permissionContext.UserGroupId == UserGroupId.Admin ||
		    permissionContext.UserGroupId == UserGroupId.Moderator)
		{
			return;
		}
		
		if (!permissionContext.IsLoggedIn || !IsAllowed(permissionContext))
		{
			throw new NotAllowedException();
		}
		else
		{
			IncrementCounter(permissionContext);
		}
	}
}
