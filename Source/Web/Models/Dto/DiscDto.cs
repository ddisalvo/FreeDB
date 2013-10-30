namespace FreeDB.Web.Models.Dto
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Core.Common;

    [DataContract(Name = "Disc")]
    public class DiscDto : DiscSummaryDto
    {
        private const int FramesPerSecond = 75;

        [DataMember]
        public string Genre { get; set; }

        [DataMember]
        public TrackDto[] Tracks { get; set; }

        [DataMember]
        public int LengthInSeconds { get; set; }

        [DataMember]
        public string Language { get; set; }

        public void CalculateTrackTimes()
        {
            if (!Tracks.Any())
                return;

            for (var i = 0; i < Tracks.Length - 1; i++)
            {
                var previous = i == 0 ? 0 : Tracks[i].Offset;
                var trackLengthInSeconds = Convert.ToDouble(Tracks[i + 1].Offset - previous)/FramesPerSecond;
                Tracks[i].Runtime = TimeSpanExtensions.GetMinutesAndSecondsString((int) trackLengthInSeconds);
            }

            var lastTrackLength = Convert.ToDouble(LengthInSeconds*FramesPerSecond - Tracks[Tracks.Length - 1].Offset)/
                                  FramesPerSecond;
            Tracks[Tracks.Length - 1].Runtime = TimeSpanExtensions.GetMinutesAndSecondsString((int)lastTrackLength);
        }
    }
}