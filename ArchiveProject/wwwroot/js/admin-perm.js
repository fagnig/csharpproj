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
    $.get('Admin/RenamePerm/' + inputField.dataset.id + '?name=' + inputField.value);

    $(btn)[0].value = "Edit";
    $(btn).unbind().bind('click', function (e) {
        e.preventDefault();
        editPerm(this);
    });
};

function copyPerm(btn,data) {
    // Find correct row
    var add = $(btn).parent().parent();
    var rows = add.siblings();
    var last = $(rows[rows.length - 1]);
    var tobe = $(last).clone();

    // Set Id
    var id_field = $(tobe).find('.id-perm');
    id_field[0].innerText = data;

    // Fix input Field
    var fields = $(tobe).find('input');
    fields[0].value = "";
    fields[0].removeAttribute("disabled");
    fields[0].setAttribute("name", data);
    fields[0].dataset.id = data;

    // Save button
    fields[1].value = "Add";
    fields[1].dataset.id = data;
    $(fields[1]).unbind().bind('click', function (e) {
        e.preventDefault();
        addArchive(this);
    });

    // Columns button
    fields[2].value = "Columns";
    fields[2].dataset.id = data;
    fields[2].setAttribute('disabled', 'true');
    $(fields[2]).unbind().bind('click', function (e) {
        e.preventDefault();
        //?
    });

    // Cancel button
    fields[3].value = "Cancel";
    fields[3].dataset.id = data;
    $(fields[3]).unbind().bind('click', function (e) {
        e.preventDefault();
        cancel(this);
    });

    $(last).after(tobe);
}

function deletePerm(btn) {
    if (confirm('Are you sure?')) {
        $.get('Admin/DeletePerm/' + $(btn)[0].dataset.id)
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
        var btn = $(this);
        $.get('Admin/GetHash', {}, function (data) { copyPerm(btn,data); });
    });
});