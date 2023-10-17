


/* Cookiebar*/
(function ($) {
    $.cookieBar = function (options, val) {
        if (options == 'cookies') {
            var doReturn = 'cookies';
        } else if (options == 'set') {
            var doReturn = 'set';
        } else {
            var doReturn = false;
        }
        var defaults = {
            message: 'Vi anvender cookies.', //Message displayed on bar
            imageUrl: '/media/1150/missingagnel.png', //Image
            acceptButton: true, //Set to true to show accept/enable button
            acceptText: 'OK', //Text on accept/enable button
            declineButton: false, //Set to true to show decline/disable button
            declineText: 'Disable Cookies', //Text on decline/disable button
            policyButton: true, //Set to true to show Privacy Policy button
            policyText: 'Om cookies', //Text on Privacy Policy button
            policyURL: '/cookies/', //URL of Privacy Policy
            autoEnable: true, //Set to true for cookies to be accepted automatically. Banner still shows
            expireDays: 365, //Number of days for cookieBar cookie to be stored for
            forceShow: false, //Force cookieBar to show regardless of user cookie preference
            effect: 'slide', //Options: slide, fade, hide
            element: 'body', //Element to append/prepend cookieBar to. Remember "." for class or "#" for id.
            append: false, //Set to true for cookieBar HTML to be placed at base of website. Actual position may change according to CSS
            fixed: true, //Set to true to add the class "fixed" to the cookie bar. Default CSS should fix the position
            zindex: '', //Can be set in CSS, although some may prefer to set here
            redirect: String(window.location.href), //Current location
            domain: String(window.location.hostname) //Location of privacy policy
        }
        var options = $.extend(defaults, options);

        //Sets expiration date for cookie
        var expireDate = new Date();
        expireDate.setTime(expireDate.getTime() + (options.expireDays * 24 * 60 * 60 * 1000));
        expireDate = expireDate.toGMTString();

        var cookieEntry = 'cb-enabled={value}; expires=' + expireDate + '; path=/'

        //Retrieves current cookie preference
        var i, cookieValue = '',
            aCookie, aCookies = document.cookie.split('; ');
        for (i = 0; i < aCookies.length; i++) {
            aCookie = aCookies[i].split('=');
            if (aCookie[0] == 'cb-enabled') {
                cookieValue = aCookie[1];
            }
        }
        //Sets up default cookie preference if not already set
        if (cookieValue == '' && options.autoEnable) {
            cookieValue = 'enabled';
            document.cookie = cookieEntry.replace('{value}', 'enabled');
        }
        if (doReturn == 'cookies') {
            //Returns true if cookies are enabled, false otherwise
            if (cookieValue == 'enabled' || cookieValue == 'accepted') {
                return true;
            } else {
                return false;
            }
        } else if (doReturn == 'set' && (val == 'accepted' || val == 'declined')) {
            //Sets value of cookie to 'accepted' or 'declined'
            document.cookie = cookieEntry.replace('{value}', val);
            if (val == 'accepted') {
                return true;
            } else {
                return false;
            }
        } else {
            //Sets up enable/accept button if required
            var message = options.message.replace('{policy_url}', options.policyURL);

            if (options.acceptButton) {
                var acceptButton = '<a href="" class="cb-enable cookie button">' + options.acceptText + '</a>';
            } else {
                var acceptButton = '';
            }
            //Sets up disable/decline button if required
            if (options.declineButton) {
                var declineButton = '<button href="" class="cb-disable">' + options.declineText + '</button>';
            } else {
                var declineButton = '';
            }
            //Sets up privacy policy button if required
            if (options.policyButton) {
                var policyButton = '<a href="' + options.policyURL + '" class="cb-policy">' + options.policyText + '</a>';
            } else {
                var policyButton = '';
            }
            //Whether to add "fixed" class to cookie bar
            if (options.fixed) {
                var fixed = ' class="fixed"';
            } else {
                var fixed = '';
            }
            if (options.zindex != '') {
                var zindex = ' style="z-index:' + options.zindex + ';"';
            } else {
                var zindex = '';
            }

            //Displays the cookie bar if arguments met
            if (options.forceShow || cookieValue == 'enabled' || cookieValue == '') {
                if (options.append) {
                    $(options.element).append('<div id="cookie-bar"' + fixed + zindex + '>' + '<div class="text">' + '<img src="' + defaults.imageUrl + '" alt="">' + ' <p> ' + message + ' </p><div> '+ acceptButton + declineButton + policyButton + '</div ></div ></div> ');
                } else {
                    $(options.element).prepend('<div id="cookie-bar"' + fixed + zindex + '>' + '<div class="text">' + '<img src="' + defaults.imageUrl + '" alt="">' + ' <p> ' + message + ' </p><div> ' + acceptButton + declineButton + policyButton + '</div ></div ></div> ');
                }
            }

            //Sets the cookie preference to accepted if enable/accept button pressed
            $('#cookie-bar .cb-enable').click(function () {
                document.cookie = cookieEntry.replace('{value}', 'accepted');
                if (cookieValue != 'enabled' && cookieValue != 'accepted') {
                    window.location = options.currentLocation;
                } else {
                    if (options.effect == 'slide') {
                        $('#cookie-bar').slideUp(300, function () {
                            $('#cookie-bar').remove()
                        });
                    } else if (options.effect == 'fade') {
                        $('#cookie-bar').fadeOut(300, function () {
                            $('#cookie-bar').remove()
                        });
                    } else {
                        $('#cookie-bar').hide(0, function () {
                            $('#cookie-bar').remove()
                        });
                    }
                    return false;
                }
            });
            //Sets the cookie preference to declined if disable/decline button pressed
            $('#cookie-bar .cb-disable').click(function () {
                var deleteDate = new Date();
                deleteDate.setTime(deleteDate.getTime() - (864000000));
                deleteDate = deleteDate.toGMTString();
                aCookies = document.cookie.split('; ');
                for (i = 0; i < aCookies.length; i++) {
                    aCookie = aCookies[i].split('=');
                    if (aCookie[0].indexOf('_') >= 0) {
                        document.cookie = aCookie[0] + '=0; expires=' + deleteDate + '; domain=' + options.domain.replace('www', '') + '; path=/';
                    } else {
                        document.cookie = aCookie[0] + '=0; expires=' + deleteDate + '; path=/';
                    }
                }
                document.cookie = cookieEntry.replace('{value}', 'declined');
                if (cookieValue == 'enabled' && cookieValue != 'accepted') {
                    window.location = options.currentLocation;
                } else {
                    if (options.effect == 'slide') {
                        $('#cookie-bar').slideUp(300, function () {
                            $('#cookie-bar').remove()
                        });
                    } else if (options.effect == 'fade') {
                        $('#cookie-bar').fadeOut(300, function () {
                            $('#cookie-bar').remove()
                        });
                    } else {
                        $('#cookie-bar').hide(0, function () {
                            $('#cookie-bar').remove()
                        });
                    }
                    return false;
                }
            });
        }
    }
})(jQuery);

$(document).ready(function () {
    $.cookieBar();
    $("#tools").click(function () {
        $(".submenu").fadeToggle();
    });
    $("#showbudget").click(function () {
        $("#budget").slideToggle("fast");
    });
    $("#andetchk").click(function () {
        $("#showandet").slideToggle("fast");
    });
});


$(document).ready(function () {


    $("a.barsbutton img").click(function () {
        $(".popup__overlay").fadeIn();
        $("#popupmainmenu").fadeIn();
        $(".popmainmenuclose").fadeIn();
    });

    $(".popmainmenuclose").click(function () {
        $(".popup__overlay").fadeOut();
        $("#popupmainmenu").fadeOut();
        $(".popmainmenuclose").fadeOut();
    });

    $(".popup__overlay--full").click(function () {
        $(".popup__overlay").fadeOut();
        $(".popmainmenuclose").fadeOut();
        $("#popupmainmenu").fadeOut();
    });

});

$("img.seclevel").click(function () {
    $(this).next().toggle('fast');
    if ($('ul.sub-menu:visible').length > 1) {
        $('ul.sub-menu:visible').hide();
        $(this).next().toggle('fast');
    }

});

$(document).ready(function () {
    var text_max = 200;
    $('#charNum').html(text_max + '');
    $('#idedesc').keyup(function () {
        var text_length = $('#idedesc').val().length;
        var text_remaining = text_max - text_length;
        $('#charNum').html(text_remaining + '');
    });
});

$(document).ready(function () {
    var text_max1 = 200;
    $('#charNum1').html(text_max1 + '');
    $('#indledning').keyup(function () {
        var text_length1 = $('#indledning').val().length;
        var text_remaining1 = text_max1 - text_length1;
        $('#charNum1').html(text_remaining1 + '');
    });
});

$(document).ready(function () {
    var text_max2 = 300;
    $('#charNum2').html(text_max2 + '');
    $('#shorttxt').keyup(function () {
        var text_length2 = $('#shorttxt').val().length;
        var text_remaining2 = text_max2 - text_length2;
        $('#charNum2').html(text_remaining2 + '');
    });
});



$(document).ready(function () {
    $('ul#leftmenu li').click(function () {
        var tab_id = $(this).attr('data-tab');

        $('ul#leftmenu li').removeClass('current');
        $('.tab-content').removeClass('current');

        $(this).addClass('current');
        $("#" + tab_id).addClass('current');
    })


    $('.nexttab').click(function () {

        var tab_id = $(this).attr('data-tab');

        $('.tab-content').removeClass('active');
        $('.navmen').removeClass('active');

        $("." + tab_id).addClass('active');
        $("#" + tab_id).addClass('active');
    })

    var editingMember = null;
    var currentProject = '';
    var currentProjectId =

        $(".admenubtn").click(function () {
            $(".admenu, .admenupool").fadeOut();

            $("#edituser").find('input[type=text]').addClass('inputfield');
            $("#edituser").find('input[type=email]').addClass('inputfield');

            window.currentProject = $(this).attr('data-planname');
            window.currentProjectId = parseInt($(this).attr('data-id'));

            window.startupCentral.getMemberById($(this).attr('data-id'), function (member) {

                if (member !== null) {
                    editingMember = member;
                    $('input[data-invoke=deaktiveruser]').attr('data-id', member.Id);
                    $('input[data-invoke=unsubscribeuser]').attr('data-id', member.Id);
                    $('.lblEditingMemberName').html(member.Name);

                    $('.lblProjectName').html(window.currentProject);

                    if (member.ContentTypeAlias != 'student') {
                        $('div[data-type=mobile-field]').hide();
                    }
                    else {
                        $('div[data-type=mobile-field]').show();
                    }

                    $('#editusername').val(member.Name);
                    $('#edituserusername').val(member.Username);
                    $('#editusermobil').val(member.Mobile);
                    $('#editusermemberkey').val(member.Key);

                    $('#edituser').find('.badge').hide();
                    switch (editingMember.ContentTypeAlias) {
                        case 'student':
                            $('#edituser').find('.studenterblue').show();
                            $('.lblMemberType').html('Student');
                            break;
                        case 'coach1':
                            $('#edituser').find('.coachdark').show();
                            $('#edituser').find('#extracoachinvestor').show();
                            $('.lblMemberType').html('Coach');
                            break;
                        case 'investor':
                            $('#edituser').find('.investordusted').show();
                            $('#edituser').find('#extracoachinvestor').show();
                            $('.lblMemberType').html('Investor');
                            break;
                        case 'teacher':
                            $('#edituser').find('.teacheryellow').show();
                            $('.lblMemberType').html('L�rer');
                            break;
                        case 'admin':
                            $('#edituser').find('.adminred').show();
                            $('.lblMemberType').html('Administrator');
                            break;
                    }

                    if (editingMember.Tags !== null) {
                        for (var i = 0; i < editingMember.Tags.length; i++) {
                            $('#edituser').find('#f_' + editingMember.Tags[i]).attr('checked', 'checked');
                        }
                    }

                    $('#edituser').find('textarea[name=shorttxt]').val(editingMember.ShortText);
                    $('#edituser').find('textarea[name=desc]').val(editingMember.Description);
                    if (editingMember.Unavailable == true) {
                        $('#edituser').find('input[name=unavailable]').attr('checked', 'checked');

                        if (editingMember.DateFrom) {
                            $('#edituser').find('input[name=datefrom]').val(moment(editingMember.DateFrom).format('YYYY-MM-DD'));
                        }
                        if (editingMember.DateTo) {
                            $('#edituser').find('input[name=dateto]').val(moment(editingMember.DateTo).format('YYYY-MM-DD'));
                        }

                        $('.daterange').show();


                    }

                    if (editingMember.NDAUrl) {
                        $('#edituser').find('a[name=currentNDA]').attr('href', editingMember.NDAUrl);
                        $('#edituser').find('a[name=currentNDA]').show();
                    }

                    if (editingMember.CVUrl) {
                        $('#edituser').find('a[name=currentCV]').attr('href', editingMember.CVUrl);
                        $('#edituser').find('a[name=currentCV]').show();
                    }

                    if (editingMember.AvatarImageUrl) {
                        $('img[name=profileImage]').attr('src', editingMember.AvatarImageUrl);
                    }


                    //$('#edituser').find('#shorttxt').val(editingMember.ShortText);


                    console.log(editingMember);


                }

            });


            $(this).next(".admenu, .admenupool").fadeIn();
        });


    $('.admenupartner').on('click', function () {
        window.startupCentral.getPartnerById($(this).parents('div[data-type=partner]').attr('data-id'), function (data) {
            console.log(data);

            $('input[data-invoke=deaktiverpartner]').attr('data-id', data.Id);
            $('.lblEditingPartnerName').html(data.Name);
        });
    });

    $('.admenuvideo').on('click', function () {
        window.startupCentral.getVideoById($(this).parents('div[data-type=video]').attr('data-id'), function (data) {
            console.log(data);

            $('input[data-invoke=deaktivervideo]').attr('data-id', data.Id);
            $('.lblEditingVideoName').html(data.Name);
        });
    });

    $('.admenufagom').on('click', function () {
        window.startupCentral.getFagomraadeById($(this).parents('div[data-type=fagomraade]').attr('data-id'), function (data) {
            console.log(data);
            if (data) {
                $('input[data-invoke=editfagomraade]').attr('data-id', data.Id);
                $('.lblEditingName').html(data.Name);
                $('input#editfagomraade').val(data.Name);
            }

        });
    });

    $(".closearrow").click(function () {
        $(".admenu, .admenupool").fadeOut();
    });

    $(".changepassword").click(function () {
        $("#changepassword").toggle();
        $(this).find('i').toggleClass('fa-arrow-circle-o-down fa-arrow-circle-o-up');
    });

    $("#unavailable_1").change(function () {
        $(".daterange").toggle();
    });

    $("#sendmessagebtn").click(function () {
        $("#sendmessageform").show();
        $(this).hide();
    });

    /*
    $('.messagebox.active #Accept').click(function () {
        var messageBox = $('.messagebox.active');

        console.log("Is checked: " +$(this,messageBox).is(':checked'));
        console.log($(this,messageBox).html());

        $(this,messageBox).prop('checked', $(this,messageBox).is(':checked'));
     
        if ($(this,messageBox).is(':checked')) {

            $('#Accept',messageBox).prop('checked', false);
            $('#Afvisning',messageBox).prop('checked', true);
        }else{
            if($("#Accept",messageBox).is(':checked')){
                $('#Afvisning',messageBox).prop('checked', true);
                $('#Accept',messageBox).prop('checked', false);
            }
            else{
                $('#Afvisning',messageBox).prop('checked', false);
            }
            
        }

    });*/

    /*
        $('#Accept').change(function () {
            var messageBox = $('.messagebox.active');
    
            if ($(this,messageBox).is(':checked')) {
                $('#Afvisning',messageBox).prop('checked', false);
                $('#Accept',messageBox).prop('checked', true);
            }else{
                if($("#Afvisning",messageBox).is(':checked')){
                    $('#Accept',messageBox).prop('checked', true);
                    $('#Afvisning',messageBox).prop('checked', false);
                }
                else{
                    $('#Accept',messageBox).prop('checked', false);
                }
            }
            
        });*/

});

$(document).ready(function () {
    $("img.seclevel").click(function () {
        $(this).parent().next().toggle('fast');
        //alert($(this).next().html());
        if ($('ul.dropdown-menu:visible').length > 1) {
            $('ul.dropdown-menu:visible').hide();
            $(this).next().toggle('fast');
        }
    });
});

$(document).ready(function () {
    $(".messagebox").click(function (e) {
        var currentConversationId = $(this).attr('data-conversationid');

        $(".messagebox").each(function (idx, obj) {
            if ($(obj).attr('data-conversationid') !== currentConversationId) {
                $(obj).hide();
            }
        });

        //$(".messagebox").hide();
        if (!$(this).is(':visible')) {
            $(this).show();
        }
        //$(this).find("div.shadowbox").css({"display": "block", "padding-top": "20px"});

        $(".teasermessage", this).hide();
        $(".fullBodyText", this).show();

        $(this).find(".showallmessage").show();
        $(this).find(".showfullmessagesline").show();
        $("a#showmessages").show();
        $(this).find(".talkname").show();
        $('.messagebox').removeClass('active');
        $(this).addClass('active');

        var dataId = $(this).attr('data-id');
        if (dataId) {
            startupCentral.messages.markAsRead(dataId);
        }

        $('.messagebox.active #btnSendMessageTo').val('Send beskeden til ' + $(this).attr('data-name'));
        $('#frmFromCoach').show();

        checkAllowed();

    });



    $("#showmessages").click(function () {
        $(this).hide();

        var messageBox = $('.messagebox.active');
        $(".teasermessage", messageBox).show();
        $(".fullBodyText", messageBox).hide();

        $(".messagebox").show();
        $(".showallmessage").hide();
        $(".showfullmessagesline").hide();
        $(".teasermessage").show();
        $(".talkname").hide();
        //$("div.shadowbox").css({"display": "flex", "padding-top": "8px"});

        $(".newsmessagewrapper").hide();
        $(".newmessageform").show();
        //$('#frmFromCoach').hide();
        checkAllowed();


    });

    $(".newmessagebtn").click(function () {
        $(".newmessageform").toggle();

        if ($('.messagebox.active').attr('data-allowed') === 'True') {
            //console.log('Allowed');

            $('[data-react="notallowedtosend"]').hide();
            $('[data-react="allowedtosend"]').show();
        }
        else {

            $('[data-react="notallowedtosend"]').show();
            $('[data-react="allowedtosend"]').hide();

            //console.log('Not allowed');
        }


    });
});

$(document).ready(function () {
    $(".wizradbtn").click(function () {
        $(".wizardmenumobile").slideToggle("slow");
    });

    if ($.isFunction($('').owlCarousel)) {
        $('.coaches-carousel').owlCarousel({
            responsiveClass: true,
            items: 1,
            loop: true,
            autoHeight: true,
            autoplay: true,
            autoplayHoverPause: true,
            nav: false,
            stagePadding: 0,
            responsive: {
                0: {
                    items: 1
                },
                540: {
                    items: 2
                },
                1024: {
                    items: 3
                }
            }
        });
    }

});

function checkAllowed() {
    if ($('.messagebox.active').attr('data-allowed').toLowerCase() === 'true') {
        //console.log('Allowed');

        $('[data-react="notallowedtosend"]').hide();
        $('[data-react="allowedtosend"]').show();
    }
    else {

        $('[data-react="notallowedtosend"]').show();
        $('[data-react="allowedtosend"]').hide();

        //console.log('Not allowed');
    }
}