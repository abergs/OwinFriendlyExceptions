# Owin Friendly Exceptions Middleware

A middleware that can translate exceptions into nice http resonses. This allows you to throw meaningfull exceptions from your framework, business code or other middlewares and translate the exceptions to nice and friendly http responses.

## Installation

`Install-package OwinFriendlyExceptions`

## Plugins for different frameworks
Owin Friendly Exceptions is extensible and can work with frameworks that normally swallows exceptions.

See [OwinFriendlyExceptions.Plugins](https://github.com/abergs/OwinFriendlyExceptions.Plugins) for plugins to handle different frameworks (ASP.NET Web API etc...) 

## Example
![Example code snippet](https://cloud.githubusercontent.com/assets/357283/6561001/44427032-c68e-11e4-8dae-f24146c9bf78.PNG)

    using System;
    using System.Net;
    using Api.Exceptions;
    using Api.Logic.Exceptions;
    using Owin;
    using OwinFriendlyExceptions;
    using OwinFriendlyExceptions.Extensions;
    using OwinFriendlyExceptions.Plugins.WebApi2;
    
    namespace Api
    {
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                // Use FriendlyExceptions before your other middlewares
                app.UseFriendlyExceptions(GetFriendlyExceptions(), new [] {new WebApi2ExceptionProvider()});
    
                // Then the rest of your application
                //app.UseCors(options);
                //app.UseWelcomePage();
                //app.UseWebApi(config)
            }
    
            private ITransformsCollection GetFriendlyExceptions()
            {
                return TransformsCollectionBuilder.Begin()
    
                    .Map<ExampleException>()
                    .To(HttpStatusCode.BadRequest, "This is the reasonphrase",
                        ex => "And this is the response content: " + ex.Message)
    
                    .Map<SomeCustomException>()
                    .To(HttpStatusCode.NoContent, "Bucket is emtpy", ex => string.Format("Inner details: {0}", ex.Message))
    
                    .Map<EntityUnknownException>()
                    .To(HttpStatusCode.NotFound, "Entity does not exist", ex => ex.Message)
    
                    .Map<InvalidAuthenticationException>()
                    .To(HttpStatusCode.Unauthorized, "Invalid authentication", ex => ex.Message)
    
                    .Map<AuthorizationException>()
                    .To(HttpStatusCode.Unauthorized, "Unauthorized", ex => ex.Message)
    
                .Done();
            }
        }
    }

### [Web Api 2 plugin](https://github.com/abergs/OwinFriendlyExceptions.Plugins)

Installation:  

1. `Install-package OwinFriendlyExceptions.Plugins.WebApi2`
2. `app.UseFriendlyExceptions(exceptionsToHandle, new [] {new WebApi2ExceptionProvider()});`
3. `config.Services.Replace(typeof(IExceptionHandler), new WebApi2ExceptionHandler(exceptionsToHandle));`
Install the package, and supply the WebApi Exception Provider to the OwinFriendlyExceptions extension method. In order for the Plugin to get swalloed wexceptions you have to replace the ExcepionHandler service in Web Api. The plugin takes a list of which exceptions we can handle, so WebApi can still take care of unhandled exceptions for you.


### Contribute
Contributions are we welcome. Just open an Issue or submit a PR. 

### Contact
You can reach me at [@bigCheeseAnders](https://twitter.com/bigcheeseanders) or via my blog: [ideasof.andersaberg.com](http://ideasof.andersaberg.com/)
