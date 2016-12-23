using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Models;

namespace TickerTools.Common
{
    public static class TwitterHelper
    {
        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static bool _initialized;

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public static void Initialize(string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            Auth.SetCredentials(new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret));
            _initialized = true;
        }

        public static async Task<String> ResolveTwitterNames(String text)
        {
            CheckInitialized();

            return await Task.Run(() =>
            {
                return Regex.Replace(text, "@(?<username>[A-Za-z0-9_]+)", X =>
                {
                    var user = Tweetinvi.User.GetUserFromScreenName(X.Groups["username"].Value);
                    return user.Name;
                });
            });
        }

        public static async Task<List<SocialNetworkMessage>> FetchTweetsAsync(String userName, Int32 maxTweets = 10)
        {
            CheckInitialized();

            return (await FetchTweetsAsyncCore(userName, maxTweets))
                .Select(X => new SocialNetworkMessage
                {
                    Text        = X.Text,
                    CreatedTime = X.CreatedAt,
                    Images      = X.Media.Select(Y => Y.MediaURL).ToList(),
                    IsRepost    = X.IsRetweet
                })
                .ToList();
        }

        public static async Task<DateTime?> GetLastTweetDateUtc(String userName)
        {
            CheckInitialized();

            var tweets = await FetchTweetsAsync(userName);
            var firstTweet = tweets.FirstOrDefault();

            if (firstTweet == null)
                return null;

            return firstTweet.CreatedTime.ToUniversalTime();
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private static void CheckInitialized()
        {
            if (!_initialized)
                throw new ApplicationException("Twitter helper has not been initialized.");
        }

        private static async Task<IEnumerable<ITweet>> FetchTweetsAsyncCore(String userName, Int32 maxTweets)
        {
            CheckInitialized();

            return await Task.Run(() =>
            {
                var param = Timeline.CreateUserTimelineParameter();
                param.ExcludeReplies = false;
                param.TrimUser = true;
                param.MaximumNumberOfTweetsToRetrieve = maxTweets;

                var tweets = Timeline.GetUserTimeline(userName, param);

                if (tweets == null)
                    return null;

                return tweets.Where(x => !x.IsRetweet);
            });
        }
    }
}
