var table;

// Ajouter une nouvelle ligne
function makeRowAdd() {
    table.makeRowAdd();
}

function envoyerDonnees() {
    var result = JSON.stringify(table.getData());
    var testt = validateChanges;
    if (validateChanges) {
        $.ajax({

            url: "/Reference/Ajouter",
            type: 'POST',
            data: { listeData: result, tableName: tableName }

        }).done(function (response) {   
            var titre = response.titre;
            var message = response.responseText;
            var typeReponse = response.success == false ? "danger" : "success";
            // Afficher une notification
            notify(titre, message, typeReponse);
            if (response.redirect != undefined && response.redirect != '') {
                setTimeout(function () { window.location.href = response.redirect + '?tableName=' + tableName + ''; }, 3000);
            }
        });
    } else {
        var titre1 = "";
        var message1 = "Merci de bien vouloir valider les changements avant la sauvegarde";
        var typeNotify1 = "danger";
        notify(titre1, message1, typeNotify1);
        setTimeout(3000);
    }


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
                "title": properties[i].NomAfficher,
                "data": properties[i].Nom + ".value",
                "className": allLists[properties[i].Nom] == undefined ? properties[i].NumericOrString : "text-left",
                "defaultContent": ""
            }
            columnsTablePropeties.push(celluleTablePropeties);
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

    $('.notifications').on('click', function (e) {
        e.preventDefault();
        var nFrom = $(this).attr('data-from');
        var nAlign = $(this).attr('data-align');
        var nIcons = $(this).attr('data-icon');
        var nType = $(this).attr('data-type');
        var nAnimIn = $(this).attr('data-animation-in');
        var nAnimOut = $(this).attr('data-animation-out');

        notify(nFrom, nAlign, nIcons, nType, nAnimIn, nAnimOut);
    });
});
