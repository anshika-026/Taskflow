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
        var json = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    private void Load()
    {
        if (!File.Exists(_filePath)) return;
        try
        {
            var json = File.ReadAllText(_filePath);
            _todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new();
            _nextId = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
        }
        catch
        {
            _todos = new();
        }
    }
}
