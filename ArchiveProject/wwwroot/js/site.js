// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function edit(btn) {
    var id = $(btn)[0].dataset.id;
    var fields = document.getElementsByName(id);
    for (i = 0; i < fields.length; i++) { fields[i].removeAttribute("disabled"); }
    $(btn)[0].value = "Save";
    $(btn).bind('click', function (e) {
        e.preventDefault();
        save(this);
    });
};

function save(btn) {
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
    $(btn).bind('click', function (e) {
        e.preventDefault();
        edit(this);
    });
};

function addRow(btn) {
    $.get('../../CreateDbRow/' + btn.dataset.table, {}, function (data) {
        var row = $(btn).parents("tr");
        var fields = $(row).find(".input-sm");
        var i;
        for (i = 0; i < fields.length; i++) {
            fields[i].setAttribute("name", data);
            fields[i].dataset.id = data;
        }

        var buttons = $(row).find(".btn");
        buttons[0].dataset.id = data;
        buttons[1].dataset.id = data;
        buttons[1].value = "Delete";
        $(buttons[1]).bind('click', function (e) {
            e.preventDefault();
            del(this);
        });
        save(buttons[0]);
    });
};

function del(btn) {
    if(confirm('Are you sure?')) {
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

    $('#archive_selector').change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        window.location = '/Archive/Index/' + selectedCountry + '/';
    });

    $('.edit').bind('click', function (e) {
        e.preventDefault();
        edit(this);
    });

    $(".delete").bind('click', function (e) {
        e.preventDefault();
        del(this);
    });

    $(".add-row").click(function (e) {
        e.preventDefault();
        var add = $(this).parent().parent();
        var rows = add.siblings();
        var last = $(rows[rows.length - 1]);
        var tobe = $(last).clone();
        var fields = $(tobe).find('input');
        fields[0].value = "Save";
        $(fields[0]).bind('click', function (e) {
            e.preventDefault();
            addRow(this);
        });
        fields[1].value = "Cancel";
        $(fields[1]).bind('click', function (e) {
            e.preventDefault();
            cancel(this);
        });

        var i;
        for (i = 2; i < fields.length; i++) {
            fields[i].value = "";
            fields[i].removeAttribute("disabled");
            fields[i].removeAttribute("name");
        }
        $(last).after(tobe);
    });

    $(".add-row-archives").click(function (e) {
        e.preventDefault();
        // Get generated hash
        var hash;
        $.get('../../GetHash', {}, function (data) {
            hash = data;
        });

        // Find correct row
        var add = $(this).parent().parent();
        var rows = add.siblings();
        var last = $(rows[rows.length - 1]);
        var tobe = $(last).clone();

        // Set Id
        var id_field = $(tobe).find('.archive-id');
        id_field[0].value = hash;

        // Fix input Field
        var fields = $(tobe).find('input');
        fields[0].value = "";
        fields[0].removeAttribute("disabled");
        fields[0].setAttribute("name", hash);
        fields.dataset.id = hash;

        // Save button
        fields[1].value = "Save";
        $(fields[1]).bind('click', function (e) {
            e.preventDefault();
            addRowArchive(this);
        });

        // Columns button
        fields[2].value = "Columns";
        fields[2].setAttribute('disabled', 'true');
        $(fields[2]).bind('click', function (e) {
            e.preventDefault();
            //?
        });

        // Cancel button
        fields[3].value = "Cancel";
        $(fields[3]).bind('click', function (e) {
            e.preventDefault();
            cancel(this);
        });

        $(last).after(tobe);
    });
});
 
