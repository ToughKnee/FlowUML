// See https://aka.ms/new-console-template for more information
using Application;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
public class Program
{
    public static ReceiveInject InjectTestt { get; set; }

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        // Get Services From Dependency Injection
        var services = new ServiceCollection();
        services.AddApplicationLayerServices();
        services.AddPresentationLayerServices();

        // Configure your services here...
        // For example, if you have a service class named 'MyService', you can add it like this:

        // Build your service provider
        var serviceProvider = services.BuildServiceProvider();

        // Use your service
        InjectTestt = serviceProvider.GetService<ReceiveInject>();


        Console.WriteLine($"Inject test -> {InjectTestt.GetNumber()}");

    }

}

