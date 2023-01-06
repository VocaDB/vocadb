#nullable disable

using System.Collections;
using System.Security.Claims;
using System.Security.Principal;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web;

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

#nullable enable
	public IPrincipal User
	{
		get => Context.User;
		set => Context.User = (ClaimsPrincipal)value;
	}
#nullable disable

	public IServerPathMapper ServerPathMapper => this;

	public string MapPath(string relative) => Path.Combine(_webHostEnvironment.WebRootPath, relative.TrimStart('~', '/'));
}
