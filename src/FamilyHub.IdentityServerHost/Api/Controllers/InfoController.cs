using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FamilyHub.IdentityServerHost.Api.Controllers;

public class WebMarker
{
}

[Route("api/[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
    // GET: api/<InfoController>
    [HttpGet]
    public IResult Get()
    {
        try
        {
            var assembly = typeof(WebMarker).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Results.Ok($"Version: {version}, Last Updated: {creationDate}");
        }
        catch (Exception ex)
        {
            //logger.LogError(ex, "An error occurred getting info (api). {exceptionMessage}", ex.Message);
            Debug.WriteLine(ex.Message);
            throw;
        }
    }

    // GET api/<InfoController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<InfoController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<InfoController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<InfoController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
