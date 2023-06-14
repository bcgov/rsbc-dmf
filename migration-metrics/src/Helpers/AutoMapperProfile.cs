using AutoMapper;
using MigrationMetrics.Entities;
using MigrationMetrics.Models.MonthlyCountStat;

namespace MigrationMetrics.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateRequest -> User
            CreateMap<CreateRequest, MonthlyCountStat>();

        }
    }
}
