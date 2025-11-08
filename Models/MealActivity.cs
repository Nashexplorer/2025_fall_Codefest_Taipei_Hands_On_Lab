using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GongCanApi.Models;

[Table("meal_activities")]
public class MealActivity
{
    [Key]
    [Column("id")]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty;

    [Column("title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(2000)]
    public string? Description { get; set; }

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    [Column("latitude")]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    public decimal? Longitude { get; set; }

    [Column("host_user_id")]
    [MaxLength(50)]
    public string? HostUserId { get; set; }

    [Column("capacity")]
    public int? Capacity { get; set; }

    [Column("current_participants")]
    public int? CurrentParticipants { get; set; }

    [Column("diet_type")]
    [MaxLength(50)]
    public string? DietType { get; set; }

    [Column("is_dine_in")]
    public bool? IsDineIn { get; set; }

    [Column("start_time")]
    public DateTime? StartTime { get; set; }

    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    [Column("signup_deadline")]
    public DateTime? SignupDeadline { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("tags")]
    [MaxLength(500)]
    public string? Tags { get; set; } // JSON 格式存储标签数组

    [Column("status")]
    [MaxLength(20)]
    public string? Status { get; set; }

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("notes")]
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

