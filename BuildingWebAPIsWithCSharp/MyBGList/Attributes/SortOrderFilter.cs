using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyBGList.Attributes
{
    public class SortOrderFilter : IParameterFilter
    {


        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var attribtues = context.
                ParameterInfo?.
                GetCustomAttributes(true).
                Union(context.ParameterInfo.ParameterType.GetProperties()
                .Where(p => p.Name == parameter.Name).
                SelectMany(p => p.GetCustomAttributes(true)))
                .OfType<SortOrderValidatorAttribute>();
            if (attribtues != null)
            {
                foreach (var attribute in attribtues)
                {
                    parameter.Schema.Extensions.Add("pattern", new OpenApiString(string.Join("|", attribute.AllowedValues.Select(v => $"^{v}$")
                        )));
                }
            }
        }
    }
}
