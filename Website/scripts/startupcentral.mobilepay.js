var mobilePay = {
    socketList: [],
    getSocketUrl: function (channelName, elementId) {
        return 'wss://www.startupcentral.dk/umbraco/api/' + channelName + '/get?memberkey=' + elementId;
    },
    getBaseUrl: function () {
        return 'https://www.startupcentral.dk';
    },
    socketOpen: function (event) {
        var socketUri = event.currentTarget.url.split('?')[0];
        if (mobilePay.debugMode) console.log('Channel opened to:', socketUri);
        mobilePay.openChannelList.push(socketUri);
        window.setTimeout(function () {
            if (mobilePay.subscribeQueue) {
                for (var i = 0; i < mobilePay.subscribeQueue.length; i++) {

                    if (mobilePay.subscribeQueue[i].uri.split('?')[0] === socketUri) {
                        mobilePay.subscribeToElement(mobilePay.subscribeQueue[i].channelName, mobilePay.subscribeQueue[i].elementId, mobilePay.subscribeQueue[i].socketMessageCallback);
                    }
                }
            }
        }, 50);
    },
    socketError: function (event) {
        if (mobilePay.debugMode) console.log('Socket error', event);
    },
    socketMessage: function (event) {

        console.log('socketMessage:', event);
        if (event) {
            if (event.data) {
                var data = JSON.parse(event.data);
                if (data) {

                    if (data.status === 'redirect') {

                        if (data.link !== null && data.links.length > 0) {
                            window.location.href = data.links[0].href;
                        }

                        //$('.second').slideDown();
                    }
                    else {
                        $('.third').slideDown();
                    }
                }
            }
        }
    },
    channelList: [],
    openChannelList: [],
    subscribeQueue: [],
    getChannel: function (uri) {
        if (uri) {

            for (var i = 0; i < window.mobilePay.socketList.length; i++) {
                if (window.mobilePay.socketList[i].url.startsWith(uri)) {
                    return window.mobilePay.socketList[i];
                }
            }
        }
        return null;
    },
    isChannelOpen: function (uri) {

        var shortUri = uri.split('?')[0];

        if ($.inArray(shortUri, mobilePay.openChannelList) >= 0) {
            return true;
        }
        return false;
    },
    serviceRequest: function (uri, callback) {
      
        return $.ajax({
            url: this.getBaseUrl() + uri,
            type: 'get',
            success: function (response, status) {
                if (response) {
                    if (response.Data) {
                        if (callback) callback(response.Data);
                    }
                    else {
                        if (callback) callback(null);
                    }
                }
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Content-Type', 'application/json'); }
        });

    },

};

$(document).ready(function () {

    var channelName = 'MobilePayApi';
    var elementId = $('#memberKey').val();
    var url = window.mobilePay.getSocketUrl(channelName, elementId);
    var socket = new WebSocket(url);
    socket.onerror = mobilePay.socketError;
    socket.onopen = mobilePay.socketOpen;
    socket.callbacks = [];
    socket.onmessage = mobilePay.socketMessage;
    socket.onclose = mobilePay.socketClose;
    window.mobilePay.socketList.push(socket);
    window.mobilePay.channelList.push(window.mobilePay.getSocketUrl(channelName, elementId).split('?')[0]);

    window.setTimeout(function () {
        mobilePay.serviceRequest('/umbraco/api/MobilePayApi/RequestStatus?memberkey=' + $('#memberKey').val(), null);
    }, 1000);


});