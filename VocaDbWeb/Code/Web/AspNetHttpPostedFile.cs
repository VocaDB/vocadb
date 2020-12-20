#nullable disable

using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetHttpPostedFile : IHttpPostedFile
	{
		public AspNetHttpPostedFile(HttpPostedFileBase file)
		{
			_file = file;
		}

		private readonly HttpPostedFileBase _file;

		public string ContentType => _file.ContentType;

		public string FileName => _file.FileName;

		public void SaveAs(string path) => _file.SaveAs(path);
	}
}