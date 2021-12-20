using System;
using Azure;
using Azure.Data.Tables;

namespace Interrobang.Content
{
    public class SongEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }
        public string Tags { get; set;}
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}