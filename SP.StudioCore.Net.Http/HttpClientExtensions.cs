using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SP.StudioCore.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpClientResponse> RequestAsync(this HttpClient client, HttpClientRequest request)
        {
            request.Encoding ??= Encoding.UTF8;

            HttpContent content = new StringContent(request.PostData ?? string.Empty, request.Encoding, request.mediaType);


            HttpRequestMessage message = new HttpRequestMessage(request.Method, request.Url);
            foreach (var item in request.Headers)
            {
                if (item.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(item.Value);
                    continue;
                }
                if (item.Key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var value in item.Value.Split(','))
                    {
                        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(value.Trim()));
                    }
                    continue;
                }
                if (item.Key.Equals("Accept-Encoding", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var value in item.Value.Split(','))
                    {
                        message.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(value.Trim()));
                    }
                    continue;
                }
                if (item.Key.Equals("Accept-Language", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var value in item.Value.Split(','))
                    {
                        message.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(value.Trim()));
                    }
                    continue;
                }
                if (item.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
                {
                    message.Headers.Add("User-Agent", item.Value);
                    continue;
                }
                content.Headers.Add(item.Key, item.Value);
            }
            message.Content = content;


            HttpResponseMessage? response = null;
            try
            {
                response = await client.SendAsync(message);
                byte[] resultData = await response.Content.ReadAsByteArrayAsync();
                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    resultData = UnGZip(resultData);
                }
                return new HttpClientResponse
                {
                    StatusCode = response.StatusCode,
                    Headers = response.Headers,
                    Content = request.Encoding.GetString(resultData)
                };
            }
            catch (HttpRequestException ex)
            {
                return new HttpClientResponse(response, ex, request);
            }
            catch (Exception ex)
            {
                return new HttpClientResponse(ex, request);
            }
        }

        public static HttpClientResponse Request(this HttpClient client, HttpClientRequest request)
        {
            return client.RequestAsync(request).Result;
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        public static HttpClientResponse Post(this HttpClient client, string url, string postData, Dictionary<string, string> headers, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return client.Request(new HttpClientRequest()
            {
                Encoding = encoding,
                Headers = headers,
                Method = HttpMethod.Post,
                PostData = postData,
                Url = url
            });
        }

        /// <summary>
        /// 异步POST请求
        /// </summary>
        public static async Task<HttpClientResponse> PostAsync(this HttpClient client, string url, string postData, Dictionary<string, string> headers, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return await client.RequestAsync(new HttpClientRequest()
            {
                Encoding = encoding,
                Headers = headers,
                Method = HttpMethod.Post,
                PostData = postData,
                Url = url
            });
        }

        public static async Task<HttpClientResponse> GetAsync(this HttpClient client, string url, Dictionary<string, string> headers, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return await client.RequestAsync(new HttpClientRequest()
            {
                Encoding = encoding,
                Headers = headers,
                Method = HttpMethod.Get,
                Url = url
            });
        }

        public static HttpClientResponse Get(this HttpClient client, string url, Dictionary<string, string> headers, Encoding? encoding = null)
        {
            return client.GetAsync(url, headers, encoding).Result;
        }


        private static byte[] UnGZip(byte[] data)
        {
            using (MemoryStream dms = new MemoryStream())
            {
                using (MemoryStream cms = new MemoryStream(data))
                {
                    using (GZipStream gzip = new GZipStream(cms, CompressionMode.Decompress))
                    {
                        byte[] bytes = new byte[1024];
                        int len = 0;
                        while ((len = gzip.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            dms.Write(bytes, 0, len);
                        }
                    }
                }
                return dms.ToArray();
            }
        }
    }
}
