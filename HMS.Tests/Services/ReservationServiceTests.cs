using AutoMapper;
using HMS.Application.DTOs.Reservations;
using HMS.Application.Interfaces;
using HMS.Application.Services;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Domain.Exceptions;
using HMS.Tests.Helpers;
using Moq;
using Xunit;

namespace HMS.Tests.Services;

public class ReservationServiceTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly IMapper _mapper = MapperTestHelper.CreateMapper();
    private readonly ReservationService _sut;

    public ReservationServiceTests()
    {
        _sut = new ReservationService(
            _reservationRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenRoomMissing_ThrowsNotFound()
    {
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Room?)null);

        var dto = new CreateReservationDto
        {
            RoomId = 1,
            GuestId = 1,
            CheckInDate = DateTime.UtcNow.Date.AddDays(1),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(3)
        };

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WhenRoomNotAvailable_ThrowsBusinessRule()
    {
        var room = new Room { Id = 1, PricePerNight = 100m, HotelId = 1, RoomNumber = "101" };
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _reservationRepositoryMock
            .Setup(r => r.IsRoomAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            RoomId = 1,
            GuestId = 1,
            CheckInDate = DateTime.UtcNow.Date.AddDays(1),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(3)
        };

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_CalculatesTotalPriceFromNights()
    {
        var room = new Room { Id = 1, PricePerNight = 100m, HotelId = 1, RoomNumber = "101" };
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _reservationRepositoryMock
            .Setup(r => r.IsRoomAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        var dto = new CreateReservationDto
        {
            RoomId = 1,
            GuestId = 1,
            CheckInDate = new DateTime(2026, 10, 1),
            CheckOutDate = new DateTime(2026, 10, 4) // 3 nights
        };

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(300m, result.TotalPrice);
        Assert.Equal(ReservationStatus.Pending, result.Status);
    }

    [Fact]
    public async Task CancelAsync_SetsStatusToCancelled()
    {
        var reservation = new Reservation
        {
            Id = 1,
            RoomId = 1,
            GuestId = 1,
            Status = ReservationStatus.Confirmed
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);

        await _sut.CancelAsync(1);

        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
