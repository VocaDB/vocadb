using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web {

	public class AspNetCoreHttpContext : IHttpContext, IServerPathMapper {

		private readonly HttpContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public AspNetCoreHttpContext(IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment) {
			_context = contextAccessor.HttpContext;
			_webHostEnvironment = webHostEnvironment;
		}

		public IDictionary Items => new Dictionary<object, object>(_context.Items);
		public IHttpRequest Request => new AspNetCoreHttpRequest(_context.Request);
		public IHttpResponse Response => new AspNetCoreHttpResponse(_context.Response);

		public IPrincipal User {
			get => _context.User;
			set => _context.User = value as ClaimsPrincipal;
		}

		public IServerPathMapper ServerPathMapper => this;

		public string MapPath(string relative) => Path.Combine(_webHostEnvironment.WebRootPath, relative.TrimStart('~', '/'));

	}

}
