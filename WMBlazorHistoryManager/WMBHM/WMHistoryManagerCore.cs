using System;
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
    private bool isNavitation { get; set; } = false;
    private bool isWatching { get; set; } = true;
    private string currentTitle { get; set; }
    private string backTitle { get; set; }
    private string forwardTitle { get; set; }
    private Action clientCallback { get; set; }
    
    public WMHistoryManagerCore(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        this.jsRuntime = jsRuntime;
        this.navigationManager = navigationManager;
    }

    public async Task Push(string url)
    {   
        if (! this.isNavitation && this.isWatching) 
        {
            var module = await this.Module;
            this.totalIndex = this.currentIndex = await module.InvokeAsync<int>("WMBHMPush", url);
            this.currentTitle = await module.InvokeAsync<string>("WMBHMGetCurrentTitle");
        }
        if (this.isNavitation)  this.isNavitation = false;
        this.backTitle = (this.HasBack()) ? await GetTitleByIndex((this.currentIndex - 1)) : null;
        this.forwardTitle = (this.HasForward()) ? await GetTitleByIndex((this.currentIndex + 1)) : null;
        if (this.clientCallback != null)  clientCallback.Invoke();
    }

    public async Task Back()
    {
        if (this.HasBack())
        {
            this.currentIndex--;
            var module = await this.Module;
            string backUrl = await module.InvokeAsync<string>("WMBHMNavigate", currentIndex);
            this.isNavitation = true;
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
            this.isNavitation = true;
            navigationManager.NavigateTo(forwardUrl);
        }
    }

    public async Task Go(int index)
    {
        if (index != 0)
        {
            int newIndex = this.currentIndex - (index * -1);
            if (index < 0 && newIndex < 0) return;
            if (index > 0 && newIndex > this.totalIndex) return;
            this.currentIndex = newIndex;
            var module = await this.Module;
            string newUrl = await module.InvokeAsync<string>("WMBHMNavigate", newIndex);
            this.isNavitation = true;
            navigationManager.NavigateTo(newUrl);
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

    public void StopWatch()
    {
        this.isWatching = false;
    }

    public void RestoreWatch()
    {
        this.isWatching = true;
    }

    public async Task SetPageTitle(string title)
    {
        var module = await this.Module;
        await module.InvokeVoidAsync("WMBHMSetPageTitle", title);
    }

    public async Task<string> GetTitleByIndex(int index)
    {
        var module = await this.Module;
        return await module.InvokeAsync<string>("WMBHMGetTitleByIndex", index);
    }

    public string GetBackTitle()
    {
        return this.backTitle;
    }

    public string GetForwardTitle()
    {
        return this.forwardTitle;
    }

    public void SetCallback(Action callback)
    {
        this.clientCallback = callback;
    }
}