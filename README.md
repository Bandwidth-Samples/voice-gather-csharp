# Gather Digits

<a href="https://dev.bandwidth.com/docs/voice/quickStart">
  <img src="./icon-voice.svg" title="Voice Quick Start Guide" alt="Voice Quick Start Guide"/></p>
</a>

 # Table of Contents

* [Description](#description)
* [Pre-Requisites](#pre-requisites)
* [Running the Application](#running-the-application)
* [Environmental Variables](#environmental-variables)
* [Callback URLs](#callback-urls)
  * [Ngrok](#ngrok)

# Description

This sample app creates an outbound call to the Bandwidth Phone Number, and if answered will prompt the user using [Gather BXML](https://dev.bandwidth.com/docs/voice/bxml/gather) to select between a list of options to hear different messages played back.

# Pre-Requisites

In order to use the Bandwidth API users need to set up the appropriate application at the [Bandwidth Dashboard](https://dashboard.bandwidth.com/) and create API tokens.

To create an application log into the [Bandwidth Dashboard](https://dashboard.bandwidth.com/) and navigate to the `Applications` tab.  Fill out the **New Application** form selecting the service (Messaging or Voice) that the application will be used for.  All Bandwidth services require publicly accessible Callback URLs, for more information on how to set one up see [Callback URLs](#callback-urls).

For more information about API credentials see our [Account Credentials](https://dev.bandwidth.com/docs/account/credentials) page.
 
# Running the Application

Use the following command/s to run the application:

```sh
cd ./Server     
dotnet run      # To start the local server
cd ../Gather
dotnet run      # To run the gather project
```

This can be accomplisehd by opening both projects in Microsoft Visual Studio and run using the button in the toolbar.
Note that the Server project must be ran before the Gather project.

# Environmental Variables

The sample app uses the below environmental variables.

```sh
BW_ACCOUNT_ID                        # Your Bandwidth Account Id
BW_USERNAME                          # Your Bandwidth API Username
BW_PASSWORD                          # Your Bandwidth API Password
BW_NUMBER                            # The Bandwidth phone number involved with this application
USER_NUMBER                          # The user's phone number involved with this application
BW_VOICE_APPLICATION_ID              # Your Voice Application Id created in the dashboard
BASE_CALLBACK_URL                    # Your public base url to receive Bandwidth Webhooks. No trailing '/'
```

# Callback URLs

For a detailed introduction, check out our [Bandwidth Voice Callbacks](https://dev.bandwidth.com/docs/voice/webhooks) page.

Below are the callback paths:
* `/callbacks/gatherCallback` Hit once the gather has been completed
* `/callbacks/callAnsweredCallback` Hit once the phone call is answered
* `/callbacks/callDisconnectCallback` Hit if the phone call is disconnected

## Ngrok

A simple way to set up a local callback URL for testing is to use the free tool [ngrok](https://ngrok.com/).  
After you have downloaded and installed `ngrok` run the following command to open a public tunnel to your port (`5000`).
`5000` is the default port for .NET and `5001` is the default for https.

```cmd
ngrok http 5000
```

You can view your public URL at `http://127.0.0.1:4040` after ngrok is running.  You can also view the status of the tunnel and requests/responses here.
