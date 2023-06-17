using Microsoft.AspNetCore.Mvc;

public class OriginHeaderCheckAttribute : TypeFilterAttribute
{
	public OriginHeaderCheckAttribute() : base(typeof(OriginValidationFilter))
	{
	}
}