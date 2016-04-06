var deleteConstant = function (ele) {
    var constantName = $(ele).closest('tr').find('td').eq(0).html();
    $.ajax({
        url: '/api/async/deleteconstant',
        method: 'POST',
        data: JSON.stringify({ name: constantName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            if (res == "true") {
                // refresh screen
            }
        }
    });
}

var addNewConstant = function () {
    $('#constantName').val('');
    $('#constantValue').val('');
    $('#constantType').val('');

    $('#constantDetails').removeClass('hidden');
}

var editConstant = function (ele) {
    var constantName = $(ele).closest('tr').find('td').eq(0).html();

    $.ajax({
        url: '/api/async/getconstantdefinition',
        method: 'POST',
        data: JSON.stringify({ name: constantName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            
            var constantDefinition = JSON.parse(res);
            $('#constantName').val(constantDefinition.constantName);
            $('#constantValue').val(constantDefinition.constantValue);
            $('#constantType').val(constantDefinition.constantTypeStr);

            $('#constantDetails').removeClass('hidden');
        }
    });
}

$('#btnCancelConstant').click(function () {
    $('#constantName').val('');
    $('#constantValue').val('');
    $('#constantType').val('');

    $('#constantDetails').addClass('hidden');
})

$('#btnSaveConstant').click(function () {
    var constantName = $('#constantName').val();
    var constantValue = $('#constantValue').val();
    var constantTypeStr = $('#constantType').val();

    $.ajax({
        url: '/api/async/addupdateconstant',
        method: 'POST',
        data: JSON.stringify({ constantName: constantName, constantValue: constantValue, constantTypeStr: constantTypeStr }),
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            if (res == "true") {
                //refresh screen

                $('#constantDetails').addClass('hidden');

            }
        }
    });
})