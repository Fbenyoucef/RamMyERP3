$(document).ready(function () {

    var table = $('#dataTable');
    // var total = $('"Total').val();
    // row click event
    table.on('click', 'tr', function (e) {
        var total
        var row = $(table).dataTable().api().row(this),
            data = row.data(),
            index = $(table).dataTable().api().cell($(e.target).closest('td')).index().column;

        if ($(table).hasClass('collapsed') && index == 0) {
            // row collapse icon
            return;
        }
        console.log(index);
        if (index > 0) {
            var total = 0;
            let table1 = $("#dataTable").DataTable();
            var combo = $("#Combo");
            let column = table1.column(index).nodes().to$();
            $.each(column, function (index, value) {
                var td = $(value);
                var val = td.find("select").find("option:selected").text().replace(",", ".");
                let intval = parseFloat(val);
                total += intval;
                if (total != 1 || total == 0) {

                 
                    column.find("select").css("border-color", "Red");
                    column.find("select").css("border-width", "2px");
                    column.css("border-width", "");
                   
                  //  column.css("border-width", "1.5px");
                }
                else {

                    column.find("select").css("border-color", "");
                    column.find("select").css("border-width", "");
                 
                }
            });
        }

    });
});
