using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GongCanApi.Models;

[Table("taipei_parking_status")]
public class TaipeiParkingStatus
{
    [Key]
    [Column("id")]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty;

    [Key]
    [Column("update_time")]
    public DateTime UpdateTime { get; set; }

    [Column("availablecar")]
    public int? AvailableCar { get; set; }

    [Column("availablemotor")]
    public int? AvailableMotor { get; set; }

    [Column("availablebus")]
    public int? AvailableBus { get; set; }

    [Column("availablepregnancy")]
    public int? AvailablePregnancy { get; set; }

    [Column("availablehandicap")]
    public int? AvailableHandicap { get; set; }

    [Column("availableheavymotor")]
    public int? AvailableHeavyMotor { get; set; }

    [Column("charge_total")]
    public int? ChargeTotal { get; set; }

    [Column("charge_busy")]
    public int? ChargeBusy { get; set; }

    [Column("charge_idle")]
    public int? ChargeIdle { get; set; }

    [Column("reserved_1")]
    [MaxLength(255)]
    public string? Reserved1 { get; set; }

    [Column("reserved_2")]
    [MaxLength(255)]
    public string? Reserved2 { get; set; }

    [Column("reserved_3")]
    [MaxLength(255)]
    public string? Reserved3 { get; set; }

    [Column("reserved_4")]
    [MaxLength(255)]
    public string? Reserved4 { get; set; }

    [Column("reserved_5")]
    [MaxLength(255)]
    public string? Reserved5 { get; set; }

    [Column("reserved_6")]
    [MaxLength(255)]
    public string? Reserved6 { get; set; }

    [Column("reserved_7")]
    [MaxLength(255)]
    public string? Reserved7 { get; set; }

    [Column("reserved_8")]
    [MaxLength(255)]
    public string? Reserved8 { get; set; }

    [Column("reserved_9")]
    [MaxLength(255)]
    public string? Reserved9 { get; set; }

    [Column("reserved_10")]
    [MaxLength(255)]
    public string? Reserved10 { get; set; }
}

