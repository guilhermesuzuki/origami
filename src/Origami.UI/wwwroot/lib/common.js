window.sleep = ms => {
    return new Promise(resolve => setTimeout(resolve, ms));
};

window.assignDotNetHelper = (element, dotNetHelper) => {
    if (element != null) {
        element.dotNetHelper = dotNetHelper;
    }
};

window.emojiClick = (id) => {
    document.getElementById(id)
        .addEventListener('emoji-click', event => {
            console.log(event.detail);
            var emoji = document.getElementById(id);
            if (emoji.dotNetHelper) {
                emoji.dotNetHelper.invokeMethodAsync('Reaction', event.detail.unicode);
            }
        });
};

window.addQueryStringWithoutReload = function (keyToAdd, value) {
    const url = new URL(window.location.href);
    url.searchParams.set(keyToAdd, value);
    window.history.replaceState({}, '', url);
};

window.removeQueryStringWithoutReload = function (keyToRemove) {
    const url = new URL(window.location.href);
    url.searchParams.delete(keyToRemove);
    window.history.replaceState({}, '', url);
};

var origami = {
    common: {
        //activates prism
        prism: () => {
            Prism.highlightAll();
        },

        //lazy images loading
        lazy: () => {
            sleep(350).then(x => { $(function () { $('.lazy').lazy(); }); });
        },

        //downloads a file from url
        downloadFileFromUrl : (url) => {
            var anchor = document.createElement('a');
            anchor.href = url;
            anchor.download = '';
            anchor.click();
        },

        title: (title) => document.title = title,
    },
    specialpages: {
        view: (id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/specialpages/${id}?url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
    },
    physicalpages: {
        view: (id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/physicalpages/${id}?url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
        viewByPath: (path) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/physicalpages/bypath/?path=${path}&url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
        viewByContent: (path, type, id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/physicalpages/bycontent/?path=${path}&type=${type}&id=${id}&url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
    },
    pages: {
        view: (id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/pages/${id}?url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
    },
    posts: {
        view: (id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/posts/${id}?url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view); 
        },
    },
    videos: {
        view: (id) => {
            var url = encodeURIComponent(location.href);
            var referrer = encodeURIComponent(document.referrer);
            var view = `/views/videos/${id}?url=${url}&referrer=${referrer}&v=${Math.random()}`;
            $.get(view);
        },
    },
    editor: {
        add: (url, name, size, type) => {

            var __tinymce = __tinymce;
            if (!__tinymce) __tinymce = window.parent.tinymce;

            // Get the active TinyMCE editor instance
            var editor = __tinymce.activeEditor;

            if (editor) {

                var selectedElement = editor.selection.getNode();
                if (selectedElement) {

                    switch (selectedElement.tagName) {
                        case "IMG":
                        case "VIDEO":
                        case "AUDIO":
                            var modifiedElement = selectedElement.cloneNode(true);
                            modifiedElement.src = url;
                            modifiedElement.title = name;
                            modifiedElement.alt = name;
                            editor.dom.setOuterHTML(selectedElement, modifiedElement.outerHTML);
                            editor.windowManager.close();
                            break;
                        case "A":
                            var modifiedElement = selectedElement.cloneNode(true);
                            modifiedElement.href = url;
                            modifiedElement.title = name;
                            modifiedElement.innerHTML = name + " (" + size + ")";
                            editor.dom.setOuterHTML(selectedElement, modifiedElement.outerHTML);
                            editor.windowManager.close();
                            break;
                        default:
                            switch (type) {
                                case "image":
                                    editor.insertContent('<img src="' + url + '" title="' + name + '" alt="' + name +'" style="width:100%;" />');
                                    break;
                                case "video":
                                    editor.insertContent('<video controls src="' + url + '" style="width:100%;" />');
                                    break;
                                case "audio":
                                    editor.insertContent('<audio controls src="' + url + '" />');
                                    break;
                                default:
                                    editor.insertContent('<a href="' + url + '" target="_blank" rel="noopener">' + name + ' (' + size + ')</a>');
                                    break;
                            }
                            
                            break;
                    }
                }
            } else {
                console.log("No active editor found");
            }
        },
    },
};

function OkToCookies() {
    $.cookie("cookie-consent", 1, { path: '/', expires: 180 });
    $(document).find(".cookie-consent-wrapper").remove();
}

window.geoLocation = {
    getCurrentPosition: function () {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject("Geolocation is not supported by this browser.");
                return;
            }
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    console.log(pos);
                    resolve({
                        latitude: pos.coords.latitude,
                        longitude: pos.coords.longitude,
                        accuracy: pos.coords.accuracy
                    });
                },
                (err) => reject(err.message)
            );
        });
    }
};

window.prepareHTMLForGLightBox = () => {
    var img = $(".content-html img");
    img.each(function () {
        var element = $(this);
        element.addClass("glightbox");
        element.addClass("cursor-pointer");
    });
    GLightbox({
        touchNavigation: true,
        loop: true,
        autoplayVideos: true,
    });
}

$.cookie.raw = true;
