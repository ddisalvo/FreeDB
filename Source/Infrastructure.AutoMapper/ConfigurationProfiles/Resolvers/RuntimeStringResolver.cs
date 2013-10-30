namespace FreeDB.Infrastructure.AutoMapper.ConfigurationProfiles.Resolvers
{
    using Core.Common;
    using Core.Model;
    using global::AutoMapper;

    public class RuntimeStringResolver : IValueResolver
    {
        public ResolutionResult Resolve(ResolutionResult source)
        {
            var disc = (Disc) source.Value;
            return source.New(TimeSpanExtensions.GetHoursAndMinutesString(disc.LengthInSeconds));
        }
    }
}