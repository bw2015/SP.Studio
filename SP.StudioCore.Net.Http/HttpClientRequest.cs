using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SP.StudioCore.Net.Http
{
    /// <summary>
    /// 请求内容
    /// </summary>
    public struct HttpClientRequest
    {
        /// <summary>
        /// 请求动作
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 请求的头部信息
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding? Encoding { get; set; }

        /// <summary>
        /// 用于POST请求的文本信息
        /// </summary>
        public string? PostData { get; set; }

        public string mediaType
        {
            get
            {
                if(this.Headers == null || !this.Headers.ContainsKey("Content-Type"))
                {
                    return "application/x-www-form-urlencoded";
                }
                return this.Headers["Content-Type"];
            }
        }
    }
}
