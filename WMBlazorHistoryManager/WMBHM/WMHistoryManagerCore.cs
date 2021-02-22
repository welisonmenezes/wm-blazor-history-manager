using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

public sealed class WMHistoryManagerCore : IWMHistoryManager
{
    private readonly IJSRuntime jsRuntime;
    private readonly NavigationManager navigationManager;
    private Task<IJSObjectReference> _module;
    private Task<IJSObjectReference> Module => _module ??= jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/WMBlazorHistoryManager/wm-blazor-history-manager.js").AsTask();
    private int currentIndex { get; set; } = 0;
    private int totalIndex { get; set; } = 0;
    private bool WMHMNavigation { get; set; } = false;
    
    public WMHistoryManagerCore(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        this.jsRuntime = jsRuntime;
        this.navigationManager = navigationManager;
    }

    public void Go(int step)
    {
        System.Console.WriteLine("Go");
    }

    public async Task Push(string url)
    {   
        if (! this.WMHMNavigation) 
        {
            var module = await this.Module;
            totalIndex = currentIndex = await module.InvokeAsync<int>("WMBHMPush", url);
            System.Console.WriteLine("" + currentIndex);
        }
        else
        {
            this.WMHMNavigation = false;
        }
    }

    public async Task Back()
    {
        if (this.HasBack())
        {
            this.currentIndex--;
            var module = await this.Module;
            string backUrl = await module.InvokeAsync<string>("WMBHMNavigate", currentIndex);
            this.WMHMNavigation = true;
            System.Console.WriteLine(backUrl);
            navigationManager.NavigateTo(backUrl);
        }
        
    }

    public async Task Forward()
    {
        if (this.HasForward())
        {
            this.currentIndex++;
            var module = await this.Module;
            string forwardUrl = await module.InvokeAsync<string>("WMBHMNavigate", currentIndex);
            this.WMHMNavigation = true;
            System.Console.WriteLine(forwardUrl);
            navigationManager.NavigateTo(forwardUrl);
        }
    }

    public async Task Clear()
    {
        var module = await this.Module;
        this.totalIndex = this.currentIndex = await module.InvokeAsync<int>("WMBHMClear", navigationManager.Uri);
    }

    public bool HasForward()
    {
        return (this.currentIndex <  this.totalIndex);
    }

    public bool HasBack()
    {
        return (this.currentIndex > 0);
    }
}