using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GongCanApi.Models;

[Table("meal_events")]
public class MealEvent
{
    [Key]
    [Column("id")]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty;

    [Column("title")]
    [MaxLength(255)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("image_url")]
    [MaxLength(512)]
    public string? ImageUrl { get; set; }

    [Column("location")]
    [MaxLength(255)]
    public string? Location { get; set; }

    [Column("latitude", TypeName = "decimal(10,6)")]
    public decimal? Latitude { get; set; }

    [Column("longitude", TypeName = "decimal(10,6)")]
    public decimal? Longitude { get; set; }

    [Column("host_user_id")]
    [MaxLength(50)]
    [Required]
    public string HostUserId { get; set; } = string.Empty;

    [Column("capacity")]
    public int Capacity { get; set; } = 0;

    [Column("current_participants")]
    public int CurrentParticipants { get; set; } = 0;

    [Column("diet_type")]
    [MaxLength(50)]
    public string? DietType { get; set; }

    [Column("is_dine_in")]
    public bool IsDineIn { get; set; } = true;

    [Column("start_time")]
    [Required]
    public DateTime StartTime { get; set; }

    [Column("end_time")]
    [Required]
    public DateTime EndTime { get; set; }

    [Column("signup_deadline")]
    public DateTime? SignupDeadline { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("tags", TypeName = "json")]
    public string? Tags { get; set; }

    [Column("status", TypeName = "ENUM('open','closed','cancelled','full')")]
    [MaxLength(20)]
    public string Status { get; set; } = "open";

    [Column("notes")]
    [MaxLength(255)]
    public string? Notes { get; set; }

    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("reserved_1")]
    [MaxLength(255)]
    public string? Reserved1 { get; set; }

    [Column("reserved_2")]
    [MaxLength(255)]
    public string? Reserved2 { get; set; }

    [Column("reserved_3")]
    [MaxLength(255)]
    public string? Reserved3 { get; set; }
}

