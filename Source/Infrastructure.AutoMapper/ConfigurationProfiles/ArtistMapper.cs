namespace FreeDB.Infrastructure.AutoMapper.ConfigurationProfiles
{
    using Core.Common;
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

            CreateMap<FreeDbSearchResult, ArtistSummaryDto>()
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.Id, c => c.MapFrom(s => s.ArtistId))
                .ForMember(d => d.Name, c => c.MapFrom(s => s.ArtistName))
                .ForMember(d => d.NumberOfDiscs, c => c.Ignore());

            CreateMap<Artist, ArtistDto>()
                .ForMember(d => d.Discs, c => c.Ignore())
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.NumberOfDiscs, c => c.Ignore())
                .ForMember(d => d.YearsActive, c => c.Ignore());
        }
    }
}
