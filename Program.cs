using TodoApp;

var repo = new TodoRepository();

while (true)
{
    Display.Header();
    Display.Menu();

    var input = Console.ReadLine()?.Trim().ToUpper();

    switch (input)
    {
        case "1":
            ViewAllTasks();
            break;
        case "2":
            AddTask();
            break;
        case "3":
            ToggleTask();
            break;
        case "4":
            DeleteTask();
            break;
        case "5":
            ViewByPriority();
            break;
        case "6":
            EditTask();
            break;
        case "7":
            SearchTasks();
            break;
        case "8":
            FilterAndSort();
            break;
        default:
            Display.Error("Invalid option. Please try again.");
            Display.Pause();
            break;
    }
}

void ViewAllTasks()
{
    Display.Header();
    var todos = repo.GetAll().OrderBy(t => t.IsCompleted).ThenBy(t => t.Priority).ToList();
    Display.TaskList(todos);
    Display.Pause();
}

void AddTask()
{
    Display.Header();
    Console.WriteLine("  ── Add New Task ──\n");

    Console.Write("  Task title: ");
    var title = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(title))
    {
        Display.Error("Title cannot be empty.");
        Display.Pause();
        return;
    }

    Console.WriteLine("\n  Priority:");
    Console.WriteLine("    [1] Low");
    Console.WriteLine("    [2] Medium");
    Console.WriteLine("    [3] High");
    Console.Write("  Choose (default Medium): ");
    var priorityInput = Console.ReadLine()?.Trim();
    var priority = priorityInput switch
    {
        "1" => Priority.Low,
        "3" => Priority.High,
        _   => Priority.Medium
    };

    Console.Write("\n  Due date (e.g. 12/25, or leave blank): ");
    var dateInput = Console.ReadLine()?.Trim();
    DateTime? dueDate = null;
    if (!string.IsNullOrEmpty(dateInput))
    {
        if (DateTime.TryParse(dateInput, out var parsed))
            dueDate = parsed;
        else
            Display.Error("Invalid date — skipping due date.");
    }

    var item = new TodoItem
    {
        Title    = title,
        Priority = priority,
        DueDate  = dueDate
    };

    repo.Add(item);
    Display.Success($"Task \"{title}\" added!");
    Display.Pause();
}

void ToggleTask()
{
    Display.Header();
    var todos = repo.GetAll();
    Display.TaskList(todos);

    Console.Write("  Enter task ID to toggle: ");
    if (int.TryParse(Console.ReadLine(), out int id) && repo.ToggleComplete(id))
    {
        var task = repo.GetById(id)!;
        Display.Success($"Task marked as {(task.IsCompleted ? "complete" : "incomplete")}.");
    }
    else
    {
        Display.Error("Task not found.");
    }
    Display.Pause();
}

void DeleteTask()
{
    Display.Header();
    var todos = repo.GetAll();
    Display.TaskList(todos);

    Console.Write("  Enter task ID to delete: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var task = repo.GetById(id);
        if (task == null) { Display.Error("Task not found."); Display.Pause(); return; }

        Console.Write($"  Delete \"{task.Title}\"? (y/N): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            repo.Delete(id);
            Display.Success("Task deleted.");
        }
        else
        {
            Console.WriteLine("  Cancelled.");
        }
    }
    else
    {
        Display.Error("Invalid ID.");
    }
    Display.Pause();
}

void ViewByPriority()
{
    Display.Header();
    Console.WriteLine("  ── Tasks by Priority ──\n");

    var high   = repo.GetAll().Where(t => t.Priority == Priority.High   && !t.IsCompleted).ToList();
    var medium = repo.GetAll().Where(t => t.Priority == Priority.Medium && !t.IsCompleted).ToList();
    var low    = repo.GetAll().Where(t => t.Priority == Priority.Low    && !t.IsCompleted).ToList();

    Display.TaskList(high,   "🔴 High Priority");
    Display.TaskList(medium, "🟡 Medium Priority");
    Display.TaskList(low,    "🟢 Low Priority");

    Display.Pause();
}

void EditTask()
{
    Display.Header();
    var todos = repo.GetAll();
    Display.TaskList(todos);

    Console.Write("  Enter task ID to edit: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Display.Error("Invalid ID.");
        Display.Pause();
        return;
    }

    var task = repo.GetById(id);
    if (task == null)
    {
        Display.Error("Task not found.");
        Display.Pause();
        return;
    }

    Console.Write($"  New title (current: {task.Title}): ");
    var title = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(title)) title = task.Title;

    Console.Write($"  New description (current: {task.Description}): ");
    var desc = Console.ReadLine()?.Trim() ?? task.Description;

    Console.WriteLine("\n  Priority: [1] Low  [2] Medium  [3] High");
    Console.Write("  Choose: ");
    var p = Console.ReadLine()?.Trim();
    var priority = p switch { "1" => Priority.Low, "3" => Priority.High, _ => Priority.Medium };

    Console.Write($"  New due date (current: {task.DueDateDisplay}): ");
    var dateInput = Console.ReadLine()?.Trim();
    DateTime? dueDate = task.DueDate;
    if (!string.IsNullOrEmpty(dateInput) && DateTime.TryParse(dateInput, out var parsed))
        dueDate = parsed;

    repo.Edit(id, title, desc, priority, dueDate);
    Display.Success("Task updated!");
    Display.Pause();
}

void SearchTasks()
{
    Display.Header();
    Console.Write("  Enter keyword to search: ");
    var keyword = Console.ReadLine()?.Trim() ?? "";
    var results = repo.Search(keyword);
    Display.TaskList(results, $"Search results for \"{keyword}\"");
    Display.Pause();
}

void FilterAndSort()
{
    Display.Header();
    Console.WriteLine("  ── Filter & Sort ──\n");
    Console.WriteLine("  [1] Show completed tasks");
    Console.WriteLine("  [2] Show pending tasks");
    Console.WriteLine("  [3] Sort by due date");
    Console.WriteLine("  [4] Sort by priority");
    Console.Write("\n  Choose: ");

    var choice = Console.ReadLine()?.Trim();

    var todos = repo.GetAll();

    var result = choice switch
    {
        "1" => todos.Where(t => t.IsCompleted).ToList(),
        "2" => todos.Where(t => !t.IsCompleted).ToList(),
        "3" => todos.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList(),
        "4" => todos.OrderBy(t => t.Priority).ToList(),
        _ => todos
    };

    var title = choice switch
    {
        "1" => "Completed Tasks",
        "2" => "Pending Tasks",
        "3" => "Sorted by Due Date",
        "4" => "Sorted by Priority",
        _ => "All Tasks"
    };

    Display.TaskList(result, title);
    Display.Pause();
}