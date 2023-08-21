using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SP.StudioCore.Net.Http
{
    public struct HttpClientResponse
    {
        public HttpClientResponse(HttpResponseMessage? response, HttpRequestException ex, HttpClientRequest request) : this()
        {
            this.StatusCode = response?.StatusCode ?? 0;
            this.Content = response?.Content.ReadAsStringAsync().Result ?? JsonConvert.SerializeObject(new
            {
                _url = request.Url,
                _exception = ex.Message
            });
            this.Headers = response?.Headers;
        }

        public HttpClientResponse(Exception ex, HttpClientRequest request) : this()
        {
            this.StatusCode = 0;
            this.Content = JsonConvert.SerializeObject(new
            {
                _url = request.Url,
                _exception = ex.Message
            });
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public HttpResponseHeaders? Headers { get; set; }

        public static implicit operator bool(HttpClientResponse result)
        {
            return result.StatusCode == HttpStatusCode.OK;
        }

        public static implicit operator string(HttpClientResponse result)
        {
            return result.Content;
        }
    }
}
