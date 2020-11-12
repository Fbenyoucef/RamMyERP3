var table;

// Ajouter une nouvelle ligne
function makeRowAdd() {
    table.makeRowAdd();
}

$(document).ready(function () {
    var ligneVide = {};
    var columnsPropeties = [];
    var columnsTablePropeties = [];
    var columnsTableVisibility = [];
    var allLists = {};
    if (listeTableLiee) {
        for (const property in listeTableLiee) {
            allLists[property] = {
                // Selectionner un element par default
                defaultSelected: '',
                // Ajouter un element vide dans la liste
                hasEmpty: true,
                // Text de l'element vide "placeHolder"
                emptyText: 'select',
                // Valeur par default de l'element vide
                emptyValue: '',
                // Les donnes de la liste
                data: listeTableLiee[property]
            };
        }
    }

    if (properties) {
        columnsTablePropeties.push(
            {
                "data": null,
                "render": function () {
                    return '<label class="fa fa-bars icon" style="font-size: small;MARGIN-TOP: 10px;"></label>';
                },
                "fnCreatedCell": function (nTd) {
                    $(nTd).css('width', '1%');
                },
                "defaultContent": ""
            },
            {
                "data": "actions.value",
                "fnCreatedCell": function (nTd) {
                    $(nTd).css('white-space', 'nowrap');
                    $(nTd).css('width', '1%');
                    $(nTd).css('padding', '5px 12px 5px 12px');
                },
                "defaultContent": ""
            });

        for (var i = 0; i < properties.length; i++) {
            if (properties[i].Visibilite) {
                columnsTableVisibility.push(i + 2);
            }
            ligneVide[properties[i]] = '';

            if (allLists[properties[i].Nom] == undefined) {
                var cellulePropeties = {
                    cell: properties[i].Nom,
                    type: properties[i].IsReadOnly ? inputType.NONE : inputType.INPUT,
                    property: properties[i].Nom,
                    classes: 'form-control',
                    name: ''
                }
                columnsPropeties.push(cellulePropeties);
            }
            else {
                var cellulePropeties = {
                    cell: properties[i].Nom,
                    type: inputType.SELECT,
                    property: properties[i].Nom,
                    classes: 'form-control',
                    name: '',
                    list: {
                        name: properties[i].Nom,
                        selectionKey: properties[i].Nom,
                        displayMember: 'NOM',
                        valueMember: 'ID'
                    }
                }
                columnsPropeties.push(cellulePropeties);
            }

            var celluleTablePropeties = {
                "data": properties[i].Nom + ".value", //"Nom"+".value" = "Nom.value"
                "defaultContent": ""
            }
            columnsTablePropeties.push(celluleTablePropeties);


            //{
            //    cell: 'Poste',
            //        type: inputType.SELECT,
            //            property: 'Poste',
            //                classes: 'form-control',
            //                    name: '',
            //                        list: {
            //        name: 'postesListe',
            //            selectionKey: 'Poste',
            //                displayMember: 'Nom',
            //                    valueMember: 'Id'
            //    }
            //}
        }
    }

    var config = {
        prefix: "tableReference",
        initialData: listeDonnees,
        emptyRow: ligneVide,
        columns: columnsPropeties,
        lists: allLists,
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

    table.table.columns(columnsTableVisibility).visible(false);

    table.reDraw();
});
