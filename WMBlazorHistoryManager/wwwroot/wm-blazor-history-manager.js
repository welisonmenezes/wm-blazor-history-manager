var WMBHMIsBack = false;
var WMBHMIsForward = false;

export function WMBHMPush(url, maxSize) {
    var entries = WMBHMGetEntries();
    var newEntry = { url: url, title: WMBHMGetCurrentTitle() };
    if ((entries.length < 1) || (entries[entries.length - 1] && entries[entries.length - 1].url !== url)) {
        entries.push(newEntry);
        WMBHMLimitHistorySize(entries, maxSize);
        if (!WMBHMIsBack) {
            window.sessionStorage.setItem('wmbhm-entries', JSON.stringify(entries));
            if (!WMBHMIsForward) {
                window.sessionStorage.setItem('wmbhm-entries-forward', JSON.stringify([]));
            }
        } 
    }
    WMBHMIsBack = false;
    WMBHMIsForward = false;
    return entries.length - 1;
}

export function WMBHMNavigate(index) {
    if (index < 0) {
        WMBHMIsBack = true;
        var entries = WMBHMGetEntries();
        var lastEntry = WMBHMGetLastItem(entries);
        var entriesForward = WMBHMGetEntriesForward();
        if (lastEntry) entriesForward.push(lastEntry);
        if (entries.length > 1) {
            window.sessionStorage.setItem('wmbhm-entries-forward', JSON.stringify(entriesForward));
            entries.pop();
        }
        window.sessionStorage.setItem('wmbhm-entries', JSON.stringify(entries));
        lastEntry = WMBHMGetLastItem(entries);
        return (lastEntry) ? lastEntry.url : null;
    } else if (index > 0) {
        WMBHMIsForward = true;
        var entriesForward = WMBHMGetEntriesForward();
        var lastEntry = WMBHMGetLastItem(entriesForward);
        entriesForward.pop();
        window.sessionStorage.setItem('wmbhm-entries-forward', JSON.stringify(entriesForward));
        return (lastEntry) ? lastEntry.url : null;
    }
    return null;
}

export function WMBHMGo(index) {
    if (index < 0) {
        var entries = WMBHMGetEntries();
        var theUrl = null;
        if (entries.length > (index * -1)) {
            for(var i = index; i < 0; i++) {
                theUrl = WMBHMNavigate(i);
            }
        }
        return theUrl;
    } else if (index > 0) {
        var entriesForward = WMBHMGetEntriesForward();
        var theUrl = null;
        if (entriesForward.length >= index) {
            for(var i = 0; i <= index; i++) {
                theUrl = WMBHMNavigate(i);
            }
        }
        return theUrl;
    }
    return null;
}

export function WMBHMCanNavigate(index) {
    if (index < 0) {
        var entries = WMBHMGetEntries();
        return (entries.length > (index * -1));
    } else if (index > 0) {
        var entriesForward = WMBHMGetEntriesForward();
        return (entriesForward.length >= index);
    }
    return false;
}

export function WMBHMGetUrlByIndex(index) {
    var entry = WMBHMGetEntryByIndex(index);
    return (entry) ? entry.url : null;
}

export function WMBHMTotalIndex() {
    var entries = WMBHMGetEntries();
    return (entries.length > 0) ? (entries.length - 1) : 0;
}

export function WMBHMClear(url) {
    window.sessionStorage.removeItem('wmbhm-entries');
    window.sessionStorage.removeItem('wmbhm-entries-forward');
    return WMBHMPush(url);
}

export function WMBHMSetPageTitle(title) {
    var tagTitle = document.querySelector('title');
    if (tagTitle) tagTitle.innerHTML = title;
}

export function WMBHMGetCurrentTitle() {
    var tagTitle = document.querySelector('title');
    return (tagTitle) ? tagTitle.innerText : null;
}

export function WMBHMGetTitleByIndex(index) {
    var entry = WMBHMGetEntryByIndex(index);
    return (entry) ? entry.title : null;
}

export function WMBHMRefresh () {
    window.history.go();
}

export function WMBHMNativeState () {
    return window.history.state ?? {};
}

export function WMBHMNativePush (state, url) {
    window.history.pushState(state,  WMBHMGetCurrentTitle(), url);
}

export function WMBHMNativeNavigate (index) {
    window.history.go(index);
}

function WMBHMGetEntries() {
    var oldEntries = JSON.parse(window.sessionStorage.getItem('wmbhm-entries'));
    return (oldEntries && Array.isArray(oldEntries)) ? oldEntries : [];
}

function WMBHMGetEntriesForward() {
    var oldEntries = JSON.parse(window.sessionStorage.getItem('wmbhm-entries-forward'));
    return (oldEntries && Array.isArray(oldEntries)) ? oldEntries : [];
}

function WMBHMLimitHistorySize(entries, maxSize) {
    if (entries.length > maxSize) entries.shift();
}

function WMBHMGetLastItem(arr) {
    try {
        return arr[arr.length - 1];
    } catch(err) {
        return null;
    }
}

function WMBHMGetEntryByIndex(index) {
    if (index < 0) {
        var entries = WMBHMGetEntries();
        var newIndex = (entries.length -1) - (index * -1);
        return (entries[newIndex]) ? entries[newIndex] : null;
    } else if (index > 0) {
        var entriesForward = WMBHMGetEntriesForward();
        return (entriesForward[index - 1]) ? entriesForward[index - 1] : null;
    }
    return null;
}