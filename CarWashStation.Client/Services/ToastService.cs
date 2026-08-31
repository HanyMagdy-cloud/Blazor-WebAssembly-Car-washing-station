namespace CarWashStation.Client.Services;

public enum ToastKind { Success, Error, Info }
public sealed record ToastMessage(Guid Id, string Text, ToastKind Kind);

public sealed class ToastService
{
    public event Action? Changed;
    public IReadOnlyList<ToastMessage> Messages => messages;
    private readonly List<ToastMessage> messages = [];

    public void Success(string text) => Add(text, ToastKind.Success);
    public void Error(string text) => Add(text, ToastKind.Error);
    public void Info(string text) => Add(text, ToastKind.Info);
    public void Remove(Guid id) { messages.RemoveAll(x => x.Id == id); Changed?.Invoke(); }
    private void Add(string text, ToastKind kind)
    {
        var message = new ToastMessage(Guid.NewGuid(), text, kind);
        messages.Add(message); Changed?.Invoke();
        _ = AutoRemove(message.Id);
    }
    private async Task AutoRemove(Guid id) { await Task.Delay(4500); Remove(id); }
}
