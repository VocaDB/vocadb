#nullable disable

using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetHttpPostedFile : IHttpPostedFile
	{
		public AspNetHttpPostedFile(HttpPostedFileBase file)
		{
			this.file = file;
		}

		private readonly HttpPostedFileBase file;

		public string ContentType => file.ContentType;

		public string FileName => file.FileName;

		public void SaveAs(string path) => file.SaveAs(path);
	}
}