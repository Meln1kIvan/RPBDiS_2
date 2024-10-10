using System;
using System.Collections.Generic;

namespace RPBD1.models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string? InventoryNumber { get; set; }

    public string? Name { get; set; }

    public DateOnly? StartDate { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<CompletedWork> CompletedWorks { get; set; } = new List<CompletedWork>();

    public virtual ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
}
