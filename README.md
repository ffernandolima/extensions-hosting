# extensions-hosting

Exposes Startup extensions to be used with generic host.

Also, it exposes task scheduling utilities based on cron expressions.

[![build-and-publish Workflow Status](https://github.com/ffernandolima/extensions-hosting/actions/workflows/build-and-publish.yml/badge.svg?branch=master)](https://github.com/ffernandolima/extensions-hosting/actions/workflows/build-and-publish.yml?branch=master)

 | Package | NuGet |
 | ------- | ----- |
 | Extensions.Hosting | [![Nuget](https://img.shields.io/badge/nuget-v2.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.Hosting)](https://www.nuget.org/packages/Extensions.Hosting/2.2.1) |
 | Extensions.Hosting.Scheduling | [![Nuget](https://img.shields.io/badge/nuget-v2.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.Hosting.Scheduling)](https://www.nuget.org/packages/Extensions.Hosting.Scheduling/2.2.1) |

## Installation

It is available on Nuget.

```
Install-Package Extensions.Hosting -Version 2.2.1
Install-Package Extensions.Hosting.Scheduling -Version 2.2.1
```

P.S.: There's no dependency between the packages. Which one has its own features.

## Usage

The following code demonstrates basic usage of Startup extensions.

```C#
public class Program
{
   public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

   public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {
               services.UseStartup<Startup>(); // Extension used here
           });
}

// Startup class should inherit IStartup interface
public class Startup : IStartup
{
   public Startup(IConfiguration configuration) => Configuration = configuration;

   public IConfiguration Configuration { get; }

   public void ConfigureServices(IServiceCollection services)
   {
       if (services == null)
       {
           throw new ArgumentNullException(nameof(services));
       }
       
       // Register your services here
   }
}
```

The following code demonstrates basic usage of task scheduling.

```C#
// Define a task which inherits from IScheduledTask interface
public class FooTask : IScheduledTask
{
   public string Schedule { get; private set; }

   public FooTask(IConfiguration configuration)
   {
       if (configuration == null)
       {
           throw new ArgumentNullException(nameof(configuration));
       }

       var schedule = configuration["Scheduling:Tasks:FooTask:Schedule"];

       if (string.IsNullOrWhiteSpace(schedule))
       {
           throw new ArgumentException(nameof(schedule));
       }

       Schedule = schedule; // Set the cron expression
   }
   
   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
       // Write your logic here
   }
}
 
public class Startup : IStartup
{
   public Startup(IConfiguration configuration) => Configuration = configuration;

   public IConfiguration Configuration { get; }

   public void ConfigureServices(IServiceCollection services)
   {
       if (services == null)
       {
           throw new ArgumentNullException(nameof(services));
       }
       
       var delay = configuration?.GetValue<TimeSpan>("Scheduling:Delay");
       
       services.AddSingleton<IScheduledTask, FooTask>();
       services.AddScheduler((sender, args) => args.SetObserved(), delay);
   }
}
```

appsettings.json:
```JSON
"Scheduling": {
    "Delay": "00:00:30",
    "Tasks": {
      "FooTask": {
        "Schedule": "* * * * *"
      }
    }
  }
```

## Support / Contributing
If you want to help with the project, feel free to open pull requests and submit issues. 

## Donate

If you would like to show your support for this project, then please feel free to buy me a coffee.

<a href="https://www.buymeacoffee.com/fernandolima" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/white_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>
