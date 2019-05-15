var map = {
    "Int32": "Number",
    "Boolean": "True/False",
    "DateTime": "Date",
    "String": "Text"
};

function addArchive(btn) {
    var row = $(btn).parent().parent();
    var inputField = $(row).find('.archive-name')[0];
    $.get('Admin/InsertArchive/' + btn.dataset.id + "?name=" + inputField.value);
    inputField.setAttribute("disabled", "true");

    //Get all buttons
    var buttons = $(row).find(".btn");

    buttons[0].value = "Edit";
    $(buttons[0]).unbind().bind('click', function (e) {
        e.preventDefault();
        editArchive(this);
    });

    buttons[1].removeAttribute("disabled");

    buttons[2].value = "Delete";
    $(buttons[2]).unbind().bind('click', function (e) {
        e.preventDefault();
        deleteArchive(this);
    });
};

function editArchive(btn) {
    var id = $(btn)[0].dataset.id;
    var inputField = $(btn).parent().parent().find(".archive-name")[0];
    inputField.removeAttribute("disabled");

    $(btn)[0].value = "Save";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        saveArchive(this);
    });
};

function saveArchive(btn) {
    var id = $(btn)[0].dataset.id;
    var inputField = $(btn).parent().parent().find(".archive-name")[0];
    inputField.setAttribute("disabled", "true");
    $.get('Admin/RenameArchive/' + inputField.dataset.id + '?name=' + inputField.value);

    $(btn)[0].value = "Edit";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        editArchive(this);
    });
};

function addArchiveModalRow(btn) {
    var id = $(btn)[0].dataset.id;
    var modalBody = $('.archive-modal-body');
    modalBody.append(buildModalRow(id, 'add'));
    var rows = $(modalBody).find('tr');
    var last = $(rows[rows.length - 1]);
    last.find(".add-archive-column-row").bind('click', function (e) {
        e.preventDefault();
        addColumn(this);
    });
}

function addColumn(btn) {
    var addBtn = $(btn)[0];
    var id = addBtn.dataset.id;
    var row = $(btn).parent().parent();
    var namefield = row.find(".name-archive-column-row");
    var name = namefield.val();
    var typefield = row.find(".type-archive-column-row");
    var type = typefield.children("option:selected").val();
    $.get('Admin/AddColToArchive/' + id + '?colName=' + name + '&colType=' + type).done(function () {
        addBtn.value = 'Delete';
        $(addBtn).unbind().bind('click', function (e) {
            e.preventDefault();
            deleteColumn(btn);
        });
        namefield[0].setAttribute("disabled", "true");
        typefield[0].setAttribute("disabled", "true");
    });
}

function deleteColumn(btn) {
    if (confirm('Are you sure?')) {
        var delBtn = $(btn);
        var id = delBtn[0].dataset.id;
        var row = delBtn.parent().parent();
        var name = row.find(".name-archive-column-row")[0].value;
        $.get('Admin/RemoveColFromArchive/' + id + '?colName=' + name)
            .done(function () {
                row.remove();
            });
    }
}

function closeArchiveModal() {
    $('.archive-modal-body').children('tr').each(function (e) {
        $(e).remove();
    });
}

function buildArchiveModal(btn) {
    var id = $(btn)[0].dataset.id;
    $('.add-archive-column-modal')[0].dataset.id = id;
    var modalBody = $('.archive-modal-body');
    $.get('Admin/GetColumns/' + id, {}, function (data) {
        var json = JSON.parse(data);
        $.each(json, function (index, value) {
            modalBody.append(buildModalRow(id, 'delete'));
            var rows = $(modalBody).find('tr');
            var last = $(rows[rows.length - 1]);
            last.find(".name-archive-column-row").val(value[0]);
            last.find(".type-archive-column-row").append('<option value="' + value[1] + '" selected = "selected">' + map[value[1]] + '</option>');
            last.find(".delete-archive-column-row").bind('click', function (e) {
                e.preventDefault();
                deleteColumn(this);
            });
        });
    });
}

function buildModalRow(id, type) {
    var row = '<tr>';
    if (type == 'add') {
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input style="width:100%" class="input-sm name-archive-column-row" type="text" value="" name="' + id + '" data-id="' + id + '"/>';
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<select style=width:100% class="input-sm type-archive-column-row" data-id="' + id + '" name="' + id + '">'
        row += '<option value="String">Text</option><option value="Int32">Number</option><option value="Boolean">True/False</option><option value="Date">Date</option></select>'
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input type="button" class="btn btn-default add-archive-column-row" data-id="' + id + '" value="Add" />';
        row += '</td>';
    } else {
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input style="width:100%" class="input-sm name-archive-column-row" type="text" value="" name="' + id + '" data-id="' + id + '" disabled />';
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<select style=width:100% class="input-sm type-archive-column-row" data-id="' + id + '" disabled></select>'
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input type="button" class="btn btn-default delete-archive-column-row" data-id="' + id + '" value="Delete" />';
        row += '</td>';
    }
    row += '</tr>';
    return row;
}

function appendArchive(btn, data) {
    var row = '<tr>';
    row += '<td style="vertical-align:middle; width:25%;">';
    row += '<div class="archive-id">' + data + '</div>';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:60%;">';
    row += '<input style="width:100%" class="input-sm archive-name" type="text" value="" name="' + data + '" data-id="' + data + '" data-column="name" data-table="ArchivePermissions" disabled />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default edit-archive" data-id="' + data + '" value="Edit" />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default columns-archive" data-toggle="modal" data-target="#archiveModal" data-backdrop="static" data-keyboard="false" data-id="' + data + '" value="Columns" />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default delete-archive" data-id="' + data + '" value="Delete" />';
    row += '</td>';
    row += '</tr>';

    var body = $('.body-archive');
    $(body).append(row);
    var rows = $(body).find('tr');
    var last = $(rows[rows.length - 1]);

    // Fix input Field
    var fields = $(last).find('input');
    fields[0].value = "";
    fields[0].removeAttribute("disabled");

    // Save button
    fields[1].value = "Add";
    $(fields[1]).unbind().bind('click', function (e) {
        e.preventDefault();
        addArchive(this);
    });

    // Columns button
    fields[2].value = "Columns";
    fields[2].setAttribute('disabled', 'true');
    $(fields[2]).unbind().bind('click', function (e) {
        e.preventDefault();
        buildArchiveModal(this);
    });

    // Cancel button
    fields[3].value = "Cancel";
    $(fields[3]).unbind().bind('click', function (e) {
        e.preventDefault();
        cancel(this);
    });
}

function deleteArchive(btn) {
    if (confirm('Are you sure?')) {
        $.get('Admin/DeleteArchive/' + $(btn)[0].dataset.id)
            .done(function () {
                $(btn).parents("tr").remove();
            });
    }
};

function cancel(btn) {
    if (confirm('Are you sure?')) {
        $(btn).parents("tr").remove();
    }
}

$(document).ready(function () {

    $('.edit-archive').unbind().bind('click', function (e) {
        e.preventDefault();
        editArchive(this);
    });

    $(".delete-archive").unbind().bind('click', function (e) {
        e.preventDefault();
        deleteArchive(this);
    });

    $(".columns-archive").unbind().bind('click', function (e) {
        e.preventDefault();
        buildArchiveModal(this);

    });

    $(".add-archive").unbind().bind('click', function (e) {
        e.preventDefault();
        var btn = $(this);
        $.get('Admin/GetHash', {}, function (data) { appendArchive(btn, data); });
    });

    $(".add-archive-column-modal").unbind().bind('click', function (e) {
        e.preventDefault();
        addArchiveModalRow(this);
    });

    $(".close-archive-modal").unbind().bind('click', function (e) {
        e.preventDefault();
        closeArchiveModal();
    });
});