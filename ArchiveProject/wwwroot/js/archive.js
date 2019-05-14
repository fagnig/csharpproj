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

function copyRow(btn) {
    // Get rows
    var add = $(btn).parent().parent();
    var rows = add.siblings();
    var last = $(rows[rows.length - 1]);
    var tobe = $(last).clone();

    // Get inputs
    var fields = $(tobe).find('input');
    fields[0].value = "Save";
    $(fields[0]).unbind().bind('click', function (e) {
        e.preventDefault();
        addRow(this);
    });
    fields[1].value = "Cancel";
    $(fields[1]).unbind().bind('click', function (e) {
        e.preventDefault();
        cancel(this);
    });

    // Clear inputs
    var i;
    for (i = 2; i < fields.length; i++) {
        fields[i].value = "";
        fields[i].removeAttribute("disabled");
        fields[i].removeAttribute("name");
    }

    // Insert row
    $(last).after(tobe);
}

function deleteRow(btn) {
    if (confirm('Are you sure?')) {
        $.get('../../DeleteDbRow/' + $(btn)[0].dataset.id + '?' + 'table=' + $(btn)[0].dataset.table)
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
    $('#archive_selector').unbind().bind('change', function (e) {
        e.preventDefault();
        var selectedCountry = $(this).children("option:selected").val();
        window.location = '/Archive/Index/' + selectedCountry + '/';
    });

    $('.edit-row').unbind().bind('click', function (e) {
        e.preventDefault();
        editRow(this);
    });

    $(".delete-row").unbind().bind('click', function (e) {
        e.preventDefault();
        deleteRow(this);
    });

    $(".add-row").unbind().bind('click', function (e) {
        e.preventDefault();
        copyRow(this);
    });
});

