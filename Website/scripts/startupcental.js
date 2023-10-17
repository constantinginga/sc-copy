var startupCentral = {
    currentMember: {},
    currentPartner: {},
    currentVideo: {},
    currentFagomraade: {},
    getMemberById: function (memberId, callback) {

        if (memberId) {
            startupCentral.requestData('/umbraco/api/memberapi/GetMemberById?memberid=' + memberId, function (data) {
                startupCentral.currentMember = data;
                if (callback) callback(data);
            });
        }
    },
    getPartnerById: function (nodeId, callback) {
        startupCentral.requestData('/umbraco/api/samarbejdspartnerapi/getbyid?nodeid=' + nodeId, function (data) {
            startupCentral.currentPartner = data;
            if (callback) callback(data);
        });
    },
    getVideoById: function (nodeId, callback) {
        startupCentral.requestData('/umbraco/api/videoapi/getbyid?nodeid=' + nodeId, function (data) {
            startupCentral.currentVideo = data;
            if (callback) callback(data);
        });
    },
    getFagomraadeById: function (nodeId, callback) {
        startupCentral.requestData('/umbraco/api/fagomraaderapi/getbyid?id=' + nodeId, function (data) {
            startupCentral.currentFagomraade = data;
            if (callback) callback(data);
        });
    },
    postRequestDataUpload: function (uri, data, callback) {
        if (userToken) {
            return $.ajax({
                url: uri,
                type: 'post',
                contentType: false,
                processData: false, 
                data: data,
                success: function (response, status) {
                    if (response) {
                        if (callback) callback(response);
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });
        }
        else {
            return $.post(uri, data, function (response) {
                if (response) {
                    if (response.Data) {
                        if (callback) callback(response.Data);
                    }
                }
            });
        }
    },
    postRequestData: function (uri, data, callback) {
        if (userToken) {
            return $.ajax({
                url: uri,
                type: 'post',
                data: JSON.stringify(data),
                success: function (response, status) {
                    if (response) {
                        if (callback) callback(response);
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken).setRequestHeader('Content-Type', 'application/json'); }
            });
        }
        else {
            return $.post(uri, JSON.stringify(data), function (response) {
                if (response) {
                    if (response.Data) {
                        if (callback) callback(response.Data);
                    }
                }
            });
        }
    },
    requestData: function (uri, callback) {
        if (userToken) {
            return $.ajax({
                url: uri,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        if (callback) callback(response);
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken).setRequestHeader('Content-Type', 'application/json'); }
            });
        }
        else {

            return $.get(uri, function (response) {
                if (response) {
                    if (response.Data) {
                        if (callback) callback(response.Data);
                    }
                }
            });
        }

    },
    validation: {
        email: function (email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        },
        mobileNo: function (mobileNo) {
            var no = mobileNo.toString().trim();

            if (no.startsWith("+45")) {
                no = no.replace("+45", '');
            }

            no = no.replace('+', '').replace('-', '').replace('(mobil)', '');
            no = no.replace(" ", '');

            if (no.length >= 8) {
                if (isNumeric(no)) {
                    return true;
                }
            }

            return false;
        },
        isNumeric: function (n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }
    },
    groupMessages: {
        getUnreadMessages: function (callback) {
            if (userToken) {
                startupCentral.requestData('/umbraco/api/beskedapi/anyunreadGroupMessages', function (data) {
                    if (callback) callback(data);
                });
            }
        },
        updateUnreadMessages: function () {
            startupCentral.groupMessages.getUnreadMessages(function (data) {
                console.log('Checkin to see if there are unread group messages');

                $('#badgeUnreadGroupMessages').html('');

                if (data !== null) {
                    if (data.Data !== null) {
                        if (data.Data.length > 0) {
                            $('#divbadgeUnreadGroupMessages').css("display", "grid");
                            $('#badgeUnreadGroupMessages').html(data.Data);
                        }
                        else {
                            $('#divbadgeUnreadGroupMessages').css("display", "none");
                        }
                    }
                }
            });
        }
    },
    messages: {
        send: function (toMemberId, subject, message, callback) {
            startupCentral.postRequestData('/umbraco/api/beskedapi/SendBesked',
                {
                    toMemberId: toMemberId,
                    emne: subject,
                    besked: message,
                }, function (data) {
                    if (callback) callback(data);
                })
            /*startupCentral.requestData('/umbraco/api/beskedapi/SendBesked?toMemberId=' + toMemberId + '&emne=' + subject + '&besked=' + message, function (data) {
                if (callback) callback(data);
            });*/
        },
        getUnreadMessages: function (callback) {
            if (userToken) {
                startupCentral.requestData('/umbraco/api/beskedapi/anyunreadmessages', function (data) {
                    if (callback) callback(data);
                });
            }
        },
        updateUnreadMessages: function () {
            startupCentral.messages.getUnreadMessages(function (data) {
                console.log('Checkin to see if there are unread messages');

                $('#badgeUnreadMessages').html('');

                if (data !== null) {
                    if (data.Data !== null) {
                        if (data.Data.length > 0) {
                            $('#divbadgeUnreadMessages').css("display", "grid");
                            $('#badgeUnreadMessages').html(data.Data.length);

                            //if ($(location).attr('pathname') == "/login/besked-til-student/") {
                            //    GetRecipients();
                            //}
                        }
                        else {
                            $('#divbadgeUnreadMessages').css("display", "none");
                        }
                    }
                }
            });
        },
        markAsRead: function (fromMemberId) {
            startupCentral.requestData('/umbraco/api/beskedapi/markread?fromMemberId=' + fromMemberId, function (data) {
                startupCentral.messages.updateUnreadMessages();
            });
        }
    }
}

$(document).ready(function () {

    window.setInterval(function () {
        startupCentral.messages.updateUnreadMessages();
        startupCentral.groupMessages.updateUnreadMessages();
    }, 19000);

    startupCentral.messages.updateUnreadMessages();

    startupCentral.groupMessages.updateUnreadMessages();

});