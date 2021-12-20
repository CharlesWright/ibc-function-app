using System;
using Azure;
using Azure.Data.Tables;

namespace Interrobang.Content
{
    public class ContentEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}