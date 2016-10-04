using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TickerTools.Common;
using Humanizer;

namespace System
{
    public static class SocialNetworkExtensions
    {
        public static IEnumerable<SocialNetworkMessage> HasImage(this IEnumerable<SocialNetworkMessage> messages)
        {
            return messages.Where(X => X.Images.Any());
        }

        public static IEnumerable<SocialNetworkMessage> HasOneImage(this IEnumerable<SocialNetworkMessage> messages)
        {
            return messages.Where(X => X.Images.Count == 1);
        }

        public static IEnumerable<SocialNetworkMessage> HasNoImage(this IEnumerable<SocialNetworkMessage> messages)
        {
            return messages.Where(X => !X.Images.Any());
        }

        public static IEnumerable<SocialNetworkMessage> CreatedAfter(this IEnumerable<SocialNetworkMessage> messages, DateTime date)
        {
            return messages.Where(X => X.CreatedTime.ToUniversalTime() > date.ToUniversalTime());
        }

        public static IEnumerable<SocialNetworkMessage> StartsWithText(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Where(X => strings.Any(S => X.Text.StartsWith(S, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<SocialNetworkMessage> StartsWithText(this IEnumerable<SocialNetworkMessage> messages, StringComparison comparison, params String[] strings)
        {
            return messages.Where(X => strings.Any(S => X.Text.StartsWith(S, comparison)));
        }

        public static IEnumerable<SocialNetworkMessage> EndsWithText(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Where(X => strings.Any(S => X.Text.EndsWith(S, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<SocialNetworkMessage> ContainsText(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Where(X => strings.Any(S => X.Text.Contains(S, CompareOptions.OrdinalIgnoreCase)));
        }

        public static IEnumerable<SocialNetworkMessage> NotContainsText(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            if (messages.Count() == 0)
                return messages;

            return messages.Where(X => strings.All(S => !X.Text.Contains(S, CompareOptions.OrdinalIgnoreCase)));
        }

        public static IEnumerable<SocialNetworkMessage> StripTextAnywhere(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Select(X =>
            {
                var post = X.Clone();

                foreach (var replace in strings)
                    post.Text = post.Text.Replace(replace, String.Empty, StringComparison.OrdinalIgnoreCase);

                return post;
            });
        }

        public static IEnumerable<SocialNetworkMessage> StripTextStart(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Select(X =>
            {
                var post = X.Clone();

                foreach (var replace in strings)
                    post.Text = post.Text.Replace(replace, String.Empty, StringComparison.OrdinalIgnoreCase, atStartOnly: true);

                return post;
            });
        }

        public static IEnumerable<SocialNetworkMessage> StripUris(this IEnumerable<SocialNetworkMessage> messages)
        {
            var pattern = @"\s*(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?\s*";

            return messages.Select(X =>
            {
                var post = X.Clone();

                post.Text = Regex.Replace(post.Text, pattern, " ");

                return post;
            });
        }

        public static IEnumerable<SocialNetworkMessage> TrimText(this IEnumerable<SocialNetworkMessage> messages, params String[] strings)
        {
            return messages.Select(X =>
            {
                var post = X.Clone();

                post.Text = post.Text.Trim();

                return post;
            });
        }

        public static IEnumerable<SocialNetworkMessage> ExpandTwitterHandles(this IEnumerable<SocialNetworkMessage> messages)
        {
            return messages.Select(X =>
            {
                var post = X.Clone();

                post.Text = post.Text.Replace("trystbrewery1", "TrystBrewery");
                post.Text = post.Text.Replace("HawksheadBrewer", "HawksheadBrewery");
                post.Text = post.Text.Replace("oskarblues", "OskarBlues");

                //text = text.Replace(" - ", "");
                post.Text = Regex.Replace(post.Text, "@(?<username>[A-Za-z0-9_]+)", Y =>
                {
                    return Y.Value.Humanize().Transform(To.TitleCase);
                });

                post.Text = Regex.Replace(post.Text, @"\s+", " ");

                return post;
            });
        }
    }
}
