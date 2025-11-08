using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GongCanApi.Models;

[Table("meal_event_participants")]
public class MealEventParticipant
{
    [Key]
    [Column("id")]
    [MaxLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("meal_event_id")]
    [Required]
    [MaxLength(50)]
    public string MealEventId { get; set; } = string.Empty;

    [Column("user_id")]
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "confirmed"; // confirmed, cancelled

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // 導航屬性
    [ForeignKey("MealEventId")]
    public MealEvent? MealEvent { get; set; }

    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("email")]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Column("participant_count")]
    public int ParticipantCount { get; set; } = 1; // 預設為 1 人

    [Column("user_name")]
    [MaxLength(255)]
    public string? UserName { get; set; }
}

