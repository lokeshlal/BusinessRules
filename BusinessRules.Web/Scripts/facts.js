$(document).load(function () {

});

var isInEditMode = false;
var editTr;

$('#btnAddNewField').click(function () {
    //$('#factName').val('');
    $('#new-field-modal').modal('show');
    resetPopupFields();
});

$('#btnAddFactModel').click(function () {
    $('#new-fact-modal').modal('hide');
    renderFactGrid();
});

var editFieldLink = function (ele) {

    isInEditMode = true;
    editTr = $(ele).closest('tr');
    $('#fieldName').val($(ele).closest('tr').find('td').eq(0).html());
    $('#fieldType').val($(ele).closest('tr').find('td').eq(1).html());
    $('#new-field-modal').modal('show');
}

var deleteFact = function (ele) {
    var factName = $(ele).closest('tr').find('td').eq(0).html();
    $.ajax({
        url: '/api/async/deleteentity',
        method: 'POST',
        data: JSON.stringify({ name: factName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            
        }
    });

}

var deleteFieldLink = function (ele) {
    $(ele).closest('tr').remove();
}

var editFact = function (ele) {
    var factName = $(ele).closest('tr').find('td').eq(0).html();

    $('#factFields').html('');
    $('#factDetails').show();


    $('#fieldType').val('');

    $('#factNameLbl').html(factName);

    $.ajax({
        url: '/api/async/GetEntityDefinition',
        method: 'POST',
        data: JSON.stringify({ name: factName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            var factDefinition = JSON.parse(res);

            $.each(factDefinition.fields, function (index, val) {
                $('#factFields').append('<tr><td>' + val.fieldName + '</td><td>' + val.fieldType
                   + '</td><td><button type="button" class="btn btn-link" onclick="editFieldLink(this);">Edit</button>'
                   + '<button type="button" class="btn btn-link" onclick="deleteFieldLink(this);">Delete</button></td></tr>');
            });

            $('#factDetails').removeClass('hidden');
        }
    });

}

var renderFactGrid = function () {
    //clear fields
    $('#factFields').html('');
    $('#factDetails').show();


    $('#fieldType').val('');
    $('#factNameLbl').html($('#factName').val());

    $('#factDetails').removeClass('hidden');
}

$('#btnAddFieldModel').click(function () {
    var fieldName = $('#fieldName').val();
    var fieldType = $('#fieldType').val();

    // ToDo: validations
    // if fail return

    if (isInEditMode) {
        editTr.replaceWith('<tr><td>' + fieldName + '</td><td>' + fieldType
            + '</td><td><button type="button" class="btn btn-link" onclick="editFieldLink(this);">Edit</button>'
            + '<button type="button" class="btn btn-link" onclick="deleteFieldLink(this);">Delete</button></td></tr>')
    }
    else {
        $('#factFields').append('<tr><td>' + fieldName + '</td><td>' + fieldType
            + '</td><td><button type="button" class="btn btn-link" onclick="editFieldLink(this);">Edit</button>'
            + '<button type="button" class="btn btn-link" onclick="deleteFieldLink(this);">Delete</button></td></tr>');
    }

    $('#new-field-modal').modal('toggle');
    resetPopupFields();
    isInEditMode = false;
});

var resetPopupFields = function () {
    $('#fieldName').val('');
    $('#fieldType').val('');
}

$('#btnSaveFact').click(function () {
    var factFields = $('#factFields');
    var fieldsJson = [];
    factFields.find('tr').each(function (index) {
        fieldsJson.push({
            fieldName: $(this).find('td').eq(0).text(),
            fieldType: $(this).find('td').eq(1).text()
        });
    });
    //$('#new-field-modal').modal('toggle');
    console.log(fieldsJson);

    $.ajax({
        url: '/api/async/addupdatefact',
        method: 'POST',
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        data: JSON.stringify({ factName: $('#factNameLbl').html(), fields: fieldsJson }),
        cache: false,
        success: function (res) {
            if (res == "true") {
                alert('done');
            }
        }
    })
});

$('#btnCancelFact').click(function () {
    var factFields = $('#factFields').html('');
    $('#factDetails').hide();
});