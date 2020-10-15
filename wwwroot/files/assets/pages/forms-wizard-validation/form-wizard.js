  'use strict';
  $(document).ready(function() {
      var form = $("#example-advanced-form").show();

      //récuperer les données
      var collaborateurID = $("#collaborateurID").val();
      var collaborateurID = $("#collaborateurID").val();
      var collaborateurID = $("#collaborateurID").val();
      var collaborateurID = $("#collaborateurID").val();
      var collaborateurID = $("#collaborateurID").val();
         var collaborateurID = $("#collaborateurID").val();




      form.steps({
          headerTag: "h3",
          bodyTag: "fieldset",
          transitionEffect: "slideLeft",
          onStepChanging: function (event, currentIndex, newIndex) {
              console.log("collaborateurID "+ collaborateurID);
              return form.valid();
          },
          onStepChanged: function(event, currentIndex, priorIndex) {

              // Used to skip the "Warning" step if the user is old enough.
              if (currentIndex === 2 && Number($("#age-2").val()) >= 18) {
                  form.steps("next");
              }
              // Used to skip the "Warning" step if the user is old enough and wants to the previous step.
              if (currentIndex === 2 && priorIndex === 3) {
                  form.steps("previous");
              }
          },
          onFinishing: function(event, currentIndex) {

              form.validate().settings.ignore = ":disabled";
              return form.valid();
          },
          onFinished: function(event, currentIndex) {
              alert("Submitted!");
              $('.content input[type="text"]').val('');
              $('.content input[type="email"]').val('');
              $('.content input[type="password"]').val('');
          }
      }).validate({
          errorPlacement: function errorPlacement(error, element) {

              element.before(error);
          },
          rules: {
              confirm: {
                  equalTo: "#password-2"
              }
          }
      });
  });
