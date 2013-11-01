namespace FreeDB.Infrastructure.AutoMapper.ConfigurationProfiles
{
    using System.Globalization;
    using System.Linq;
    using Core.Common;
    using Core.Model;
    using Lucene.Net.Documents;
    using Web.Models.Dto;
    using global::AutoMapper;

    public class DiscMapper : Profile
    {
        protected override void Configure()
        {
            CreateMap<Disc, DiscDto>()
                .ForMember(d => d.ArtistId, c => c.MapFrom(s => s.Artist.Id))
                .ForMember(d => d.ArtistName, c => c.MapFrom(s => s.Artist.Name))
                .ForMember(d => d.ArtistHRef, c => c.Ignore())
                .ForMember(d => d.Genre, c => c.MapFrom(s => s.Genre.Title))
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.NumberOfTracks, c => c.MapFrom(s => s.Tracks.Count))
                .ForMember(d => d.Released,
                           c =>
                           c.MapFrom(
                               s =>
                               s.Released.HasValue ? s.Released.Value.Year.ToString(CultureInfo.InvariantCulture) : null))
                .ForMember(d => d.Runtime,
                           c => c.MapFrom(s => TimeSpanExtensions.GetHoursAndMinutesString(s.LengthInSeconds)));

            CreateMap<Disc, DiscSummaryDto>()
                .ForMember(d => d.ArtistId, c => c.MapFrom(s => s.Artist.Id))
                .ForMember(d => d.ArtistName, c => c.MapFrom(s => s.Artist.Name))
                .ForMember(d => d.ArtistHRef, c => c.Ignore())
                .ForMember(d => d.Genre, c => c.MapFrom(s => s.Genre.Title))
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.NumberOfTracks, c => c.MapFrom(s => s.Tracks.Count))
                .ForMember(d => d.Released,
                           c =>
                           c.MapFrom(
                               s =>
                               s.Released.HasValue ? s.Released.Value.Year.ToString(CultureInfo.InvariantCulture) : null))
                .ForMember(d => d.Runtime,
                           c => c.MapFrom(s => TimeSpanExtensions.GetHoursAndMinutesString(s.LengthInSeconds)));

            CreateMap<Document, FreeDbSearchResult>()
                .ForMember(d => d.DiscId, c => c.MapFrom(s => s.Get("DiscId")))
                .ForMember(d => d.DiscTitle, c => c.MapFrom(s => s.Get("DiscTitle")))
                .ForMember(d => d.DiscLengthInSeconds, c => c.MapFrom(s => s.Get("DiscLengthInSeconds")))
                .ForMember(d => d.DiscReleaseDate, c => c.MapFrom(s => s.Get("DiscReleaseDate")))
                .ForMember(d => d.ArtistId, c => c.MapFrom(s => s.Get("ArtistId")))
                .ForMember(d => d.ArtistName, c => c.MapFrom(s => s.Get("ArtistName")))
                .ForMember(d => d.Genre, c => c.MapFrom(s => s.Get("Genre")))
                .ForMember(d => d.Tracks,
                           c =>
                           c.MapFrom(
                               s => s.Get("Tracks").Split("`~".ToCharArray()).Where(t => !string.IsNullOrWhiteSpace(t))));

            CreateMap<FreeDbSearchResult, DiscSummaryDto>()
                .ForMember(d => d.ArtistHRef, c => c.Ignore())
                .ForMember(d => d.HRef, c => c.Ignore())
                .ForMember(d => d.Id, c => c.MapFrom(s => s.DiscId))
                .ForMember(d => d.NumberOfTracks, c => c.MapFrom(s => s.Tracks.Length))
                .ForMember(d => d.Released,
                           c =>
                           c.MapFrom(
                               s =>
                               s.DiscReleaseDate.HasValue
                                   ? s.DiscReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture)
                                   : null))
                .ForMember(d => d.Title, c => c.MapFrom(s => s.DiscTitle))
                .ForMember(d => d.Runtime,
                           c => c.MapFrom(s => TimeSpanExtensions.GetHoursAndMinutesString(s.DiscLengthInSeconds)));
        }
    }
}
