#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.SongLists
{
	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public class SongListEditViewModel
	{
		public SongListEditViewModel()
		{
			SongLinks = new List<SongInListEditContract>();
			UpdateNotes = string.Empty;
		}

		public SongListEditViewModel(SongListContract contract, IUserPermissionContext permissionContext)
			: this()
		{
			ParamIs.NotNull(() => contract);

			CurrentName = contract.Name;
			Deleted = contract.Deleted;
			Description = contract.Description;
			EventDate = contract.EventDate;
			FeaturedCategory = contract.FeaturedCategory;
			Id = contract.Id;
			Name = contract.Name;
			Status = contract.Status;
			Thumb = contract.Thumb;

			AllowedEntryStatuses = new[] { EntryStatus.Draft, EntryStatus.Finished };
			CanCreateFeaturedLists = EntryPermissionManager.CanManageFeaturedLists(permissionContext);
		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public bool CanCreateFeaturedLists { get; set; }

		public string CurrentName { get; set; }

		public bool Deleted { get; set; }

		[StringLength(2000)]
		public string Description { get; set; }

		public DateTime? EventDate { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongListFeaturedCategory FeaturedCategory { get; set; }

		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		public IList<SongInListEditContract> SongLinks { get; set; }

		public EntryStatus Status { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string UpdateNotes { get; set; }

		public SongListForEditContract ToContract()
		{
			return new SongListForEditContract
			{
				Description = Description ?? string.Empty,
				EventDate = EventDate,
				FeaturedCategory = FeaturedCategory,
				Id = Id,
				Name = Name,
				SongLinks = SongLinks.ToArray(),
				Status = Status,
				UpdateNotes = UpdateNotes ?? string.Empty
			};
		}
	}
}