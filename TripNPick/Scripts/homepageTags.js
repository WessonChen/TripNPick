$(document).ready(function () {
    $("#HomeDD").select2({
        closeOnSelect: false,
        placeholder: "When you want to travel? (Multichoice)",
        allowClear: true
    });

    $("#HomeDD").on("select2:unselect", function (evt) {
        if (!evt.params.originalEvent) {
            return;
        }

        evt.params.originalEvent.stopPropagation();
    });
});