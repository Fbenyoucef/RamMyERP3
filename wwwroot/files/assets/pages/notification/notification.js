
    //function notify(from, align, icon, type, animIn, animOut){
    //    $.growl({
    //        icon: icon,
    //        title: ' Bootstrap Growl ',
    //        message: 'Turning standard Bootstrap alerts into awesome notifications',
    //        url: ''
    //    },{
    //        element: 'body',
    //        type: type,
    //        allow_dismiss: true,
    //        placement: {
    //            from: from,
    //            align: align
    //        },
    //        offset: {
    //            x: 30,
    //            y: 30
    //        },
    //        spacing: 10,
    //        z_index: 999999,
    //        delay: 2500,
    //        timer: 1000,
    //        url_target: '_blank',
    //        mouse_over: false,
    //        animate: {
    //            enter: animIn,
    //            exit: animOut
    //        },
    //        icon_type: 'class',
    //        template: '<div data-growl="container" class="alert" role="alert">' +
    //        '<button type="button" class="close" data-growl="dismiss">' +
    //        '<span aria-hidden="true">&times;</span>' +
    //        '<span class="sr-only">Close</span>' +
    //        '</button>' +
    //        '<span data-growl="icon"></span>' +
    //        '<span data-growl="title"></span>' +
    //        '<span data-growl="message"></span>' +
    //        '<a href="#" data-growl="url"></a>' +
    //        '</div>'
    //    });
    //};




function notify(title, message, type, icon = '', from = 'top', align = 'center', animIn = '', animOut = '') {
    $.growl({
        icon: type == "danger" ? "icofont icofont-ui-delete" : "icofont icofont-info",
        title: title,
        message: message,
        url: ''
    }, {
        element: 'body',
        type: type,
        allow_dismiss: true,
        placement: {
            from: from,
            align: align
        },
        offset: {
            x: 30,
            y: 30
        },
        spacing: 10,
        z_index: 999999,
        delay: 2500,
        timer: 2000,
        url_target: '_blank',
        mouse_over: false,
        animate: {
            enter: animIn,
            exit: animOut
        },
        icon_type: 'class',
        template: '<div data-growl="container" class="alert" role="alert" style="max-width : 50%;">' +
            '<button type="button" class="close" data-growl="dismiss">' +
            '<span aria-hidden="true">&times;</span>' +
            '<span class="sr-only">Close</span>' +
            '</button>' +
            '<span data-growl="icon" style="margin-right: 15px;"></span>' +
            '<span data-growl="title" style="font-weight: bold; margin-right: 5px;"></span>' +
            '<span data-growl="message"></span>' +
            '<a href="#" data-growl="url"></a>' +
            '</div>'
    });
};