using System;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Domain.PVs {

	public class PV : IEquatable<PV>, IEditablePV {

		public static string GetUrl(PVService service, string pvId, PVExtendedMetadata extendedMetadata = null) {
			return VideoServiceHelper.Services[service].GetUrlById(pvId, extendedMetadata);
		}

		private string author;
		private PVExtendedMetadata extendedMetadata;
		private string name;
		private string pvId;

		public PV() {
			Author = string.Empty;
			pvId = string.Empty;
			Name = string.Empty;
			Service = PVService.Youtube;
			PVType = PVType.Other;
		}

		public PV(PVContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			Service = contract.Service;
			PVId = contract.PVId;
			PVType = contract.PVType;
			Name = contract.Name ?? string.Empty;
			PublishDate = contract.PublishDate;
			Author = contract.Author ?? string.Empty;
			ExtendedMetadata = contract.ExtendedMetadata;

		}

		public virtual string Author {
			get => author;
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual bool Disabled {
			get => false;
			set => throw new NotSupportedException();
		}

		public virtual int Id { get; set; }

		public virtual PVExtendedMetadata ExtendedMetadata {
			get => extendedMetadata;
			set => extendedMetadata = value;
		}

		public virtual string Name {
			get => name;
			set {
				ParamIs.NotNull(() => value);
				name = value;
			}
		}

		public virtual DateTime? PublishDate { get; set; }

		public virtual string PVId {
			get => pvId;
			set {
				ParamIs.NotNullOrEmpty(() => value);
				pvId = value;
			}
		}

		public virtual PVService Service { get; set; }

		public virtual PVType PVType { get; set; }

		public virtual string Url => GetUrl(Service, PVId, ExtendedMetadata);

		public virtual bool ContentEquals(PVContract pv) {

			if (pv == null)
				return false;

			return (Name == pv.Name);

		}

		public virtual void CopyMetaFrom(PVContract contract) {

			ParamIs.NotNull(() => contract);

			Author = contract.Author;
			Name = contract.Name;
			PVType = contract.PVType;

		}

		public virtual bool Equals(PV another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as PV);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public virtual void OnDelete() {}

		public override string ToString() {
			return string.Format("PV '{0}' on {1} [{2}]", PVId, Service, Id);
		}

	}

}
