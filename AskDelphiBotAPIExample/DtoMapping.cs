using System;
using System.Collections.Generic;
using System.Text;

namespace AskDelphiBotAPIExample
{
    /// <summary>
    /// Data transfer object for defining the mapping output of the bot API.
    /// </summary>
    public class DtoMapping
    {
        /// <summary>
        /// Specify excel to generate Excel output, or json to return a JSON document (as a UTF-8 encoded byte array)
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// In case of XML output (not supported currently) will contain the name of the root element. For Excel this
        /// is used to name the worksheet. For JSON this is not used (should still contain a valid value...)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// This defines the columns (or in case of JSON properties, and for XML elements) that are extracted from an API search result.
        /// </summary>
        public List<DtoMappingColumn> columns { get; set; }

    }
}
