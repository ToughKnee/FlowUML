// See https://aka.ms/new-console-template for more information
using Application;
using Infrastructure;
using Infrastructure.Antlr;
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
        services.AddInfrastructureLayerServices();
        services.AddPresentationLayerServices();

        // Build your service provider
        var serviceProvider = services.BuildServiceProvider();

        // Use your service
        var antlrService = serviceProvider.GetService<ANTLRService>();

        //===========================  ANTLR initialization
        antlrService.InitializeAntlr("C:\\Users\\Usuario\\source\\repos\\FlowUML\\Infrastructure\\ANTLR\\CSharp\\testFile.txt", true);
        antlrService.RunVisitorWithSpecificStartingRule("classDeclaration");

    }

}

