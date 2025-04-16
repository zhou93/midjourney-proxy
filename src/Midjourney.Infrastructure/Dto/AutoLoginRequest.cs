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

using System.ComponentModel.DataAnnotations;

namespace Midjourney.Infrastructure.Dto
{
    /// <summary>
    /// 自动登录请求
    /// </summary>
    public class AutoLoginRequest
    {
        /// <summary>
        /// 账号（用于自动登录）
        /// </summary>
        /// [Required]
        public string LoginAccount { get; set; }

        /// <summary>
        /// 密码（用于自动登录）
        /// </summary>
        /// [Required]
        public string LoginPassword { get; set; }

        /// <summary>
        /// 2FA 密钥（用于自动登录）
        /// </summary>
        /// [Required]
        public string Login2fa { get; set; }

        /// <summary>
        /// 登陆前的启用状态
        /// </summary>
        public bool LoginBeforeEnabled { get; set; }

        /// <summary>
        /// 自定义参数 = ChannelId
        /// </summary>
        [MaxLength(4000)]
        public string State { get; set; }

        /// <summary>
        /// 登录成功后的 Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 通知回调的密钥，防止篡改
        /// </summary>
        [MaxLength(4000)]
        public string Secret { get; set; }

        /// <summary>
        /// 回调地址, 为空时使用全局notifyHook。
        /// </summary>
        [MaxLength(4000)]
        public string NotifyHook { get; set; }

        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [MaxLength(4000)]
        public string Message { get; set; }
    }
}
