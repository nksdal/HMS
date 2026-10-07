using AutoMapper;
using HMS.Application.DTOs.Hotels;
using HMS.Application.Interfaces;
using HMS.Application.Services;
using HMS.Domain.Entities;
using HMS.Domain.Exceptions;
using HMS.Tests.Helpers;
using Moq;
using Xunit;

namespace HMS.Tests.Services;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
    private readonly IMapper _mapper = MapperTestHelper.CreateMapper();
    private readonly HotelService _sut;

    public HotelServiceTests()
    {
        _sut = new HotelService(_hotelRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedHotels()
    {
        var hotels = new List<Hotel>
        {
            new() { Id = 1, Name = "Hotel A", Address = "A", City = "A", Country = "A" },
            new() { Id = 2, Name = "Hotel B", Address = "B", City = "B", Country = "B" }
        };
        _hotelRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(hotels);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, h => h.Name == "Hotel A");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ThrowsNotFound()
    {
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_AddsAndSaves()
    {
        var dto = new CreateHotelDto { Name = "New", Address = "A", City = "C", Country = "X" };

        var result = await _sut.CreateAsync(dto);

        Assert.Equal("New", result.Name);
        _hotelRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Hotel>()), Times.Once);
        _hotelRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_DoesNotDelete()
    {
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
        _hotelRepositoryMock.Verify(r => r.Delete(It.IsAny<Hotel>()), Times.Never);
    }
}
