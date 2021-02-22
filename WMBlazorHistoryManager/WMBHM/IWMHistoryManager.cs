using System.Threading.Tasks;

public interface IWMHistoryManager
{
    void Go(int step);
    Task Forward();
    Task Push(string url);
    Task Back();
    bool HasForward();
    bool HasBack();
    Task Clear();
}