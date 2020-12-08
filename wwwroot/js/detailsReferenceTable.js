var table;
var listrowAdd = [];
var ix = 0;
var originalData;
// Ajouter une nouvelle ligne
function makeRowAdd() {
    table.saveRowEditable();
    table.saveRowAdd();
    table.makeRowAdd();
    listrowAdd.push(ix++);
}

function envoyerDonnees() {
    table.saveRowEditable();
    table.saveRowAdd();
    var result = JSON.stringify(table.getData());
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
                $(window).unbind('beforeunload');
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

function supprimerDonnees(idTodelete) {
    if (true) {
        $.ajax({

            url: "/Reference/Supprimer",
            type: 'POST',
            data: { id: idTodelete, tableName: tableName }

        }).done(function (response) {
            var titre = response.titre;
            var message = response.responseText;
            var typeReponse = response.success == false ? "danger" : "success";
            // Afficher une notification
            notify(titre, message, typeReponse);
            if (response.redirect != undefined && response.redirect != '') {
                $(window).unbind('beforeunload');
                setTimeout(function () { window.location.href = response.redirect + '?tableName=' + tableName + ''; }, 0);
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
    originalData = listeDonnees;
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
                "className": allLists[properties[i].Nom] == undefined ? textAlign(properties[i].NumericOrString) : "text-left",
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

    function textAlign(numOrString) {
        if (numOrString == "Numeric")
            return "text-right";
        if (numOrString == "String")
            return "text-left";
        if (numOrString == "Autres")
            return "text-center";
    }
    
    table = myDataTableFactory(config);

    table.fillData();
    table.table = $('#tableReference').DataTable({
        "data": table.initialData,
        "columns": columnsTablePropeties,
        "columnDefs": [
            { orderable: false, className: 'reorder', targets: 0 },
            { orderable: true, targets: '_all' }
        ],
        rowReorder: true
    });
    $('#tableReference').on('page.dt', function (a, b, c) {
        table.saveRowEditable();
        table.saveRowAdd();
    });

    table.table.columns(columnsTableVisibility).visible(false);

    table.table.on('row-reorder', function (e, diff) {
        // Declarer un nouveau tableau des données
        var newInitialData = [];
        // Declarer un dictionnaire des proprietes
        var dictionnaire = {};
        // Parcourir le tableau des diffirences
        for (var i = 0; i < diff.length; i++) {
            // Recuperer la ligne avec l'ancienne valeur
            var rowOld = clone(table.dataList[diff[i].oldPosition]);
            // Recuperer la ligne avec les nouvelles valeurs
            var rowNew = table.dataList[diff[i].newPosition];
            // Mettre a jour la ligne
            rowOld.POSITION.data = rowNew.POSITION.data;
            // Remplir le dictionnaire
            dictionnaire[diff[i].newPosition] = Object.assign({}, rowOld);
        }
        // Parcourir le dictionnaire
        for (const property in dictionnaire) {
            // Verifier si le dictionnaire a des proprietes
            if (Object.prototype.hasOwnProperty.call(dictionnaire, property)) {
                // Recuperer la valeur de la propriete
                var valeur = dictionnaire[property];
                // Mettre a jour la cellule
                table.dataList[property] = valeur;
                // Mettre a jour l'index de la cellule
                table.dataList[property].metadata.index = parseInt(property);
                // declarer un objet resultat
                var resultat = {};
                // Parcourir les proprietes de chaque valeur dans le dictionnaire
                for (const prop in valeur) {
                    // Verifier si le la valeur a des proprietes
                    if (Object.prototype.hasOwnProperty.call(valeur, prop)) {
                        // Si le propriete de la valeur dans le dictionnaire
                        // est diffirente de "metadata" et "actions"
                        if (prop !== "metadata" && prop !== "actions") {
                            // Mettre a jour la donnée dans l'objet resultat
                            resultat[prop] = valeur[prop].data;
                        }
                    }
                }
                // Mettre a jour les données a afficher dans le tableau
                newInitialData.push(resultat);
            }
        }
        // Actualiser les données de la table
        table.initialData = newInitialData;
        table.saveRowAdd();
        table.saveRowEditable();
        // Actualiser les cellules
        table.refreshData();
        // Redessiner la table avec les données mises a jours
        table.reDraw();
        table.makeRowEditable();
    });

    table.reDraw();
    table.makeRowEditable();
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
    $(window).bind('beforeunload', function (e) {
        return 'Are you sure you want to leave?';
    });
});

