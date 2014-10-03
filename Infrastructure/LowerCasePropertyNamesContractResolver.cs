using Newtonsoft.Json.Serialization;

namespace Detrack.Infrastructure
{
	public class LowerCasePropertyNamesContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			return propertyName.ToLower();
		}
	}
}
