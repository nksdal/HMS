using FluentValidation;
using HMS.Application.DTOs.Reservations;

namespace HMS.Application.Validators;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.GuestId).GreaterThan(0);
        RuleFor(x => x.RoomId).GreaterThan(0);
        RuleFor(x => x.CheckOutDate).GreaterThan(x => x.CheckInDate);
        RuleFor(x => x.CheckInDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}
