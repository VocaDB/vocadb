using NLog;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers;

/// <summary>
/// Manages concurrent entry edits.
/// </summary>
public class ConcurrentEntryEditManager
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private static readonly ConcurrentEntryEditManager s_staticInstance = new();

	public static readonly EntryEditDataContract Nothing = new();

	public static IEnumerable<KeyValuePair<EntryRef, EntryEditDataContract>> Editors => s_staticInstance._editors;

	public static EntryEditDataContract CheckConcurrentEdits(EntryRef entry, IUser user)
	{
		return s_staticInstance.CheckConcurrentEditsInst(entry, user);
	}

	private readonly Dictionary<EntryRef, EntryEditDataContract> _editors = new();

	private void ClearExpiredUsages()
	{
		var cutoffDate = DateTime.Now - TimeSpan.FromMinutes(3);

		lock (_editors)
		{
			var expired = _editors
				.Where(e => e.Value.Time < cutoffDate)
				.Select(e => e.Key)
				.ToArray();

			foreach (var e in expired)
				_editors.Remove(e);
		}
	}

	private void AddOrUpdate(EntryRef entry, IUser user)
	{
		ParamIs.NotNull(() => entry);
		ParamIs.NotNull(() => user);

		lock (_editors)
		{
			if (_editors.ContainsKey(entry))
				_editors[entry].Refresh(user);
			else
			{
				s_log.Debug("{0} starting to edit {1}", user, entry);
				_editors.Add(entry, CreateEntryEditData(user));
			}
		}
	}

	private EntryEditDataContract GetEditor(EntryRef entry)
	{
		ParamIs.NotNull(() => entry);

		lock (_editors)
		{
			if (_editors.ContainsKey(entry))
				return _editors[entry];
		}

		return Nothing;
	}

	/// <summary>
	/// Checks for concurrent edits.
	/// </summary>
	/// <param name="entry">Entry to be checked. Cannot be null.</param>
	/// <param name="user">User attempting to edit the entry. Cannot be null.</param>
	/// <returns>Edit data for the active editor. Cannot be null.</returns>
	public EntryEditDataContract CheckConcurrentEditsInst(EntryRef entry, IUser user)
	{
		ParamIs.NotNull(() => entry);
		ParamIs.NotNull(() => user);

		ClearExpiredUsages();

		var editor = GetEditor(entry);

		if (editor.UserId != Nothing.UserId && editor.UserId != user.Id)
			return editor;

		AddOrUpdate(entry, user);

		return Nothing;
	}

	public virtual EntryEditDataContract CreateEntryEditData(IUser user)
	{
		return new EntryEditDataContract(user);
	}
}
