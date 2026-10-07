using AutoMapper;
using HMS.Application.DTOs.Rooms;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Exceptions;

namespace HMS.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId)
    {
        var rooms = await _roomRepository.GetByHotelIdAsync(hotelId);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomDto> GetByIdAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Room), id);
        return _mapper.Map<RoomDto>(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        var room = _mapper.Map<Room>(dto);
        await _roomRepository.AddAsync(room);
        await _roomRepository.SaveChangesAsync();
        return _mapper.Map<RoomDto>(room);
    }

    public async Task UpdateAsync(int id, CreateRoomDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Room), id);

        _mapper.Map(dto, room);
        _roomRepository.Update(room);
        await _roomRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Room), id);

        _roomRepository.Delete(room);
        await _roomRepository.SaveChangesAsync();
    }
}
