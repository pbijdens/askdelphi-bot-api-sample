using System;
using System.Collections.Generic;
using System.Text;

namespace AskDelphiBotAPIExample
{

    /// <summary>
    /// <code>
    ///   sample search result: 
    ///   {
    ///   "score": 10000.0,
    ///   "relations": [
    ///   	{
    ///   		"relationtypeid": "687860f5-98bd-4d84-9406-c0f947719f42",
    ///   		"tag": "",
    ///   		"sequencenumber": 0,
    ///   		"publicationid": "9e3ac79e-399d-4e19-89e5-e2e959379d87",
    ///   		"topictypeid": "9037b27d-6c9d-43f7-ba0c-7722704b1c39",
    ///   		"id": "06198325-54a8-4775-aa54-0fed647f153c",
    ///   		"title": "Criticality",
    ///   		"url": "https://askdelphi-staging.azurewebsites.net/en-US/9e3ac79e-399d-4e19-89e5-e2e959379d87/topic/06198325-54a8-4775-aa54-0fed647f153c"
    ///
    ///       }
    ///   ],
    ///   "description": "",
    ///   "publicationid": "9e3ac79e-399d-4e19-89e5-e2e959379d87",
    ///   "topictypeid": "a27fbb10-4194-42ea-873e-2f0f5380417a",
    ///   "id": "fe0474eb-a2e5-41f3-8f8b-125a439d7fa9",
    ///   "title": "Betriebsweise mit Kunden planen",
    ///   "url": "https://askdelphi-staging.azurewebsites.net/en-US/9e3ac79e-399d-4e19-89e5-e2e959379d87/topic/fe0474eb-a2e5-41f3-8f8b-125a439d7fa9"    
    ///   "bodyXML": "&lt;imola-nugget xmlns='http://tempuri.org/imola-nugget'&gt;>...&lt;/imola-nugget&gt;"
    ///   }
    ///   </code>
    /// </summary>
    public class DtoMappingColumn
    {
        /// <summary>
        /// Column name in the output. When requesting json output, make sure it's a legal json identifier.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// source of the base value, possibly followed by ;xpath
        /// </summary>
        /// <remarks>
        /// <para>
        /// The source should be a JSON selector. In the implementation we pass in a single search result row from the API 
        /// results (with added a bodyXML property containing the XML contant of the topic) as a JObject, and then invoke
        /// jObject.SelectToken(sourceBeforeFirstSemicolon) on that JObject, any input to that is valid here.
        /// </para>
        /// <para>
        /// If there is a ; in the source property then the part before the first ; is used as a json selector, the 
        /// remainder of the string (excluding the ;) is used as an xpath to select a value from the property value, provided
        /// the property resolved to an XML document (as e.g. the bodyXML property of the search result rows does).
        /// </para>
        /// <para>
        /// When multiple nodes match the xpath, the first match is used.
        /// </para>
        /// </remarks>
        public string source { get; set; }

        /// <summary>
        /// Van be one of raw, text, xslt
        /// </summary>
        /// <remarks>
        /// <para>In case of raw, the raw value of the property is used. XPaths will not be applied, not will XSLT translations.</para>
        /// <para>In case of text, the text value of the property is used to populate the column. If the property is an XML 
        /// document or the result of applying an XPath to such document (e.g. bodyXML) then the inner text of that XML document is 
        /// used.</para>
        /// <para>In case of xslt, if the property value is not an XML document then the result is empty. If it is an XML
        /// document and the value of the xslt property for this column definition contains a valid XSLT transformation script
        /// then the value in the column will be the result of applying that XSLT to the XML document from the prroperty 
        /// value.</para>
        /// </remarks>
        public string mapping { get; set; }

        /// <summary>
        /// If the mapping is set to xslt this must contain a valid XSLT script. Otherwise this must be null.
        /// </summary>
        public string xslt { get; set; }
    }
}
