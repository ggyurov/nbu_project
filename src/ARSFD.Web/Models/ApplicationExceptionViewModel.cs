using System;
using Newtonsoft.Json;

namespace SmartFuel.Web.ViewModels
{
	public class ApplicationExceptionViewModel
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("typeName")]
		public string TypeName { get; set; }

		[JsonProperty("stackTrace", NullValueHandling = NullValueHandling.Ignore)]
		public string StackTrace { get; set; }

		[JsonProperty("innerError", NullValueHandling = NullValueHandling.Ignore)]
		public ApplicationExceptionViewModel InnerException { get; set; }

		[JsonIgnore]
		public Exception Exception { get; set; }
	}
}