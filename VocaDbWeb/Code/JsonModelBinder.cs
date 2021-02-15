using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NLog;

namespace VocaDb.Web.Code
{
	public class JsonModelBinder : IModelBinder
	{
		private static readonly ILogger s_log = LogManager.GetCurrentClassLogger();

		// Code from: https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-5.0
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
				throw new ArgumentNullException(nameof(bindingContext));

			var modelName = bindingContext.ModelName;

			var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
			if (valueProviderResult == ValueProviderResult.None)
				return Task.CompletedTask;

			bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

			var value = valueProviderResult.FirstValue;
			if (string.IsNullOrEmpty(value))
				return Task.CompletedTask;

			object obj;

			try
			{
				obj = JsonConvert.DeserializeObject(value, bindingContext.ModelMetadata.ModelType, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			catch (JsonReaderException x)
			{
				s_log.Error(x, "Unable to process JSON, content is " + value);
				throw;
			}

			bindingContext.Result = ModelBindingResult.Success(obj);

			return Task.CompletedTask;
		}
	}
}
