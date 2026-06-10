using AutoMapper;
using Microsoft.Extensions.Logging;

namespace FieldServiceManagement.Business.MappingBusiness
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ObjectMapper).Assembly);

            }, loggerFactory);

            var mapper = config.CreateMapper();
            return mapper;
        });
        public static IMapper Mapper => Lazy.Value;
    }
}
