using System;

namespace VocaDb.Model.Domain.Security
{
	public class IPRule : IEntryWithIntId
	{
		private string address;
		private string notes;

		public IPRule()
		{
			address = string.Empty;
			Created = DateTime.Now;
			Notes = string.Empty;
		}

		public IPRule(string address, string notes = "")
		{
			Address = address;
			Notes = notes;
			Created = DateTime.Now;
		}

		public virtual string Address
		{
			get { return address; }
			set
			{
				ParamIs.NotNull(() => value);
				address = value;
			}
		}

		public virtual DateTime Created { get; set; }

		public virtual int Id { get; set; }

		public virtual string Notes
		{
			get { return notes; }
			set
			{
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		public override string ToString()
		{
			return $"IPRule for {Address}, created at {Created}, notes {Notes}";
		}
	}
}
