using Postgrest.Attributes;
using Postgrest.Models;

namespace SupabaseStateStore
{
    [Table("dapr_state_store")]
    public class KeyValue : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("key")]
        public string Key { get; set; }

        [Column("value")]
        public string Value { get; set; }
    }
}