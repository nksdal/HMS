using AutoMapper;
using HMS.Application.DTOs.Hotels;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Exceptions;

namespace HMS.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync()
    {
        var hotels = await _hotelRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<HotelDto>>(hotels);
    }

    public async Task<HotelDto> GetByIdAsync(int id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Hotel), id);
        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto)
    {
        var hotel = _mapper.Map<Hotel>(dto);
        await _hotelRepository.AddAsync(hotel);
        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task UpdateAsync(int id, CreateHotelDto dto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Hotel), id);

        _mapper.Map(dto, hotel);
        _hotelRepository.Update(hotel);
        await _hotelRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Hotel), id);

        _hotelRepository.Delete(hotel);
        await _hotelRepository.SaveChangesAsync();
    }
}
