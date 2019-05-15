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

        $.get('../../UpdateDbValue/'
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

function addRow(btn) {
    $.get('../../CreateDbRow/' + btn.dataset.table, {}, function (data) {
        // Set field name to new id
        var row = $(btn).parents("tr");
        var fields = $(row).find(".field-value");
        var i;
        for (i = 0; i < fields.length; i++) {
            fields[i].setAttribute("name", data);
            fields[i].dataset.id = data;
        }

        // Set id
        var editButton = $(row).find(".edit-row");
        editButton.dataset.id = data;

        // Set id & correct eventlistener
        var deleteButton = $(row).find(".delete-row");
        deleteButton.dataset.id = data;
        deleteButton.value = "Delete";
        $(buttons[1]).unbind().bind('click', function (e) {
            e.preventDefault();
            deleteRow(this);
        });
        save(editButton);
    });
};

function deleteRow(btn) {
    if (confirm('Are you sure?')) {
        $.get('../../DeleteDbRow/' + $(btn)[0].dataset.id + '?' + 'table=' + $(btn)[0].dataset.table)
            .done(function () {
                $(btn).parents("tr").remove();
            });
    }
};

function buildTableTR(tableHash, rowData, newRow) {
    var row = '<tr>';
    var disabled;
    if (newRow) {
        disabled = "";
        row += '<td><input type="button" class="btn btn-default save-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Save" /></td>';
    }
    else {
        disabled = "disabled";
        row += '<td><input type="button" class="btn btn-default edit-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Edit" /></td>';
    }
    row += '<td><input type="button" class="btn btn-default delete-row" data-id="' + rowData[0][0] + '" data-table="' + tableHash + '" value="Delete" /></td>';

    for (j = 1; j < tableData[i].length; j++) {
        var type = map[rowData[i][2]];
        row += '<input class="input-sm field-value" type="' + type + '" value="' + rowData[i][0] + '" name="' + rowData[0][0] + '" data-id="' + rowData[0][0] + '" data-column="' + rowData[i][1] + '" data-table="' + tableHash + '" ' + disabled + ' />';
    }
    row += '</tr>';
    return row;
}

function buildTableHead(tableData) {
    var head = "";
    var i;
    head += '<tr><th></th><th></th>';
    for (i = 1; i < tableData[0].length; i++) {
        head += '<th>' + tableData[0][i][1] + '</th>';
    }
    head += '</tr>';
    return head;  
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
    $('.add-row')[0].dataset.id = id;
    var head = $('.table-data-head');
    var body = $('.table-data-body');
    $.get('Admin/GetColumns/' + id, {}, function (data) {
        var json = JSON.parse(data);
        head.append(buildTableHead(json));
        body.append(buildTableBody(id, json));
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
}

function cleanTable() {
    $('.table-data-head').children('tr').each(function (e) { $(this).remove(); });
    $('.table-data-body').children('tr').each(function (e) { $(this).remove(); });
}

$(document).ready(function () {
    $('#archive_selector').unbind().bind('change', function (e) {
        e.preventDefault();
        var selectedArchive = $(this).children("option:selected").val();
        cleanTable();
        buildTable(selectedArchive);
    });
});

