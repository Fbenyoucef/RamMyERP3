$(document).ready(function () {

    $("#minimizeSynthese").on("click", function () {
        var $this = $(this);
        var port = $($this.parents('.card.syntheseContainer'));
        var card = $(port).children('.card-body').slideToggle();
        $(this).toggleClass("icon-minus").fadeIn('slow');
        $(this).toggleClass("icon-plus").fadeIn('slow');
    });

    $('#Ajout').on("click", function (event) {
        var JoursTravailles = $('#JoursTravailles').val();
        var Anneemois = $('#AnneeMois');
        console.log("JoursTravailles " + JoursTravailles);
        if (Anneemois.val() == "") {
            Anneemois.css("border-color", "Red");
            event.preventDefault();
            var message = "Merci de choisir le mois";
            $.notify({ message: message }, { type: "danger" });
        }

        var sum = 0.0;
        var quantity = 0;
        $('.Combo').each(function (i, value) {
            var td = $(value);
            var val = td.find("select").find("option:selected").text().replace(",", ".");
            let intval = parseFloat(val);
            sum += intval;
        });
        console.log("total " + sum);
        // alert("le total =" + sum);
        $('#total').html(sum);
        // $('#total_quantity').html(quantity);
        if (sum < JoursTravailles) {
            event.preventDefault();
            var message = "Merci de remplir toutes les cellules du tableau, manque : " + (JoursTravailles - sum) + " jours";
            $.notify({ message: message }, { type: "danger" });
        }
        else {
            if (sum > JoursTravailles) {
                event.preventDefault();
                var message = "Merci de verifier votre saisie, trop de jours saisie : " + (sum - JoursTravailles) + " jours";
                $.notify({ message: message }, { type: "danger" });
            }
        }



    });
    gererInformationRam();
    function gererInformationRam() {
        mode = $('.mode').val();
        input = $('input');
        if (mode == 'update') {
            $("#AnneeMois").attr('disabled', true);
            $("#DATE_SIGNATURE").attr('disabled', true);
            $("#COMMENTAIRE").attr('disabled', true);
            $('#signature').attr('show', 'true');

        }
        else {
            $('#JOURS_TRAVAILLES').attr('value', '0.0');
            $('#JOURS_ABSENCE').attr('value', '0.0');
            $('#total').attr('value', '0.0');
            $('#signature').attr('hidden', true);
        }
    }
});
//$(document).ready(function () {

//    mode = $('.mode').val();
//    input = $('input');
//    if (mode == 'update') {
//        input.prop('disabled', true);
//    }
//    else {
//        $('#JOURS_TRAVAILLES').attr('value', '0.0');
//        $('#JOURS_ABSENCE').attr('value', '0.0');
//        $('#total').attr('value', '0.0');
//    }
//});
