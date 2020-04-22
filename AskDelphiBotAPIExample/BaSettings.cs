using System;
using System.Collections.Generic;
using System.Text;

namespace AskDelphiBotAPIExample
{
    public class BaSettings
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public string ApiServer { get; set; }
        public string PublicationGuid { get; set; }
        public string TenantGuid { get; set; }
        public string SearchTopicGuid { get; set; }
    }
}
