namespace Tharga.Wpf.ApplicationUpdate;

public interface ISplash
{
    void UpdateInfo(string message);
    void Show();
    void Hide();
    void SetErrorMessage(string message);
    void ShowCloseButton();
    void Close();
    bool IsCloseButtonVisible { get; }
    void ClearMessages();
}