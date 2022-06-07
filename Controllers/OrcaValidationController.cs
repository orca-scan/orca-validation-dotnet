using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OrcaValidationDotNet.Controllers
{
    [ApiController]
    [Route("/")]
    public class OrcaValidationDotNet : ControllerBase
    {
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> validationReceiver()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                // get the raw JSON string
                string json = await reader.ReadToEndAsync();

                // convert into .net object
                dynamic data = JObject.Parse(json);

                // NOTE:
                // orca system fields start with ___
                // you can access the value of each field using the fieldname (data.Name, data.Barcode, data.Location)
                string name = (data.Name != null) ? (string)data.Name : "";

                //validation example
                if(name.Length > 20){
                    //return json error message
                    return Ok(new {
                        title = "Name is too long",
                        message = "Name cannot contain more than 20 characters"
                    });
                }
            }

            // return HTTP Status 200 with no body
            return Ok("");
        }
    }
}