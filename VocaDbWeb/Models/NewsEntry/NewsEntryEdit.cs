using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Activityfeed;

namespace VocaDb.Web.Models.NewsEntry {

	public class NewsEntryEdit {

		public NewsEntryEdit() {

			Text = string.Empty;

		}

		public NewsEntryEdit(NewsEntryContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			Anonymous = contract.Anonymous;
			CreateDate = contract.CreateDate;
			Id = contract.Id;
			Important = contract.Important;
			Stickied = contract.Stickied;
			Text = contract.Text;

		}

		public bool Anonymous { get; set; }

		public DateTime CreateDate { get; set; }

		public int Id { get; set; }

		public bool Important { get; set; }

		public bool Stickied { get; set; }

		[Required]
		[StringLength(2000)]
		public string Text { get; set; }

		public NewsEntryContract ToContract() {
			return new NewsEntryContract {
				Anonymous = this.Anonymous,
				Id = this.Id,
				Important = this.Important,
				Stickied = this.Stickied,
				Text = this.Text
			};
		}
	
	}

}