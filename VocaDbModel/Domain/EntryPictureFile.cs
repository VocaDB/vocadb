using System;
using System.IO;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain {

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
	public abstract class EntryPictureFile {

		/// <summary>
		/// Get file extension based on MIME type.
		/// </summary>
		/// <param name="mime">MIME type. Cannot be null.</param>
		/// <returns>File extension, including the leading dot. For example ".jpg". Cannot be null. Can be empty if the MIME is not recognized.</returns>
		private static string GetExtension(string mime) {
			return ImageHelper.GetExtensionFromMime(mime) ?? string.Empty;
		}

		private static string GetFileName(int id, string mime, string suffix) {
			return string.Format("{0}{1}{2}", id, suffix, GetExtension(mime));
		}

		/// <summary>
		/// Gets the file name for the original image.
		/// </summary>
		/// <param name="id">Entry Id.</param>
		/// <param name="mime">MIME type. Can be null, but usually this makes no sense.</param>
		/// <returns>File name. Cannot be null or empty.</returns>
		public static string GetFileName(int id, string mime) {
			return string.Format("{0}{1}", id, GetExtension(mime));
		}

		/// <summary>
		/// Gets the file name for the small thumbnail (default size is 70x70 pixels).
		/// </summary>
		/// <param name="id">Picture Id.</param>
		/// <param name="mime">MIME type. Can be null, but usually this makes no sense.</param>
		/// <returns>File name. Cannot be null or empty.</returns>
		public static string GetFileNameSmallThumb(int id, string mime) {
			return string.Format("{0}-st{1}", id, GetExtension(mime));
		}

		/// <summary>
		/// Gets the file name for the larger thumbnail (default size is 250x250 pixels).
		/// </summary>
		/// <param name="id">Picture Id.</param>
		/// <param name="mime">MIME type. Can be null, but usually this makes no sense.</param>
		/// <returns>File name. Cannot be null or empty.</returns>
		public static string GetFileNameThumb(int id, string mime) {
			return string.Format("{0}-t{1}", id, GetExtension(mime));
		}

		private User author;
		private string mime;
		private string name;

		/// <summary>
		/// Extension for this picture file, determined based on the MIME type.
		/// Cannot be null. Can be empty if the MIME is not recognized.
		/// </summary>
		protected string Extension {
			get {
				return ImageHelper.GetExtensionFromMime(Mime) ?? string.Empty;
			}
		}

		protected EntryPictureFile() {
			Created = DateTime.Now;
		}

		protected EntryPictureFile(string name, string mime, User author)
			: this() {

			Name = name;
			Mime = mime;
			Author = author;

		}

		/// <summary>
		/// User who uploaded this picture. Cannot be null.
		/// </summary>
		public virtual User Author {
			get { return author; }
			set { 
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
		/// Filename of the original image. Cannot be null or empty.
		/// This field is determined based on picture Id and MIME type. Not mapped to database.
		/// </summary>
		public virtual string FileName {
			get {
				return GetFileName(Id, Mime);
			}
		}

		/// <summary>
		/// Filename of the thumbnail (by default, 250x250px). Cannot be null or empty.
		/// The actual picture might be the same as the original.
		/// This field is determined based on picture Id and MIME type. Not mapped to database.
		/// </summary>
		public virtual string FileNameThumb {
			get {
				return GetFileNameThumb(Id, Mime);
			}
		}

		/// <summary>
		/// Unique picture file Id.
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Image MIME type. Cannot be null.
		/// MIME types for jpg, png, gif and bmp are recognized.
		/// </summary>
		public virtual string Mime {
			get { return mime; }
			set { 
				ParamIs.NotNull(() => value);
				mime = value; 
			}
		}

		/// <summary>
		/// Image user-friendly name. Cannot be null.
		/// </summary>
		public virtual string Name {
			get { return name; }
			set { 
				ParamIs.NotNull(() => value);
				name = value; 
			}
		}

		/// <summary>
		/// Uploaded file. This field is not mapped, only used for uploading.
		/// Null for files loaded from the DB.
		/// </summary>
		public virtual Stream UploadedFile { get; set; }

		public override string ToString() {
			return string.Format("Picture file {0} [{1}]", Name, Id);
		}

	}

}
