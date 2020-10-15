$(document).ready(function () {

    // row click event
    $('#dataTable').on('click', 'tr', function (e) {
        alert("sdfsdfsdfs");

        //var row = $(table).dataTable().api().row(this),
        //    data = row.data(),
        //    index = $(table).dataTable().api().cell($(e.target).closest('td')).index().column;

        //if ($(table).hasClass('collapsed') && index == 0) {
        //    // row collapse icon
        //    return;
        //}

        //if (index == 1) {
        //    console.log("textklsfjlkdfjlskdfjmlsdkjm");
        //}
    });

    $("#AnneeMois").change(function () {
        var Anneemois = $('#AnneeMois');
        Anneemois.css("border-color", "");
        var Date = $(this).val().split('-');
        var mois = Date[1];
        var annee = Date[0];
        var area = $("#Details");
        $.get('GenererTableauRam?mois=' + mois + '&annee=' + annee, function (data) {
            area.html(data);
            DrawDataTable();

        });
    });
    DrawDataTable();


    function DrawDataTable(id = "dataTable") {
        $('#dataTable').DataTable(

            {
                "lengthChange": false,

                "responsive": false,

                "searching": false,

                "ordering": false,

                "stateSave": true,

                "fixedHeader": false,

                "select": true,
                "paging": false,
                "autoWidth": false,

                scrollX: true,
                fixedColumns: {
                    leftColumns: 1
                },
                "language": {
                    "search": "Recherche",
                }

            });

        $(".DTFC_LeftBodyLiner table").attr('style', 'margin-top: -7px !important;margin-bottom: -7px !important;background: white;');
        $(".dataTables_info").closest("div.row").remove();
    }






});
