namespace TodoApp;

public enum Priority
{
    Low,
    Medium,
    High
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string StatusIcon => IsCompleted ? "✓" : "○";

    public string PriorityIcon => Priority switch
    {
        Priority.High   => "🔴",
        Priority.Medium => "🟡",
        Priority.Low    => "🟢",
        _               => "  "
    };

    public string DueDateDisplay
    {
        get
        {
            if (DueDate == null) return "No due date";
            var diff = DueDate.Value.Date - DateTime.Today;
            if (diff.TotalDays < 0)  return $"Overdue ({DueDate.Value:MMM d})";
            if (diff.TotalDays == 0) return "Due today";
            if (diff.TotalDays == 1) return "Due tomorrow";
            return $"Due {DueDate.Value:MMM d}";
        }
    }
}
