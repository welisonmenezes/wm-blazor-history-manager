using System;
using System.Threading.Tasks;

public interface IWMHistoryManager
{
    void Configure(bool useBrowserNativeBehavior);
    Task Go(int index);
    Task Forward();
    Task Push(string url);
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
    void SetCallback(Action callback);
    void RemoveCallback(Action callback);
    Task Refresh();
    bool IsUsingBrowserNativeBehavior();
}