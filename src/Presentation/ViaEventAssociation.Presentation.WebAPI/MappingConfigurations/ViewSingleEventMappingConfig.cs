using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

public class ViewSingleEventRequestToQuery : IMappingConfig<ViewSingleEventRequest, ViewSingleEvent.Query>
{
    public ViewSingleEvent.Query Map(ViewSingleEventRequest input)
        => new(input.Id);
}

public class ViewSingleEventAnswerToResponse : IMappingConfig<ViewSingleEvent.Answer, ViewSingleEventResponse>
{
    public ViewSingleEventResponse Map(ViewSingleEvent.Answer input)
        => new(
            input.EventId,
            input.Title,
            input.Description,
            input.LocationName,
            input.Date,
            input.StartTime,
            input.Visibility,
            input.MaxGuests
        );
}