//based on https://github.com/ayende/XmcdParser

namespace FreeDB.DataFileImporter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core.Model;

    public class FileParser
    {
        private readonly IList<DiscRegexContext> _actionContexts = new List<DiscRegexContext>();

        public FileParser()
        {
            AddDiscRegexes();
        }

        public Disc Parse(string text)
        {
            var disc = new Disc();
            foreach (var actionContext in _actionContexts)
            {
                var collection = actionContext.Regex.Matches(text);
                try
                {
                    actionContext.Action(disc, collection);
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine(text);
                    Console.WriteLine(actionContext.Regex);
                    Console.WriteLine(e);
                    throw;
                }
            }

            return disc;
        }

        private void AddDiscRegexes()
        {
            Add(@"^\#\s+xmcd", (disc, collection) =>
                {
                    if (collection.Count == 0)
                        throw new InvalidDataException("Not an XMCD file");
                });

            Add(@"Disc \s+ length \s*: \s* (\d+)",
                (disc, collection) => disc.LengthInSeconds = int.Parse(collection[0].Groups[1].Value)
                );

            Add(@"DISCID=(.+)", (disc, collection) =>
                {
                    var value = collection[0].Groups[1].Value.Trim();
                    if (value.Contains(","))
                        value = value.Split(',').First();

                    disc.Id = Int64.Parse(value, NumberStyles.HexNumber);
                });

            Add("DTITLE=(.+)", (disc, collection) =>
            {
                var parts = collection[0].Groups[1].Value.Split(new[] { "/" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    disc.Artist = new Artist { Name = parts[0].Trim() };
                    disc.Title = parts[1].Trim();
                }
                else
                {
                    disc.Title = parts[0].Trim();
                }
            });

            Add(@"DYEAR=(\d+)", (disc, collection) =>
                {
                    if (collection.Count == 0)
                        return;
                    var value = collection[0].Groups[1].Value;
                    if (int.Parse(value) == 0)
                        return;

                    if (value.Length > 4)
                        value = value.Substring(value.Length - 4);

                    if (int.Parse(value) <= DateTime.MaxValue.Year)
                        disc.Released = new DateTime(int.Parse(value), 1, 1);
                });

            Add(@"DGENRE=(.+)", (disc, collection) =>
                {
                    if (collection.Count == 0)
                        return;

                    var value = collection[0].Groups[1].Value.Trim();
                    if (value.Contains(","))
                        value = value.Split(',').First();

                    disc.Genre = new Genre { Title = value };
                });

            Add(@"TTITLE\d+=(.+)", (disc, collection) =>
            {
                foreach (Match match in collection)
                {
                    var track = new Track {Title = match.Groups[1].Value.Trim(), TrackNumber = disc.Tracks.Count + 1};
                    disc.Tracks.Add(track);
                }
            });

            //needs to be after TTITLE parsing
            Add(@"^\# \s* Track \s+ frame \s+ offsets \s*: \s* \n (^\# \s* (\d+) \s* \n)+", (disc, collection) =>
                {
                    var i = 0;
                    foreach (Capture capture in collection[0].Groups[2].Captures)
                    {
                        var trackOffset = int.Parse(capture.Value);

                        if (i < disc.Tracks.Count)
                            disc.Tracks.ToList()[i].Offset = trackOffset;

                        i++;
                    }
                });

            Add(@"(EXTD\d*)=(.+)", (disc, collection) =>
                {
                    foreach (Match match in collection)
                    {
                        var key = match.Groups[1].Value;
                        //todo: not implemented
                    }
                });
        }

        private void Add(string regex, Action<Disc, MatchCollection> action)
        {
            var key = new Regex(regex,
                                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace |
                                RegexOptions.Multiline);
            _actionContexts.Add(new DiscRegexContext(key, action));
        }
    }

    internal class DiscRegexContext
    {
        public Regex Regex { get; private set; }
        public Action<Disc, MatchCollection> Action { get; private set; }

        public DiscRegexContext(Regex regex, Action<Disc, MatchCollection> action)
        {
            Regex = regex;
            Action = action;
        }
    }
}
