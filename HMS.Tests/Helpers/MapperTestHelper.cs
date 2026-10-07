using AutoMapper;
using HMS.Application.Mappings;
using Microsoft.Extensions.Logging.Abstractions;

namespace HMS.Tests.Helpers;

public static class MapperTestHelper
{
    // AutoMapper 15+ requires an ILoggerFactory on the MapperConfiguration constructor.
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        return config.CreateMapper();
    }
}
