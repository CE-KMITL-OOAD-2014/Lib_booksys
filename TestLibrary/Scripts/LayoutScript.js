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
    var str = '<ul><li><a href="/">Home</a></li><li><a href="/BookSearch/">Search</a></li><li><a href="/Home/TopTen">Top 10</a></li> <li><a href="/Home/About">About Us</a></li><li><a href="/Home/Contact">Contact</a></li><li><a href="/Home/ChangeLog">Change log</a></li><li class="last-menu"><a href="/Home/LibraryApi">API</a></li></ul>';
    $("div#content").css("max-width", 930);
    $("div#header,div#menubar").css("max-width", 960);
    $("div#menubar").prepend('<img class="menu-btt"/>');
    resizewindow();
    $("img.back-to-top-btt,div.menu-slider").hide();
    $("img.menu-btt").attr("src", "/Content/menu.png");
    $("div.menu-slider").append(str);
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
    $("img.close-btt").click(function () {
        $(this).parent().remove();
    })
});