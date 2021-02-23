using System.Threading.Tasks;

public interface IWMHistoryManager
{
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
}