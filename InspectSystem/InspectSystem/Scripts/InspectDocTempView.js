$(document).ready(function () {
    //Default class content is first item from tablinks.
    var tabs = document.getElementsByClassName("tablinks");
    tabs.item(0).click();

    // When the user scrolls down 20px from the top of the document, show the button
    window.onscroll = function () {
        scrollFunction();
    };
});

function openClassContent(evt, acid, docID) {
    // Declare all variables
    var i, tablinks;

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Add an "active" class to the link button clicked.
    evt.currentTarget.className += " active";

    $.ajax({
        type: "GET",
        url: "/InspectDocTempView/ClassContentOfArea",
        data: { ACID: acid, DocID: docID },
        beforeSend: function () {
            $("#loadingModal").modal("show");
        },
        complete: function () {
            $("#loadingModal").modal("hide");
        },
        success: function (result) {
            console.log(result); //For debug
            $("#tabContent").html(result);
        },
        error: function (msg) {
            alert("讀取錯誤");
        }
    });
}

function scrollFunction() {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        document.getElementById("goToTopBtn").style.display = "block";
    } else {
        document.getElementById("goToTopBtn").style.display = "none";
    }
}

// When the user clicks on the button, scroll to the top of the document
function topFunction() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}