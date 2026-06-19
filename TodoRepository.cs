 using System.Text.Json;

namespace TodoApp;

public class TodoRepository
{
    private readonly string _filePath;
    private List<TodoItem> _todos = new();
    private int _nextId = 1;

    public TodoRepository(string filePath = "todos.json")
    {
        _filePath = filePath;
        Load();
    }

    public List<TodoItem> GetAll() => _todos.ToList();

    public List<TodoItem> GetByStatus(bool completed) =>
        _todos.Where(t => t.IsCompleted == completed).ToList();

    public TodoItem? GetById(int id) =>
        _todos.FirstOrDefault(t => t.Id == id);

    public void Add(TodoItem item)
    {
        item.Id = _nextId++;
        _todos.Add(item);
        Save();
    }

    public bool Delete(int id)
    {
        var item = GetById(id);
        if (item == null) return false;
        _todos.Remove(item);
        Save();
        return true;
    }

    public bool ToggleComplete(int id)
    {
        var item = GetById(id);
        if (item == null) return false;
        item.IsCompleted = !item.IsCompleted;
        Save();
        return true;
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_todos,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (IOException ex)
        {
            LogError($"Failed to save tasks: {ex.Message}");
            Console.WriteLine("  Warning: Could not save tasks to file.");
        }
    }

    private void Load()
    {
        if (!File.Exists(_filePath))
        {
            _todos = new();
            return;
        }
        try
        {
            var json = File.ReadAllText(_filePath);
            _todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new();
            _nextId = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
        }
        catch (JsonException ex)
        {
            LogError($"Failed to read tasks file: {ex.Message}");
            File.Copy(_filePath, _filePath + ".backup", overwrite: true);
            _todos = new();
        }
        catch (IOException ex)
        {
            LogError($"File access error: {ex.Message}");
            _todos = new();
        }
    }
    public bool Edit(int id, string newTitle, string newDescription, Priority newPriority, DateTime? newDueDate)
    {
        var item = GetById(id);
        if (item == null) return false;
        item.Title = newTitle;
        item.Description = newDescription;
        item.Priority = newPriority;
        item.DueDate = newDueDate;
        Save();
        return true;
    }

    public List<TodoItem> Search(string keyword)
    {
        return _todos.Where(t =>
            t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void LogError(string message)
    {
        try
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
        catch
        {
            // If logging fails, we silently ignore it to avoid crashing the app.
        }
    }
}
