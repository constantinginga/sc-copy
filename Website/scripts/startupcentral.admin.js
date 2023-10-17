
var creating = false;
var hideTimer = null;

function createMember(callback) {

    if (creating == true) return;
    creating = true;

    window.clearTimeout(hideTimer);

    $.ajax({
        url: '/umbraco/api/memberapi/create',
        type: 'post',
        data: new FormData($('#createuser')[0]),
        contentType: false,
        cache: false,
        processData: false,
        traditional: true,
        enctype: 'multipart/form-data',
        success: function (response, status) {
            if (response) {

                if (response.Success) {
                    $(".successtxt").show();
                }
                else {
                    $(".errortxt").html(response.Message)
                    $(".errortxt").show();
                    hideTimer = window.setTimeout(function () {
                        $(".errortxt").hide();
                    }, 10000);

                }

            }

            creating = false;

            if (callback) callback();
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
    });

}

function createFagomraade(callback) {

    if (creating == true) return;
    creating = true;

    window.clearTimeout(hideTimer);

    $.ajax({
        url: '/umbraco/api/fagomraaderapi/create',
        type: 'post',
        data: new FormData($('#opretfagomraadeform')[0]),
        contentType: false,
        cache: false,
        processData: false,
        traditional: true,
        enctype: 'multipart/form-data',
        success: function (response, status) {
            if (response) {

                if (response.Success) {
                    $(".successtxt").show();
                }
                else {
                    $(".errortxt").html(response.Message)
                    $(".errortxt").show();
                    hideTimer = window.setTimeout(function () {
                        $(".errortxt").hide();
                    }, 10000);

                }

            }

            creating = false;

            if (callback) callback();
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
    });

}

function saveFagomraade(callback) {

    if (creating == true) return;
    creating = true;

    window.clearTimeout(hideTimer);

    $.ajax({
        url: '/umbraco/api/fagomraaderapi/update?id=' + $('input[data-invoke=editfagomraade]').attr('data-id') + '&name=' + $('input#editfagomraade').val(),
        type: 'get',
        success: function (response, status) {
            if (response) {

                if (response.Success) {
                    $(".successtxt").show();
                }
                else {
                    $(".errortxt").html(response.Message)
                    $(".errortxt").show();
                    hideTimer = window.setTimeout(function () {
                        $(".errortxt").hide();
                    }, 10000);

                }

            }

            creating = false;

            if (callback) callback();
        },
        beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
    });

}




$(document).ready(function () {

    /* Members */
    $('input[data-invoke=createuser]').off('click').on('click', function () {

        $(".errortxt").hide();
        $(".successtxt").hide();

        window.clearTimeout(hideTimer);

        createMember(function () {
            console.log('');
        });
    });

    $('input[data-invoke=deaktiveruser]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deaktiveruser]').is(':checked')) {

            var dataId = $(this).attr('data-id');

            $.ajax({
                url: '/umbraco/api/MemberApi/Deactivate?memberId=' + dataId,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxt').show();

                            $('div[data-type=member][data-id=' + dataId + ']').find('#aktiverbruger').show();
                            $('div[data-type=member][data-id=' + dataId + ']').find('#deaktiverbruger').hide();

                            $('div[data-type=member][data-id=' + dataId + ']').find('div[data-type=membername]').css('text-decoration', 'line-through');
                            $('div[data-type=member][data-id=' + dataId + ']').find('div[data-type=memberemail]').css('text-decoration', 'line-through');
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deaktiveruser]').text() + '", før du kan deaktivere brugeren.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });

    $('.aktiverbruger').off('click').on('click', function () {

        $('.errortxt').hide();

        var dataId = $(this).attr('data-id');

        $.ajax({
            url: '/umbraco/api/MemberApi/activate?memberId=' + dataId,
            type: 'get',
            success: function (response, status) {
                if (response) {
                    console.log(response);
                    if (response.Success) {
                        $('.successtxt').show();

                        $('div[data-type=member][data-id=' + dataId + ']').find('#aktiverbruger').hide();
                        $('div[data-type=member][data-id=' + dataId + ']').find('#deaktiverbruger').show();

                        $('div[data-type=member][data-id=' + dataId + ']').find('div[data-type=membername]').css('text-decoration', 'none');
                        $('div[data-type=member][data-id=' + dataId + ']').find('div[data-type=memberemail]').css('text-decoration', 'none');


                    }
                }
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
        });


    });

    $('input[data-invoke=deleteuser]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deleteuser]').is(':checked')) {

            $.ajax({
                url: '/umbraco/api/MemberApi/delete?memberId=' + startupCentral.currentMember.Id,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxt').show();
                            window.setTimeout(function () {
                                $('button.close').trigger('click');
                                $('div[data-type=member][data-id=' + startupCentral.currentMember.Id + ']').slideUp();
                            }, 1500);
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deleteuser]').parent().text() + '", før du kan slette brugeren.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });


    /* Diverse */
    $(".pdf-download-link").on('click', function () {

        window.open('/pdf?n=' + $(this).attr('data-id'));

        //$.ajax({
        //    url: '/umbraco/api/forretningsplanapi/SetDownloaded?documentid=' + $(this).attr('data-id'),
        //    type: 'get',
        //    success: function (response, status) {
        //        if (response) {
        //        }
        //    },
        //    beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
        //});

    });

    $('form#edituser').submit(function () {

        $('.saveusererrortxt').hide();

        if ($('#editusername').val() === '' || $('#editusername').val().length < 3) {
            $('.saveusererrortxt').html('Navn skal udfyldes.');
            $('#editusername').focus();
            $('.saveusererrortxt').show();
            return false;
        }

        if ($('#edituserusername').val() === '' || !startupCentral.validation.email($('#edituserusername').val())) {
            $('.saveusererrortxt').html('E-mail skal udfyldes og være korrekt.');
            $('#edituserusername').focus();
            $('.saveusererrortxt').show();
            return false;
        }

        if ($('#changepasswordone').val() !== $('#changepasswordrepeat').val()) {
            $('.saveusererrortxt').html('De to angivede adgangskoder skal være ens.');
            $('#changepasswordone').focus();
            $('.saveusererrortxt').show();
            return false;
        }

        if ($('#changepasswordone').val().length > 0 && $('#changepasswordone').val().length < 6) {
            $('#changepasswordone').focus();
            $('.saveusererrortxt').html('Adgangskoden skal minimum være 6 karakterer.');
            $('.saveusererrortxt').show();
            return false;
        }

        return true;

    });

    /* Partner / Samarbejdspartnere */
    $('input[data-invoke=deaktiverpartner]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deaktiverpartner]').is(':checked')) {

            var dataId = $(this).attr('data-id');

            $.ajax({
                url: '/umbraco/api/SamarbejdspartnerApi/Deactivate?nodeId=' + dataId,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxt').show();

                            $('div[data-type=partner][data-id=' + dataId + ']').find('#aktiverpartner').show();
                            $('div[data-type=partner][data-id=' + dataId + ']').find('#deaktiverpartner').hide();

                            $('div[data-type=partner][data-id=' + dataId + ']').find('.activetoggle').addClass('deaktiveret');
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deaktiverpartner]').text() + '", før du kan deaktivere samarbejdspartneren.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });

    $('.aktiverpartner').off('click').on('click', function () {

        $('.errortxt').hide();

        var dataId = $(this).attr('data-id');

        $.ajax({
            url: '/umbraco/api/SamarbejdspartnerApi/activate?nodeId=' + dataId,
            type: 'get',
            success: function (response, status) {
                if (response) {
                    console.log(response);
                    if (response.Success) {
                        $('.successtxt').show();

                        $('div[data-type=partner][data-id=' + dataId + ']').find('#aktiverpartner').hide();
                        $('div[data-type=partner][data-id=' + dataId + ']').find('#deaktiverpartner').show();

                        $('div[data-type=partner][data-id=' + dataId + ']').find('.activetoggle').removeClass('deaktiveret');


                    }
                }
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
        });


    });

    $('input[data-invoke=deletepartner]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deletepartner]').is(':checked')) {

            $.ajax({
                url: '/umbraco/api/samarbejdspartnerapi/delete?nodeId=' + startupCentral.currentPartner.Id,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxtpartnerdelete').show();
                            window.setTimeout(function () {
                                $('button.close').trigger('click');
                                $('div[data-type=partner][data-id=' + startupCentral.currentPartner.Id + ']').slideUp();
                            }, 1500);
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deletepartner]').parent().text() + '", før du kan slette samarbejdspartneren.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });

    /* Video Templates */
    /* Video Templates */
    $('input[data-invoke=deaktivervideo]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deaktivervideo]').is(':checked')) {

            var dataId = $(this).attr('data-id');

            $.ajax({
                url: '/umbraco/api/VideoApi/Deactivate?nodeId=' + dataId,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successvideotxt').show();

                            $('div[data-type=video][data-id=' + dataId + ']').find('#aktivervideo').show();
                            $('div[data-type=video][data-id=' + dataId + ']').find('#deaktivervideo').hide();

                            $('div[data-type=video][data-id=' + dataId + ']').find('.activetoggle').addClass('deaktiveret');
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deaktivervideo]').text() + '", før du kan deaktivere videoen.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });

    $('.aktivervideo').off('click').on('click', function () {

        $('.errortxt').hide();

        var dataId = $(this).attr('data-id');

        $.ajax({
            url: '/umbraco/api/VideoApi/activate?nodeId=' + dataId,
            type: 'get',
            success: function (response, status) {
                if (response) {
                    console.log(response);
                    if (response.Success) {
                        $('.successtxt').show();

                        $('div[data-type=video][data-id=' + dataId + ']').find('#aktivervideo').hide();
                        $('div[data-type=video][data-id=' + dataId + ']').find('#deaktivervideo').show();

                        $('div[data-type=video][data-id=' + dataId + ']').find('.activetoggle').removeClass('deaktiveret');


                    }
                }
            },
            beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
        });


    });

    $('input[data-invoke=deletevideo]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);

        if ($('input[data-confirm=deletevideo]').is(':checked')) {

            $.ajax({
                url: '/umbraco/api/VideoApi/delete?nodeId=' + startupCentral.currentVideo.Id,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxtvideodelete').show();
                            window.setTimeout(function () {
                                $('button.close').trigger('click');
                                $('div[data-type=video][data-id=' + startupCentral.currentVideo.Id + ']').slideUp();
                            }, 1500);
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deletevideo]').parent().text() + '", før du kan slette videoen.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }

    });

    // Fagområder
    $('input[data-invoke=createfagomraade]').off('click').on('click', function () {

        $(".errortxt").hide();
        $(".successtxt").hide();

        window.clearTimeout(hideTimer);

        createFagomraade(function () {
            location.reload();
        });
    });

    $('input[data-invoke=editfagomraade]').off('click').on('click', function () {

        saveFagomraade(function () {
            location.reload();
        });

    });

    $('input[data-invoke=deletefagomraade]').off('click').on('click', function () {

        $('.errortxt').hide();
        window.clearTimeout(hideTimer);
        if ($('input[data-confirm=deletefagomraade]').is(':checked')) {

            $.ajax({
                url: '/umbraco/api/fagomraaderapi/delete?id=' + startupCentral.currentFagomraade.Id,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxtpartnerdelete').show();
                            window.setTimeout(function () {
                                $('button.close').trigger('click');
                                $('div[data-type=fagomraade][data-id=' + startupCentral.currentFagomraade.Id + ']').slideUp();
                            }, 1500);
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });
        }
        else {
            $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=deletefagomraade]').parent().first().text() + '", før du kan slette fagområdet.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);
        }


    });

    $('input[data-invoke=unsubscribeuser]').off('click').on('click', function () {
        if ($('input[data-confirm=unsubscribeuser]').is(':checked')) {
            var id = $('input[data-invoke=unsubscribeuser]').attr('data-id')
            $.ajax({
                url: '/umbraco/api/UpodiApi/UnsubscribeUser?UserId=' + id,
                type: 'get',
                success: function (response, status) {
                    if (response) {
                        console.log(response);
                        if (response.Success) {
                            $('.successtxtpartnerdelete').show();
                            window.setTimeout(function () {
                                $('button.close').trigger('click');
                                $('div[data-type=fagomraade][data-id=' + startupCentral.currentFagomraade.Id + ']').slideUp();
                                location = location;
                            }, 1500);
                        }
                    }
                },
                beforeSend: function (xhr, settings) { xhr.setRequestHeader('Authorization', 'Bearer ' + userToken); }
            });

        } else {
           /* $('.errortxt').html('Du skal afkrydse "' + $('input[data-confirm=unsubscribeuser]').parent().first().text() + '", før du kan slette fagområdet.').show();
            hideTimer = window.setTimeout(function () {
                $('.errortxt').hide();
            }, 7000);*/
        }
    })
});