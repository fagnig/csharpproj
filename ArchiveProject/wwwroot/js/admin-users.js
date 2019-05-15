function buildUserAssignRow(idUser, idPerm, name, assigned) {
    var row = '<tr>';
    row += '<td style="vertical-align:middle; width:80%;">';
    row += '<div>' + name + '</div>';
    row += '</td>';
    row += '<td style="vertical-align:middle; width:20%;">';
    if (assigned) { row += '<input type="checkbox" class="assign-user-row" data-id-user="' + idUser + '" data-id-perm="' + idPerm + '" checked>' }
    else { row += '<input type="checkbox" class="assign-user-row" data-id-user="' + idUser + '" data-id-perm="' + idPerm + '">' }
    row += '</td>';
    row += '</tr>';
    return row;
}

function buildUserModal(btn) {
    var id = $(btn)[0].dataset.id;
    var modalBody = $('.user-modal-body');
    $.get('Admin/GetUserMapping/' + id, {}, function (data) {
        var json = JSON.parse(data);
        $.each(json, function (index, value) {
            modalBody.append(buildUserAssignRow(id, value[0], value[1], value[2]));
            var rows = $(modalBody).find('tr');
            var last = $(rows[rows.length - 1]);
            last.find(".assign-user-row").bind('change', function (e) {
                e.preventDefault();
                registerUserAssign(this);
            });
        });
    });
}

function registerUserAssign(btn) {
    var checkbox = $(btn);
    var idUser = checkbox[0].dataset.idUser;
    var idPerm = checkbox[0].dataset.idPerm;
    var assign = checkbox.is(":checked");
    $.get('Admin/SetUserMapping/' + idUser + '?idPerm=' + idPerm + '&assign=' + assign);
}

function closeUserModal() {
    $('.user-modal-body').children('tr').each(function (e) { $(this).remove(); });
}

$(document).ready(function () {
    $(".assign-user").unbind().bind('click', function (e) {
        e.preventDefault();
        buildUserModal(this);
    });

    $(".close-user-modal").unbind().bind('click', function (e) {
        e.preventDefault();
        closeUserModal();
    });
});