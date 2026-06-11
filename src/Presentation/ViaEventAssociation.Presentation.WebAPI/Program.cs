using ViaEventAssociation.Core.Application.Extensions;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;
using ViaEventAssociation.Infrastructure.EfcQueries;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;
using ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new ViaEventAssociation.Presentation.WebAPI.Infrastructure.TimeOnlyJsonConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "VIA Event Association API",
        Version = "v1",
        Description = """

            **Commands** return `204 No Content` on success, `400 Bad Request` with an array of domain errors on failure.

            **Queries** return the requested resource directly.
            """
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

var writeDb = builder.Configuration.GetConnectionString("WriteDb") ?? "Data Source=VEADatabase.db";
var readDb = builder.Configuration.GetConnectionString("ReadDb") ?? "Data Source=VEADatabase.db";

builder.Services.AddCommandDispatcher();
builder.Services.AddWritePersistence(writeDb);
builder.Services.AddQueryPersistence(readDb);

builder.Services.AddScoped<IMapper, ObjectMapper>();

builder.Services.AddScoped<IMappingConfig<ViewSingleEventRequest, ViewSingleEvent.Query>, ViewSingleEventRequestToQuery>();
builder.Services.AddScoped<IMappingConfig<ViewSingleEvent.Answer, ViewSingleEventResponse>, ViewSingleEventAnswerToResponse>();

builder.Services.AddScoped<IMappingConfig<BrowseUpcomingEventsRequest, BrowseUpcomingEvents.Query>, BrowseUpcomingEventsRequestToQuery>();
builder.Services.AddScoped<IMappingConfig<BrowseUpcomingEvents.Answer, BrowseUpcomingEventsResponse>, BrowseUpcomingEventsAnswerToResponse>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ViaEventAssociation.Infrastructure.EfcDmPersistence.EfcDbContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VEA API v1"));

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();
app.Run();

public partial class Program;