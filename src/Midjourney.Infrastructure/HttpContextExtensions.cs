// Midjourney Proxy - Proxy for Midjourney's Discord, enabling AI drawings via API with one-click face swap. A free, non-profit drawing API project.
// Copyright (C) 2024 trueai.org

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

// Additional Terms:
// This software shall not be used for any illegal activities.
// Users must comply with all applicable laws and regulations,
// particularly those related to image and video processing.
// The use of this software for any form of illegal face swapping,
// invasion of privacy, or any other unlawful purposes is strictly prohibited.
// Violation of these terms may result in termination of the license and may subject the violator to legal action.

using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net.Sockets;
using System.Text;

namespace Midjourney.Infrastructure
{
    /// <summary>
    /// HttpContext 扩展
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取请求主体内容
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetRequestBody(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                return null;
            }

            httpRequest.EnableBuffering();

            // 重置 position
            httpRequest.Body.Seek(0, SeekOrigin.Begin);
            // or
            //httpRequest.Body.Position = 0;

            StreamReader sr = new StreamReader(httpRequest.Body);

            var content = sr.ReadToEndAsync().Result;

            httpRequest.Body.Seek(0, SeekOrigin.Begin);
            // or
            // httpRequest.Body.Position = 0;

            return content;
        }

        /// <summary>
        /// 获取请求链接地址
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetUrl(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                return string.Empty;
            }

            return new StringBuilder()
                .Append(httpRequest.Scheme).Append("://")
                .Append(httpRequest.Host).Append(httpRequest.PathBase)
                .Append(httpRequest.Path).Append(httpRequest.QueryString).ToString();
        }

        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="ignoreLocalIpAddress">验证时是否忽略本地 IP 地址，如果忽略本地 IP 地址，则当判断为本地 IP 地址时返回可能为空</param>
        /// <returns></returns>
        public static string GetIP(this HttpRequest httpRequest, bool ignoreLocalIpAddress = false)
        {
            if (httpRequest == null)
            {
                return string.Empty;
            }

            var ip = string.Empty;
            var headers = httpRequest.Headers;

            // 优先从 X-Real-IP 获取
            if (headers.ContainsKey("X-Real-IP"))
            {
                ip = headers["X-Real-IP"].ToString();
            }
            // 其次从 X-Forwarded-For 获取
            else if (headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedIps = headers["X-Forwarded-For"].ToString().Split(',');
                ip = forwardedIps.FirstOrDefault().Trim();
            }
            // 最后从 RemoteIpAddress 获取
            else
            {
                var address = httpRequest.HttpContext.Connection.RemoteIpAddress;

                // 如果是本地IP且设置了忽略本地IP，则返回空
                if (ignoreLocalIpAddress && address == httpRequest.HttpContext.Connection.LocalIpAddress)
                {
                    ip = string.Empty;
                }
                else if (address?.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ip = address?.MapToIPv4()?.ToString();
                }
                else
                {
                    ip = address?.ToString();
                }
            }

            // 如果获取到的IP是本地IP，则尝试从其他头信息获取
            if (ip == "127.0.0.1" || ip == "::1")
            {
                if (headers.ContainsKey("CF-Connecting-IP"))
                {
                    ip = headers["CF-Connecting-IP"].ToString();
                }
                else if (headers.ContainsKey("True-Client-IP"))
                {
                    ip = headers["True-Client-IP"].ToString();
                }
            }

            // 如果还是获取不到IP，则使用 Host
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = httpRequest.Host.Host ?? httpRequest.Host.Value;
            }

            // 处理多个IP的情况
            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
            {
                ip = ip.Split(',')[0].Trim();
            }

            // 如果最终都无法获取IP，则记录日志并返回默认值
            if (string.IsNullOrEmpty(ip))
            {
                Log.Logger.Warning("无法获取客户端IP地址，请求头信息: {@Headers}", headers);
                ip = "1.1.1.1";
            }

            return ip;
        }
    }
}