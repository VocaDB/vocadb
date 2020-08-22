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

		private readonly HttpContext context;
		private readonly IWebHostEnvironment webHostEnvironment;

		public AspNetCoreHttpContext(IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment) {
			context = contextAccessor.HttpContext;
			this.webHostEnvironment = webHostEnvironment;
		}

		public IDictionary Items => new Dictionary<object, object>(context.Items);
		public IHttpRequest Request => new AspNetCoreHttpRequest(context.Request);
		public IHttpResponse Response => new AspNetCoreHttpResponse(context.Response);

		public IPrincipal User {
			get => context.User;
			set => context.User = value as ClaimsPrincipal;
		}

		public IServerPathMapper ServerPathMapper => this;

		public string MapPath(string relative) => Path.Combine(webHostEnvironment.WebRootPath, relative.TrimStart('~', '/'));

	}

}
