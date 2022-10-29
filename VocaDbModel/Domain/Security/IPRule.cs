#nullable disable


namespace VocaDb.Model.Domain.Security
{
	public class IPRule : IEntryWithIntId
	{
		private string _address;
		private string _notes;

		public IPRule()
		{
			_address = string.Empty;
			CreatedUtc = DateTime.Now;
			Notes = string.Empty;
		}

		public IPRule(string address, string notes = "")
		{
			Address = address;
			Notes = notes;
			CreatedUtc = DateTime.Now;
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

		public virtual DateTime CreatedUtc { get; set; }

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
			return $"IPRule for {Address}, created at {CreatedUtc}, notes {Notes}";
		}
#nullable disable
	}
}
