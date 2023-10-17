$(document).ready(function () {

    // Send forretningsplan fra Upload template
    //$('.btnUploadToInvestor').on('click', function () {
    //    var planId = $(this).parents('form').first().attr('data-id');
    //    var investorId = $(this).parents('form').first().find('#investorname').find('option:selected').val();

    //    if (investorId === null || investorId === 0 || !isNumeric(investorId)) {
    //        showModalMessage('BusinessPlanSentError.html', 'Du skal angive en investor.', 'Du mangler at angive den investor, som du ønsker at sende forretningsplanen til.');
    //    }
      
    //    sendForretningsplan(planId, investorId);
    //});


    // Send forretningsplan fra BusinessAngelsOverview template
    //$('.sendplantoinvestor').on('click', function () {
    //    var planId = $(this).parents('form').first().find('#sendplantoinvestor').find('option:selected').val();
    //    var investorId = $(this).parents('form').first().attr('data-id'); 
    //    var besked = $(this).parents('form').first().find('#coachmessage').val();
       
    //    if ((planId === null || planId === 0 || !isNumeric(planId)) && besked === '') {
    //        showModalMessage('BusinessPlanSentError.html', 'Du skal angive en forretningsplan og/eller besked.', 'Du mangler at angive den forretningsplan eller skrive en besked, som du ønsker at sende.');
    // return;	        
    //
    
    //    if (planId === null || planId === '') {
    //        sendBesked(investorId, '', besked, function () {
    //            showModalMessage('BusinessPlanSent.html', 'Din besked er nu sendt.', ' ');
    //        });
    //    } else {
    //        sendForretningsplan(planId, investorId, besked, function () {
    //            showModalMessage('BusinessPlanSent.html', 'Din beskrivelse af din forretning er nu fremsendt', 'Vores coach vil nu kigge dit projekt igennem og vende tilbage til dig.');
    //        });
    //    }
    //});

    // Send message from investor to student
    //$('.sendplantostudent').on('click', function () {
    //    var besked = $('#studentmessage').val();
    //    var studentId = window.currentProjectId;

    //    if (besked !== '') {
    //        sendBesked(studentId, '', besked, function () {
    //            $('#answerpoolplan').modal('hide');
    //            showModalMessage('BusinessPlanSent.html', 'Din besked er nu sendt.', ' ');
    //        });
    //    }
    //});

    $("#sendplantoinvestor").on("click", function () {
        var investorId = $("#sendplantoinvestor").attr('data-id');
        var planId = $("#bussinessplansDisplay").val();
        var pitchId = $("#pitchDisplay").val();
        var besked = $("#sendview #coachmessage").val();

        if ((planId === '' && pitchId === '') && besked === '') {
            showModalMessage('BusinessPlanSentError.html', 'Du skal angive (en forretningsplan og/eller et pitch deck) og en besked.', 'Du mangler at angive en forretningsplan, et pitch deck eller skrive en besked, som du ønsker at sende.');
        }
        if ((planId !== '' && pitchId !== '') && besked === '') {
            showModalMessage('BusinessPlanSentError.html', 'Tilføj en beskrivelse', 'Venligst tilføj en beskrivelse af din forretningsplan og dit pitch deck, for at kunne sende beskeden.');
        }
        else if (((planId === null || planId === '') && (pitchId === null || pitchId === '')) && besked !== '') {
            sendBesked(investorId, '', besked, function () {
                showModalMessage('BusinessPlanSent.html', 'Din besked er nu sendt.', ' ');
            });
        }
        else if ((planId !== '' && pitchId === '') && besked === '') {
            showModalMessage('BusinessPlanSentError.html', 'Tilføj en beskrivelse', 'Venligst tilføj en beskrivelse af din forretningsplan, for at kunne sende beskeden.');
        }
        else if ((planId === '' && pitchId !== '') && besked === '') {
            showModalMessage('BusinessPlanSentError.html', 'Tilføj en beskrivelse', 'Venligst tilføj en beskrivelse af dit pitch, for at kunne sende beskeden.');
        }
        else if ((planId !== '' && pitchId === '') && besked !== '') {
            sendForretningsplan(planId, investorId, "", besked, function () {
                showModalMessage('BusinessPlanSent.html', 'Din beskrivelse af din forretning er nu fremsendt', 'Vores coach vil nu kigge dit projekt igennem og vende tilbage til dig.');
            });
        }
        else if ((planId === '' && pitchId !== '') && besked !== '') {
            sendForretningsplan("", investorId, pitchId, besked, function () {
                showModalMessage('BusinessPlanSent.html', 'Din beskrivelse af dit pitch deck er nu fremsendt', 'Vores coach vil nu kigge dit projekt igennem og vende tilbage til dig.');
            });
        }
        else if ((planId !== '' && pitchId !== '') && besked !== '') {
            sendForretningsplan(planId, investorId, pitchId, besked, function () {
                showModalMessage('BusinessPlanSent.html', 'Din beskrivelse af din  forretning og dit pitch deck er nu fremsendt', 'Vores coach vil nu kigge dit projekt igennem og vende tilbage til dig.');
            });
        }
    });

    $('.downloadBtnInvoice').on('click', function () {
        var invoiceId = $(this).attr('data-id');
        downloadPdfInvoice(invoiceId);
    });

    $('#cvrInfo').on('click', function () {
        var cvr = $("#cvrSettings").val();
        findByCvrNumber(cvr);
    });

    $('.offerButton').on('click', function () {
        var nId = $(this).attr('id');
        var uId = $(this).attr('data-user');
        sendOffer(uId, nId);
    });

    $('.offerButton').on('click', function () {
        var nId = $(this).attr('id');
        var uId = $(this).attr('data-user');
        sendOfferToPartner(uId, nId);
    });

    $('.notify').on('click', function () {
        var pointer = $(this).attr('data-p');

        if (pointer == "True") {
            var partnerId = $(this).attr('data-partnerId');
            var userId = $(this).attr('data-userId');
            sendNotification(partnerId, userId);
        }
    });

    $('.reveal').on('click', function () {
        var pointer = $(this).attr('data-p');

        if (pointer == "True") {
            var partnerId = $(this).attr('data-partnerId');
            var userId = $(this).attr('data-userId');
            requestPromoCode(pointer, partnerId, userId);
        } else {
            requestPromoCode(false, 0, 0);
        }
    });

    $('.actsub').on('click', function () {
        resSub();
    });

    $('.opensend').click(function () {
        var modal = $('#sendview');

        modal.addClass('is-displayed');
        $('body').addClass('scroll-lock');
    });

    $('#getRidOf').click(function () {
        var modal = $('#sendview');

        modal.removeClass('is-displayed');
        $('body').removeClass('scroll-lock');
    });
});



function sendForretningsplan(planId, investorId, pitchId,besked, callback) {
    //startupCentral.requestData('/umbraco/api/beskedapi/SendForretningsPlanToInvestor?planid=' + planId + '&investorId=' + investorId+'&pitchId=' + pitchId + '&besked=' + besked, function (data) {
    startupCentral.postRequestData('/umbraco/api/beskedapi/SendAttachmentsToInvestor',
        {
            planId: planId,
            investorId: investorId,
            besked: besked,
            pitchId: pitchId
        },
        function (data) {
            if (data) {
                if (data.Success) {
                    if (callback) callback();
                }
                else {
                    showModalMessage('BusinessPlanSentError.html', 'Forretningsplanen/pitch deck kunne ikke sendes.', 'Der opstod en fejl i forbindelse med at sende beskeden.');
                }

                $('#sendview').removeClass('is-displayed');
                $('body').removeClass('scroll-lock');
            }
    });

}

function sendBesked(toMemberId, subject, besked, callback) {
    //startupCentral.requestData('/umbraco/api/beskedapi/SendBesked?toMemberId=' + toMemberId + '&emne=' + subject + '&besked=' + besked, function (data) {
    startupCentral.postRequestData('/umbraco/api/beskedapi/SendBesked',
        {toMemberId:toMemberId,
            emne: subject,
            besked: besked,}, 
        function (data) {
            if (data) {
                if (data.Success) {
                    if (callback) callback();
                }
                else {
                    showModalMessage('BusinessPlanSentError.html', 'Beskeden kunne ikke sendes.', 'Der opstod en fejl i forbindelse med at sende beskeden.');
                }

                $('#sendview').removeClass('is-displayed');
                $('body').removeClass('scroll-lock');
            }
        });

}

function downloadPdfInvoice(invoiceId) {
    $.ajax({
        url: '/umbraco/api/UpodiApi/DownloadInvoiceAsync?InvoicesId=' + invoiceId,
        type: 'get',
        success: function (response, status) {
            if (response) {

                var link = document.createElement('a');

                link.href = "data:application/octet-stream;base64," + response.Data;
                link.target = '_blank';
                link.download = invoiceId + '.pdf';
                link.click();

            }
        },
    });
}

function findByCvrNumber(cvr) {
    $.ajax({
        url: '/umbraco/api/Cvr/GetCompanyInfo?name=' + cvr,
        type: 'get',
        success: function (response, status) {
            if (response) {
                if (response.Success) {
                    $("#companyNameSettings").val(response.Data.Name);
                    $("#companyAddressSettings").val(response.Data.Address);
                    $("#companyZipCodeSettings").val(response.Data.Zipcode);
                    $("#companyCitySettings").val(response.Data.City);
                    $("#companyIndustryCodeSettings").val(response.Data.Industrycode);
                    $("#companyIndustryDescSettings").val(response.Data.Industrydesc);
                    $("#companyStartDateSettings").val(response.Data.Startdate);
                    $("#companyEmployeesSettings").val(response.Data.Employees);

                    $('.result').html('2. Gem dine informationer.');
                    console.log(response.Data.Name, response.Data.Address, response.Data.Zipcode, response.Data.City, response.Data.Industrycode, response.Data.Industrydesc, response.Data.Startdate, response.Data.Employees);

                    var btnChangeCVR = document.getElementById('changeCVR');
                    btnChangeCVR.style.display = 'block';
                }
                else {
                    $('.result').html('2. Vi kan ikke finde en virksomhed med dette CVR-nummer. Venligst prøv igen.');
                    btnChangeCVR.style.display = 'none';
                }
            }
        },
    });
}

//extra offer to email.
function sendOffer(userId, nodeId) {
    $.ajax({
        url: '/umbraco/api/MemberApi/SendOfferEmail?userId=' + userId + '&nodeId=' + nodeId,
        type: 'get',
        success: function (response, status) {
            var btnClaimOffer = document.getElementById(nodeId);

            if (response) {
                if (response.Success) {
                    btnClaimOffer.style.display = 'none';
                    $("." + nodeId.toString()).html("Du har succesfuldt anmodt om tilbuddet. Du vil modtage de nødvendige informationer på mail.");
                }
                else {
                    btnClaimOffer.style.display = 'block';
                    //$("." + nodeId.toString()).html("Der opstod et problem, venligst kontakt os.");
                    $("." + nodeId.toString()).html("Siden er ved at blive opdateret. Skriv til kontakt@startupcentral.dk for at modtage dit unikke tilbud");
                }
            }
        },
        error: function (request, status, error) {
            console.log(request);
            console.log(status);
            console.log(error); 
        }
    });
}

function sendOfferToPartner(userId, nodeId) {
        console.log(userId, nodeId)
        $.ajax({
        url: '/umbraco/api/MemberApi/SendOfferEmailToPartner?userId=' + userId + '&nodeId=' + nodeId,
        type: 'get',
        success: function (response, status) {
            var btnClaimOffer = document.getElementById(nodeId);

            if (response) {
                if (response.Success) {
                    btnClaimOffer.style.display = 'none';
                    $("." + nodeId.toString()).html("Du har succesfuldt anmodt om tilbuddet. Du vil modtage de nødvendige informationer på mail.");
                }
                else {
                    btnClaimOffer.style.display = 'block';
                    $("." + nodeId.toString()).html("Der opstod et problem, venligst kontakt os.");
                }
            }
        },
    
            error: function (request, status, error) {
                console.log(request);
                console.log(status);
                console.log(error);
            }
        });
}

function sendNotification(partnerId, userId) {

    $.ajax({
        url: '/umbraco/api/MemberApi/SendNotificationPartner?userId=' + userId + '&partnerId=' + partnerId,
        type: 'get',
        success: function (response, status) {
            if (response) {
                if (response.Success) {
                    console.log("Click logged successful."); 
                    
                }
                else {
                    console.log("There was a problem logging the click."); 
                }
            }
        },
    });

    
}

function requestPromoCode(request, partnerId, userId) {

    $.ajax({
        url: '/umbraco/api/MemberApi/RequestPromoCode?p=' + request +'&userId=' + userId + '&partnerId=' + partnerId,
        type: 'GET',
        success: function (result) {
            if (result.Success) {
                $("#promo-code span").text(result.Data);
                $(".discount-card__promo").addClass("is-activated");

                console.log('Success: ' + result.Message); 
                var copyText = document.getElementById("promo-code");
                var textArea = document.createElement("textarea");
                textArea.value = copyText.textContent;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand("Copy");
                textArea.remove();
                $("#copy-text").show();

                setTimeout(() => {
                    $("#copy-text").hide();
                }, 1500);
            } else {
                //const fail = addNotification(NOTIFICATION_TYPES.DANGER, 'Der opstod en fejl, prøv igen. Kontakt os hvis problemet varer ved.'); //Problem was raised/faced, please contact us.

                console.log("Problem: " + result.Message);

                //setTimeout(() => {
                //    removeNotification(fail);
                //}, 5000);
            }
        },
        beforeSend: function (xhr, settings) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + userToken);
        }
    });
}

function resSub(id) {

    $.ajax({
        url: '/umbraco/api/UpodiApi/ResumeSubscription?id=' + id,
        type: 'POST',
        success: function (result) {
            if (result.Success) {
                $('#resumLeb').hide();
                $('#getRidOf').click();

                console.log('Sucssess: ' + result.Message);
                location.reload();
            } else {
                $("#resumLab").empty();
                $("#resumLab").append(result.Message);

                console.log("Problem: " + result.Message);
            }
        },
        beforeSend: function (xhr, settings) {
            $('#resumLeb').show();
            xhr.setRequestHeader('Authorization', 'Bearer ' + userToken);
        }
    });
}

function showModalMessage(templateName, subject, message, callback) {

    if ($('#' + templateName.toString().replace('.html', '')).length > 0) {
        $('#' + templateName.toString().replace('.html', '')).remove();
    }

    $.ajax({
        url: '/HtmlTemplates/Modals/' + templateName,
        type: 'get',
        success: function (response, status) {
            if (response) {

                if (subject) {
                    response = response.toString().replace('{{subject}}', subject);
                }

                if (message) {
                    response = response.toString().replace('{{message}}', message);
                }


                $(response).appendTo($(document.body));

                if ($('#' + templateName.toString().replace('.html', '')).length > 0) {
  
                    $('#' + templateName.toString().replace('.html', '')).addClass('is-displayed');
                    if (callback) callback();
                }

            }
        }
    });

}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}