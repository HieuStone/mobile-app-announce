using CallAlert.Api.Dtos.WatchNumbers;

namespace CallAlert.Api.Services.Abstractions;

public interface IWatchNumberService
{
    Task<IEnumerable<WatchNumberDto>> GetAsync(int userId);
    Task<WatchNumberDto> CreateAsync(int userId, CreateWatchNumberRequest request);
    Task<WatchNumberDto?> UpdateAsync(int userId, int id, UpdateWatchNumberRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}


