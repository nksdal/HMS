using AutoMapper;
using HMS.Application.DTOs.Guests;
using HMS.Application.DTOs.Hotels;
using HMS.Application.DTOs.Managers;
using HMS.Application.DTOs.Reservations;
using HMS.Application.DTOs.Rooms;
using HMS.Domain.Entities;

namespace HMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<CreateHotelDto, Hotel>();

        CreateMap<Room, RoomDto>().ReverseMap();
        CreateMap<CreateRoomDto, Room>();

        CreateMap<Guest, GuestDto>();
        CreateMap<CreateGuestDto, Guest>();

        CreateMap<Manager, ManagerDto>();

        CreateMap<Reservation, ReservationDto>().ReverseMap();
        CreateMap<CreateReservationDto, Reservation>();
    }
}
