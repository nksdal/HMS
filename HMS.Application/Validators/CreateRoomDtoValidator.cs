using FluentValidation;
using HMS.Application.DTOs.Rooms;

namespace HMS.Application.Validators;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.RoomNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PricePerNight).GreaterThan(0);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.HotelId).GreaterThan(0);
    }
}
