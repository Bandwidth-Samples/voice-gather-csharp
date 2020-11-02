using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bandwidth.Standard;
using Bandwidth.Standard.Http.Response;
using Bandwidth.Standard.Voice.Exceptions;
using Bandwidth.Standard.Voice.Models;

namespace Gather
{
    class Program
    {
        // Bandwidth provided username.
        private static readonly string Username = System.Environment.GetEnvironmentVariable("BANDWIDTH_API_USER");

        // Bandwidth provided password.
        private static readonly string Password = System.Environment.GetEnvironmentVariable("BANDWIDTH_API_PASSWORD");

        // Bandwidth provided messaging token.
        private static readonly string Token = System.Environment.GetEnvironmentVariable("BANDWIDTH_MESSAGING_TOKEN");
        
        // Bandwidth provided messaging secret.
        private static readonly string Secret = System.Environment.GetEnvironmentVariable("BANDWIDTH_MESSAGING_SECRET");

        // Bandwidth provided application id.
        private static readonly string ApplicationId = System.Environment.GetEnvironmentVariable("BANDWIDTH_VOICE_APPLICATION_ID");

        // Bandwidth provided account id.
        private static readonly string AccountId = System.Environment.GetEnvironmentVariable("BANDWIDTH_ACCOUNT_ID");

        // The phone number to send the message from.
        private static readonly string From = System.Environment.GetEnvironmentVariable("BANDWIDTH_FROM");
        
        // The phone number to send the message to.
        private static readonly string To = System.Environment.GetEnvironmentVariable("BANDWIDTH_TO");

        // The base url to use for voice callbacks.
        private static readonly string BaseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");

        static async Task Main(string[] args)
        {
            // Creates a Bandwidth client instance for creating calls.
            var client = new BandwidthClient.Builder()
                .Environment(Bandwidth.Standard.Environment.Production)
                .VoiceBasicAuthCredentials(Username, Password)
                .Build();

            // A voice request containing the required information to create a call using the client.
            var request = new ApiCreateCallRequest()
            {
                ApplicationId = ApplicationId,
                To = To,
                AnswerUrl = $"{BaseUrl}/callbacks/callAnsweredCallback",
                DisconnectUrl = $"{BaseUrl}/callbacks/callDisconnectCallback",
                From = From
            };

            try
            {
                var response = await client.Voice.APIController.CreateCallAsync(AccountId, request);
            }
            catch (ApiErrorResponseException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
