$(document).on('click', '.menuItem', function () {
    console.log("kliknuto");
    $(".menuItem").each(function () {
        $(this).removeClass("active");
    });
    $(this).addClass("active");
})