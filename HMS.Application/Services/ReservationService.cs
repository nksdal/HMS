using AutoMapper;
using HMS.Application.DTOs.Reservations;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Domain.Exceptions;

namespace HMS.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto> GetByIdAsync(int id)
    {
       
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Reservation), id);
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(dto.RoomId)
            ?? throw new NotFoundException(nameof(Room), dto.RoomId);

        var isAvailable = await _reservationRepository.IsRoomAvailableAsync(
            dto.RoomId, dto.CheckInDate, dto.CheckOutDate);

        if (!isAvailable)
            throw new BusinessRuleException("Room is not available for the selected dates.");

        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        var reservation = _mapper.Map<Reservation>(dto);
        reservation.TotalPrice = nights * room.PricePerNight;
        reservation.Status = ReservationStatus.Pending;

        await _reservationRepository.AddAsync(reservation);
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task CancelAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Reservation), id);

        reservation.Status = ReservationStatus.Cancelled;
        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
    }
}
