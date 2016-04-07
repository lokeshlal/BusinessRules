$(document).ready(function () {
    $('[role=navigation]').eq(0).append('<div id="toggleBtn" onclick="toggleControls()" style="cursor: pointer; font-weight: bold; float:right;margin-top: 25px;background-color: lightgray;">hide controls</div>');
});

var helperClick = function () {
    var v = $(activeDivElement).text();
    var textBefore = v.substring(0, currentPosition);
    var textAfter = v.substring(currentPosition, v.length);

    $(activeDivElement).text(textBefore + " " + $(this).attr("data-value") + " " + textAfter);
}

var helperClick1 = function (obj) {
    var v = $(activeDivElement).text();
    var textBefore = v.substring(0, currentPosition);
    var textAfter = v.substring(currentPosition, v.length);

    $(activeDivElement).text(textBefore + " " + $(obj).attr("data-value") + " " + textAfter);
}

$("#accordion button").click(helperClick);

var deleteCondition = function(ele)
{
    var ruleexecutionContainer = $(ele).closest('.ruleexecution').eq(0).remove();
}
var deleteRule = function (ele) {
    var ruleName = $(ele).closest('tr').find('td').eq(0).html();
    $.ajax({
        url: '/api/async/deleterule',
        method: 'POST',
        data: JSON.stringify({ name: ruleName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            if (res == "true") {
                window.location.href = window.location.href;
                return;
            }
            else {
                alert("Error occured. Please check.")
                return;
            }
        }
    });

}

var toggleControls = function () {
    $('#controlsHolder').toggle('toggle');
    if ($('#toggleBtn').html() == 'hide controls') $('#toggleBtn').html('show controls');
    else $('#toggleBtn').html('hide controls');
}

var editRule = function (ele) {
    var ruleName = $(ele).closest('tr').find('td').eq(0).html();

    $.ajax({
        url: '/api/async/GetRuleDefinition',
        method: 'POST',
        data: JSON.stringify({ name: ruleName }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            if (res == "false") {
                alert("Error occured. Please check.")
                return;
            }

            ruleDefinition = JSON.parse(res);

            var factName = ruleDefinition.entityName;

            $.ajax({
                url: '/api/async/GetEntityDefinition',
                method: 'POST',
                data: JSON.stringify({ name: factName }), //"entityName=" +
                cache: false,
                contentType: 'application/json; charset=UTF-8',
                dataType: 'json',
                success: function (res1) {
                    if (res1 == "false") {
                        alert("Error occured. Please check.")
                        return;
                    }

                    factDefinition = JSON.parse(res1);

                    $('#propertiesCollapse').html('');
                    propertySelectElement = '<select class="form-control property">';
                    $.each(factDefinition.fields, function (index, val) {
                        $('#propertiesCollapse').append('<button type="button" onclick="helperClick1(this)" class="btn btn-info" style="display:block; font-size: 12px; margin-bottom: 2px;" data-value-type="' + val.fieldType + '" data-value="' + ruleFact + '.' + val.fieldName + '">' + val.fieldName + '</button>')
                        propertySelectElement += '<option value=' + val.fieldName + '>' + val.fieldName + '</option>';
                    });
                    propertySelectElement += '</select>';

                    $('#ruleNameLbl').html(ruleName);
                    $('#entityName').val(ruleDefinition.entityName);
                    $('#ruleGroup').val(ruleDefinition.ruleGroup);
                    $('#priority').val(ruleDefinition.priority);
                    $('#condition').text(ruleDefinition.ruleCondition);

                    $('.executionContainer').eq(0).text('');

                    $.each(ruleDefinition.ruleExecution, function (index, val) {

                        var propertySelectElement1 = '<select class="form-control property">';
                        $.each(factDefinition.fields, function (index1, val1) {
                            if (val1.fieldName == val.propertyName)
                                propertySelectElement1 += '<option selected value=' + val1.fieldName + '>' + val1.fieldName + '</option>';
                            else
                                propertySelectElement1 += '<option value=' + val1.fieldName + '>' + val1.fieldName + '</option>';
                        });
                        propertySelectElement1 += '</select>';


                        $('.executionContainer').eq(0).append('<div class="ruleexecution"><div class="form-group"><div class="removeExecution" onclick="deleteCondition(this);">Delete</div></div><div class="form-group"><label>Property:</label><div class="propertyContainer">'
                            + propertySelectElement1 + '</div></div><div class="form-group"><label>Order:</label><input class="form-control order" value="' + val.order + '"></div><div contenteditable="true" class="executionDiv" onkeydown="return keydown(event);"  onblur="activeDiv(this);" style="border: 1px solid lightgray; min-height: 75px; margin-bottom: 10px;">'
                            + val.execution + '</div></div>');
                    });

                    $('#ruleDetails').removeClass('hidden');
                }
            });
        }
    });
}

var factDefinition = {};
var ruleDefinition = {};
var propertySelectElement;

$('#btnAddRuleModel').click(function () {
    var ruleName = $('#ruleName').val();
    var ruleFact = $('#ruleFact').val();

    $.ajax({
        url: '/api/async/GetEntityDefinition',
        method: 'POST',
        data: JSON.stringify({ name: ruleFact }), //"entityName=" +
        cache: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        success: function (res) {
            if (res == "false") {
                alert("Error occured. Please check.")
                return;
            }
            factDefinition = JSON.parse(res);

            $('#propertiesCollapse').html('');
            propertySelectElement = '<select class="form-control property">';
            $.each(factDefinition.fields, function (index, val) {
                $('#propertiesCollapse').append('<button type="button" onclick="helperClick1(this)" class="btn btn-info" style="display:block; font-size: 12px; margin-bottom: 2px;" data-value-type="' + val.fieldType + '" data-value="' + ruleFact + '.' + val.fieldName + '">' + val.fieldName + '</button>')
                propertySelectElement += '<option value=' + val.fieldName + '>' + val.fieldName + '</option>';
            });
            propertySelectElement += '</select>';
            $('.propertyContainer').html(propertySelectElement);
            $('#ruleNameLbl').html(ruleName);
            $('#ruleDetails').removeClass('hidden');
            $('#entityName').val(ruleFact);
            $('#new-rule-modal').modal('toggle');
        }
    });
})

$('#btnSaveRule').click(function () {
    var ruleExecutions = [];

    $('.executionContainer').find('.ruleexecution').each(function (index) {
        ruleExecutions.push({
            propertyName: $(this).find('.property').eq(0).val(),
            execution: $(this).find('.executionDiv').eq(0).text(),
            order: $(this).find('.order').eq(0).val()
        });
    });

    $.ajax({
        url: '/api/async/AddUpdateRule',
        method: 'POST',
        contentType: 'application/json; charset=UTF-8',
        dataType: 'json',
        data: JSON.stringify({
            ruleName: $('#ruleNameLbl').html(),
            entityName: $('#entityName').val(),
            ruleGroup: $('#ruleGroup').val(),
            priority: $('#priority').val(),
            ruleCondition: $('#condition').text(),
            ruleExecution: ruleExecutions
        }),
        cache: false,
        success: function (res) {
            if (res == "true") {
                window.location.href = window.location.href;
                return;
            }
            else {
                alert("Error occured. Please check.")
                return;
            }
        }
    })
});

var addExecution = function () {
    $('.executionContainer').append('<div class="ruleexecution"><div class="form-group"><label>Property:</label><div class="propertyContainer">' + propertySelectElement + '</div></div><div class="form-group"><label>Order:</label><input class="form-control order"></div><div contenteditable="true" class="executionDiv" onkeydown="return keydown(event);"  onblur="activeDiv(this);" style="border: 1px solid lightgray; min-height: 75px; margin-bottom: 10px;"></div></div>')
}

var keydown = function (e) {
    if (e.keyCode == 13) return false;
}

var ruleContainer = function () {
    this.containerElement;
    this.init = function () {
        var containerElement = $(document.createElement('div'));
    }
}

var activeDivElement;
var activeDiv = function (ele) {
    activeDivElement = ele;
    currentPosition = getCaretPosition(ele);
}

var appendCommand = function (commandText) {

}

function getCaretPosition(editableDiv) {
    var caretPos = 0,
      sel, range;
    if (window.getSelection) {
        sel = window.getSelection();
        if (sel.rangeCount) {
            range = sel.getRangeAt(0);
            if (range.commonAncestorContainer.parentNode == editableDiv) {
                caretPos = range.endOffset;
            }
        }
    } else if (document.selection && document.selection.createRange) {
        range = document.selection.createRange();
        if (range.parentElement() == editableDiv) {
            var tempEl = document.createElement("span");
            editableDiv.insertBefore(tempEl, editableDiv.firstChild);
            var tempRange = range.duplicate();
            tempRange.moveToElementText(tempEl);
            tempRange.setEndPoint("EndToEnd", range);
            caretPos = tempRange.text.length;
        }
    }
    return caretPos;
}

var currentPosition;
var element;

var update = function (ele) {
    element = ele;
    currentPosition = $(ele).html(getCaretPosition(this));
};