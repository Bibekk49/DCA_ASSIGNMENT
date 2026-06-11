using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Tools.ObjectMapper;

namespace UnitTests.Common.ObjectMapper;

public class ObjectMapperTests
{
    [Fact]
    public void GivenMatchingProperties_WhenNoConfigRegistered_ThenJsonFallbackMaps()
    {
        var provider = new ServiceCollection()
            .AddScoped<IMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .BuildServiceProvider();

        var mapper = provider.GetRequiredService<IMapper>();
        var result = mapper.Map<DestDto>(new SourceDto("Hello", 42));

        Assert.Equal("Hello", result.Name);
        Assert.Equal(42, result.Count);
    }

    [Fact]
    public void GivenRegisteredMappingConfig_WhenMap_ThenConfigIsUsedInsteadOfJson()
    {
        var provider = new ServiceCollection()
            .AddScoped<IMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<SourceDto, DestDto>, UpperCaseMappingConfig>()
            .BuildServiceProvider();

        var mapper = provider.GetRequiredService<IMapper>();
        var result = mapper.Map<DestDto>(new SourceDto("hello", 5));

        Assert.Equal("HELLO", result.Name);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public void GivenConfigForOtherType_WhenMapDifferentSource_ThenFallsBackToJson()
    {
        var provider = new ServiceCollection()
            .AddScoped<IMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<SourceDto, DestDto>, UpperCaseMappingConfig>()
            .BuildServiceProvider();

        var mapper = provider.GetRequiredService<IMapper>();
        // OtherDto → DestDto: no specific config, falls back to JSON round-trip
        var result = mapper.Map<DestDto>(new OtherDto("world", 7));

        Assert.Equal("world", result.Name);
        Assert.Equal(7, result.Count);
    }

    private record SourceDto(string Name, int Count);
    private record DestDto(string Name, int Count);
    private record OtherDto(string Name, int Count);

    private class UpperCaseMappingConfig : IMappingConfig<SourceDto, DestDto>
    {
        public DestDto Map(SourceDto input) => new(input.Name.ToUpper(), input.Count * 2);
    }
}