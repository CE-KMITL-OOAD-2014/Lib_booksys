$(document).ready(function () {
    $("table.list").on("click", "tr.test button.view-btt", function () {
        view($(this).val());
    });

    $.getJSON("/api/BookQuery").done(function (data) {
        $.each(data, function (key, item) {
            $("table.list").append("<tr class=\"test\"><td>" + item.BookID + "</td><td>"
            + item.BookName + "</td><td>" + item.Author + "</td>"
            + "<td class=\"btt\"><button class=\"view-btt\" value=\"" + item.BookID + "\">View</button></td></tr>");
        }).fail(function (jQxSR, error, err) {
            $("#test").append(err);
        });
    });
});


function findBookByName() {
    $.getJSON("/api/BookQuery/?name=" + $("input[name='bookname']").val())
        .done(function (data) {
            $("table.list").show();
            $("table.list tr.test").remove();
            $("p.noti-msg").text("");
            $.each(data, function (key, item) {
                $("table.list").append("<tr class=\"test\"><td>" + item.BookID + "</td><td>"
                + item.BookName + "</td><td>" + item.Author + "</td>"
                + "<td><button class=\"view-btt\" value=\"" + item.BookID + "\">View</button></td></tr>");
            });
        })
        .fail(function (JqxCR, error, err) {
            $("table.list").hide();
            $("p.noti-msg").text(err);
        });

}

function view(id) {
    $.getJSON("/api/BookQuery/" + id)
        .done(function (data) {
            $("div.detail").children().remove();
            $("div.detail").append("<p><b>" + data.BookID + " " + data.BookName + "<br>Detail:" + data.Detail + "<br>Year:" + data.Year + "</p>");
        });
}

function findAuthor() {
    var str = $("#in2").val();
    $.post("/api/BookQuery/", { '': str }).fail(function (jQxCR, error, err) {
        $("p.noti-msg").text(err);
    }).done(function (data) {
        $("p.noti-msg").text("");
        $("table.list tr.test").remove();
        $.each(data, function (key, item) {
            $("table.list").append("<tr class=\"test\"><td>" + item.BookID + "</td><td>"
            + item.BookName + "</td><td>" + item.Author + "</td>"
            + "<td><button class=\"view-btt\" value=\"" + item.BookID + "\">View</button></td></tr>");
        });
    });
}

function findBook() {
    var target = new Object();
    target.BookName = $("input[name='bookname']").val();
    target.Author = $("input[name='author']").val();
    target.Publisher = $("input[name='publisher']").val();
    target.Year = $("input[name='year']").val();

    $.ajax({
        type: "POST",
        url: "/api/BookQuery/",
        data: JSON.stringify(target),
        contentType: "application/json"
    }).done(function (data) {
        $("p.noti-msg").text("");
        $("table.list tr.test").remove();
        $("table.list").show();
        $.each(data, function (key, item) {
            $("table.list").append("<tr class=\"test\"><td>" + item.BookID + "</td><td>"
            + item.BookName + "</td><td>" + item.Author + "</td>"
            + "<td><button class=\"view-btt\" value=\"" + item.BookID + "\">View</button></td></tr>");
        });
    }).fail(function (JqrdR, error, err) {
        $("table.list").hide();
        $("p.noti-msg").text(err);
    });
}