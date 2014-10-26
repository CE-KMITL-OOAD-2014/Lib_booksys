$(document).ready(function () {
    $.ajaxSetup({
        async: false
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

function getBookByCallNo(callno) {
    var received_data;
    $.getJSON("/api/BookQuery/?callno=" + callno).done(function (data) {
        received_data = data;
    }).fail(function () {
        received_data = null;
    });
    return received_data;
}


function getBookByAllProperties(callno,name,author,publisher,year) {
    var received_data;

    var target = new Object();
    target.BookName = name;
    target.Author = author;
    target.Publisher = publisher;
    target.Year = year;
    target.CallNumber = callno;
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