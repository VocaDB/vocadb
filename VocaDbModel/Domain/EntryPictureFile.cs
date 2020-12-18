#nullable disable

using System;
using System.IO;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Picture file with multiple sizes for an entry, such as album or artist.
	/// Multiple pictures may be attached to an entry.
	/// Files are named like 123.jpg + 123-t.jpg, when picture Id is 123 and file type is jpg.
	/// 
	/// This type is used when the entry has multiple pictures, so the pictures are saved individually,
	/// separate from the entry itself. Currently this means additional pictures for albums and artists.
	/// Thus, <see cref="Id"/> is not the entry Id, it's the picture Id.
	/// 
	/// At the moment these files are saved under the application folder, but this should be changed so that they're
	/// saved to the static files folder.
	/// </summary>
	public abstract class EntryPictureFile : IEntryPictureFile
	{
		// Not versioned.
		int IEntryImageInformation.Version => 0;

		private User author;
		private string mime;
		private string name;

		/// <summary>
		/// Extension for this picture file, determined based on the MIME type.
		/// Cannot be null. Can be empty if the MIME is not recognized.
		/// </summary>
		protected string Extension => ImageHelper.GetExtensionFromMime(Mime) ?? string.Empty;

		protected EntryPictureFile()
		{
			Created = DateTime.Now;
		}

		protected EntryPictureFile(string name, string mime, User author)
			: this()
		{
			Name = name;
			Mime = mime;
			Author = author;
		}

		/// <summary>
		/// User who uploaded this picture. Cannot be null.
		/// </summary>
		public virtual User Author
		{
			get => author;
			set
			{
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime Created { get; set; }

		/// <summary>
		/// Type of entry for which this picture is for. 
		/// Entry type determines the folder in which the file is saved.
		/// </summary>
		public abstract EntryType EntryType { get; }

		/// <summary>
		/// Unique picture file Id.
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Image MIME type. Cannot be null.
		/// MIME types for jpg, png, gif and bmp are recognized.
		/// </summary>
		public virtual string Mime
		{
			get => mime;
			set
			{
				ParamIs.NotNull(() => value);
				mime = value;
			}
		}

		/// <summary>
		/// Image user-friendly name. Cannot be null.
		/// </summary>
		public virtual string Name
		{
			get => name;
			set
			{
				ParamIs.NotNull(() => value);
				name = value;
			}
		}

		public abstract int OwnerEntryId { get; }

		public virtual ImagePurpose Purpose => ImagePurpose.Additional;

		/// <summary>
		/// Uploaded file. This field is not mapped, only used for uploading.
		/// Null for files loaded from the DB.
		/// </summary>
		public virtual Stream UploadedFile { get; set; }

		public override string ToString()
		{
			return $"Picture file {Name} [{Id}]";
		}
	}

	public interface IEntryPictureFile : IEntryImageInformation
	{
		/// <summary>
		/// ID of the entry owning the picture.
		/// </summary>
		int OwnerEntryId { get; }
	}
}
