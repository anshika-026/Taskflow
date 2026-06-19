namespace TodoApp;

public static class Display
{
    public static void Header()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════╗");
        Console.WriteLine("║        📝  TODO LIST         ║");
        Console.WriteLine("╚══════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void Menu()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─────────────────────────────────");
        Console.ResetColor();
        Console.WriteLine("  [1] View all tasks");
        Console.WriteLine("  [2] Add a task");
        Console.WriteLine("  [3] Mark task complete/incomplete");
        Console.WriteLine("  [4] Delete a task");
        Console.WriteLine("  [5] View by priority");
        Console.WriteLine("  [6] Edit a task");
        Console.WriteLine("  [7] Search tasks");
        Console.WriteLine("  [8] Filter & sort tasks");
        Console.WriteLine("  [Q] Quit");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─────────────────────────────────");
        Console.ResetColor();
        Console.Write("  Choose an option: ");
    }

    public static void TaskList(List<TodoItem> todos, string title = "All Tasks")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {title} ({todos.Count})");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─────────────────────────────────────────────────────");
        Console.ResetColor();

        if (todos.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  No tasks found.");
            Console.ResetColor();
        }

        foreach (var todo in todos)
        {
            var titleColor = todo.IsCompleted ? ConsoleColor.DarkGray : ConsoleColor.White;

            Console.Write($"  [{todo.Id,2}] ");

            Console.ForegroundColor = todo.IsCompleted ? ConsoleColor.Green : ConsoleColor.DarkGray;
            Console.Write($"{todo.StatusIcon} ");

            Console.Write(todo.PriorityIcon + " ");

            Console.ForegroundColor = titleColor;
            var titleText = todo.IsCompleted ? $"~~{todo.Title}~~" : todo.Title;
            Console.Write($"{todo.Title,-30}");

            Console.ForegroundColor = todo.DueDateDisplay.StartsWith("Overdue")
                ? ConsoleColor.Red
                : ConsoleColor.DarkCyan;
            Console.WriteLine($"  {todo.DueDateDisplay}");

            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ {message}");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ {message}");
        Console.ResetColor();
    }

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\n  Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
