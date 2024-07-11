﻿namespace Midjourney.Infrastructure
{
    public enum TranslateWay
    {
        /// <summary>
        /// 百度翻译
        /// </summary>
        BAIDU,

        /// <summary>
        /// GPT翻译
        /// </summary>
        GPT,

        /// <summary>
        /// 不翻译
        /// </summary>
        NULL
    }

    /// <summary>
    /// 任务状态枚举.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 未启动.
        /// </summary>
        NOT_START = 0,

        /// <summary>
        /// 已提交.
        /// </summary>
        SUBMITTED = 1,

        /// <summary>
        /// 执行中.
        /// </summary>
        IN_PROGRESS = 3,

        /// <summary>
        /// 失败.
        /// </summary>
        FAILURE = 4,

        /// <summary>
        /// 成功.
        /// </summary>
        SUCCESS = 5,

        /// <summary>
        /// 弹窗
        /// </summary>
        MODAL = 6,

        /// <summary>
        /// 取消
        /// </summary>
        CANCEL = 7
    }

    public static class TaskStatusExtensions
    {
        public static int GetOrder(this TaskStatus status)
        {
            // This method should return an integer that represents the order of the status
            // Replace the following line with the actual implementation
            return status switch
            {
                TaskStatus.NOT_START => 0,
                TaskStatus.SUBMITTED => 1,
                TaskStatus.IN_PROGRESS => 3,
                TaskStatus.FAILURE => 4,
                TaskStatus.SUCCESS => 5,
                _ => 0
            };
        }
    }

    /// <summary>
    /// 任务操作枚举.
    /// </summary>
    public enum TaskAction
    {
        /// <summary>
        /// 生成图片.
        /// </summary>
        IMAGINE,

        /// <summary>
        /// 选中放大.
        /// </summary>
        UPSCALE,

        /// <summary>
        /// 选中其中的一张图，生成四张相似的.
        /// </summary>
        VARIATION,

        /// <summary>
        /// 重新执行.
        /// </summary>
        REROLL,

        /// <summary>
        /// 图转 prompt.
        /// </summary>
        DESCRIBE,

        /// <summary>
        /// 多图混合.
        /// </summary>
        BLEND,

        /// <summary>
        /// 提交动作
        /// </summary>
        ACTION,

        /// <summary>
        /// 平移
        /// </summary>
        PAN,

        /// <summary>
        /// 变焦
        /// </summary>
        OUTPAINT,

        /// <summary>
        /// 局部重绘
        /// </summary>
        INPAINT,

        ///// <summary>
        ///// 自定义变焦
        ///// </summary>
        //ZOOM,
        //SHORTEN
    }

    /// <summary>
    /// 消息类型枚举.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 创建.
        /// </summary>
        CREATE,

        /// <summary>
        /// 修改.
        /// </summary>
        UPDATE,

        /// <summary>
        /// 删除.
        /// </summary>
        DELETE,

        /// <summary>
        /// 
        /// </summary>
        INTERACTION_CREATE,

        /// <summary>
        /// 
        /// </summary>
        INTERACTION_SUCCESS,

        /// <summary>
        /// 
        /// </summary>
        INTERACTION_IFRAME_MODAL_CREATE,

        /// <summary>
        /// 
        /// </summary>
        INTERACTION_MODAL_CREATE
    }

    /// <summary>
    /// 生成速度模式枚举.
    /// </summary>
    public enum GenerationSpeedMode
    {
        RELAX,
        FAST,
        TURBO
    }

    public static class MessageTypeExtensions
    {
        /// <summary>
        /// 将字符串转换为对应的消息类型枚举.
        /// </summary>
        /// <param name="type">消息类型字符串.</param>
        /// <returns>对应的消息类型枚举.</returns>
        public static MessageType? Of(string type)
        {
            return type switch
            {
                "MESSAGE_CREATE" => MessageType.CREATE,
                "MESSAGE_UPDATE" => MessageType.UPDATE,
                "MESSAGE_DELETE" => MessageType.DELETE,
                "INTERACTION_CREATE" => MessageType.INTERACTION_CREATE,
                "INTERACTION_SUCCESS" => MessageType.INTERACTION_SUCCESS,
                "INTERACTION_IFRAME_MODAL_CREATE" => MessageType.INTERACTION_IFRAME_MODAL_CREATE,
                "INTERACTION_MODAL_CREATE" => MessageType.INTERACTION_MODAL_CREATE,
                _ => null
            };
        }
    }

    /// <summary>
    /// 图片混合维度枚举.
    /// </summary>
    public enum BlendDimensions
    {
        /// <summary>
        /// 纵向.
        /// </summary>
        PORTRAIT,

        /// <summary>
        /// 正方形.
        /// </summary>
        SQUARE,

        /// <summary>
        /// 横向.
        /// </summary>
        LANDSCAPE
    }

    public static class BlendDimensionsExtensions
    {
        /// <summary>
        /// 获取图片混合维度的字符串值.
        /// </summary>
        /// <param name="dimension">图片混合维度.</param>
        /// <returns>图片混合维度的字符串值.</returns>
        public static string GetValue(this BlendDimensions dimension)
        {
            return dimension switch
            {
                BlendDimensions.PORTRAIT => "2:3",
                BlendDimensions.SQUARE => "1:1",
                BlendDimensions.LANDSCAPE => "3:2",
                _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null)
            };
        }
    }
}