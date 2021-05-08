using System.IO;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetCoreHttpPostedFile : IHttpPostedFile
	{
		public AspNetCoreHttpPostedFile(IFormFile file)
		{
			_file = file;
		}

		private readonly IFormFile _file;

		public string ContentType => _file.ContentType;

		public string FileName => _file.FileName;

		public void SaveAs(string path)
		{
			using var target = new FileStream(path, FileMode.Create);
			_file.CopyTo(target);
		}
	}
}
