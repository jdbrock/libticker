using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickerTools.Common
{
    public class SocialNetworkMessage
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================
        
        public String Text { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<String> Images { get; set; }
        public Boolean IsRepost { get; set; }
        public String PostedBy { get; set; }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public SocialNetworkMessage Clone()
        {
            return new SocialNetworkMessage
            {
                Text = Text,
                CreatedTime = CreatedTime,
                Images = new List<String>(Images),
                IsRepost = IsRepost,
                PostedBy = PostedBy
            };
        }

        public override string ToString()
        {
            return $"({CreatedTime}) {Text}";
        }
    }
}
