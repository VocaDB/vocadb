using System.Collections;
using System.Security.Principal;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web {

	public class AspNetHttpContext : IHttpContext {

		public AspNetHttpContext(HttpContext context) {
			this.context = context;
		}

		private readonly HttpContext context;

		public IDictionary Items => context.Items;

		public IHttpRequest Request => new AspNetHttpRequest(context.Request);

		public IHttpResponse Response => new AspNetHttpResponse(context.Response);

		public IPrincipal User { 
			get => context.User;
			set => context.User = value; 
		}

	}

}