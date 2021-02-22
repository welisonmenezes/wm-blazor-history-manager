export function WMBHMPush(url) {
    var entries = getEntries();
    var newEntry = { url: url };

    if ((entries.length < 1) || (entries[entries.length - 1] && entries[entries.length - 1].url !== url)) {
        entries.push(newEntry);
        window.sessionStorage.setItem('wmbhm-entries', JSON.stringify(entries));
    }
    console.log('New Entry: ', entries);
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


function getEntries() {
    var oldEntries = JSON.parse(window.sessionStorage.getItem('wmbhm-entries'));
    return (oldEntries && Array.isArray(oldEntries)) ? oldEntries : [];
}