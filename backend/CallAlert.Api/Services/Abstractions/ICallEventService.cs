using CallAlert.Api.Dtos.CallEvents;

namespace CallAlert.Api.Services.Abstractions;

public interface ICallEventService
{
    Task<IEnumerable<CallEventDto>> GetAsync(int userId, DateTime? from, DateTime? to, bool? isWatched);
    Task<CallEventDto> CreateAsync(int userId, CreateCallEventRequest request);
}


