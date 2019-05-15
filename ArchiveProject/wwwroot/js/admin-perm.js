function editPerm(btn) {
    var id = $(btn)[0].dataset.id;
    var inputField = $(btn).parent().parent().find(".name-perm")[0];
    inputField.removeAttribute("disabled");

    $(btn)[0].value = "Save";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        savePerm(this);
    });
};

function savePerm(btn) {
    var id = $(btn)[0].dataset.id;
    var inputField = $(btn).parent().parent().find(".name-perm")[0];
    inputField.setAttribute("disabled", "true");
    $.get('Admin/RenamePermission/' + inputField.dataset.id + '?name=' + inputField.value);

    $(btn)[0].value = "Edit";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        editPerm(this);
    });
};

function addPerm(btn) {
    var row = $(btn).parent().parent();
    var inputField = $(row).find('.name-perm')[0];
    $.get('Admin/InsertPerm/' + btn.dataset.id + "?name=" + inputField.value);
    inputField.setAttribute("disabled", "true");

    //Get all buttons
    var buttons = $(row).find(".btn");

    buttons[0].value = "Edit";
    $(buttons[0]).unbind().bind('click', function (e) {
        e.preventDefault();
        editPerm(this);
    });

    buttons[1].removeAttribute("disabled");

    buttons[2].value = "Delete";
    $(buttons[2]).unbind().bind('click', function (e) {
        e.preventDefault();
        deletePerm(this);
    });
};

function buildPermAssignRow(id,name,assigned) {
    var row = '<tr>';
    row += '<td style="vertical-align:middle; width:80%;">';
    row += '<div class="id-perm">' + name + '</div>';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:20%;">';
    if (assigned) { row += '<input type="checkbox" style="width:100%" class="btn btn-default assign-perm-row" data-id="' + id + '" checked>' }
    else { row += '<input type="checkbox" style="width:100%" class="btn btn-default assign-perm-row" data-id="' + id + '">' }
    row += '</td>';
    row += '</tr>';
    return row;
}

function buildArchiveModal(btn) {
    var id = $(btn)[0].dataset.id;
    var modalBody = $('.perm-modal-body');
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

function appendPerm(data) {
    var row = '<tr>';
    row += '<td style="vertical-align:middle; width:25%;">';
    row += '<div class="id-perm">' + data + '</div>';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:60%;">';
    row += '<input style="width:100%" class="input-sm name-perm" type="text" value="" name="' + data + '" data-id="' + data + '" data-column="name" data-table="ArchivePermissions" disabled />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default edit-perm" data-id="' + data + '" value="Edit" />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default assign-perm" data-toggle="modal" data-target="#permModal" data-backdrop="static" data-keyboard="false" data-id="' + data + '" value="Assign" />';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:5%;">';
    row += '<input type="button" class="btn btn-default delete-perm" data-id="' + data + '" value="Delete" />';
    row += '</td>';
    row += '</tr>';

    var body = $('.body-perm');
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
        addPerm(this);
    });

    // Columns button
    fields[2].value = "Assign";
    fields[2].setAttribute('disabled', 'true');
    $(fields[2]).unbind().bind('click', function (e) {
        e.preventDefault();
        buildPermModal(this);
    });

    // Cancel button
    fields[3].value = "Cancel";
    $(fields[3]).unbind().bind('click', function (e) {
        e.preventDefault();
        cancel(this);
    });
}

function deletePerm(btn) {
    if (confirm('Are you sure?')) {
        $.get('Admin/DeletePermission/' + $(btn)[0].dataset.id)
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
    $('.edit-perm').unbind().bind('click', function (e) {
        e.preventDefault();
        editPerm(this);
    });

    $(".delete-perm").unbind().bind('click', function (e) {
        e.preventDefault();
        deletePerm(this);
    });

    $(".add-perm").unbind().bind('click', function (e) {
        e.preventDefault();
        $.get('Admin/GetHash', {}, function (data) { appendPerm(data); });
    });
});