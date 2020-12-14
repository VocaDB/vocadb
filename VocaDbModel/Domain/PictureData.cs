#nullable disable

using System;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Picture data for an entry, persisted in the database as BLOB.
	/// Currently in use for artist and album main pictures, everything else is saved on disk.
	/// </summary>
	public class PictureData
	{
		public static bool IsNullOrEmpty(PictureData pictureData)
		{
			return pictureData == null || pictureData.IsEmpty;
		}

		public PictureData() { }

		public PictureData(PictureDataContract contract)
		{
			ParamIs.NotNull(() => contract);

			Bytes = contract.Bytes;
		}

		public virtual Byte[] Bytes { get; set; }

		public virtual bool IsEmpty => Bytes == null || Bytes.Length == 0;
	}
}
