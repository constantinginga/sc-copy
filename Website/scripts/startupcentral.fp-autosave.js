
$(document).ready(function () {
    window.setInterval(function () {
        saveCurrentPage();
    }, (1000 * 60 * 2)); // Every 2 min. 

    console.log('Click event added to elements: ', $('#autosave, .wizardmenupkt').length);
    $('#autosave, .wizardmenupkt').off('click').on('click', function () {

        console.log('AutoSave or WizardMenuPunkt clicked');

        var form = $(this).parents('form').first();
        if ($(form).attr('data-action') === 'submit') {

            console.log('Form submitting');

            $('<input type="hidden" name="noredirect" value="true" />').appendTo($(form));
            
            $(form).submit();

        }
        else
        {

            console.log('saveCurrentPage()');

            saveCurrentPage();
        }

    });

});

function getFormValues() {

    $.each(formFields, function(index, obj) {
        obj.Value = $("#" + obj.FormFieldName).val();
    });

    return JSON.stringify(formFields);
}

function saveCurrentPage() {



    var draftSaveButtonText = $('#autosave').html();
    $('#autosave').html('Auto gemmer...');

    console.log('POST /umbraco/api/forretningsplanapi/autosave');

    $.ajax({
        type: 'POST',
        async: false,
        url: '/umbraco/api/forretningsplanapi/autosave?id='+ planId +'&values=',
        data: getFormValues(),
        success: function (response, status) {
            if (response) {
                console.log(response);
                $('#autosave').html(draftSaveButtonText);
                $('#submitbsp').html(draftSaveButtonText);

                $('#submitbsp').show();
                $('a#sendingBlock').hide();

            }
        }
    });


    //$.get('/umbraco/api/forretningsplanapi/autosave?values=' + getFormValues(), function (response) {
    //    console.log(response);
    //    $('#autosave').html(draftSaveButtonText);
    //});
}

