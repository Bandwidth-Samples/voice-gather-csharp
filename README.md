# Gather Digits

<a href="https://dev.bandwidth.com/docs/voice/quickStart">
  <p style="text-align:center:"><img src="./icon-voice.svg" width="200" height="auto" title="Voice Quick Start Guide" alt="Voice Quick Start Guide"/></p>
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
dotnet run
```

Or open the project in Microsoft Visual Studio and run using the button in the toolbar.

# Environmental Variables

The sample app uses the below environmental variables.

```sh
BASE_CALLBACK_URL                    # Your public base url to receive Bandwidth Webhooks. No trailing '/'
```

# Callback URLs

For a detailed introduction, check out our [Bandwidth Voice Callbacks](https://dev.bandwidth.com/docs/voice/webhooks) page.

Below are the callback paths:
* `/callbacks/gatherCallback` Hit once the gather has been completed
* `/callbacks/callAnsweredCallback` Hit once the phone call is answered

## Ngrok

A simple way to set up a local callback URL for testing is to use the free tool [ngrok](https://ngrok.com/).  
After you have downloaded and installed `ngrok` run the following command to open a public tunnel to your port (`5000`).
`5000` is the default port for .NET and `5001` is the default for https.

```cmd
ngrok http 5000
```

You can view your public URL at `http://127.0.0.1:4040` after ngrok is running.  You can also view the status of the tunnel and requests/responses here.
