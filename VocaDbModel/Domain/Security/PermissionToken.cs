using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Security {

	[DataContract(Namespace = Schemas.VocaDb)]
	public struct PermissionToken : IEquatable<PermissionToken>, IPermissionToken {

		private Guid id;
		private string name;

		private static readonly IDictionary<Guid, PermissionToken> all = new Dictionary<Guid, PermissionToken>(50);

		private static PermissionToken New(string guid, string name) {
			var token = new PermissionToken(new Guid(guid), name);
			all.Add(token.Id, token);
			return token;
		}

		/// <summary>
		/// Special token used to indicate the absence of permissions.
		/// </summary>
		public static readonly PermissionToken Nothing = new PermissionToken(Guid.Empty, "Nothing");

		public static readonly PermissionToken AccessManageMenu =		New("b54de61d-9341-4435-8cb1-31e5e295d577", "AccessManageMenu");

		/// <summary>
		/// Add raw media file links (such as .mp3) to songs.
		/// </summary>
		public static readonly PermissionToken AddRawFileMedia =		New("9c1992d8-3fca-4008-b248-7d7f2a7f15dc", "AddRawFileMedia");
		public static readonly PermissionToken Admin =					New("1c98077f-f36f-4ef2-8cf3-cd9e347d389a", "Admin");
		public static readonly PermissionToken ApproveEntries =			New("e3b4b909-5128-4a0e-9f26-2bf1d5e497ab", "ApproveEntries");
		public static readonly PermissionToken BulkDeletePVs =			New("caa8f4d7-322e-44f7-ad79-7de767ef1128", "BulkDeletePVs");

		/// <summary>
		/// Can post comments, for now applies to discussion topics as well. 
		/// Can delete own comments and topics.
		/// </summary>
		public static readonly PermissionToken CreateComments =			New("be2deee9-ee12-48b4-a9a5-e369915fc156", "CreateComments");

		/// <summary>
		/// Delete any comment (not just own comments). 
		/// Also allows editing any comment. 
		/// Also allows editing and deleting any discussion topic.
		/// Also allows moving discussion topics.
		/// </summary>
		public static readonly PermissionToken DeleteComments =			New("1b1dfcfa-6b96-4a8a-8aca-d76465439ffb", "DeleteComments");
		public static readonly PermissionToken DeleteEntries =			New("cc51c6b6-be93-4942-a6e4-fdf88f4520b9", "DeleteEntries");
		public static readonly PermissionToken DesignatedStaff =		New("b995a14b-49b4-4f1e-8fac-36a34967ddb0", "DesignatedStaff");
		public static readonly PermissionToken DisableUsers =			New("cb46dfbe-5221-4af4-9968-53aec5faa3d4", "DisableUsers");
		public static readonly PermissionToken EditAllSongLists =		New("b4873d98-b21f-40ee-b1d4-94102ae6e528", "EditAllSongLists");
		public static readonly PermissionToken EditFeaturedLists =		New("a639e4a3-86fe-429a-81ea-d0aa05161e40", "EditFeaturedLists");
		public static readonly PermissionToken EditProfile =			New("4f79b01a-7154-4a7f-bc87-a8a9259a9905", "EditProfile");
		public static readonly PermissionToken LockEntries =			New("eb02e92e-207f-4330-a763-6bafd2cedde1", "LockEntries");
		public static readonly PermissionToken ManageDatabase =			New("d762d720-79ef-4e60-8397-1d638c26d82b", "ManageDatabase");
		public static readonly PermissionToken ManageEntryReports =		New("f9eb1d22-9142-4a04-9238-f4ebe5f1fc17", "ManageEntryReports");
		public static readonly PermissionToken ManageEventSeries =		New("cf39509b-b9c5-4efc-9b13-2743ffec9aac", "ManageEventSeries");
		public static readonly PermissionToken ManageIPRules =			New("f125fe5b-6474-4d52-823f-955c7d19f7c8", "ManageIPRules");
		public static readonly PermissionToken ManageUserPermissions =	New("c0eb147e-10f5-4fea-9b19-b412ef613479", "ManageUserPermissions");
		public static readonly PermissionToken MergeEntries =			New("eb336a5b-8455-4048-bc3a-8003dc522dc5", "MergeEntries");
		public static readonly PermissionToken MikuDbImport =			New("0b879c57-5eba-462a-b842-d9f7dd0befd8", "MikuDbImport");
		public static readonly PermissionToken MoveToTrash =			New("99c333a2-ea0a-4a7b-91cb-ceef6f667389", "MoveToTrash");
		public static readonly PermissionToken RemoveTagUsages =		New("135aaf49-08d5-42bb-b8ed-ef1ceb910a69", "RemoveTagUsages");
		public static readonly PermissionToken RestoreRevisions =		New("e99a1e1c-1742-48c1-877b-17cb2964e8bc", "RestoreRevisions");
		public static readonly PermissionToken UploadMedia =			New("be1a2f04-6dc2-4d63-b34e-4499f6389231", "UploadMedia");
		public static readonly PermissionToken ViewAuditLog	=			New("8d3d5395-12c9-440a-8120-4911034b9a7e", "ViewAuditLog");

		/// <summary>
		/// All tokens except Nothing
		/// </summary>
		public static readonly IEnumerable<PermissionToken> All = all.Values;

		public static PermissionToken GetById(Guid id) {

			PermissionToken token;
			if (all.TryGetValue(id, out token))
				return token;

			throw new ArgumentException(string.Format("Invalid permission token: {0}.", id), "id");

		}

		public static bool IsValid(PermissionToken token) {
			return all.ContainsKey(token.Id);
		}

		public static bool TryGetById(Guid id, out PermissionToken token) {

			if (all.TryGetValue(id, out token))
				return true;

			return false;

		}

		public static bool operator ==(PermissionToken left, PermissionToken right) {
			return left.Equals(right);
		}

		public static bool operator !=(PermissionToken left, PermissionToken right) {
			return !left.Equals(right);
		}

		public PermissionToken(Guid id, string name) {
			this.id = id;
			this.name = name;
		}

		[DataMember]
		public Guid Id {
			get { return id; }
			set { id = value; }
		}

		[DataMember]
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public bool Equals(PermissionToken token) {
			return (token.Id == Id);
		}

		public override bool Equals(object obj) {

			if (!(obj is PermissionToken))
				return false;

			return Equals((PermissionToken)obj);

		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return Name;
		}

	}

	public interface IPermissionToken {

		Guid Id { get; }

		string Name { get; }

	}

}
