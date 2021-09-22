using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bandwidth.Standard.Voice.Bxml;
using Bandwidth.Standard.Voice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CallbacksController : ControllerBase
    {
        // The base url to use for voice callbacks.
        private static readonly string BaseUrl = System.Environment.GetEnvironmentVariable("BASE_CALLBACK_URL");

        private readonly ILogger<CallbacksController> _logger;

        public CallbacksController(ILogger<CallbacksController> logger)
        {
            _logger = logger;
        }

        [HttpPost("gatherCallback")]
        public async Task<ActionResult> Gather()
        {
            _logger.LogInformation($"{Request.Path.Value} requested.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            var json = JObject.Parse(body);
            
            var response = new Response();

            var eventType = (string)json["eventType"];
            switch (eventType)
            {
                case "gather":
                    var digits = (string)json["digits"];

                    // Respond to the user's selection with a response that a valid or invalid selection was made.
                    var sentence = new List<string> { "1", "2" }
                        .Any(digits.Contains) ? $"You have chosen option {digits}, thank you." : "An invalid option has been chosen.";

                    var gather = new Gather()
                    {
                        GatherUrl = $"{BaseUrl}/callbacks/gatherCallback",
                        TerminatingDigits = "#",
                        RepeatCount = 3,
                        SpeakSentence = new SpeakSentence
                        {
                            Sentence = sentence
                        }
                    };

                    response.Add(gather);
                    break;
                default:
                    // When an invalid event type is returned respond with the event type and end the call.
                    var speakSentence = new SpeakSentence
                    {
                        Sentence = $"{eventType} event received, ending call."
                    };

                    response.Add(speakSentence);
                    break;
            }

            // Convert the response to BXML before sending.
            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callAnsweredCallback")]
        public ActionResult CallAnswered()
        {
            _logger.LogInformation($"{Request.Path.Value} requested.");

            // Start gathering the user's input for the call.
            var gather = new Gather()
            {
                GatherUrl = $"{BaseUrl}/callbacks/gatherCallback",
                TerminatingDigits = "#",
                RepeatCount = 3,
                SpeakSentence = new SpeakSentence
                {
                    Sentence = "Hit one for option 1, hit two for option 2, then hit pound."
                }
            };

            var response = new Response(gather);

            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callDisconnectCallback")]
        public ActionResult CallDisconnect()
        {
            _logger.LogInformation($"{Request.Path.Value} requested.");
            _logger.LogInformation("Disconnect event received. Call ended.");

            return new OkResult();
        }
                   
        [HttpPost("callInitiatedCallback")]
        public ActionResult CallInitiated()
        {
            _logger.LogInformation($"{Request.Path.Value} requested.");

            var speakSentence = new SpeakSentence();
            speakSentence.Sentence = "Initiate event received. Ending call.";

            var response = new Response(speakSentence);

            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callStatusCallback")]
        public ActionResult CallStatus()
        {
            _logger.LogInformation($"{Request.Path.Value} requested.");

            return new OkResult();
        }
    }
}
