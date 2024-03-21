﻿using static mango.web.frontend.Utility.SD;

namespace mango.web.frontend.Models
{
    public class WebAPIRequest
    {
        public ApiType apiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
