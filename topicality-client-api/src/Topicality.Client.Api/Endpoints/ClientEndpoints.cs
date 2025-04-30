using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Xml.Linq;
using Topicality.Client.Application.Services;

namespace Topicality.Client.Api.Endpoints;

/// <summary>
/// Ccn2 client endpoints
/// </summary>
public static class ClientEndpoints
{
    /// <summary>
    /// Map Client endpoints and handlers
    /// </summary>
    /// <param name="app"></param>
    public static void MapClientEndpoints(this WebApplication app)
    {
        //app.MapPost("api/ccn2/{country}", SendMessageAsync)
        // .WithName("SendMessage")
        // .Produces(StatusCodes.Status204NoContent)
        // .WithMetadata(
        //     new SwaggerOperationAttribute(
        //         summary: "Nosūtīt DPI XML  ziņojumu.",
        //         description: "DPI XML ziņojums tiks nosūtīts uz CCN2.")
        // ).Accepts<object>("application/xml");
    }

  //  internal static async Task<IResult> SendMessageAsync(
  //      [SwaggerParameter("country")] [FromRoute]string country,
  //      [FromQuery]MessageType messageType,
  //      HttpContext context,
  //      ICcn2Service ccn2Service,
  //      CancellationToken cancellationToken = default)
  //  {
  //      var reader = new StreamReader(context.Request.Body);
  //      var xml = reader.ReadToEndAsync().Result;
  //      if (messageType == MessageType.DPI401)
  //      {
           // var result = await ccn2Service.SendInitialMessageAsync(xml, country);
  //          return Results.Ok(result.ToString());
  //      }        
  //      else
  //      {
  //          return Results.NotFound(messageType);
  //      }
  //  }
}
