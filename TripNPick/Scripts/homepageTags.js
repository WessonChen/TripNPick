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

    var scrollTop;
    $('#HomeDD').on("select2:selecting", function (event) {
        var $pr = $('#' + event.params.args.data._resultId).parent();
        scrollTop = $pr.prop('scrollTop');
    });
    $('#HomeDD').on("select2:select", function (event) {
        var $pr = $('#' + event.params.data._resultId).parent();
        $pr.prop('scrollTop', scrollTop);
    });
});