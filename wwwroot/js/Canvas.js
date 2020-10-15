{ // https://github.com/Microsoft/TypeScript/wiki/JSDoc-support-in-JavaScript

    /** @type {HTMLElement} */
    /** @type {number[]} */ // array types
    /** @type {{ a: string, b: number }} */
    /** @type {function(string, boolean): number} Closure syntax */

    // Parameters may be declared in a variety of syntactic forms
    /**
     * @param {string}  p1 - A string param.
     * @param {string=} p2 - An optional param (Closure syntax)
     * @param {string} [p3] - Another optional param (JSDoc syntax).
     * @param {string} [p4="test"] - An optional param with a default value
     * @return {string} This is the result
     */
    // function stringsStringStrings(p1, p2, p3, p4){ // TODO  }

    /**  @param {string}  p1 - A string param.  */
    /**  @param {Toto}  p1 - A Toto param.  */
}


class CanvasObjet {
    constructor() { //Paramètres du canvas
        this.canvas = document.getElementById("canvas");
        this.ctx = this.canvas.getContext('2d');
        this.ctx.strokeStyle = '#000000';
        this.ctx.lineWidth = 3;
        this.draw = false;
        this.mousePosition = {
            x: 0,
            y: 0
        };
        this.lastPosition = this.mousePosition;
        this.clearButton = document.getElementById("bt-clear");
        this.canvas.width = 200;
        this.canvas.height = 150;
    }

    //Gestion des événements 
    evenements() {
        var self = this;
        //Souris
        this.canvas.addEventListener("mousedown", function (e) {
            self.draw = true;
            self.lastPosition = self.getMposition(e);
        });

        this.canvas.addEventListener("mousemove", function (e) {
            self.mousePosition = self.getMposition(e);
            self.canvasResult()
        });

        // this.canvas.addEventListener("mouseup", function (e) {
        //     self.draw = false;
        // });
        document.addEventListener("mouseup", function (e) {
            self.draw = false;
        });


        // Stop scrolling (touch)
        document.body.addEventListener("touchstart", function (e) {
            if (e.target == self.canvas) {
                e.preventDefault();
            }
        });

        document.body.addEventListener("touchend", function (e) {
            if (e.target == self.canvas) {
                e.preventDefault();
            }
        });

        document.body.addEventListener("touchmove", function (e) {
            if (e.target == self.canvas) {
                e.preventDefault();
            }
        });


        // Touchpad
        this.canvas.addEventListener("touchstart", function (e) {
            self.mousePosition = self.getTposition(e);
            var touch = e.touches[0];
            var mouseEvent = new MouseEvent("mousedown", {
                clientX: touch.clientX,
                clientY: touch.clientY
            });
            self.canvas.dispatchEvent(mouseEvent);
        });

        this.canvas.addEventListener("touchmove", function (e) {
            var touch = e.touches[0];
            var mouseEvent = new MouseEvent("mousemove", {
                clientX: touch.clientX,
                clientY: touch.clientY
            });
            self.canvas.dispatchEvent(mouseEvent);
        });

        this.canvas.addEventListener("touchend", function (e) {
            var mouseEvent = new MouseEvent("mouseup", {});
            self.canvas.dispatchEvent(mouseEvent);
        });


        //Effacer
        this.clearButton.addEventListener("click", function (e) {
            self.clearCanvas()
        });
    }

    // Renvoie les coordonnées de la souris 
    getMposition(mouseEvent) {
        if (this.draw) {
            var oRect = this.canvas.getBoundingClientRect();
            return {
                x: mouseEvent.clientX - oRect.left,
                y: mouseEvent.clientY - oRect.top
            };
        }
    }

    // Renvoie les coordonnées du pad 
    getTposition(touchEvent) {
        var oRect = this.canvas.getBoundingClientRect();
        return {
            x: touchEvent.touches[0].clientX - oRect.left,
            y: touchEvent.touches[0].clientY - oRect.top
        };
    }

    // Dessin du canvas
    canvasResult() {
        if (this.draw) {
            this.ctx.beginPath();
            this.ctx.moveTo(this.lastPosition.x, this.lastPosition.y);
            this.ctx.lineTo(this.mousePosition.x, this.mousePosition.y);
            this.ctx.stroke();
            this.lastPosition = this.mousePosition;
        }
    };

    // Vide le dessin du canvas
    clearCanvas() {
        this.canvas.width = this.canvas.width;
        this.ctx.lineWidth = 3;
    }

}
$(document).ready(function () {
    $("#btnSave").click(function () {

        var image = document.getElementById("canvas").toDataURL("image/png");
        image = image.replace('data:image/png;base64,', '');

        var id = $("#ID").val();

        var ImageSend = new Object();
        ImageSend.Image = image;

        ImageSend.id = id;
        var paramSent = { "Image": ImageSend.Image, "id": ImageSend.id };
        $.ajax({
            type: 'POST',
            url: "/Rams/UploadImage",
            data: JSON.stringify(paramSent),
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                $('#SignerRam').modal('hide');
            }
        });
    });
})

var obj = new CanvasObjet();
obj.evenements();
