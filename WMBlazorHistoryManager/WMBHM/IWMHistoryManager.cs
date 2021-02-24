using System;
using System.Threading.Tasks;

public interface IWMHistoryManager
{
    void Configure(bool useBrowserNativeBehavior);
    Task Go(int index);
    Task Forward();
    Task Push(string url, int maxSize);
    Task Back();
    bool HasForward();
    bool HasBack();
    Task Clear();
    void StopWatch();
    void RestoreWatch();
    Task SetPageTitle(string title);
    Task<string> GetTitleByIndex(int index);
    string GetBackTitle();
    string GetForwardTitle();
    Task<string> GetGoTitle(int index);
    void SetCallback(Action callback);
    void RemoveCallback(Action callback);
    Task Refresh();
    bool IsUsingBrowserNativeBehavior();
    bool CanNavigate(int index);
    Task<bool> IsSameUrl(int index);
}