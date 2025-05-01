using Topicality.Client.Application;
using Topicality.Client.Api.Endpoints;
using Topicality.Client.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Setup Sentry
builder.WebHost.UseSentry(a => { a.UseAsyncFileIO = true; });
builder.Services.AddSentry();

// Setup services
builder.Services.AddLogging();
builder.Services.ConfigureInfrastructure(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.AddDateOnlyTimeOnlyStringConverters();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options => 
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter{});
    }).AddXmlSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1",
        new OpenApiInfo()
        {
            Title = "DAC7 Ccn2 Client API",
            Description = "DAC7 DPI dokumentu apstrādes tīmekļa mikropakalpe",
            Version = "v1"
        });

    config.UseAllOfForInheritance();
    config.UseAllOfToExtendReferenceSchemas();
    config.UseOneOfForPolymorphism();
    config.EnableAnnotations(true, true);
    config.SupportNonNullableReferenceTypes();
    config.DescribeAllParametersInCamelCase();
    config.UseDateOnlyTimeOnlyStringConverters();

    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    if (!string.IsNullOrWhiteSpace(path))
    {
        foreach (var filePath in System.IO.Directory.GetFiles(path, "*.xml"))
        {
            try
            {
                config.IncludeXmlComments(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}).AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.ConfigureExceptionHandler();
app.UseSentryTracing();
app.UseSwagger();
app.UseSwaggerUI();
app.MapClientEndpoints();
app.Run();
