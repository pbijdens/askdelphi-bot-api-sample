using System.Collections.Generic;

namespace AskDelphiBotAPIExample
{
    public class DtoSearchParams
    {
        public string language{ get; set; }
        public string searchTopicGuid{ get; set; }
        public string publicationGuid{ get; set; }
        public string tenantGuid{ get; set; }
        public string query{ get; set; }
        public int skip{ get; set; }
        public int count{ get; set; }
        public List<DtoSearchFacet> facets{ get; set; }
        public List<string> topicTypes{ get; set; }
        public List<string> publications{ get; set; }

        public class DtoSearchFacet
        {
            public string hierarchy{ get; set; }
            public string node{ get; set; }
        }
    }
}