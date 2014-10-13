function resizewindow() {
    if (window.innerWidth < 785) {
        $("ul.menulist").hide();
        $("img.menu-btt").show();
    }
    else {
        $("img.menu-btt,div.menu-slider").hide();
        $("ul.menulist").show();
    }
}
$(document).ready(function () {
    $("div#content").css("max-width", 930);
    $("div#header,div#menubar").css("max-width", 960);
    resizewindow();
    $("img.back-to-top-btt,div.menu-slider").hide();
    $(window).resize(resizewindow);
    $("img.menu-btt").click(function () {
        $("div.menu-slider").slideToggle("300");
    });
    $("img.delete-btt").mouseover(function () {
        $(this).attr("src", "/Content/delete-black-btt.png");
    });
    $("img.delete-btt").mouseout(function () {
        $(this).attr("src", "/Content/delete-gray-btt.png");
    });
    $("img.edit-btt").mouseover(function () {
        $(this).attr("src", "/Content/edit-black-btt.png");
    });
    $("img.edit-btt").mouseout(function () {
        $(this).attr("src", "/Content/edit-gray-btt.png");
    });
    $("img.view-btt").mouseover(function () {
        $(this).attr("src", "/Content/view-black-btt.png");
    });
    $("img.view-btt").mouseout(function () {
        $(this).attr("src", "/Content/view-gray-btt.png");
    });

    $(window).scroll(function () {
        if ($(this).scrollTop() > 30)
            $("img.back-to-top-btt").fadeIn();
        else
            $("img.back-to-top-btt").fadeOut();
    });
    $("img.back-to-top-btt").click(function () {
        $("html").animate({ scrollTop: 0 }, "500", "easeOutExpo");
    });
    $("select.autosubmit").change(function () {
        $(this).parent().submit();
    });
});