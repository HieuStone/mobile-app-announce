using CallAlert.Api.Data;
using CallAlert.Api.Dtos.WatchNumbers;
using CallAlert.Api.Entities;
using CallAlert.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CallAlert.Api.Services.Implementations;

public class WatchNumberService : IWatchNumberService
{
    private readonly AppDbContext _dbContext;

    public WatchNumberService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<WatchNumberDto>> GetAsync(int userId)
    {
        var data = await _dbContext.WatchNumbers
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => MapToDto(w))
            .ToListAsync();

        return data;
    }

    public async Task<WatchNumberDto> CreateAsync(int userId, CreateWatchNumberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new ArgumentException("Số điện thoại không được để trống.");
        }

        var normalizedPhone = request.PhoneNumber.Trim();
        var exists = await _dbContext.WatchNumbers
            .AnyAsync(w => w.UserId == userId && w.PhoneNumber == normalizedPhone);

        if (exists)
        {
            throw new InvalidOperationException("Số điện thoại đã tồn tại trong danh sách theo dõi.");
        }

        var entity = new WatchNumber
        {
            UserId = userId,
            PhoneNumber = normalizedPhone,
            Label = request.Label,
            IsActive = true
        };

        _dbContext.WatchNumbers.Add(entity);
        await _dbContext.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<WatchNumberDto?> UpdateAsync(int userId, int id, UpdateWatchNumberRequest request)
    {
        var entity = await _dbContext.WatchNumbers
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (entity is null)
        {
            return null;
        }

        if (request.Label is not null)
        {
            entity.Label = request.Label;
        }

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var entity = await _dbContext.WatchNumbers
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (entity is null)
        {
            return false;
        }

        _dbContext.WatchNumbers.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static WatchNumberDto MapToDto(WatchNumber entity) =>
        new(entity.Id, entity.PhoneNumber, entity.Label, entity.IsActive, entity.CreatedAt, entity.UpdatedAt);
}


