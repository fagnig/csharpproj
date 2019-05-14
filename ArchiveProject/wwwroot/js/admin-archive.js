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
    $(btn).bind('click', function (e) {
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
    $(btn).bind('click', function (e) {
        e.preventDefault();
        editArchive(this);
    });
};

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

    $('.edit-archive').bind('click', function (e) {
        e.preventDefault();
        editArchive(this);
    });

    $(".delete-archive").bind('click', function (e) {
        e.preventDefault();
        deleteArchive(this);
    });

    $(".add-archive").click(function (e) {
        e.preventDefault();
        // Get generated hash
        var btn = $(this);
        $.get('Admin/GetHash', {}, function (data) {
            // Find correct row
            var add = $(btn).parent().parent();
            var rows = add.siblings();
            var last = $(rows[rows.length - 1]);
            var tobe = $(last).clone();

            // Set Id
            var id_field = $(tobe).find('.archive-id');
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
            $(fields[1]).bind('click', function (e) {
                e.preventDefault();
                addArchive(this);
            });

            // Columns button
            fields[2].value = "Columns";
            fields[2].dataset.id = data;
            fields[2].setAttribute('disabled', 'true');
            $(fields[2]).bind('click', function (e) {
                e.preventDefault();
                //?
            });

            // Cancel button
            fields[3].value = "Cancel";
            fields[3].dataset.id = data;
            $(fields[3]).bind('click', function (e) {
                e.preventDefault();
                cancel(this);
            });

            $(last).after(tobe);
        });
    });
});