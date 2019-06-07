# Essenbee.Alexa and Essenbee.Alexa.Lib

[![Build status](https://ci.appveyor.com/api/projects/status/6dbvrjgbvsk1hl58/branch/master?svg=true)](https://ci.appveyor.com/project/essenbee/essenbee-alexa/branch/master)

We are building an Alexa Skill live on-stream at [Codebase Alpha](https://twitch.tv/codebasealpha) on Twitch. The skill, called _DevStreams_, is the voice interface for the **DevStreams** community website that is being developed on the [DevChatter](https://twitch.tv/devchatter) Twitch stream by Brendan Enrick and contributors. A fork of that repo can be found in this Github. Integration has been achieved through the introduction of a GraphQL endpoint into the DevStreams solution (also developed on-stream at Codebase Alpha). The `Essenbee.Alexa` code uses this endpoint to query DevStreams and thus provide users with the information about live coding streams that they have requested.

The primary reusable component created in this project is the `Essenbee.Alexa.Lib` NuGet package. Please see below for installation and usage instructions.

### YouTube

The main phase of development for this code is encompassed by Episodes 4 - 11 of Codebase Alpha. The videos for these episodes are archived on YouTube [here](https://www.youtube.com/channel/UCFFtfkaWjMb9UMDpPVnC1Sg).

# Installation

[![nuget](https://img.shields.io/nuget/v/Essenbee.Alexa.Lib.svg)](https://www.nuget.org/packages/Essenbee.Alexa.Lib/)

Install the `Essenbee.Alexa.Lib` NET Standard 2.0 library into your own ASP.NET Core (2.2+) Web App or Web API via Nuget with the command:

`dotnet add package Essenbee.Alexa.Lib`

# Current Features

The library contains the following features at this stage:

- [X] Middleware and methods to satisfy Amazon's security requirements for Skills, see [here](https://developer.amazon.com/docs/custom-skills/security-testing-for-an-alexa-skill.html)
- [X] `AlexaRequest` and `AlexaResponse` classes for speech, cards and dialogs, but not yet for AudioPlayer requests at this time
- [X] A fluent `ResponseBuilder` that makes creating responses easy
- [X] A strongly-typed HTTP client (`AlexaClient`) that you can use to access Device Settings or User Address details (with the user's permission)

Further features will be added over time, with the intent being to provide library support for all of the features that Alexa skills can exploit. However, that takes time, especially doing the work live on Twitch, so please check back here often to see what is new.

# Using the Library

Install the latest NuGet package into your project. In the `Startup.cs` class, add the following code to the `Configure` method:

```
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/alexa"), (appBuilder) =>
{
    appBuilder.UseAlexaRequestValidation();
});
```

To use the strongly-typed HTTP client, add this line to your `ConfigureServices` method:

```
services.AddHttpClient<IAlexaClient, AlexaClient>();
```

Make sure that you store your Alexa Skill Application ID (securely) in your `Configuration`. In the example code, it is held in `Configuration["SkillId"]`. This is needed for the `AlexaRequest`ShouldProcessRequest()` static method call shown below. It is this method that, alongside the custom Middleware, ensures that your Skill staisfies Amazon's security requirements.

Create an `AlexaController` using the example below as a starting point:

```
[ApiController]
[Produces("application/json")]
public class AlexaController : ControllerBase
{
    private IConfiguration _config;
    private ILogger<AlexaController> _logger;
    private readonly IAlexaClient _client;

    public AlexaController(IConfiguration config, ILogger<AlexaController> logger, IAlexaClient client)
    {
        _config = config;
        _logger = logger;
        _client = client;
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(AlexaResponse))]
    [ProducesResponseType(400)]
    [Route("api/alexa/your-endpoint")]
    public async Task<ActionResult<AlexaResponse>> MySkill ([FromBody] AlexaRequest alexaRequest )
    {
        if (!AlexaRequest.ShouldProcessRequest(_config["SkillId"], alexaRequest))
        {
            _logger.LogError("Bad Request - application id did not match or timestamp tolerance exceeded!");
            return BadRequest();
        }
        
        // Your skill's code here
    }        
}
```

You use the `ResponseBuilder` like this:

#### Simple Speech Response
```
var response = new ResponseBuilder()
                .Say("To use this skill, ask me about the schedule of your favourite stream.")
                .Build(); 
```
#### Speech Response with Card
```
var response = new ResponseBuilder()
                .Say("Codebase Alpha is streaming now")
                .WriteSimpleCard("Streaming Now!", "Codebase Alpha")
                .Build();
```
#### SSML Speech Response
```
var ssml = @"<speak>Welcome to my skill</speak>";
var response = new ResponseBuilder()
                .SayWithSsml(ssml)
                .Build(); 
```
#### Ask Something, Expecting a User to Respond (with Reprompt or Not)
```
var response = new ResponseBuilder()
                .Ask("Try asking me who is streaming now",
                     "You can ask me who is streaming now")
                .Build();
```
#### Delegate Dialog to Alexa Manually
```
var response = new ResponseBuilder()
                .DelegateDialog()
                .Build();
```

## Discord

If you have any questions, suggestions or bug reports relating to the `Essenbee.Alexa.Lib` library, head over to my Discord [here](https://discord.gg/Rz8r93q). Why not join me for some live coding on Twitch? We work on a variety of projects, not just Alexa skills.
