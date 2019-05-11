// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $('.edit').bind('click',
        function (e) {
            e.preventDefault();

            var btn = $(this)[0];
            if (btn.value == "Edit") {
                var fields = document.getElementsByName(btn.dataset.id)
                var i;
                for (i = 0; i < fields.length; i++) { fields[i].removeAttribute("disabled"); }
                btn.value = "Save";
            } else {
                var fields = document.getElementsByName(btn.dataset.id)
                var i;
                for (i = 0; i < fields.length; i++) {
                    fields[i].setAttribute("disabled", "true");
                    var inputvalue;

                    if (fields[i].type == "checkbox") {
                        inputvalue = fields[i].checked;
                    } else {
                        inputvalue = fields[i].value;
                    }                   

                    $.get('../UpdateDbValue/'
                        + fields[i].dataset.id + '?'
                        + 'column=' + fields[i].dataset.column + '&'
                        + 'table=' + fields[i].dataset.table + '&'
                        + 'value=' + inputvalue).done(function () {  });
                }
                btn.value = "Edit"
            }

        });
});
 
