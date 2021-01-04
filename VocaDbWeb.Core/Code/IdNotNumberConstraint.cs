using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace VocaDb.Web.Code
{
	public class IdNotNumberConstraint : IRouteConstraint
	{
		public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var val = values[routeKey]?.ToString();
			return !int.TryParse(val, out _);
		}
	}
}