/**
* Configuration par defaut de toutes les DataTable
*/
$.extend($.fn.dataTable.defaults, {
    "lengthChange": true,
    "responsive": false,
    "searching": true,
    "ordering": true,
    "stateSave": true,
    "fixedHeader": true,
    "select": true,
    "autoWidth": true,
    "lengthMenu": [[15, 30, 50, 100, -1], [15, 30, 50, 100, "Tout"]],
    "pageLength": 50,
    "language": {
        "processing": "Traitement en cours...",
        "search": "Recherche",
        "lengthMenu": "Par page&thinsp; _MENU_ ",
        "info": "Affichage de l'&eacute;lement _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
        "infoEmpty": "Affichage de l'&eacute;lement 0 &agrave; 0 sur 0 &eacute;l&eacute;ments",
        "infoFiltered": "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
        "infoPostFix": "",
        "loadingRecords": "Chargement en cours...",
        "zeroRecords": "Aucun &eacute;l&eacute;ment &agrave; afficher",
        "emptyTable": "Aucunes données disponible dans le tableau",
        "paginate": {
            "first": "Premier",
            "previous": "Pr&eacute;c&eacute;dent",
            "next": "Suivant",
            "last": "Dernier"
        },
        "aria": {
            "sortAscending": ": activer pour trier la colonne par ordre croissant",
            "sortDescending": ": activer pour trier la colonne par ordre décroissant"
        }
    }
});