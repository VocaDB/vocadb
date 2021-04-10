#nullable disable

using System;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Domain.PVs
{
	public class PV : IEquatable<PV>, IEditablePV
	{
		public static string GetUrl(PVService service, string pvId, PVExtendedMetadata extendedMetadata = null)
		{
			return VideoServiceHelper.Services[service].GetUrlById(pvId, extendedMetadata);
		}

		private string _author;
		private PVExtendedMetadata _extendedMetadata;
		private string _name;
		private string _pvId;

		public PV()
		{
			Author = string.Empty;
			_pvId = string.Empty;
			Name = string.Empty;
			Service = PVService.Youtube;
			PVType = PVType.Other;
		}

#nullable enable
		public PV(PVContract contract)
			: this()
		{
			ParamIs.NotNull(() => contract);

			Service = contract.Service;
			PVId = contract.PVId;
			PVType = contract.PVType;
			Name = contract.Name ?? string.Empty;
			PublishDate = contract.PublishDate;
			Author = contract.Author ?? string.Empty;
			ExtendedMetadata = contract.ExtendedMetadata;
		}
#nullable disable

		public virtual string Author
		{
			get => _author;
			set
			{
				ParamIs.NotNull(() => value);
				_author = value;
			}
		}

		public virtual bool Disabled
		{
			get => false;
			set => throw new NotSupportedException();
		}

		public virtual int Id { get; set; }

		public virtual PVExtendedMetadata ExtendedMetadata
		{
			get => _extendedMetadata;
			set => _extendedMetadata = value;
		}

		public virtual string Name
		{
			get => _name;
			set
			{
				ParamIs.NotNull(() => value);
				_name = value;
			}
		}

		public virtual DateTime? PublishDate { get; set; }

		public virtual string PVId
		{
			get => _pvId;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_pvId = value;
			}
		}

		public virtual PVService Service { get; set; }

		public virtual PVType PVType { get; set; }

		public virtual string Url => GetUrl(Service, PVId, ExtendedMetadata);

#nullable enable
		public virtual bool ContentEquals(PVContract? pv)
		{
			if (pv == null)
				return false;

			return (Name == pv.Name);
		}

		public virtual void CopyMetaFrom(PVContract contract)
		{
			ParamIs.NotNull(() => contract);

			Author = contract.Author;
			Name = contract.Name;
			PVType = contract.PVType;
		}

		public virtual bool Equals(PV? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as PV);
		}
#nullable disable

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public virtual void OnDelete() { }

		public override string ToString()
		{
			return $"PV '{PVId}' on {Service} [{Id}]";
		}
	}
}
