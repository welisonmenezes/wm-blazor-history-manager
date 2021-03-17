# WM Blazor History Manager

Add Back and Foward links for your Blazor Wasm or Blazor Server aplication. 

It is a Blazor history API implementation using either native HTML5 History API or custom behaviors implementation with more features.

When the implementation of the custom behavior is turned off, the HTML 5 History API will be used as a background for the functioning of the component.

But, when the custom behaviour implementation is on, the component will work based on the `sessionStorage` to manage the navigation history. Here, more functionalities will be add to the navigation manager.

## Live Demo

https://welisonmenezes.github.io/wm-blazor-history-manager/

## NuGet Package

https://www.nuget.org/packages/WMBlazorHistoryManager/

---

## Setup

First, import the namespaces in `_Imports.razor`
```c#
@using  WMBlazorHistoryManager.WMBHM
```

Then, inside your main `Startup`/`Program`, call `AddWMBOS`.
```c#
builder.Services.AddWMHistoryManager();
```
_**Above, the custom behaviour implementation is on.**_

*Here, by default, the custom behaviour implementation will be used. But, if you want to use the native HTML 5 History API instead, but pass the `true` value as parameter. Example:*
```c#
builder.Services.AddWMHistoryManager(true);
```
_**Above, the custom behaviour implementation is off.**_

## The History Manager component

Now, inside your `App.razor` file, below the `Router` component, add the `WMHistoryManager` component.
```html
<WMHistoryManager />
```
*Here, you can set if the history must clear on page reload or not and the max size of the history entries. **Works just when custom behaviour implementation is on**. You can do that by the parameters: `ClearOnReload` and `MaxSize`. Example:*
```html
<WMHistoryManager ClearOnReload="true" MaxSize="100" />
```

### Available attributes

*The default values are:*
|Attibute|value|Description|
|--|--|--|
|ClearOnReload|false|Defines if the history will be cleaned on pages refresh.|
|MaxSize|1000|Defines the max size of the history array.|

---

## The Back and Forward components

To get a link to previous or next page, you can use the `WMHistoryBack` and `WMHistoryForward` components respectivally.

```html
<WMHistoryBack Label="Back" />

<WMHistoryForward Label="Forward" />
```

### You may want to set the page title in Blazor applications:

By using the `WMPageTitle` component:
```html
<WMPageTitle  PageTitle="your-page-title-here" />
```

Or, by using the `SetPageTitle` attribute:
```html
<WMHistoryBack 
	Label="Back to: " 
	ConcatPageTitle="true"
	SetPageTitle="your-page-title-here" />
```

**The next options works just when custom behaviour implementation is on**

You can use the page title as a label, or concatenate both:

```html
<WMPageTitle  PageTitle="your-page-title-here" />

<WMHistoryBack Label="Back to: " ConcatPageTitle="true" />

<WMHistoryForward Label="Forward to: " ConcatPageTitle="true" />
```

By default, the component will not be shown if there are no history entries. But you can change this behaviour by force the visibility:

```html
<WMHistoryBack 
	Label="Back to: "
	ConcatPageTitle="true"
	AlwaysVisible="true" />
```

But, if the `AlwaysVisible` is `false`, you can provide a custom element to be rendered when there are no history entry:

```html
<WMHistoryBack
	Label="Back to: "
	ConcatPageTitle="true"
	AlwaysVisible="false">
	<OptionalContent>
		<a  href="/"  class="btn btn-primary">Go to Home.</a>
	</OptionalContent>
</WMHistoryBack>
```

There are two available themes: `default` and `classic`. You can use them if you want to:

```html
<WMHistoryBack
	Label="Back to: "
	ConcatPageTitle="true"
	AlwaysVisible="false"
	Theme="default">
	<OptionalContent>
		<a  href="/"  class="btn btn-primary">Go to Home.</a>
	</OptionalContent>
</WMHistoryBack>
```

But, you want to customize your button, you can pass a css class:
```html
<WMHistoryBack
	Label="Back to: "
	ConcatPageTitle="true"
	AlwaysVisible="false"
	class="your-custom-class" />
```

### Available attributes

|Attibute|Default value|Description|
|--|--|--|
|Label||Sets the text of the button.|
|ConcatPageTitle|false|If true, concatenates the button text with the page title.
|AlwaysVisible|false|If true, the button always will be shown even when there are no entries.|
|Theme||Applies built in themes. Possible values: `default` and `classic`.|
|SetPageTitle||Defines the page title updating the browser tab title.|

## The Go component

Works like the `WMHistoryBack` and `WMHistoryForward`, but, here, you can choose the index of the desired history item:

```html
<WMHistoryGo
	Index="-1"
	Label="Go -1 to: "
	ConcatPageTitle="true"
	AlwaysVisible="false"
	Theme="default">
	<OptionalContent>
		<a  href="/"  class="btn btn-primary">Go to Home.</a>
	</OptionalContent>
</WMHistoryGo>

<WMHistoryGo
	Index="1"
	Label="Go +1 to: "
	ConcatPageTitle="true"
	AlwaysVisible="false"
	Theme="default">
	<OptionalContent>
		No go +1
	</OptionalContent>
</WMHistoryGo>
```

### Available attributes

All attributes as the `WMHistoryBack` and `WMHistoryForward` componentes plus:

|Attibute|Default value|Description|
|--|--|--|
|Index||A negative or positive integer. Zero is not allowed.|

## Methods

You can call some methods directilly from the  History Manager Core. To do this, you must to inject the `IWMHistoryManager` interface into your page:

```c#
@code  {
	[Inject] IWMHistoryManager  historyManager { get; set; }
}
```

### Available methods

|Method|Parameters|Return|Description|
|--|--|--|--|
|Back||`void`|Navigates to the previous history item.|
|Forward||`void`|Navigates the next history item.|
|Go|`int index`|void|Navigates to the history item correspondent to the index param.|
|HasBack||`boolean`|Checks if there is a previous history item.|
|HasForward||`boolean`|Checks if there is a next history item.|
|CanNavigate|`int index`|`boolean`|Checks if there is a history item correspondent to the index param.|
|Clear||`void`|Clears the history array and reset the history manager state.|
|StopWatch||`void`|Freezes the history manager.|
|RestoreWatch||`void`|Reactivates the history manager.|
|SetPageTitle|`string pageTitle`|`void`|Sets the page title.|
|GetBackTitle||`string`|Returns the previous page title if it exists.|
|GetForwardTitle||`string`|Returns the next page title if it exists.|
|GetGoTitle|`int index`|`string`|Returns the page title correspondent to the index param if it exists.|
|Refresh||`void`|Reload the window.|
|IsUsingBrowserNativeBehavior||`boolean`|Returns true if the custom behaviour implementation is off.|
|SetCallback|`Action`|`void`|Subscribes the given method to be called when the History Manager was done.|
|RemoveCallback|`Action`|`void`|Unsubscribes the given method when the History Manager is dispose.|

Example:
```html
<button @onclick="@(e  => historyManager.Back())">Back</button>

@code  {
	[Inject] IWMHistoryManager  historyManager { get; set; }
}
```

## Back and Forward programmatically

The following is an example of implementing the back and forward links programmatically:

```html
@page  "/your-path-here"

@if (hasBack)
{
	<button class="btn btn-outline-primary" @onclick="@(e => historyManager.Back())">
		Back to: @backLabel
	</button>
}
else
{
	<a  href="/" class="btn btn-primary">Go to Home.</a>
}

@if (hasForward)
{
	<button class="btn btn-outline-primary" @onclick="@(e => historyManager.Forward())">
		Forward to: @forwardLabel
	</button>
}
else
{
	<span>No forward</span>
}
```

```c#
@implements  IDisposable

@code  {
	[Inject] IWMHistoryManager  historyManager { get; set; }
	string  backLabel { get; set; }
	string  forwardLabel { get; set; }
	bool  hasBack { get; set; } = false;
	bool  hasForward { get; set; } = false;
	  
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			historyManager.SetPageTitle("Step Three");
			historyManager.SetCallback(SetInfos);
		}
	}

	public async void SetInfos()
	{
		backLabel = historyManager.GetBackTitle();
		forwardLabel = historyManager.GetForwardTitle();
		hasBack = await historyManager.HasBack();
		hasForward = await historyManager.HasForward();
		StateHasChanged();
	}

	public void Dispose()
	{
		historyManager.RemoveCallback(SetInfos);
	}
}
```

**Note that some informations must be get into a callback method which is set into method `SetCallback`. It is important to remove this callback in on dispose of the page.**

## Other methods example

```html
@page  "/your-path-here"
<WMPageTitle PageTitle="your-page-title-here" />

@if (canNavigate)
{
	<button class="btn btn-outline-primary" @onclick="@(e => historyManager.Go(goIndex))">
		Back to: @goLabel
	</button>
}
else
{
	<span>No go -1</span>
}

<button class="btn btn-secondary" @onclick="@(e => historyManager.Clear())">
	Clear
</button>
<button class="btn btn-secondary" @onclick="@(e => historyManager.StopWatch())">
	Stop Watch
</button>
<button class="btn btn-secondary" @onclick="@(e => historyManager.RestoreWatch())">
	Restore Watch
</button>
<button class="btn btn-secondary" @onclick="@(e => historyManager.Refresh())">
	Refresh
</button>
<button class="btn btn-secondary" @onclick="@(e => ChangePageTitle())">
	Change Page Title
</button>
<button class="btn btn-secondary" @onclick="@(e => IsNative())">
	Is native? @isNativeBehavior
</button>
```
```c#
@implements  IDisposable

@code  {
	[Inject] IWMHistoryManager historyManager { get; set; }
	int goIndex = -1;
	string goLabel { get; set; }
	bool canNavigate { get; set; } = false;
	string isNativeBehavior { get; set; } = "";

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			historyManager.SetCallback(SetInfos);
		}
	}

	public async void SetInfos()
	{
		canNavigate = await historyManager.CanNavigate(goIndex);
		goLabel = await historyManager.GetGoTitle(goIndex);
		StateHasChanged();
	}

	public async void ChangePageTitle()
	{
		await historyManager.SetPageTitle("Page title changed!");
	}

	public void IsNative()
	{
		isNativeBehavior = (historyManager.IsUsingBrowserNativeBehavior()) ? "Is native" : "is not native";
	}

	public void Dispose()
	{
		historyManager.RemoveCallback(SetInfos);
	}
}
```
