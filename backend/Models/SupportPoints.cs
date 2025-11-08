using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GongCanApi.Models;
[Table("support_points")]
public class SupportPoint
{
    [Key]
    [Column("_id")]
    public int Id { get; set; }

    [Column("_importdate")]
    public DateTime? ImportDate { get; set; }

    [Column("org_group_name")]
    public string? OrgGroupName { get; set; }

    [Column("org_name")]
    public string? OrgName { get; set; }

    [Column("division")]
    public string? Division { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("lat")]
    public decimal? Lat { get; set; }

    [Column("lon")]
    public decimal? Lon { get; set; }
}
