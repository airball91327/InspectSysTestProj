$(document).ready(function () {
    //Default class content is class 1.
    document.getElementById("1").click();

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
        url: "/InspectDocDetails/ClassContentOfArea",
        data: { ACID: acid, DocID: docID },
        success: function (result) {
            console.log(result); //For debug
            $("#tabContent").html(result);
        },
        error: function (msg) {
            alert(msg);
        }
    });
}