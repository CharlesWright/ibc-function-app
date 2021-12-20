using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;

namespace Interrobang.Content
{
    public class AlbumEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Title { get; set; }        
        public string SongList {get => JsonSerializer.Serialize(TrueSongList); set=> TrueSongList = JsonSerializer.Deserialize<List<string>>(value);}
        
        [IgnoreDataMember]
        public List<string> TrueSongList { get; set;}
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}