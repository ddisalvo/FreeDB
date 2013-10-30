namespace FreeDB.Infrastructure.AutoMapper.ConfigurationProfiles
{
    using Core.Model;
    using Web.Models.Dto;
    using global::AutoMapper;

    public class ArtistMapper : Profile
    {
        protected override void Configure()
        {
            CreateMap<Artist, ArtistSummaryDto>()
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.NumberOfDiscs, c => c.Ignore());

            CreateMap<Artist, ArtistDto>()
                .ForMember(d => d.Discs, c => c.Ignore())
                .ForMember(d => d.HRef, c => c.Ignore());
        }
    }
}
