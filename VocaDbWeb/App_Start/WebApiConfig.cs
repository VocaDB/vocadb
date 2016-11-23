using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using VocaDb.Web.Code.WebApi;
using WebApiContrib.Formatting.Jsonp;

namespace VocaDb.Web.App_Start {

	/// <summary>
	/// Configures ASP.NET Web API
	/// </summary>
	public static class WebApiConfig {

		public static void Configure(HttpConfiguration config) {
			
			var json = config.Formatters.JsonFormatter;
			json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // All properties in camel case
			json.SerializerSettings.Converters.Add(new StringEnumConverter());	// All enums as strings by default
			json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

			config.Formatters.Add(new CsvMediaTypeFormatter());
			config.AddJsonpFormatter(json);

			var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "get");
			config.EnableCors(cors);
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				"DefaultApi", "api/{controller}/{id}",
				new { id = RouteParameter.Optional });

			config.Filters.Add(new ObjectNotFoundExceptionFilterAttribute());
			config.Services.Add(typeof(IExceptionLogger), new UnhandledExceptionLogger());

		}

	}

}