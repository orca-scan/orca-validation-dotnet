using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Handle all POST requests to the root path.
// Orca Scan sends barcode scan data here as JSON.
app.MapPost("/", async (HttpContext context) =>
{
    // Read the raw JSON body sent by Orca Scan.
    // This is the JSON data containing the barcode scan and all sheet field values.
    using var reader = new StreamReader(context.Request.Body);
    string json = await reader.ReadToEndAsync();

    // Parse the JSON so we can access individual fields by name.
    // Fields starting with ___ are Orca system fields (e.g. ___orca_sheet_name).
    // All other fields match your sheet column names exactly (case and spaces matter).
    // For example: data["Barcode"], data["Name"], data["___orca_sheet_name"]
    var data = JsonNode.Parse(json)?.AsObject();

    // Get the value of the Name field from the incoming data.
    // If the field is missing, name will be an empty string.
    string name = data?["Name"]?.GetValue<string>() ?? "";

    // ---------------------------------------------------------------
    // OPTION 1: Reject the scan and show an error dialog in the app.
    // Return HTTP 400 with ___orca_message to block the save and
    // display the message to the user. They must dismiss the dialog
    // before they can try again.
    // ---------------------------------------------------------------
    if (name.Length > 20)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new
        {
            ___orca_message = new
            {
                display = "dialog",
                type = "error",
                title = "Invalid Name",
                message = "Name cannot be longer than 20 characters"
            }
        });
        return;
    }

    // ---------------------------------------------------------------
    // OPTION 2: Modify the data before it saves.
    // Return HTTP 200 with only the fields you want to change.
    // Orca Scan will update those fields and allow the save.
    // ---------------------------------------------------------------
    // context.Response.StatusCode = 200;
    // await context.Response.WriteAsJsonAsync(new
    // {
    //     Name = name.Trim() // example: trim whitespace before saving
    // });
    // return;

    // ---------------------------------------------------------------
    // OPTION 3: Show a success notification (green banner in the app).
    // The data still saves - this just gives the user feedback.
    // Return HTTP 200 with ___orca_message to show the notification.
    // ---------------------------------------------------------------
    // context.Response.StatusCode = 200;
    // await context.Response.WriteAsJsonAsync(new
    // {
    //     ___orca_message = new
    //     {
    //         display = "notification",
    //         type = "success",
    //         message = "Barcode scanned successfully"
    //     }
    // });
    // return;

    // ---------------------------------------------------------------
    // SECURITY: Verify the request came from your specific Orca sheet.
    // Set a secret in Orca Scan (Integrations > Events API > Secret)
    // then check it matches here before trusting the data.
    // ---------------------------------------------------------------
    // string secret = context.Request.Headers["orca-secret"];
    // if (secret != Environment.GetEnvironmentVariable("ORCA_SECRET"))
    // {
    //     context.Response.StatusCode = 401;
    //     return;
    // }

    // All good - return HTTP 204 to allow the data to save with no changes.
    // HTTP 204 means "success, no content" - Orca Scan will save the data as-is.
    context.Response.StatusCode = 204;
});

// Use the PORT environment variable if set, otherwise default to 8888.
// This makes the server easy to deploy to cloud platforms that set PORT for you.
string port = Environment.GetEnvironmentVariable("PORT") ?? "8888";

Console.WriteLine("Listening on port " + port + ". Ready for Orca Scan requests.");
app.Run("http://0.0.0.0:" + port);
