using AutoMapper;
using HMS.Application.DTOs.Managers;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Domain.Exceptions;

namespace HMS.Application.Services;

public class ManagerService : IManagerService
{
    private readonly IManagerRepository _managerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public ManagerService(
        IManagerRepository managerRepository,
        IUserRepository userRepository,
        IHotelRepository hotelRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _managerRepository = managerRepository;
        _userRepository = userRepository;
        _hotelRepository = hotelRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ManagerDto>> GetByHotelIdAsync(int hotelId)
    {
        var managers = await _managerRepository.GetByHotelIdAsync(hotelId);
        return _mapper.Map<IEnumerable<ManagerDto>>(managers);
    }

    public async Task<ManagerDto> GetByIdAsync(int id)
    {
        var manager = await _managerRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Manager), id);
        return _mapper.Map<ManagerDto>(manager);
    }

    public async Task<ManagerDto> CreateAsync(int hotelId, CreateManagerDto dto)
    {
        _ = await _hotelRepository.GetByIdAsync(hotelId)
            ?? throw new NotFoundException(nameof(Hotel), hotelId);

        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        if (await _userRepository.ExistsByPersonalNumberAsync(dto.PersonalNumber))
            throw new ConflictException($"Personal number '{dto.PersonalNumber}' is already registered.");

        if (await _userRepository.ExistsByPhoneNumberAsync(dto.PhoneNumber))
            throw new ConflictException($"Phone number '{dto.PhoneNumber}' is already registered.");

        var manager = new Manager
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PersonalNumber = dto.PersonalNumber,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            HotelId = hotelId
        };

        await _managerRepository.AddAsync(manager);
        await _managerRepository.SaveChangesAsync();

        var (hash, salt) = _passwordHasher.HashPassword(dto.Password);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PersonalNumber = dto.PersonalNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Manager,
            EmailConfirmed = true,
            IsActive = true,
            ManagerId = manager.Id
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<ManagerDto>(manager);
    }

    public async Task DeleteAsync(int id)
    {
        var manager = await _managerRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Manager), id);

        _managerRepository.Delete(manager);
        await _managerRepository.SaveChangesAsync();
    }
}
