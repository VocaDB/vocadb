#nullable disable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Security
{
	/// <summary>
	/// Permission tokens are immutable objects that represent global permissions.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public struct PermissionToken : IEquatable<PermissionToken>, IPermissionToken
	{
		private Guid _id;
		private string _name;

		private static readonly IDictionary<Guid, PermissionToken> s_all = new Dictionary<Guid, PermissionToken>(50);

		private static PermissionToken New(string guid, string name)
		{
			var token = new PermissionToken(new Guid(guid), name);
			s_all.Add(token.Id, token);
			return token;
		}

		/// <summary>
		/// Special token used to indicate the absence of permissions.
		/// </summary>
		public static readonly PermissionToken Nothing = new(Guid.Empty, "Nothing");

		public static readonly PermissionToken AccessManageMenu = New("b54de61d-9341-4435-8cb1-31e5e295d577", nameof(AccessManageMenu));

		/// <summary>
		/// Add raw media file links (such as .mp3) to songs.
		/// </summary>
		public static readonly PermissionToken AddRawFileMedia = New("9c1992d8-3fca-4008-b248-7d7f2a7f15dc", nameof(AddRawFileMedia));
		public static readonly PermissionToken Admin = New("1c98077f-f36f-4ef2-8cf3-cd9e347d389a", nameof(Admin));
		public static readonly PermissionToken ApplyAnyTag = New("100e81ce-6bc9-4083-bff4-2a47fcfb37d3", nameof(ApplyAnyTag));
		public static readonly PermissionToken ApproveEntries = New("e3b4b909-5128-4a0e-9f26-2bf1d5e497ab", nameof(ApproveEntries));
		public static readonly PermissionToken BulkDeletePVs = New("caa8f4d7-322e-44f7-ad79-7de767ef1128", nameof(BulkDeletePVs));

		/// <summary>
		/// Can post comments, for now applies to discussion topics as well. 
		/// Can delete own comments and topics.
		/// </summary>
		public static readonly PermissionToken CreateComments = New("be2deee9-ee12-48b4-a9a5-e369915fc156", nameof(CreateComments));

		/// <summary>
		/// Delete any comment (not just own comments). 
		/// Also allows editing any comment. 
		/// Also allows editing and deleting any discussion topic.
		/// Also allows moving discussion topics.
		/// </summary>
		public static readonly PermissionToken DeleteComments = New("1b1dfcfa-6b96-4a8a-8aca-d76465439ffb", nameof(DeleteComments));
		/// <summary>
		/// Delete any entry (entry state still matters, meaning edit permission is required).
		/// </summary>
		public static readonly PermissionToken DeleteEntries = New("cc51c6b6-be93-4942-a6e4-fdf88f4520b9", nameof(DeleteEntries));
		public static readonly PermissionToken DesignatedStaff = New("b995a14b-49b4-4f1e-8fac-36a34967ddb0", nameof(DesignatedStaff));
		public static readonly PermissionToken DisableUsers = New("cb46dfbe-5221-4af4-9968-53aec5faa3d4", nameof(DisableUsers));
		public static readonly PermissionToken EditAllSongLists = New("b4873d98-b21f-40ee-b1d4-94102ae6e528", nameof(EditAllSongLists));
		public static readonly PermissionToken EditFeaturedLists = New("a639e4a3-86fe-429a-81ea-d0aa05161e40", nameof(EditFeaturedLists));
		public static readonly PermissionToken EditProfile = New("4f79b01a-7154-4a7f-bc87-a8a9259a9905", nameof(EditProfile));
		public static readonly PermissionToken LockEntries = New("eb02e92e-207f-4330-a763-6bafd2cedde1", nameof(LockEntries));
		public static readonly PermissionToken ManageDatabase = New("d762d720-79ef-4e60-8397-1d638c26d82b", nameof(ManageDatabase));
		public static readonly PermissionToken EditTags = New("2ab6da19-bc5a-4a28-86d4-8ddce399ca0b", nameof(EditTags));
		public static readonly PermissionToken ManageEntryReports = New("f9eb1d22-9142-4a04-9238-f4ebe5f1fc17", nameof(ManageEntryReports));
		[Obsolete]
		public static readonly PermissionToken ManageEventSeries = New("cf39509b-b9c5-4efc-9b13-2743ffec9aac", nameof(ManageEventSeries));
		public static readonly PermissionToken ManageIPRules = New("f125fe5b-6474-4d52-823f-955c7d19f7c8", nameof(ManageIPRules));
		public static readonly PermissionToken ManageUserPermissions = New("c0eb147e-10f5-4fea-9b19-b412ef613479", nameof(ManageUserPermissions));
		public static readonly PermissionToken ManageTagMappings = New("c93667a9-a270-486d-ab00-db3ff390dd83", nameof(ManageTagMappings));
		public static readonly PermissionToken MergeEntries = New("eb336a5b-8455-4048-bc3a-8003dc522dc5", nameof(MergeEntries));
		public static readonly PermissionToken MikuDbImport = New("0b879c57-5eba-462a-b842-d9f7dd0befd8", nameof(MikuDbImport));
		public static readonly PermissionToken MoveToTrash = New("99c333a2-ea0a-4a7b-91cb-ceef6f667389", nameof(MoveToTrash));
		public static readonly PermissionToken ReportUser = New("85c730a1-402c-4922-b617-6c6068f5f98e", nameof(ReportUser));
		public static readonly PermissionToken RemoveEditPermission = New("6b258b3e-97a1-4cf7-9c4f-80787c2d2266", nameof(RemoveEditPermission));
		public static readonly PermissionToken RemoveTagUsages = New("135aaf49-08d5-42bb-b8ed-ef1ceb910a69", nameof(RemoveTagUsages));
		public static readonly PermissionToken RestoreRevisions = New("e99a1e1c-1742-48c1-877b-17cb2964e8bc", nameof(RestoreRevisions));
		public static readonly PermissionToken UploadMedia = New("be1a2f04-6dc2-4d63-b34e-4499f6389231", nameof(UploadMedia));
		public static readonly PermissionToken ViewAuditLog = New("8d3d5395-12c9-440a-8120-4911034b9a7e", nameof(ViewAuditLog));
		/// <summary>
		/// View ratings by users whose ratings are normally hidden.
		/// </summary>
		public static readonly PermissionToken ViewHiddenRatings = New("47bcc523-5667-403d-bd20-d2728e1f9c5f", nameof(ViewHiddenRatings));
		public static readonly PermissionToken ViewHiddenRevisions = New("c3b753d0-7aa8-4c03-8bca-5311fb2bdd2d", nameof(ViewHiddenRevisions));
		public static readonly PermissionToken ManageWebhooks = New("838dde1d-51ba-423b-ad8e-c1e2c2024a37", nameof(ManageWebhooks));
		public static readonly PermissionToken CreateXmlDump = New("d3dffb90-2408-4434-ae3a-c26352293281", nameof(CreateXmlDump));

		/// <summary>
		/// All tokens except Nothing
		/// </summary>
		public static readonly IEnumerable<PermissionToken> All = s_all.Values;

		public static PermissionToken GetById(Guid id)
		{
			if (s_all.TryGetValue(id, out PermissionToken token))
				return token;

			throw new ArgumentException($"Invalid permission token: {id}.", "id");
		}

		public static bool IsValid(PermissionToken token)
		{
			return s_all.ContainsKey(token.Id);
		}

		public static bool TryGetById(Guid id, out PermissionToken token)
		{
			if (s_all.TryGetValue(id, out token))
				return true;

			return false;
		}

		public static bool operator ==(PermissionToken left, PermissionToken right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PermissionToken left, PermissionToken right)
		{
			return !left.Equals(right);
		}

		public PermissionToken(Guid id, string name)
		{
			_id = id;
			_name = name;
		}

		[DataMember]
		public Guid Id
		{
			get => _id;
			set => _id = value;
		}

		[DataMember]
		public string Name
		{
			get => _name;
			set => _name = value;
		}

#nullable enable
		public bool Equals(PermissionToken token)
		{
			return (token.Id == Id);
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is PermissionToken))
				return false;

			return Equals((PermissionToken)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}
#nullable disable
	}

	public interface IPermissionToken
	{
		Guid Id { get; }

		string Name { get; }
	}
}
