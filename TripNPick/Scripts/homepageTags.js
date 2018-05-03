$(document).ready(function () {
    $("#HomeDD").select2({
        closeOnSelect: false,
        placeholder: "Tell us when you want to travel..",
        allowClear: true
    });

    $("#HomeDD").on("select2:unselect", function (evt) {
        if (!evt.params.originalEvent) {
            return;
        }

        evt.params.originalEvent.stopPropagation();
    });
});