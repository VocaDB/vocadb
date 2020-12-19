#nullable disable

using System.Collections;
using System.Security.Principal;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetHttpContext : IHttpContext, IServerPathMapper
	{
		public AspNetHttpContext(HttpContext context)
		{
			_context = context;
		}

		private readonly HttpContext _context;

		public IDictionary Items => _context.Items;

		public IHttpRequest Request => new AspNetHttpRequest(_context.Request);

		public IHttpResponse Response => new AspNetHttpResponse(_context.Response);

		public IPrincipal User
		{
			get => _context?.User;
			set
			{
				if (_context == null)
					return;

				_context.User = value;
			}
		}

		public IServerPathMapper ServerPathMapper => this;

		public string MapPath(string relative) => _context.Server.MapPath(relative);
	}
}