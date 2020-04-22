using System;
using System.Collections.Generic;
using System.Text;

namespace AskDelphiBotAPIExample
{
    /// <summary>
    /// API result for the token refresh call.
    /// </summary>
    public class DtoTokenRefreshResult
    {
        /// <summary>
        /// Same refresh token (make sure it hasn't changed).
        /// </summary>
        public string refresh { get; set; }
        /// <summary>
        /// New JWT token for authentication.
        /// </summary>
        public string token { get; set; }
    }
}
