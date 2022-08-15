namespace Music_Shop.Data
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RedirectionResponse
    {
        public Status Status { get; set; }

        public string RedirectUri { get; set; }

        public string OrderId { get; set; }
    }

    public partial class Status
    {
        public string StatusCode { get; set; }
    }
}
