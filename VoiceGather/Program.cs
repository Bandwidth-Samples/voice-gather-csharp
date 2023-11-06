using Bandwidth.Standard.Api;
using Bandwidth.Standard.Client;
using Bandwidth.Standard.Model;
using Bandwidth.Standard.Model.Bxml;
using Bandwidth.Standard.Model.Bxml.Verbs;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string BW_USERNAME;
string BW_PASSWORD;
string BW_VOICE_APPLICATION_ID;
string BW_ACCOUNT_ID;
string BW_NUMBER;
string BASE_CALLBACK_URL;

//Setting up environment variables
try
{
    BW_USERNAME = Environment.GetEnvironmentVariable("BW_USERNAME");
    BW_PASSWORD = Environment.GetEnvironmentVariable("BW_PASSWORD");
    BW_VOICE_APPLICATION_ID = Environment.GetEnvironmentVariable("BW_VOICE_APPLICATION_ID");
    BW_ACCOUNT_ID = Environment.GetEnvironmentVariable("BW_ACCOUNT_ID");
    BW_NUMBER = Environment.GetEnvironmentVariable("BW_NUMBER");
    BASE_CALLBACK_URL = Environment.GetEnvironmentVariable("BASE_CALLBACK_URL");
}
catch (Exception)
{
    Console.WriteLine("Please set the environmental variables defined in the README");
    throw;
}

Configuration configuration = new Configuration();
configuration.Username = BW_USERNAME;
configuration.Password = BW_PASSWORD;

app.MapPost("/calls", async (HttpContext context) =>
{ 
    var requestBody = new Dictionary<string, string>();
    using(var streamReader = new StreamReader(context.Request.Body))
    {
        var body = await streamReader.ReadToEndAsync();
        requestBody = JsonConvert.DeserializeObject<Dictionary<string,string>>(body);
    }

    CreateCall createCall = new CreateCall(
        to: requestBody["to"],
        from: BW_NUMBER,
        applicationId: BW_VOICE_APPLICATION_ID,
        answerUrl: BASE_CALLBACK_URL + "/callbacks/outbound/voice"
    );

    CallsApi apiInstance = new CallsApi(configuration);
    try
    {
        // Create a call
        var result = await apiInstance.CreateCallAsync(BW_ACCOUNT_ID, createCall);
    }
    catch (ApiException e)
    {
        Console.WriteLine("Exception when calling CallsApi.CreateCall: " + e.Message);
    }

});

app.MapPost("/callbacks/outbound/voice", async (HttpContext context) =>
{
    var requestBody = new Dictionary<string, string>();
    using(var streamReader = new StreamReader(context.Request.Body))
    {
        var body = await streamReader.ReadToEndAsync();
        requestBody = JsonConvert.DeserializeObject<Dictionary<string,string>>(body);
    }
    
    var response = new Response();

    switch (requestBody["eventType"])
    {
        case "answer":
            response.Add
            (
                new Gather()
                {
                    GatherUrl = BASE_CALLBACK_URL + "/callbacks/outbound/gather",
                    TerminatingDigits = "#",
                    SpeakSentence = new List<SpeakSentence>()
                    {
                        new SpeakSentence()
                        {
                            Text = "Press 1 to choose option 1. Press 2 to say you chose option 2. Hit pound when you are finished."
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
            Console.WriteLine("The Disconnect event is fired when a call ends, for any reason. The cause for a disconnect event on a call can be:");
            Console.WriteLine($"Call {requestBody["callId"]} has disconnected");
            break;
        default:
            Console.WriteLine($"Unexpected event type {requestBody["eventType"]} received");
            break;
    }

    return response.ToBXML();
});

app.MapPost("/callbacks/outbound/gather", async (HttpContext context) =>
{
    var requestBody = new Dictionary<string, string>();
    using(var streamReader = new StreamReader(context.Request.Body))
    {
        var body = await streamReader.ReadToEndAsync();
        requestBody = JsonConvert.DeserializeObject<Dictionary<string,string>>(body);
    }
    
    var response = new Response();

    if (requestBody["eventType"] == "gather")
    {
        var digits = requestBody["digits"];
        if (digits == "1")
        {
            response.Add
            (
                new SpeakSentence()
                {
                    Text = "You chose option 1. Good choice!"
                }
            );
        }
        else if (digits == "2")
        {
            response.Add
            (
                new SpeakSentence()
                {
                    Text = "You chose option 2. Good choice!"
                }
            );
        }
        else
        {
            response.Add
            (
                new SpeakSentence()
                {
                    Text = "You did not choose a valid option. Goodbye!"
                }
            );
        }
    }

    return response.ToBXML();
});



app.Run();
