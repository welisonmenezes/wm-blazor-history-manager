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

    public async Task Push(string url, int maxSize)
    {   
        if (! this.useBrowserNativeBehavior)
        {
            if (this.isWatching) 
            {
                var module = await this.Module;
                await module.InvokeAsync<int>("WMBHMPush", url, maxSize);
                this.currentTitle = await module.InvokeAsync<string>("WMBHMGetCurrentTitle");
            }
            bool ICanBack = await this.HasBack();
            bool ICanForward = await this.HasForward();
            this.backTitle = (ICanBack) ? await GetTitleByIndex((this.currentIndex - 1)) : null;
            this.forwardTitle = (ICanForward) ? await GetTitleByIndex((this.currentIndex + 1)) : null;
        }
        this.RunCallbacks();
    }

    public async Task Back()
    {
        var module = await this.Module;
        if (! this.useBrowserNativeBehavior)
        {
            bool ICanNav = await this.HasBack();
            if (ICanNav)
            {
                string backUrl = await module.InvokeAsync<string>("WMBHMNavigate", -1);
                if (backUrl != null) this.navigationManager.NavigateTo(backUrl);
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
            bool ICanNav = await this.HasForward();
            if (ICanNav)
            {
                string forwardUrl = await module.InvokeAsync<string>("WMBHMNavigate", 1);
                if (forwardUrl != null) this.navigationManager.NavigateTo(forwardUrl);
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
                bool ICanNav = await CanNavigate(index);
                if (!ICanNav) return;
                string newUrl = await module.InvokeAsync<string>("WMBHMGo", index);
                if (newUrl != null) this.navigationManager.NavigateTo(newUrl);
            }
            else
            {
                await module.InvokeVoidAsync("WMBHMNativeNavigate", index);
            }
        }
    }

    public async Task<bool> CanNavigate(int index)
    {
        var module = await this.Module;
        return await module.InvokeAsync<bool>("WMBHMCanNavigate", index);
    }

    public async Task Clear()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        var module = await this.Module;
        await module.InvokeAsync<int>("WMBHMClear", navigationManager.Uri);
        this.backTitle = this.forwardTitle = null;
        this.isWatching = true;
        this.RunCallbacks();
    }

    public async Task<bool> HasForward()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return await this.CanNavigate(1);
    }

    public async Task<bool> HasBack()
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        return await this.CanNavigate(-1);
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

    public async Task<string> GetGoTitle(int index)
    {
        if (this.useBrowserNativeBehavior) throw new ArgumentException("Not supported if useBrowserNativeBehavior is true.");
        int newIndex = this.currentIndex - (index * -1);
        var module = await this.Module;
        return await module.InvokeAsync<string>("WMBHMGetTitleByIndex", newIndex);
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
        var module = await this.Module;
        string url = await module.InvokeAsync<string>("WMBHMGetUrlByIndex", index);
        if (url != null)
        {
            return (url.Equals(navigationManager.Uri));
        }
        return true;
    }
}