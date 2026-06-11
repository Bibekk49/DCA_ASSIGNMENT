using ViaEventAssociation.Core.Application.Extensions;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;
using ViaEventAssociation.Infrastructure.EfcQueries;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;
using ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();

public partial class Program;