using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RootMobile.Models;

[Table("user_badges")]
public  class UserBadgeModel : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("badge_id")]
    public long BadgeId { get; set; }

    [Column("received_at")]
    public DateTime ReceivedAt { get; set; }
}