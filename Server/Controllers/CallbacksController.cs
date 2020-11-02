using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bandwidth.Standard.Messaging.Models;
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
        private static readonly string BaseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");

        private readonly ILogger<CallbacksController> _logger;

        public CallbacksController(ILogger<CallbacksController> logger)
        {
            _logger = logger;
        }

        [HttpPost("gatherCallback")]
        public async Task<ActionResult> Gather()
        {
            _logger.LogInformation("Received gather callback request.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            var json = JObject.Parse(body);
            
            var response = new Response();

            var eventType = (string)json["eventType"];
            switch (eventType)
            {
                case "gather":
                    var digits = (string)json["digits"];

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
                    var speakSentence = new SpeakSentence
                    {
                        Sentence = $"{eventType} event received, ending call."
                    };

                    response.Add(speakSentence);
                    break;
            }

            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callAnsweredCallback")]
        public async Task<ActionResult> CallAnswered()
        {
            _logger.LogInformation("Received call answered callback request.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            _logger.LogInformation(body);

            var response = new Response();

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

            response.Add(gather);
            
            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callDisconnectCallback")]
        public async Task<ActionResult> CallDisconnect()
        {
            _logger.LogInformation("Received call disconnect callback request.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            _logger.LogInformation(body);

            return new OkResult();
        }
                   
        [HttpPost("callInitiatedCallback")]
        public async Task<ActionResult> CallInitiated()
        {
            _logger.LogInformation("Received call initiated callback request.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            
            _logger.LogInformation(body);

            var speakSentence = new SpeakSentence();
            speakSentence.Sentence = "Initiate event received. Ending call.";

            var response = new Response();
            response.Add(speakSentence);

            return new OkObjectResult(response.ToBXML());
        }

        [HttpPost("callStatusCallback")]
        public async Task<ActionResult> CallStatus()
        {
            _logger.LogInformation("Received call status callback request.");

            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            _logger.LogInformation(body);

            return new OkResult();
        }
    }
}
