#nullable disable

using System.Web;
using System.Web.Routing;

namespace VocaDb.Web.Code
{
	public class IdNotNumberConstraint : IRouteConstraint
	{
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var val = values[parameterName].ToString();
			return !int.TryParse(val, out _);
		}
	}
}