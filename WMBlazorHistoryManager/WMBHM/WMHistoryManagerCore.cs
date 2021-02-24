using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

public sealed class WMHistoryManagerCore : IWMHistoryManager
{
    private readonly IJSRuntime jsRuntime;
    private readonly NavigationManager navigationManager;
    private Task<IJSObjectReference> _module;
    private Task<IJSObjectReference> Module => _module ??= jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/WMBlazorHistoryManager/wm-blazor-history-manager.js").AsTask();
    private bool useBrowserNativeBehavior { get; set; } = false;
    private int currentIndex { get; set; } = 0;
    private int totalIndex { get; set; } = 0;
    private bool isNavitation { get; set; } = false;
    private bool isWatching { get; set; } = true;
    private string currentTitle { get; set; }
    private string backTitle { get; set; }
    private string forwardTitle { get; set; }
    private List<Action> clientCallbacks { get; set; }
    private EventHandler WMHMEventHandler { get; set; }
    
    public WMHistoryManagerCore(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        this.jsRuntime = jsRuntime;
        this.navigationManager = navigationManager;
        this.clientCallbacks = new List<Action>();
    }

    public void Configure(bool useBrowserNativeBehavior)
    {
        this.useBrowserNativeBehavior = useBrowserNativeBehavior;
    }

    public async Task Push(string url)
    {   
        if (! this.useBrowserNativeBehavior)
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
        }
        this.RunCallbacks();
    }

    public async Task Back()
    {
        var module = await this.Module;
        if (! this.useBrowserNativeBehavior)
        {
            if (this.HasBack())
            {
                this.currentIndex--;
                string backUrl = await module.InvokeAsync<string>("WMBHMNavigate", this.currentIndex);
                this.isNavitation = true;
                this.navigationManager.NavigateTo(backUrl);
            }
        } 
        else
        {
            await module.InvokeVoidAsync("WMBHMNativeNavigate", -1);
        }
    }

    public async Task Forward()
    {
        var module = await this.Module;
        if (! this.useBrowserNativeBehavior)
        {
            if (this.HasForward())
            {
                this.currentIndex++;
                string forwardUrl = await module.InvokeAsync<string>("WMBHMNavigate", this.currentIndex);
                this.isNavitation = true;
                this.navigationManager.NavigateTo(forwardUrl);
            }
        }
        else
        {
            await module.InvokeVoidAsync("WMBHMNativeNavigate", +1);
        }
    }

    public async Task Go(int index)
    {
        if (index != 0)
        {
            var module = await this.Module;
            if (! this.useBrowserNativeBehavior)
            {
                if (!CanNavigate(index)) return;
                int newIndex = this.currentIndex - (index * -1);
                this.currentIndex = newIndex;
                string newUrl = await module.InvokeAsync<string>("WMBHMNavigate", newIndex);
                this.isNavitation = true;
                this.navigationManager.NavigateTo(newUrl);
            }
            else
            {
                await module.InvokeVoidAsync("WMBHMNativeNavigate", index);
            }
        }
    }

    public bool CanNavigate(int index)
    {
        int newIndex = this.currentIndex - (index * -1);
        if (index < 0 && newIndex < 0) return false;
        if (index > 0 && newIndex > this.totalIndex) return false;
        return true;
    }

    public async Task Clear()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        var module = await this.Module;
        this.totalIndex = this.currentIndex = await module.InvokeAsync<int>("WMBHMClear", navigationManager.Uri);
        this.backTitle = this.forwardTitle = null;
        this.isNavitation = false;
        this.isWatching = true;
        this.RunCallbacks();
    }

    public bool HasForward()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return (this.currentIndex <  this.totalIndex);
    }

    public bool HasBack()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return (this.currentIndex > 0);
    }

    public void StopWatch()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        this.isWatching = false;
    }

    public void RestoreWatch()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        this.isWatching = true;
    }

    public async Task SetPageTitle(string title)
    {
        var module = await this.Module;
        await module.InvokeVoidAsync("WMBHMSetPageTitle", title);
    }

    public async Task<string> GetTitleByIndex(int index)
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        var module = await this.Module;
        return await module.InvokeAsync<string>("WMBHMGetTitleByIndex", index);
    }

    public string GetBackTitle()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return this.backTitle;
    }

    public string GetForwardTitle()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return this.forwardTitle;
    }

    public void SetCallback(Action callback)
    {
        if (! this.useBrowserNativeBehavior)
        {
            this.clientCallbacks.Add(callback);
        }
    }

    public void RemoveCallback(Action callback)
    {
        if (! this.useBrowserNativeBehavior)
        {
            this.clientCallbacks.Clear();
        }
    }

    public async Task Refresh()
    {
        var module = await this.Module;
        await module.InvokeVoidAsync("WMBHMRefresh");
    }

    public bool IsUsingBrowserNativeBehavior()
    {
        return this.useBrowserNativeBehavior;
    }

    private void RunCallbacks()
    {
        if (this.clientCallbacks != null)
        {
            foreach (Action callback in this.clientCallbacks)
            {
                callback.Invoke();
            }
        }
    }

    public async Task<bool> IsSameUrl(int index)
    {
        int newIndex = this.currentIndex - (index * -1);
        var module = await this.Module;
        string url = await module.InvokeAsync<string>("WMBHMGetUrlByIndex", newIndex);
        if (url != null)
        {
            return (url.Equals(navigationManager.Uri));
        }
        return true;
    }
}