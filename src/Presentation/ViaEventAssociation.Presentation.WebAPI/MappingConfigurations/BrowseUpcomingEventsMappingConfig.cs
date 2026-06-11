using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

public class BrowseUpcomingEventsRequestToQuery : IMappingConfig<BrowseUpcomingEventsRequest, BrowseUpcomingEvents.Query>
{
    public BrowseUpcomingEvents.Query Map(BrowseUpcomingEventsRequest input)
        => new(input.PageNum, input.PageSize, input.TitleSearch);
}

public class BrowseUpcomingEventsAnswerToResponse : IMappingConfig<BrowseUpcomingEvents.Answer, BrowseUpcomingEventsResponse>
{
    public BrowseUpcomingEventsResponse Map(BrowseUpcomingEvents.Answer input)
        => new(
            input.Events.Select(e => new BrowseUpcomingEventsResponse.EventSummary(
                e.EventId,
                e.Title,
                e.Description,
                e.Date,
                e.StartTime,
                e.MaxGuests,
                e.Visibility
            )).ToList(),
            input.TotalPages
        );
}