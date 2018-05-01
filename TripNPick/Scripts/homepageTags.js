$(document).ready(function () {
    var count_checked = $('.dropdown').find('input:checkbox:checked').length;

    if (count_checked == 0) {
        $(".tag-list span").hide();
    }

    $('.dropdown').find('input:checkbox').change(function () {
        if ($('.dropdown').find('input:checkbox:checked').length) {
            $('.tag-list span').hide();

            $('.dropdown').find('input:checkbox:checked').each(function () {
                $('.tag-list span[data-id*="' + $(this).attr('id') + '"]').show();
            });

        } else if (count_checked == 0) {
            $(".tag-list span").hide();
        }
    });


    $('.tag-list').find('span').click(function () {

        $('.dropdown').find('input:checkbox:checked[id=' + $(this).attr('data-id') + ']').prop("checked", false);

        $(this).hide();

    });
});