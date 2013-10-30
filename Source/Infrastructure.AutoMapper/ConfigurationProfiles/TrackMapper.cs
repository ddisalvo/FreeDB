namespace FreeDB.Infrastructure.AutoMapper.ConfigurationProfiles
{
    using Core.Model;
    using Web.Models.Dto;
    using global::AutoMapper;

    public class TrackMapper : Profile
    {
        protected override void Configure()
        {
            CreateMap<Track, TrackDto>()
                .ForMember(d => d.Runtime, c => c.Ignore());
        }
    }
}
