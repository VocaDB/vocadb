#nullable disable

using System;

namespace VocaDb.Model.Domain.Security
{
	public class IPRule : IEntryWithIntId
	{
		private string _address;
		private string _notes;

		public IPRule()
		{
			_address = string.Empty;
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
			get => _address;
			set
			{
				ParamIs.NotNull(() => value);
				_address = value;
			}
		}

		public virtual DateTime Created { get; set; }

		public virtual int Id { get; set; }

		public virtual string Notes
		{
			get => _notes;
			set
			{
				ParamIs.NotNull(() => value);
				_notes = value;
			}
		}

#nullable enable
		public override string ToString()
		{
			return $"IPRule for {Address}, created at {Created}, notes {Notes}";
		}
#nullable disable
	}
}
