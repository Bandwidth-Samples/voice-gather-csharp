# Gather Digits

<a href="https://dev.bandwidth.com/docs/voice/quickStart">
  <img src="./icon-voice.svg" title="Voice Quick Start Guide" alt="Voice Quick Start Guide"/></p>
</a>

 # Table of Contents

- [Gather Digits](#gather-digits)
- [Table of Contents](#table-of-contents)
- [Description](#description)
- [Pre-Requisites](#pre-requisites)
- [Environmental Variables](#environmental-variables)
- [Running the Application](#running-the-application)
- [Callback URLs](#callback-urls)
  - [Ngrok](#ngrok)

# Description

This sample app creates an outbound call to the Bandwidth Phone Number, and if answered will prompt the user using [Gather BXML](https://dev.bandwidth.com/docs/voice/bxml/gather) to select between a list of options to hear different messages played back.

# Pre-Requisites

In order to use the Bandwidth API users need to set up the appropriate application at the [Bandwidth Dashboard](https://dashboard.bandwidth.com/) and create API tokens.

To create an application log into the [Bandwidth Dashboard](https://dashboard.bandwidth.com/) and navigate to the `Applications` tab.  Fill out the **New Application** form selecting the service that the application will be used for (this sample app uses a Voice application).  All Bandwidth services require publicly accessible Callback URLs, for more information on how to set one up see [Callback URLs](#callback-urls).

For more information about API credentials see our [Account Credentials](https://dev.bandwidth.com/docs/account/credentials) page.

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
 
# Running the Application

Use the following commands to run the application:

```sh
cd VoiceGather/     
dotnet run
```

# Callback URLs

For a detailed introduction to Bandwidth Callbacks see https://dev.bandwidth.com/guides/callbacks/callbacks.html

Below are the callback paths:
* `/calls`                          - POST to create a call to a phone number specified
* `/callbacks/outbound/voice`       - Bandwidth will POST a callback to this endpoint (setup in https://dashboard.bandwidth.com)
* `/callbacks/outbound/gather`      - Bandwidth will POST a callback here once the Gather has finished.

## Ngrok

A simple way to set up a local callback URL for testing is to use the free tool [ngrok](https://ngrok.com/).  
After you have downloaded and installed `ngrok` run the following command to open a public tunnel to your port (5001)
```cmd
ngrok http 5001
```

You can view your public URL at `http://127.0.0.1:4040` after ngrok is running.  You can also view the status of the tunnel and requests/responses here.

*Note: If you would like to change your port number feel free to do so. However, if you do change the port you will also need to change the number appended to the application URL in the `launchSettings.json` file located in `SendReceiveSMS/Properties/`*
