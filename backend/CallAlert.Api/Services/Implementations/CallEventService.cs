using CallAlert.Api.Data;
using CallAlert.Api.Dtos.CallEvents;
using CallAlert.Api.Entities;
using CallAlert.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CallAlert.Api.Services.Implementations;

public class CallEventService : ICallEventService
{
    private readonly AppDbContext _dbContext;
    private readonly IMqttPublisherService _mqttPublisherService;
    private readonly ILogger<CallEventService> _logger;

    public CallEventService(
        AppDbContext dbContext,
        IMqttPublisherService mqttPublisherService,
        ILogger<CallEventService> logger)
    {
        _dbContext = dbContext;
        _mqttPublisherService = mqttPublisherService;
        _logger = logger;
    }

    public async Task<IEnumerable<CallEventDto>> GetAsync(int userId, DateTime? from, DateTime? to, bool? isWatched)
    {
        var query = _dbContext.CallEvents
            .AsNoTracking()
            .Where(c => c.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(c => c.CalledAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(c => c.CalledAt <= to.Value);
        }

        if (isWatched.HasValue)
        {
            query = query.Where(c => c.IsWatchedNumber == isWatched.Value);
        }

        var list = await query
            .OrderByDescending(c => c.CalledAt)
            .Select(c => MapToDto(c))
            .ToListAsync();

        return list;
    }

    public async Task<CallEventDto> CreateAsync(int userId, CreateCallEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CallerNumber))
        {
            throw new ArgumentException("CallerNumber không được để trống.");
        }

        var calledAt = request.CalledAt == default ? DateTime.UtcNow : request.CalledAt;
        var normalizedPhone = request.CallerNumber.Trim();

        var isWatched = await _dbContext.WatchNumbers
            .AnyAsync(w => w.UserId == userId && w.PhoneNumber == normalizedPhone && w.IsActive);

        var entity = new CallEvent
        {
            UserId = userId,
            CallerNumber = normalizedPhone,
            CalledAt = calledAt,
            CallStatus = request.CallStatus,
            DeviceId = request.DeviceId,
            IsWatchedNumber = isWatched,
            MqttPublished = false
        };

        _dbContext.CallEvents.Add(entity);
        await _dbContext.SaveChangesAsync();

        if (isWatched)
        {
            try
            {
                var payload = new
                {
                    userId,
                    callerNumber = entity.CallerNumber,
                    calledAt = entity.CalledAt,
                    callStatus = entity.CallStatus,
                    deviceId = entity.DeviceId
                };

                await _mqttPublisherService.PublishIncomingCallAsync(payload);

                entity.MqttPublished = true;
                entity.MqttPublishedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể publish MQTT cho call event {CallEventId}", entity.Id);
            }
        }

        return MapToDto(entity);
    }

    private static CallEventDto MapToDto(CallEvent entity) =>
        new(entity.Id, entity.CallerNumber, entity.CalledAt, entity.CallStatus, entity.DeviceId,
            entity.IsWatchedNumber, entity.MqttPublished, entity.MqttPublishedAt, entity.CreatedAt);
    public async Task<bool> TestMQTT()
    {
            await _mqttPublisherService.PublishIncomingCallAsync("helo");
            return true;
    }
}


