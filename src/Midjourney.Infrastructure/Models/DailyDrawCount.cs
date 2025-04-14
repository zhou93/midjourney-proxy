using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Midjourney.Infrastructure.Data;

namespace Midjourney.Infrastructure.Models
{
    /// <summary>
    /// 每日绘图统计
    /// </summary>
    [BsonCollection("daily_draw_count")]
    [BsonIgnoreExtraElements]
    [Serializable]
    public class DailyDrawCount : DomainObject
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 绘图次数
        /// </summary>
        public int Count { get; set; }
    }
} 