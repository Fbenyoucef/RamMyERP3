$(document).ready(function () {

    // row click event
    $('#dataTable').on('click', 'tr', function (e) {
        alert("sdfsdfsdfs");

    });

    $("#AnneeMois").change(function () {
        var Anneemois = $('#AnneeMois');
        Anneemois.css("border-color", "");
        var date = $(this).val().split('-');
        var mois = date[1];
        var annee = date[0];
        var area = $("#Details");
        $.get('GenererTableauRam?mois=' + mois + '&annee=' + annee, function (data) {
            area.html(data);
            drawDataTable();

        });
    });
    drawDataTable();


    function drawDataTable(id = "dataTable") {
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
