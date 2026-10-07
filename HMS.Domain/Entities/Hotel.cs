using HMS.Domain.Common;

namespace HMS.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Manager> Managers { get; set; } = new List<Manager>();
}
