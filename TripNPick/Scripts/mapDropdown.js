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

    var scrollTop1;
    $('#monthDD').on("select2:selecting", function (event) {
        var $pr = $('#' + event.params.args.data._resultId).parent();
        scrollTop1 = $pr.prop('scrollTop');
    });
    $('#monthDD').on("select2:select", function (event) {
        var $pr = $('#' + event.params.data._resultId).parent();
        $pr.prop('scrollTop', scrollTop1);
    });

    $("#interetsDD").select2({
        closeOnSelect: false,
        placeholder: "Tell us what you like..",
        allowClear: true
    });

    $("#interetsDD").on("select2:unselect", function (evt) {
        if (!evt.params.originalEvent) {
            return;
        }

        evt.params.originalEvent.stopPropagation();
    });

    var scrollTop2;
    $('#interetsDD').on("select2:selecting", function (event) {
        var $pr = $('#' + event.params.args.data._resultId).parent();
        scrollTop2 = $pr.prop('scrollTop');
    });
    $('#interetsDD').on("select2:select", function (event) {
        var $pr = $('#' + event.params.data._resultId).parent();
        $pr.prop('scrollTop', scrollTop2);
    });
});