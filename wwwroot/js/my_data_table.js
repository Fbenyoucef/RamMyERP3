var inputType = {
    NONE: 'NONE',
    INPUT: 'INPUT',
    TEXTAREA: 'TEXTAREA',
    CHECKBOX: 'CHECKBOX',
    SELECT: 'SELECT',
    DEFAULT_BUTTONS: 'DEFAULT_BUTTONS',
    EDIT_BUTTONS: 'EDIT_BUTTONS'
};

var validateChanges = false;
function clone(item) {
    if (!item) { return item; } // null, undefined values check

    var types = [Number, String, Boolean],
        result;

    // normalizing primitives if someone did new String('aaa'), or new Number('444');
    types.forEach(function (type) {
        if (item instanceof type) {
            result = type(item);
        }
    });

    if (typeof result == "undefined") {
        if (Object.prototype.toString.call(item) === "[object Array]") {
            result = [];
            item.forEach(function (child, index, array) {
                result[index] = clone(child);
            });
        } else if (typeof item == "object") {
            // testing that this is DOM
            if (item.nodeType && typeof item.cloneNode == "function") {
                result = item.cloneNode(true);
            } else if (!item.prototype) { // check that this is a literal
                if (item instanceof Date) {
                    result = new Date(item);
                } else {
                    // it is an object literal
                    result = {};
                    for (var i in item) {
                        result[i] = clone(item[i]);
                    }
                }
            } else {
                // depending what you would like here,
                // just keep the reference, or create new object
                if (false && item.constructor) {
                    // would not advice to do that, reason? Read below
                    result = new item.constructor();
                } else {
                    result = item;
                }
            }
        } else {
            result = item;
        }
    }

    return result;
}
//configurable

var default_my_data_table = {
    initialData: [],

    // Declarer variable this.table
    table: {},

    globalIndex: 0,
    // Liste des fonctions deja crees
    dataList: [],

    prefix: "datatable1",

    adding: false,

    defaultColumn: {
        cell: '',
        index: 0,
        type: inputType.NONE,
        property: '',
        classes: '',
        name: '',
        list: {
            name: '',
            selectionKey: '',
            displayMember: '',
            valueMember: ''
        }

    },

    columns: [],

    lists: {},

    onPreUpdate: function (row, index, context) {
        return;
    },

    onPostUpdate: function (row, index, context) {
        return;
    },

    onPreAdd: function (row, index, context) {
        return;
    },

    onPostAdd: function (row, index, context) {
        return;
    },

    checkType: (type) => (type != inputType.NONE &&
        type != inputType.DEFAULT_BUTTONS &&
        type != inputType.EDIT_BUTTONS),

    emptyRow: {
    },

    getData() {
        var res = [];
        for (var i = 0; i < this.dataList.length; i++) {
            var row = this.dataList[i];
            row.metadata.index = i;
            var item = {};
            for (const property in row) {
                if (property != "metadata" && property != "actions") {
                    var typeElement = row[property].properties.type;
                    var cell = row[property].data;
                    if (typeElement == inputType.SELECT) {
                        var cell = row[property].original;
                    }
                    item[property] = cell;

                }
            }
            res.push(item);
        }
        return res;
    },

    // Remplir les données this.initialData
    fillData: function () {
        //this.globalIndex = 0;

        for (var i = 0; i < this.initialData.length; i++) {
            var index = this.globalIndex++;
            this.makeRow(this.initialData[i], index);
        }
    },

    makeRow: function (data, index, inEdit = false) {
        var res = {
            metadata: {
                index: index,
                id: data.Id,
                inEdit: inEdit,
                getId: (indexElement) => {
                    if (this.dataList.length == 0) return '';
                    return this.dataList[indexElement].metadata.id;
                },
                updateDataFromInputs: (indexElement) => {
                    var row = this.dataList[indexElement];
                    var oldRow = clone(this.dataList[indexElement]);
                    this.onPreUpdate(row, indexElement, this);
                    for (const property in row) {
                        if (Object.prototype.hasOwnProperty.call(row, property)) {
                            if (property != "metadata") {
                                var typeElement = row[property].properties.type;
                                var idElement = row[property].id;
                                if (typeElement != inputType.NONE &&
                                    typeElement != inputType.DEFAULT_BUTTONS &&
                                    typeElement != inputType.EDIT_BUTTONS) {
                                    var input = $(`#${idElement}`);
                                    switch (typeElement) {
                                        case inputType.INPUT:
                                        case inputType.TEXTAREA:
                                            if (input.val() !== undefined) {
                                                this.dataList[indexElement][property].data = input.val();
                                            }
                                            break;
                                        case inputType.SELECT:
                                            if (input.val() !== undefined) {
                                                this.dataList[indexElement][property].data = input.val();
                                                if (row[property].properties.type == inputType.SELECT) {
                                                    if (row[property].properties.list != '') {
                                                        var listData = this.lists[row[property].properties.list.name].data;
                                                        for (var i = 0; i < listData.length; i++) {
                                                            var option = listData[i];
                                                            var value = this.resolve(row[property].properties.list.valueMember, option);
                                                            if (value == this.dataList[indexElement][property].data) {
                                                                this.dataList[indexElement][property].original =
                                                                    this.dataList[indexElement][property].data;
                                                                var text = this.resolve(row[property].properties.list.displayMember, option);
                                                                this.dataList[indexElement][property].data = text;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                    }
                                    this.dataList[indexElement][property].html =
                                        this.makeCellHtml(row[property], typeElement);
                                    this.dataList[indexElement][property].htmladd =
                                        this.makeCellHtmlAdd(row[property], typeElement);
                                    //if (this.checkType(typeElement)) {
                                    //    this.dataList[indexElement][property].html.value =
                                    //        this.dataList[indexElement][property].html.data +
                                    //        ` <i class="fa fa-pencil" onclick="window.mydatatables.${this.prefix
                                    //        }.makeRowEditable(${index})"/>`;
                                    //} else {
                                    //    this.dataList[indexElement][property].html.value =
                                    //        this.dataList[indexElement][property].html.data;
                                    //}
                                    this.dataList[indexElement][property].html.value =
                                        this.dataList[indexElement][property].html.data;
                                }
                            }
                        }
                    }
                    this.onPostUpdate(row, oldRow, indexElement, this);

                },
                updateDataFromInputsAfterAdd: (indexElement) => {
                    var row = this.dataList[indexElement];
                    this.onPreAdd(row, indexElement, this);
                    for (const property in row) {
                        if (Object.prototype.hasOwnProperty.call(row, property)) {
                            if (property != "metadata") {
                                var typeElement = row[property].properties.type;
                                var idElement = row[property].idadd;
                                if (typeElement != inputType.NONE &&
                                    typeElement != inputType.DEFAULT_BUTTONS &&
                                    typeElement != inputType.EDIT_BUTTONS) {
                                    var input = $(`#${idElement}`);
                                    switch (typeElement) {
                                        case inputType.INPUT:
                                        case inputType.TEXTAREA:
                                            if (input.val() !== undefined) {
                                                this.dataList[indexElement][property].data = input.val();
                                            }
                                            break;
                                        case inputType.SELECT:
                                            if (input.val() !== undefined) {
                                                this.dataList[indexElement][property].data = input.val();
                                                if (row[property].properties.type === inputType.SELECT) {
                                                    if (row[property].properties.list != '') {
                                                        var listData = this.lists[row[property].properties.list.name].data;
                                                        for (var i = 0; i < listData.length; i++) {
                                                            var option = listData[i];
                                                            var value = this.resolve(row[property].properties.list.valueMember, option);
                                                            if (value == this.dataList[indexElement][property].data) {
                                                                this.dataList[indexElement][property].original =
                                                                    this.dataList[indexElement][property].data;
                                                                var text = this.resolve(row[property].properties.list.displayMember, option);
                                                                this.dataList[indexElement][property].data = text;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                    }
                                    this.dataList[indexElement][property].html =
                                        this.makeCellHtml(row[property], typeElement);
                                    this.dataList[indexElement][property].htmladd =
                                        this.makeCellHtmlAdd(row[property], typeElement);
                                    //if (this.checkType(typeElement)) {
                                    //    this.dataList[indexElement][property].html.value =
                                    //        this.dataList[indexElement][property].html.data +
                                    //        ` <i class="fa fa-pencil" onclick="window.mydatatables.${this.prefix
                                    //        }.makeRowEditable(${index})"/>`;
                                    //} else {
                                    //    this.dataList[indexElement][property].html.value =
                                    //        this.dataList[indexElement][property].html.data;
                                    //}
                                    this.dataList[indexElement][property].html.value =
                                        this.dataList[indexElement][property].html.data;
                                }
                            }
                        }
                    }
                    this.onPostAdd(row, indexElement, this);
                }
            }

        };

        for (var i = 0; i < this.columns.length; i++) {
            var col = this.columns[i];
            var column = Object.assign(this.defaultColumn, col);
            var resultdata;
            if (data[col.cell] == undefined) {
                typedata = properties.filter(e => e.Nom == col.cell);
                if (typedata[0].NumericOrString == "String")
                    resultdata = "";
                else if (typedata[0].NumericOrString == "Autres")
                    resultdata = "";
                else
                    resultdata = 0;
            } else
                resultdata = data[col.cell];
            column.cell = resultdata;
            column.index = index;
            column.original = data[col.cell];

            res[col.cell] = this.makeCell(column);
        }
        this.dataList.push(res);

        res.actions = this.makeButtonsCell(res);
        return res;
    },

    refreshData: function () {
        this.globalIndex = 0;
        for (var i = 0; i < this.dataList.length; i++) {
            var row = this.dataList[i];
            row.metadata.index = i;
            for (const property in row) {
                if (property != "metadata" && property != "actions") {
                    var original = row[property].original;
                    var cell = row[property].data;
                    var type = row[property].properties.type;
                    row[property].properties.index = i;
                    var id = `${this.prefix}_${property}_${i}`;
                    var classes = row[property].properties.class;
                    var column = Object.assign(this.defaultColumn, { original: original, cell: cell, index: i, type: type, property: id, classes: classes });
                    row[property] = this.makeCell(column);
                }
            }
            row.actions = this.makeButtonsCell(row);
        }
    },

    reset: function () {
        this.globalIndex = 0;

        var res = [];
        var rows = table.table.rows().data();
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            row.metadata.index = i;
            for (const property in row) {
                if (property != "metadata" && property != "actions") {

                    var original = row[property].original;
                    var cell = row[property].data;
                    var type = row[property].properties.type;
                    row[property].properties.index = i;
                    var id = `${this.prefix}_${property}_${i}`;
                    var classes = row[property].properties.class;
                    var column = Object.assign(this.defaultColumn, { original: original, cell: cell, index: i, type: type, property: id, classes: classes });
                    row[property] = this.makeCell(column, false);
                }
            }
            row.actions = this.makeButtonsCell(row);
            res.push(row);
        }
        this.dataList = res;
        //for (var i = 0; i < this.dataList.length; i++) {
        //    var row = this.dataList[i];
        //    row.metadata.index = i;
        //    for (const property in row) {
        //        if (property != "metadata" && property != "actions") {
        //            var original = row[property].original;
        //            var cell = row[property].data;
        //            var type = row[property].properties.type;
        //            row[property].properties.index = i;
        //            var id = `${this.prefix}_${property}_${i}`;
        //            var classes = row[property].properties.class;
        //            var column = Object.assign(this.defaultColumn, { original: original, cell: cell, index: i, type: type, property: id, classes: classes });
        //            row[property] = this.makeCell(column);
        //        }
        //    }
        //    row.actions = this.makeButtonsCell(row);
        //}
    },

    makeCell: function (arg = this.defaultColumn) {
        var id = `${this.prefix}_${arg.property}_${arg.index}`;
        var idadd = `${this.prefix}_add_${arg.property}_${arg.index}`;
        var obj = {
            id: id,
            idadd: idadd,
            name: arg.name,
            properties: {
                type: arg.type,
                "class": arg.classes,
                list: {
                    name: arg.list.name,
                    selectionKey: arg.list.selectionKey,
                    displayMember: arg.list.displayMember,
                    valueMember: arg.list.valueMember,
                }
            }
        };

        obj.data = arg.cell;
        obj.original = arg.original;

        if (obj.properties.type == inputType.SELECT) {
            if (obj.properties.list != '') {
                var listData = this.lists[obj.properties.list.name].data;
                for (var i = 0; i < listData.length; i++) {
                    var option = listData[i];
                    var value = this.resolve(obj.properties.list.valueMember, option);
                    if (value == arg.cell) {
                        var text = this.resolve(obj.properties.list.displayMember, option);
                        obj.data = text;
                    }
                }
            }
        }


        obj.html = this.makeCellHtml(obj, arg.type);
        obj.htmladd = this.makeCellHtmlAdd(obj, arg.type);
        //if (this.checkType(arg.type)) {
        //    obj.value = obj.data +
        //        ` <i class="fa fa-pencil" onclick="window.mydatatables.${this.prefix}.makeRowEditable(${arg.index})"/>`;
        //} else {
        //    obj.value = obj.data;
        //}
        obj.value = obj.data;
        return obj;
    },

    makeRowEditable: function (index) {
        for (var i = 0; i < this.dataList.length; i++) {
            var row = this.dataList[i];
            for (const property in row) {
                if (property != "metadata") {
                    row[property] = this.switchToEdit(row, property, i);
                }
            }
            this.dataList[i] = row;
            var current = this.table.page();
            this.reDraw();
            this.table.page(current).draw('page');
            //this.table.page('previous').draw('page');
            validateChanges = false;
            //this.table.rowReorder.disable();
        }
    },



    makeEmpty: function () {
        return this.emptyRow;
    },

    makeRowAdd: function () {
        if (this.adding) return;
        this.adding = true;

        var newRow = this.makeRow(this.makeEmpty(), this.dataList.length);
        var index = this.dataList.length - 1;

        for (const property in newRow) {
            if (property != "metadata") {
                newRow[property] = this.switchToAdd(newRow, property, index);
            }
        }
        this.dataList[index] = newRow;
        //this.makeRowEditable(index);
        this.reDraw();
        this.table.page('last').draw('page');

        //this.table.rowReorder.disable();
    },

    saveRowEditable: function (index) {
        /*this.table.rowReorder.enable()*/;
        for (var i = 0; i < this.dataList.length; i++) {
            var row = this.dataList[i];
            row.metadata.updateDataFromInputs(i);
            for (const property in row) {
                if (Object.prototype.hasOwnProperty.call(row, property)) {
                    if (property != "metadata") {
                        row[property] = this.switchToEdit(row, property, i);
                    }
                }
            }
            //row.metadata.inEdit = false;
            this.dataList[i] = row;
            //this.updateRowdata(index);
            //this.addRowData(index);
            var current = this.table.page();
            //setTimeout(() => {
            //    this.reset();
            //    this.reDraw();
            //    this.table.page(current).draw('page');
            //}, 5);
            validateChanges = true;
        }
    },

    cancelRowEditable: function (index) {
        //this.table.rowReorder.enable();
        var row = this.dataList[index];
        for (const property in row) {
            if (Object.prototype.hasOwnProperty.call(row, property)) {
                if (property != "metadata") {
                    row[property] = this.switchToEdit(row, property, index, true);
                }
            }
        }
        row.metadata.inEdit = false;
        this.dataList[index] = row;
        this.reDraw();
        validateChanges = false;

    },

    saveRowAdd: function (index) {
        /*this.table.rowReorder.enable()*/;
        for (var i = 0; i < this.dataList.length; i++) {


            this.adding = false;

            var row = this.dataList[i];
            row.metadata.updateDataFromInputsAfterAdd(i);
            for (const property in row) {
                if (Object.prototype.hasOwnProperty.call(row, property)) {
                    if (property != "metadata") {
                        row[property] = this.switchToEdit(row, property, i);
                    }
                }
            }
            row.metadata.inEdit = false;
            this.dataList[i] = row;
            //this.addRowData(index);
            //this.reDraw();
            //        this.reDrawRow(index);
            //this.refreshData();
            //this.reDraw();
            //this.table.page('last').draw('page');
            validateChanges = true;
        }
    },

    cancelRowAdd: function (index) {
        //this.table.rowReorder.enable();
        this.adding = false;
        this.dataList.splice(index, 1);

        //this.reDraw();
        this.reDrawRow(index, true);
        validateChanges = false;
    },

    isNew: function () {
        return true;
    },

    deleteRow: function (e, index) {
        if (this.isNew(index)) {
            var row = this.dataList[index];
            supprimerDonnees(row.ID.data);
            this.adding = false;
            this.dataList.splice(index, 1);
            //this.initialData.splice(index, 1);
            //this.reDrawRow(index, true);
            //e.cancelBubble = true;
            //e.stopPropagation();
            this.saveRowEditable()
            this.saveRowAdd()
            this.refreshData();
            this.reDraw();
            table.makeRowEditable();
        }
        validateChanges = true;
    },

    switchToEdit: function (row, property, index, reverse = false) {
        if (!reverse) {
            row[property].value = unescape(row[property].html);
        } else {
            var type = row[property].properties.type;
            //if (this.checkType(type)) {
            //    row[property].value =
            //        unescape(row[property].data + ` <i class="fa fa-pencil" onclick="window.mydatatables.${this.prefix}.makeRowEditable(${index})"/>`);
            //} else {
            //    row[property].value = unescape(row[property].data);
            //}
            row[property].value = unescape(row[property].data);
        }
        return row[property];
    },

    switchToAdd: function (row, property, index, reverse = false) {
        if (!reverse) {
            row[property].value = unescape(row[property].htmladd);
        } else {
            var type = row[property].properties.type;
            //if (this.checkType(type)) {
            //    row[property].value = unescape(row[property].data + ` <i class="fa fa-pencil" onclick="this.saveRowAdd(${index})"/>`);
            //} else {
            row[property].value = unescape(row[property].data);
            //}
        }
        return row[property];
    },

    makeButtonsCell: function (element) {
        var obj = {
            id: '',
            name: '',
            properties: {
                type: inputType.NONE,
                "class": ''
            }
        };
        obj.data = this.makeDefaultButtons(element);
        obj.html = this.makeEditButtons(element.metadata.index);
        obj.htmladd = this.makeAddButtons(element.metadata.index);
        obj.value = unescape(obj.data);
        if (element.metadata.inEdit) {
            obj.value = unescape(obj.html);
        } else {
            obj.value = unescape(obj.data);
        }
        return obj;
    },

    makeCellHtml: function (obj, type = inputType.NONE) {
        switch (type) {
            case inputType.INPUT:
                return escape(this.makeInput(obj));
            case inputType.TEXTAREA:
                return escape(this.makeTextArea(obj));
            case inputType.SELECT:
                return escape(this.makeSelect(obj));
            default:
                return obj.data;
        }
    },

    makeCellHtmlAdd: function (obj, type = inputType.NONE) {
        switch (type) {
            case inputType.INPUT:
                return escape(this.makeInputAdd(obj));
            case inputType.TEXTAREA:
                return escape(this.makeTextAreaAdd(obj));
            case inputType.SELECT:
                return escape(this.makeSelectAdd(obj));
            default:
                return obj.data;
        }
    },

    makeDefaultButtons: function (row) {

        return `<div class="tabledit-toolbar btn-toolbar" style="text-align: left;"><div class= "btn-group btn-group-sm" style="float: none; padding: 5px;">` +
            //`<a class="tabledit-edit-button btn btn-primary waves-effect waves-light" ` +
            //`style="float: none; margin: 0px; color: white" title="Editer" onclick="window.mydatatables.${this.prefix}.makeRowEditable(${row.metadata.index})" > ` +
            //`<span class="icofont icofont-ui-edit"></span> </a> ` +
            `<a  class="tabledit-delete-button btn btn-danger waves-effect waves-light" style="float: none; margin: 0px; color: white;" ` +
            `id="deleteBtn" data-toggle="modal" data-target="#confirmation-Modal" data-index="${row.metadata.index}" data-prefix="${this.prefix}" data-id="${row.metadata.getId(row.metadata.index)}"  title="Supprimer">` +
            `<span class="icofont icofont-ui-delete"></span></a></div></div>`;
    },

    makeEditButtons: function (index) {

        return `<div class="tabledit-toolbar btn-toolbar" style="text-align: left;"><div class= "btn-group btn-group-sm" style="float: none; padding: 5px;">` +
            //`<a class="tabledit-edit-button btn btn-primary waves-effect waves-light" ` +
            //`style="float: none; margin: 0px; color: white; background-color: #0ac282;" title="Valider" onclick="window.mydatatables.${this.prefix}.saveRowEditable(${index})" > ` +
            //`<span class="icofont icofont-ui-check"></span> </a> ` +
            `<a  class="tabledit-delete-button btn btn-danger waves-effect waves-light alert-confirm" style="float: none; margin: 0px; color: white;" ` + 
            ` title="Supprimer" data-toggle="modal" data-target="#confirmation-Modal" data-index="${index}" data-prefix="${this.prefix}">` +
            `<span class="icofont icofont-ui-delete"></span></a></div></div>`;
    },
    //<button type="button" class="btn btn-warning alert-confirm m-b-10" onclick="_gaq.push(['_trackEvent', 'example', 'try', 'alert-confirm']);">Confirm</button>

    makeAddButtons: function (index) {

        return `<div class="tabledit-toolbar btn-toolbar" style="text-align: left;"><div class= "btn-group btn-group-sm" style="float: none; padding: 5px;">` +
            //`<a class="tabledit-edit-button btn btn-primary waves-effect waves-light" ` +
            //`style="float: none; margin: 0px; color: white" title="Valider" onclick="window.mydatatables.${this.prefix}.saveRowAdd(${index})" > ` +
            //`<span class="icofont icofont-ui-check"></span> </a> ` +
            `<a  class="tabledit-delete-button btn btn-danger waves-effect waves-light" style="float: none; margin: 0px; color: white;" ` +
            ` title="Annuler" onclick="window.mydatatables.${this.prefix}.cancelRowAdd(${index})">` +
            `<span class="icofont icofont-ui-close"></span></a></div></div>`;

    },

    makeInput: function (obj) {
        return `<input class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available; height: 33px;\" type=\'text' id=\'${obj.id}\' name=\'${obj.name}\' value=\'${obj.data}\'>`;
    },

    makeInputAdd: function (obj) {
        return `<input class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available; height: 33px;\" type=\'text' id=\'${obj.idadd}\' name=\'${obj.name}\' value=\'${obj.data}\'>`;
    },

    makeTextArea: function (obj) {
        return `<textarea class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available;\" type=\'text\' id=\'${obj.id}\' name=\'${obj.name}\' rows=\'2\' cols=\'20\'>${obj.data}</textarea>`;
    },

    makeTextAreaAdd: function (obj) {
        return `<textarea class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available;\" type=\'text\' id=\'${obj.idadd}\' name=\'${obj.name}\' rows=\'2\' cols=\'20\'>${obj.data}</textarea>`;
    },

    makeSelect: function (obj) {
        var options = '';
        var optionEmpty = '';
        var selected = false;
        var selected_ = '';
        //try {
        var listData = this.lists[obj.properties.list.name];

        if (obj.properties.list != '') {
            for (var i = 0; i < listData.data.length; i++) {
                var option = listData.data[i];
                selected_ = '';

                var value = this.resolve(obj.properties.list.valueMember, option);
                var text = this.resolve(obj.properties.list.displayMember, option);
                if (value == obj.original && !selected) {
                    selected = true;
                    selected_ = 'selected';
                }

                options += `<option value="${value}" ${selected_}>${text}</option>`;
            }
        }

        if (listData.hasEmpty) {
            var selected_ = !selected ? 'selected' : '';

            optionEmpty = `<option value="${listData.emptyValue}" ${selected_}>${listData.emptyText}</option>`;
        }
        options = optionEmpty + options;
        //} catch (e) {

        //}
        return `<select class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available; height: 33px;\" name=\'${obj.name}\'  id=\'${obj.id}\'>${options}</select>`;
    },

    makeSelectAdd: function (obj) {
        var options = '';
        var optionEmpty = '';
        var selected = false;
        var selected_ = '';
        //try {
        var listData = this.lists[obj.properties.list.name];

        if (obj.properties.list != '') {
            for (var i = 0; i < listData.data.length; i++) {
                var option = listData.data[i];

                var value = this.resolve(obj.properties.list.valueMember, option);
                var text = this.resolve(obj.properties.list.displayMember, option);
                selected_ = false ? 'selected' : '';

                options += `<option value="${value}" ${selected_}>${text}</option>`;
            }
        }

        if (listData.hasEmpty) {
            var selected_ = !selected ? 'selected' : '';

            optionEmpty = `<option value="${listData.emptyValue}" ${selected_}>${listData.emptyText}</option>`;
        }
        options = optionEmpty + options;
        //} catch (e) {

        //}
        return `<select class=\'${obj.properties.class}\' style=\"width: -webkit-fill-available; height: 33px;\" name=\'${obj.name}\'  id=\'${obj.idadd}\'>${options}</select>`;
    },

    makeJson: function (data, type, full, meta) {
        var obj = {
            data
            //type:type,
            //full:full,
            //meta: meta
        };
        return this.addSlashes(JSON.stringify(obj).replace(/"/g, '\''));
    },

    addSlashes: function (str) {
        return str.replace(/\'/g, "\\\'");
    },

    deleteSlashes: function (s) {
        s = s.replace(/\\n/g, "\\n")
            .replace(/\\'/g, "\\'")
            .replace(/\\"/g, '\\"')
            .replace(/\\&/g, "\\&")
            .replace(/\\r/g, "\\r")
            .replace(/\\t/g, "\\t")
            .replace(/\\b/g, "\\b")
            .replace(/\\f/g, "\\f");
        // remove non-printable and other non-valid JSON chars
        s = s.replace(/[\u0000-\u0019]+/g, "");
        s = s.replaceAll('\'', '"');
        return s;
    },
    makeAllRowsEditable: function () {
        for (var i = 0; i < this.dataList.length; i++) {
            var row = this.dataList[i];
            for (const property in row) {
                if (property != "metadata") {
                    row[property] = this.switchToEdit(row, property, i);
                }
            }

            this.dataList[i] = row;
            var current = this.table.page();
            this.reDraw();
            this.table.page(current).draw('page');
            //this.table.page('previous').draw('page');
            validateChanges = false;
            //this.table.rowReorder.disable();
        }
    },
    reDraw: function (withRefrech = false) {
        if (withRefrech) {
            this.refreshData();
        }
        this.table.clear().draw();
        this.table.rows.add(this.dataList); // Add new data
        this.table.columns.adjust().draw(); // Redraw the DataTable
    },

    reDrawRow: function (index, deleted = false) {
        if (deleted) {
            this.table.row(index).remove().draw(true);
        } else {
            this.table.row(index).data(this.dataList[index]).draw(true);
        }
    },

    toCamelCase: function (str) {
        return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
            if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
            return index === 0 ? match.toLowerCase() : match.toUpperCase();
        });
    },

    resolve: function (path, obj = self, separator = '.') {
        var properties = Array.isArray(path) ? path : path.split(separator);
        return properties.reduce((prev, curr) => prev && prev[curr], obj);
    }
};

function myDataTableFactory(config) {
    const datatable = Object.assign(clone(default_my_data_table), config);
    if (!window.mydatatables) {
        window.mydatatables = {};
    }
    window.mydatatables[datatable.prefix] = datatable;

    return datatable;
}




