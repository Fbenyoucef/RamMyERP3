var table;

// Ajouter une nouvelle ligne
function makeRowAdd() {
    table.makeRowAdd();
}

$(document).ready(function () {
    var ligneVide = {};
    var columnsPropeties = [];
    var columnsTablePropeties = [];
    columnsTablePropeties.push(
        // Afficher adidas 
        {
            "data": null,
            "render": function () {
                return '<label class="fa fa-bars icon" style="font-size: small;MARGIN-TOP: 10px;"></label>';
            },
            "width": "2%",
            "defaultContent": ""
        });
    // Affiche la colonne des actions
    columnsTablePropeties.push(
        {
            "data": "actions.value",
            "fnCreatedCell": function (nTd) {
                $(nTd).css('white-space', 'nowrap');
                $(nTd).css('width', '1%');
                $(nTd).css('padding', '5px 12px 5px 12px');
            },
            "defaultContent": ""
        });
    if (properties) {
        for (var i = 0; i < properties.length; i++) {
            ligneVide[properties[i]] = 0;
            var cellulePropeties = {
                cell: properties[i],
                // if for example properties[i] is editable type: inputType.INPUT else type: inputType.NONE
                type: inputType.INPUT,
                property: properties[i],
                classes: 'form-control',
                name: ''
            }
            var celluleTablePropeties = {
                "data": properties[i] + ".value", //"Nom"+".value" = "Nom.value"
                "defaultContent": ""
            }
            columnsPropeties.push(cellulePropeties);
            columnsTablePropeties.push(celluleTablePropeties);
        }
    }

    var config = {
        prefix: "tableReference",
        initialData: listeDonnees,
        emptyRow: ligneVide,
        columns: columnsPropeties,
    };

    table = myDataTableFactory(config);
    table.fillData();
    table.table = $('#tableReference').DataTable({
        "data": table.initialData,
        "columns": columnsTablePropeties,
        "columnDefs": [
            { orderable: false, className: 'reorder', targets: 0 },
            { orderable: true, targets: '_all' }
        ],
        rowReorder: true,
    });

    table.reDraw();
});
