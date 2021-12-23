//禁用默认Ctrl+F
document.onkeydown = function (e) {
    var ev = window.event || e;
    var code = ev.keyCode || ev.which;
    if ((ev.ctrlKey) && (ev.keyCode == 70)) {
        ev.preventDefault();
    }
}

//最大化窗口
window.notifyMaximizeWindow = () => {
    window.chrome.webview.postMessage("MaximizeWindow")
};
//退出程序
window.notifyExitApp = () => {
    window.chrome.webview.postMessage("ExitApp")
};

window.ensureActiveRowIntoView = (direction) => {

    let el = document.querySelector(".stocklist-table tbody tr.active-row");
    // let pel = document.querySelector(".stocklist-table-wrap");
    let pel = el.offsetParent;


    const viewPortHeight = pel.clientHeight;
    const offsetTop = el.offsetTop
    const scrollTop = pel.scrollTop
    const top = offsetTop - scrollTop
    if (top >= viewPortHeight) {
        if (direction == "next")
            el.scrollIntoView(false);
        else
            el.scrollIntoView(true);
    }
}