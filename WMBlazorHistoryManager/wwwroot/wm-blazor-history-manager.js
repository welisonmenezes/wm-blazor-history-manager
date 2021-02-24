export function WMBHMPush(url) {
    var entries = getEntries();
    var newEntry = { url: url, title: WMBHMGetCurrentTitle() };
    if ((entries.length < 1) || (entries[entries.length - 1] && entries[entries.length - 1].url !== url)) {
        entries.push(newEntry);
        window.sessionStorage.setItem('wmbhm-entries', JSON.stringify(entries));
    }
    console.log(entries)
    return entries.length - 1;
}

export function WMBHMNavigate(index) {
    var entries = getEntries();
    return (entries[index]) ? entries[index].url : null;
}

export function WMBHMTotalIndex() {
    var entries = getEntries();
    return (entries.length > 0) ? (entries.length - 1) : 0;
}

export function WMBHMClear(url) {
    window.sessionStorage.removeItem('wmbhm-entries');
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
    var entries = getEntries();
    return (entries[index]) ? entries[index].title : null;
}

export function WMBHMGetUrlByIndex(index) {
    var entries = getEntries();
    return (entries[index]) ? entries[index].url : null;
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

function getEntries() {
    var oldEntries = JSON.parse(window.sessionStorage.getItem('wmbhm-entries'));
    return (oldEntries && Array.isArray(oldEntries)) ? oldEntries : [];
}