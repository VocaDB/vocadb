using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers
{
	/// <summary>
	/// Manages concurrent entry edits.
	/// </summary>
	public class ConcurrentEntryEditManager
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly ConcurrentEntryEditManager staticInstance = new ConcurrentEntryEditManager();

		public static readonly EntryEditData Nothing = new EntryEditData();

		public class EntryEditData
		{
			public EntryEditData() { }

			public EntryEditData(IUser user)
				: this()
			{
				UserId = user.Id;
				UserName = user.Name;
				Time = DateTime.Now;
			}

			public DateTime Time { get; set; }

			public int UserId { get; private set; }

			public string UserName { get; private set; }

			public void Refresh(IUser user)
			{
				if (user.Id == UserId)
					Time = DateTime.Now;
			}
		}

		public static IEnumerable<KeyValuePair<EntryRef, EntryEditData>> Editors => staticInstance.editors;

		public static EntryEditData CheckConcurrentEdits(EntryRef entry, IUser user)
		{
			return staticInstance.CheckConcurrentEditsInst(entry, user);
		}

		private readonly Dictionary<EntryRef, EntryEditData> editors = new Dictionary<EntryRef, EntryEditData>();

		private void ClearExpiredUsages()
		{
			var cutoffDate = DateTime.Now - TimeSpan.FromMinutes(3);

			lock (editors)
			{
				var expired = editors.Where(e => e.Value.Time < cutoffDate).Select(e => e.Key).ToArray();

				foreach (var e in expired)
					editors.Remove(e);
			}
		}

		private void AddOrUpdate(EntryRef entry, IUser user)
		{
			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => user);

			lock (editors)
			{
				if (editors.ContainsKey(entry))
					editors[entry].Refresh(user);
				else
				{
					log.Debug("{0} starting to edit {1}", user, entry);
					editors.Add(entry, CreateEntryEditData(user));
				}
			}
		}

		private EntryEditData GetEditor(EntryRef entry)
		{
			ParamIs.NotNull(() => entry);

			lock (editors)
			{
				if (editors.ContainsKey(entry))
					return editors[entry];
			}

			return Nothing;
		}

		/// <summary>
		/// Checks for concurrent edits.
		/// </summary>
		/// <param name="entry">Entry to be checked. Cannot be null.</param>
		/// <param name="user">User attempting to edit the entry. Cannot be null.</param>
		/// <returns>Edit data for the active editor. Cannot be null.</returns>
		public EntryEditData CheckConcurrentEditsInst(EntryRef entry, IUser user)
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

		public virtual EntryEditData CreateEntryEditData(IUser user)
		{
			return new EntryEditData(user);
		}
	}
}
