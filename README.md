# orca-validation-dotnet

Example of [how to validate barcode scans in real-time](https://orcascan.com/guides/how-to-validate-barcode-scans-in-real-time-56928ff9) in C# using [ASP.NET Core](https://dotnet.microsoft.com/learn/aspnet/what-is-aspnet-core).

## Install

```bash
git clone git@github.com:orca-scan/orca-validation-dotnet.git
cd orca-validation-dotnet
dotnet restore
```

## Run

```bash
dotnet run
```

Your server will now be running on port 3000.

You can emulate an Orca Scan Validation input using [cURL](https://dev.to/ibmdeveloper/what-is-curl-and-why-is-it-all-over-api-docs-9mh) by running the following:

```bash
curl --location --request POST 'http://127.0.0.1:5000/' \
--header 'Content-Type: application/json' \
--data-raw '{
    "___orca_sheet_name": "Vehicle Checks",
    "___orca_user_email": "hidden@requires.https",
    "Barcode": "orca-scan-test",
    "Date": "2022-04-19T16:45:02.851Z",
    "Name": Orca Scan Validation Example,
}'
```

### Important things to note

1. Only Orca Scan system fields start with `___`
2. Properties in the JSON payload are an exact match to the  field names in your sheet _(case and space)_

## How this example works

A simple [ASP.NET Core](https://dotnet.microsoft.com/learn/aspnet/what-is-aspnet-core) controller that listens for HTTP POSTS.

```csharp
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

        // return HTTP Status 204 (No Content)
        return NoContent();
    }
}
```

## Test server locally against Orca Cloud

To expose the server securely from localhost and test it easily against the real Orca Cloud environment you can use [Secure Tunnels](https://ngrok.com/docs/secure-tunnels#what-are-ngrok-secure-tunnels). Take a look at [Ngrok](https://ngrok.com/) or [Cloudflare](https://www.cloudflare.com/).

```bash
ngrok http 3000
```

## Troubleshooting

If you run into any issues not listed here, please [open a ticket](https://github.com/orca-scan/orca-validation-dotnet/issues).

## Examples in other langauges
* [orca-validation-dotnet](https://github.com/orca-scan/orca-validation-dotnet)
* [orca-validation-python](https://github.com/orca-scan/orca-validation-python)
* [orca-validation-go](https://github.com/orca-scan/orca-validation-go)
* [orca-validation-java](https://github.com/orca-scan/orca-validation-java)
* [orca-validation-php](https://github.com/orca-scan/orca-validation-php)
* [orca-validation-node](https://github.com/orca-scan/orca-validation-node)

## History

For change-log, check [releases](https://github.com/orca-scan/orca-validation-dotnet/releases).

## License

&copy; Orca Scan, the [Barcode Scanner app for iOS and Android](https://orcascan.com)