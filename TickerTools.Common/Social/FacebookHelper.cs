using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickerTools.Common
{
    public static class FacebookHelper
    {
        // ===========================================================================
        // = Constants
        // ===========================================================================
        
        private const String BASE_URI = "https://graph.facebook.com/";

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static FacebookClient _client;
        private static bool _initialized;

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public static void Initialize(string accessToken, string appId, string appSecret)
        {
            _client = new Facebook.FacebookClient(accessToken);
            _client.AppId = appId;
            _client.AppSecret = appSecret;

            _initialized = true;
        }

        public async static Task<List<SocialNetworkMessage>> FetchPagePosts(String pageName)
        {
            CheckInitialized();

            return (await FetchPagePostsCore(pageName))
                .Select(X => new SocialNetworkMessage
                {
                    Text = X.message,
                    CreatedTime = DateTime.Parse(X.created_time),
                    Images = null,
                    IsRepost = false
                })
                .ToList();
        }

        public async static Task<List<SocialNetworkMessage>> FetchEventPosts(Int64 eventId)
        {
            return (await FetchEventPostsCore(eventId))
                .Select(X => new SocialNetworkMessage
                {
                    Text = X.message,
                    CreatedTime = DateTime.Parse(X.updated_time),
                    Images = null,
                    IsRepost = false,
                    PostedBy = X.id.ToString()
                })
                .ToList();
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================
        
        private static void CheckInitialized()
        {
            if (!_initialized)
                throw new ApplicationException("Facebook helper has not been initialized.");
        }

        private async static Task<IEnumerable<dynamic>> FetchEventPostsCore(Int64 eventId)
        {
            // TODO: paging.
            dynamic feed = await _client.GetTaskAsync($"/{eventId}/feed?limit=500");
            //string prev = feed.paging.previous as String;

            //var pagesRetrieved = 1;
            var feedData = new List<dynamic>();

            feedData.AddRange((IEnumerable<dynamic>)feed.data);

            //while (prev != null && pagesRetrieved < pages)
            //{
            //    feed = await _client.GetTaskAsync(GetPath(prev));
            //    prev = feed.paging.previous as String;

            //    feedData.AddRange((IEnumerable<dynamic>)feed.data);

            //    pagesRetrieved++;
            //}

            return feedData;
        }

        private async static Task<IEnumerable<dynamic>> FetchPagePostsCore(String pageName)
        {
            dynamic feed = await _client.GetTaskAsync($"/{pageName}/posts");
            return (IEnumerable<dynamic>)feed.data;
        }

        private static String GetPath(String fullUri)
        {
            var uri = new Uri(fullUri);
            return uri.PathAndQuery;
        }
    }
}
