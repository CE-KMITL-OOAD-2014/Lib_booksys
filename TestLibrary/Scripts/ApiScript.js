$(document).ready(function () {
    $.ajaxSetup({
        async: false
    });
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

function viewBookDetail(id) {
    if (id == "")
        return null;
    var received_data;
    $.getJSON("/api/BookQuery/" + id)
        .done(function (data) {
            received_data = data;
        }).fail(function () {
            received_data = null;
        });
    return received_data;
}

function getAllBook() {
    var received_data;
    $.getJSON("/api/BookQuery/").done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookByName(name) {
    var received_data;
    $.getJSON("/api/BookQuery/?name="+name).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookByAuthor(author) {
    var received_data;
    $.getJSON("/api/BookQuery/?author="+author).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookByPublisher(publisher) {
    var received_data;
    $.getJSON("/api/BookQuery/?publisher=" + publisher).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookByYear(year) {
    var received_data;
    $.getJSON("/api/BookQuery/?year=" + year).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookByAllProperties(name,author,publisher,year) {
    var received_data;

    var target = new Object();
    target.BookName = name;
    target.Author = author;
    target.Publisher = publisher;
    target.Year = year;

    $.ajax({
        type: "POST",
        url: "/api/BookQuery/",
        data: JSON.stringify(target),
        contentType: "application/json"
    }).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}

function getBookStatus(value) {
    switch (value) {
        case 0: return "Available";
        case 1: return "Borrowed";
        case 2: return "Reserved";
        case 3: return "Lost";
        default: return null;
    }
}