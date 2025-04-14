﻿// Midjourney Proxy - Proxy for Midjourney's Discord, enabling AI drawings via API with one-click face swap. A free, non-profit drawing API project.
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Midjourney.API
{
    /// <summary>
    /// 简单的授权中间件
    /// </summary>
    public class SimpleAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SimpleAuthMiddleware> _logger;

        public SimpleAuthMiddleware(RequestDelegate next, ILogger<SimpleAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, WorkContext workContext)
        {
            var path = context.Request.Path.Value;
            
            if (path.StartsWith("/mj-turbo"))
            {
                context.Items["Mode"] = "turbo";
            }
            else if (path.StartsWith("/mj-relax"))
            {
                context.Items["Mode"] = "relax";
            }
            else if (path.StartsWith("/mj-fast"))
            {
                context.Items["Mode"] = "fast";
            }
            else
            {
                context.Items["Mode"] = "";
            }

            // 演示模式下不需要验证
            if (GlobalConfiguration.IsDemoMode == true)
            {
                await _next(context);
                return;
            }

            // 检查是否有 AllowAnonymous 特性
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
            
            if (!allowAnonymous)
            {
                var user = workContext.GetUser();

                // 如果用户被禁用
                if (user?.Status == EUserStatus.DISABLED)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("账号已禁用");
                    return;
                }

                // 如果账号不可用
                if (user != null && !user.IsAvailable)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("账号不可用");
                    return;
                }

                // 如果是管理员接口，需要管理员角色
                if (context.Request.Path.StartsWithSegments("/mj/admin"))
                {
                    // if (user?.Role != EUserRole.ADMIN)
                    // {
                    //     context.Response.StatusCode = 401;
                    //     await context.Response.WriteAsync("账号无权限");
                    //     return;
                    // }
                }
                else
                {
                    // 未开启访客
                    // 并且不允许匿名访问，则返回 401
                    if (user == null && !GlobalConfiguration.Setting.EnableGuest)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Authorization header missing.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}