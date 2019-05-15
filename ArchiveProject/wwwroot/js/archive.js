var map = {
    "Int32": "number",
    "Boolean": "checkbox",
    "DateTime": "date",
    "String": "text"
};

function editRow(btn) {
    var id = $(btn)[0].dataset.id;
    var fields = document.getElementsByName(id);
    for (i = 0; i < fields.length; i++) { fields[i].removeAttribute("disabled"); }
    $(btn)[0].value = "Save";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        saveRow(this);
    });
};

function saveRow(btn) {
    var id = $(btn)[0].dataset.id;
    var fields = document.getElementsByName(id);

    var i;
    for (i = 0; i < fields.length; i++) {
        fields[i].setAttribute("disabled", "true");
        var inputvalue;
        if (fields[i].type == "checkbox") {
            inputvalue = fields[i].checked;
        } else {
            inputvalue = fields[i].value;
        }

        $.get('Archive/UpdateDbValue/'
            + fields[i].dataset.id + '?'
            + 'column=' + fields[i].dataset.column + '&'
            + 'table=' + fields[i].dataset.table + '&'
            + 'value=' + inputvalue).done(function () { });
    }

    $(btn)[0].value = "Edit";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        editRow(this);
    });
};

function addRow() {
    var tableHash = $('.add-row')[0].dataset.id;
    $.get('Archive/CreateDbRow/' + tableHash, {}, function (data) {
        var rowId = data;
        $.get('Archive/GetArchiveHeader/' + tableHash, {}, function (data) {
            var json = JSON.parse(data);
            $('.table-data-body').append(buildNewTableTR(tableHash, rowId, json));

            $('.save-row').unbind().bind('click', function (e) {
                e.preventDefault();
                saveRow(this);
            });
            $('.edit-row').unbind().bind('click', function (e) {
                e.preventDefault();
                editRow(this);
            });
            $('.delete-row').unbind().bind('click', function (e) {
                e.preventDefault();
                deleteRow(this);
            });
        });
    });
};

function deleteRow(btn) {
    if (confirm('Are you sure?')) {
        $.get('Archive/DeleteDbRow/' + $(btn)[0].dataset.id + '?' + 'table=' + $(btn)[0].dataset.table)
            .done(function () {
                $(btn).parents("tr").remove();
            });
    }
};

function buildNewTableTR(tableHash, rowId, rowData) {
    var row = '<tr>';
    row += '<td><input type="button" style="vertical-align:middle;" class="btn btn-default save-row" data-id="' + rowId + '" data-table="' + tableHash + '" value="Save" /></td>';
    row += '<td><input type="button" style="vertical-align:middle;" class="btn btn-default delete-row" data-id="' + rowId + '" data-table="' + tableHash + '" value="Delete" /></td>';
    var i;
    for (i = 1; i < rowData.length; i++) {
        var type = map[rowData[i][1]];
        row += '<td><input style="vertical-align:middle;" class="form-control field-value" type="' + type + '" name="' + rowId + '" data-id="' + rowId + '" data-column="' + rowData[i][0] + '" data-table="' + tableHash + '" /></td>';
    }
    row += '</tr>';
    return row;
}

function buildTableTR(tableHash, rowData, newRow) {
    var row = '<tr>';
    var disabled;
    if (newRow) {
        disabled = "";
        row += '<td><input type="button" style="vertical-align:middle;" class="btn btn-default save-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Save" /></td>';
    }
    else {
        disabled = "disabled";
        row += '<td><input type="button" style="vertical-align:middle;" class="btn btn-default edit-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Edit" /></td>';
    }
    row += '<td><input type="button" style="vertical-align:middle;" class="btn btn-default delete-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Delete" /></td>';
    var i;
    for (i = 1; i < rowData.length; i++) {
        var type = map[rowData[i][2]];
        if (type == "text" || type == "number") {
            row += '<td><input style="vertical-align:middle;" class="form-control field-value" type="' + type + '" value="' + rowData[i][0] + '" name="' + rowData[0][0] + '" data-id="' + rowData[0][0] + '" data-column="' + rowData[i][1] + '" data-table="' + tableHash + '" ' + disabled + ' /></td>';
        }
        else if (type == "checkbox") {
            var checked = '';
            if (rowData[i][0]) { checked = 'checked'; }
            row += '<td><input style="vertical-align:middle;" class="form-control field-value" type="' + type + '" name="' + rowData[0][0] + '" data-id="' + rowData[0][0] + '" data-column="' + rowData[i][1] + '" data-table="' + tableHash + '" ' + disabled + ' ' + checked + ' /></td>';
        }
        else if (type == "date") {
            var date = ""
            if (rowData[i][0] != null) {
                date = rowData[i][0].substring(0, 10);
            } 
            row += '<td><input style="vertical-align:middle;" class="form-control field-value" type="' + type + '" value="' + date + '" name="' + rowData[0][0] + '" data-id="' + rowData[0][0] + '" data-column="' + rowData[i][1] + '" data-table="' + tableHash + '" ' + disabled + ' /></td>';
        }
    }
    row += '</tr>';
    return row;
}

function buildTableHead(tableHash) {
    $.get('Archive/GetArchiveHeader/' + tableHash, {}, function (data) {
        var json = JSON.parse(data);
        var head = "";
        var i;
        head += '<tr><th></th><th></th>';
        for (i = 1; i < json.length; i++) {
            head += '<th>' + json[i][0] + '</th>';
        }
        head += '</tr>';
        $('.table-data-head').append(head);
    });
}

function buildTableBody(tableHash, tableData) {
    var body = "";
    var i;
    for (i = 0; i < tableData.length; i++) {
        body += buildTableTR(tableHash, tableData[i], false);
    }  
    return body;  
}

function buildTable(id) {
    $.get('Archive/GetArchive/' + id, {}, function (data) {
        var json = JSON.parse(data);
        buildTableHead(id);
        $('.table-data-body').append(buildTableBody(id, json));
        $('.table-data-foot').append('<tr><td colspan="' + (json[0].length + 2) +'"><input type="button" style="width:100%" class="btn btn-default add-row" value="Add Row" /></td></tr>');
        $('.add-row')[0].dataset.id = id;
        $('.save-row').unbind().bind('click', function (e) {
            e.preventDefault();
            saveRow(this);
        });
        $('.edit-row').unbind().bind('click', function (e) {
            e.preventDefault();
            editRow(this);
        });
        $('.delete-row').unbind().bind('click', function (e) {
            e.preventDefault();
            deleteRow(this);
        });
        $('.add-row').unbind().bind('click', function (e) {
            e.preventDefault();
            addRow();
        });
    });
}

function cleanTable() {
    $('.table-data-head').children('tr').each(function (e) { $(this).remove(); });
    $('.table-data-body').children('tr').each(function (e) { $(this).remove(); });
    $('.table-data-foot').children('tr').each(function (e) { $(this).remove(); });
}

$(document).ready(function () {
    $('#archive_selector').unbind().bind('change', function (e) {
        e.preventDefault();
        var selectedArchive = $(this).children("option:selected").val();
        cleanTable();

        if (!(selectedArchive == "Choose Archive")) {
            buildTable(selectedArchive);
        }
    });
});

