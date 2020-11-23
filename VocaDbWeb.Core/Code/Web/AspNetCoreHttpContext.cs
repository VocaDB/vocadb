using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetCoreHttpContext : IHttpContext, IServerPathMapper
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public AspNetCoreHttpContext(IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment)
		{
			_contextAccessor = contextAccessor;
			_webHostEnvironment = webHostEnvironment;
		}

		private HttpContext Context => _contextAccessor.HttpContext;

		public IDictionary Items => new Dictionary<object, object>(Context.Items);
		public IHttpRequest Request => new AspNetCoreHttpRequest(Context.Request);
		public IHttpResponse Response => new AspNetCoreHttpResponse(Context.Response);

		public IPrincipal User
		{
			get => Context.User;
			set => Context.User = value as ClaimsPrincipal;
		}

		public IServerPathMapper ServerPathMapper => this;

		public string MapPath(string relative) => Path.Combine(_webHostEnvironment.WebRootPath, relative.TrimStart('~', '/'));
	}
}
