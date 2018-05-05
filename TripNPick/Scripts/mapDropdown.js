$(document).ready(function () {
    $("#monthDD").select2({
        closeOnSelect: false,
        placeholder: "Tell us when you want to travel..",
        allowClear: true
    });

    $("#monthDD").on("select2:unselect", function (evt) {
        if (!evt.params.originalEvent) {
            return;
        }

        evt.params.originalEvent.stopPropagation();
    });

    $("#interetsDD").select2({
        closeOnSelect: false,
        placeholder: "Tell us about your interests and what you want to do while fruitpicking..",
        allowClear: true
    });

    $("#interetsDD").on("select2:unselect", function (evt) {
        if (!evt.params.originalEvent) {
            return;
        }

        evt.params.originalEvent.stopPropagation();
    });
});