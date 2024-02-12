using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpenRIMS.PV.Main.API.Infrastructure.OperationFilters
{
    public class GetAppointmentOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.OperationId == "GetAppointmentByIdentifier")
            //{
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.main.detail.v1+json",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(LinkedCollectionResourceWrapperDto<ProductDetailDto>))
            //        });
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.main.detail.v1+xml",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(LinkedCollectionResourceWrapperDto<ProductDetailDto>))
            //        });
            //}
        }
    }
}
