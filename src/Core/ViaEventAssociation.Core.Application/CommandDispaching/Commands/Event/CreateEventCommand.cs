using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class CreateEventCommand : ICommand
{
	public EventTitle Title { get; }
	public EventDescription Description { get; }
	public EventMaxGuests MaxGuests { get; }

	private CreateEventCommand(EventTitle title, EventDescription description, EventMaxGuests maxGuests)
	{
		Title = title;
		Description = description;
		MaxGuests = maxGuests;
	}

	public static Result<CreateEventCommand> Create(string title, string? description, int maxGuests)
	{
		var errors = new List<ResultError>();

		EventTitle? eventTitle = null;
		var titleResult = EventTitle.Create(title);
		if (titleResult is Failure<EventTitle> titleFailure)
			errors.AddRange(titleFailure.Errors);
		else
			eventTitle = ((Success<EventTitle>)titleResult).Value;

		EventDescription? eventDescription = null;
		var descriptionResult = EventDescription.Create(description);
		if (descriptionResult is Failure<EventDescription> descriptionFailure)
			errors.AddRange(descriptionFailure.Errors);
		else
			eventDescription = ((Success<EventDescription>)descriptionResult).Value;

		EventMaxGuests? eventMaxGuests = null;
		var maxGuestsResult = EventMaxGuests.Create(maxGuests);
		if (maxGuestsResult is Failure<EventMaxGuests> maxGuestsFailure)
			errors.AddRange(maxGuestsFailure.Errors);
		else
			eventMaxGuests = ((Success<EventMaxGuests>)maxGuestsResult).Value;

		if (errors.Count > 0)
			return ResultHelper.Failure<CreateEventCommand>(errors);

		return ResultHelper.Success(new CreateEventCommand(eventTitle!, eventDescription!, eventMaxGuests!));
	}
}