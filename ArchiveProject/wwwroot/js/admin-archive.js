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

function buildArchiveModal(btn) {
    var id = $(btn)[0].dataset.id;
    var modalBody = $('.archive-modal-body');

    modalBody.append(row);
}

function buildModalRow(id, type) {
    var row = '<tr>';
    if (type == 'add') {
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input style="width:100%" class="input-sm name-modal" type="text" value="" name="' + id + '" data-id="' + id + '"/>';
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<select style=width:100% class="input-sm type-modal" data-id="' + id + '" name="' + id + '">'
        row += '<option value="String">Text</option><option value="Int32">Number</option><option value="Boolean">True/False</option><option value="Date">Date</option></select>'
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input type="button" class="btn btn-default add-modal" data-id="' + id + '" value="Add" />';
        row += '</td>';
    } else {
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input style="width:100%" class="input-sm name-modal" type="text" value="" name="' + id + '" data-id="' + id + '" disabled />';
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<select style=width:100% class="input-sm type-modal" data-id="' + id + '" disabled></select>'
        row += '</td>';
        row += '<td style="vertical-align:middle; width:33%;">';
        row += '<input type="button" class="btn btn-default delete-modal" data-id="' + id + '" value="Delete" />';
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
    row += '<input type="button" class="btn btn-default columns-archive" data-id="' + data + '" value="Columns" />';
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
});