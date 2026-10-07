using AutoMapper;
using HMS.Application.DTOs.Guests;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Exceptions;

namespace HMS.Application.Services;

public class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepository;
    private readonly IMapper _mapper;

    public GuestService(IGuestRepository guestRepository, IMapper mapper)
    {
        _guestRepository = guestRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GuestDto>> GetAllAsync()
    {
        var guests = await _guestRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GuestDto>>(guests);
    }

    public async Task<GuestDto> GetByIdAsync(int id)
    {
        var guest = await _guestRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Guest), id);
        return _mapper.Map<GuestDto>(guest);
    }

    public async Task<GuestDto> CreateAsync(CreateGuestDto dto)
    {
        var existing = await _guestRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new ConflictException($"A guest with email '{dto.Email}' already exists.");

        var guest = _mapper.Map<Guest>(dto);
        await _guestRepository.AddAsync(guest);
        await _guestRepository.SaveChangesAsync();
        return _mapper.Map<GuestDto>(guest);
    }

    public async Task UpdateAsync(int id, CreateGuestDto dto)
    {
        var guest = await _guestRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Guest), id);

        _mapper.Map(dto, guest);
        _guestRepository.Update(guest);
        await _guestRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var guest = await _guestRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Guest), id);

        _guestRepository.Delete(guest);
        await _guestRepository.SaveChangesAsync();
    }
}
