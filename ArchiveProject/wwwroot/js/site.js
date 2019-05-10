// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $('#edit').bind('click',
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
                    alert(inputvalue);
                   

                    $.get('../UpdateDbValue/'
                        + fields[i].dataset.id + '?'
                        + 'column=' + fields[i].dataset.column + '&'
                        + 'table=' + fields[i].dataset.table + '&'
                        + 'value=' + inputvalue).done(function () {  });
                }
                btn.value = "Edit"
            }

        });




    //$(".registrering-submit").unbind('change').bind('change', function () {
    //    var that = $(this);
    //    var indexIdTemp = that[0].id;
    //    var indexId = indexIdTemp.substr(0, indexIdTemp.lastIndexOf('_') + 1);
    //    var index = $("#" + indexId + "Id")[0].value;

    //    var expectedVal = $("#" + indexId + "Forventet")[0].value;
    //    var currentVal = $("#" + indexId + "Realiseret")[0].value;
    //    var deviationVal = $("#" + indexId + "Afvigelse")[0].value;
    //    $.get('Registrations/SetHourlyRegistrations/?expected=' +
    //        expectedVal +
    //        '&current=' +
    //        currentVal +
    //        '&deviation=' +
    //        deviationVal +
    //        '&sId=' +
    //        index).done(function () {
    //            SumRegistreringer();
    //        });
    //});


    //$(".team-escalation").unbind('click').bind('click',
    //    function (e) {
    //        e.preventDefault();
    //        var buttons = this.parentNode.children;
    //        buttons[0].classList = "btn team-escalation team-escalation-not-chosen";
    //        buttons[1].classList = "btn team-escalation team-escalation-not-chosen";
    //        $.get('SetEscalation/' +
    //            this.dataset.regId +
    //            '?type=' +
    //            this.dataset.type +
    //            '&choice=' +
    //            this.dataset.choice);
    //        this.classList = "btn team-escalation team-escalation-chosen";
    //    });

    //function SumRegistreringer() {
    //    var listRealiseret = $('[id$=__Realiseret]');
    //    var totalRealiseret = 0;

    //    listRealiseret.each(function () {
    //        totalRealiseret += Number.parseFloat($(this)[0].value.replace(',', '.'));
    //    });
    //    $("#totalrealiseret")[0].value = String(totalRealiseret.toFixed(2)).replace('.', ',');

    //    var listForventet = $('[id$=__Forventet]');
    //    var totalForventet = 0;
    //    listForventet.each(function () {
    //        totalForventet += Number.parseFloat($(this)[0].value.replace(',', '.'));
    //    });
    //    $("#totalforventet")[0].value = String(totalForventet.toFixed(2)).replace('.', ',');
    //}

});

