using Bandwidth.Standard.Api;
using Bandwidth.Standard.Client;
using Bandwidth.Standard.Model;
using Bandwidth.Standard.Model.Bxml;
using Bandwidth.Standard.Model.Bxml.Verbs;
using Newtonsoft.Json;

//Setting up environment variables
string? BW_ACCOUNT_ID = Environment.GetEnvironmentVariable("BW_ACCOUNT_ID");
string? BW_CLIENT_ID = Environment.GetEnvironmentVariable("BW_CLIENT_ID");
string? BW_CLIENT_SECRET = Environment.GetEnvironmentVariable("BW_CLIENT_SECRET");
string? BW_VOICE_APPLICATION_ID = Environment.GetEnvironmentVariable("BW_VOICE_APPLICATION_ID");
string? BW_NUMBER = Environment.GetEnvironmentVariable("BW_NUMBER");
string? BASE_CALLBACK_URL = Environment.GetEnvironmentVariable("BASE_CALLBACK_URL");
string? LOCAL_PORT = Environment.GetEnvironmentVariable("LOCAL_PORT");

if (BW_ACCOUNT_ID == null || BW_CLIENT_ID == null || BW_CLIENT_SECRET == null || BW_VOICE_APPLICATION_ID == null
    || BW_NUMBER == null || BASE_CALLBACK_URL == null || !int.TryParse(LOCAL_PORT, out int port))
{
    Console.WriteLine("Please set the environmental variables defined in the README");
    Environment.Exit(1);
    return;
}

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls($"http://localhost:{port}");
var app = builder.Build();

Configuration configuration = new Configuration();
configuration.OAuthClientId = BW_CLIENT_ID;
configuration.OAuthClientSecret = BW_CLIENT_SECRET;

async Task<Dictionary<string, object>?> ReadBody(HttpContext context)
{
    try
    {
        using var streamReader = new StreamReader(context.Request.Body);
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(await streamReader.ReadToEndAsync());
    }
    catch (JsonException)
    {
        return null;
    }
}

app.MapPost("/calls", async (HttpContext context) =>
{
    var requestBody = await ReadBody(context);
    if (requestBody == null || !requestBody.TryGetValue("to", out var to) || to is not string toNumber)
    {
        return Results.BadRequest();
    }

    CreateCall createCall = new CreateCall(
        to: toNumber,
        from: BW_NUMBER,
        applicationId: BW_VOICE_APPLICATION_ID,
        answerUrl: BASE_CALLBACK_URL + "/callbacks/outbound/voice"
    );

    try
    {
        CallsApi apiInstance = new CallsApi(configuration);
        await apiInstance.CreateCallAsync(BW_ACCOUNT_ID, createCall);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Failed to create call: {e.Message}");
        return Results.StatusCode(500);
    }

    return Results.NoContent();
});

app.MapPost("/callbacks/outbound/voice", async (HttpContext context) =>
{
    var requestBody = await ReadBody(context);
    if (requestBody == null)
    {
        return Results.BadRequest();
    }

    var response = new Response();
    requestBody.TryGetValue("eventType", out var eventType);

    switch (eventType?.ToString())
    {
        case "answer":
            response.Add
            (
                new Gather()
                {
                    GatherUrl = "/callbacks/outbound/voice/gather",
                    TerminatingDigits = "#",
                    SpeakSentence = new List<SpeakSentence>()
                    {
                        new SpeakSentence()
                        {
                            Text = "Press 1 to choose option 1. Press 2 to choose option 2. Press pound when you are finished."
                        }
                    }
                }
            );
            break;
        case "initiate":
            response.Add
            (
                new SpeakSentence()
                {
                    Text = "Initiate event received but not intended. Ending call."
                }
            );
            response.Add
            (
                new Hangup()
            );
            break;
        case "disconnect":
            Console.WriteLine($"Call {requestBody.GetValueOrDefault("callId")} has disconnected");
            break;
        default:
            Console.WriteLine($"Unexpected event type {eventType} received");
            break;
    }

    return Results.Content(response.ToBXML(), "application/xml");
});

app.MapPost("/callbacks/outbound/voice/gather", async (HttpContext context) =>
{
    var requestBody = await ReadBody(context);
    if (requestBody == null || requestBody.GetValueOrDefault("eventType")?.ToString() != "gather")
    {
        return Results.BadRequest();
    }

    var response = new Response();
    var digits = requestBody.GetValueOrDefault("digits")?.ToString();

    if (digits == "1")
    {
        response.Add
        (
            new SpeakSentence()
            {
                Text = "You chose option 1. Goodbye."
            }
        );
    }
    else if (digits == "2")
    {
        response.Add
        (
            new SpeakSentence()
            {
                Text = "You chose option 2. Goodbye."
            }
        );
    }
    else
    {
        response.Add
        (
            new SpeakSentence()
            {
                Text = "You did not choose a valid option. Goodbye."
            }
        );
    }

    return Results.Content(response.ToBXML(), "application/xml");
});

app.Run();
