// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleConfirmDeleteUserButton(uniqueId, isDeleteClicked) {
    var deleteSpan = "deleteSpan_" + uniqueId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + uniqueId;
    var animationTime = 100;

    $("[id^=confirmDeleteSpan_]").each(function() {
        $("[id^=deleteSpan_]").show();
        $("[id^=confirmDeleteSpan_]").hide();
    });

    if (isDeleteClicked) {
        $("#" + deleteSpan).hide(animationTime);
        $("#" + confirmDeleteSpan).show(animationTime);
    } else {
        $("#" + deleteSpan).show(animationTime);
        $("#" + confirmDeleteSpan).hide(animationTime);
    }
}

function confirmDeleteUserAjax(userId, userNum, userEmail) {
    $.ajax({
            type: "POST",
            url: "Administration/DeleteUser",
            data: {
                id: userId
            },
            success: function() {
                $.notify(userEmail + " deleted", { color: "#fff", background: "#D44950", position: "top center" });

                $(".user" + userNum).hide('slow', function() { $(".user" + userNum).remove(); });
            },
            failure: function(response) {
                alert(response.text);
            },
            error: function(response) {
                alert(response.text);
            }
        }
    );
}
/*

$("#searchSubmit").click(function() {
    searchUsers($("#searchUserString").val());
});

function searchUsers(searchString) {
    $.ajax({
            type: "POST",
            url: "Administration/ListUsers",
            data: {
                searchString: searchString
            },
            success: function(data, textStatus, jqXHR) {
                console.log("data:\n" + data + "\ntextStatus:\n" + textStatus + "\njqXHR:\n" + jqXHR.toString());
            },
            complete: function(response) {
                console.log(response);

            },
            failure: function(response) {
                console.log(response);
            },
            error: function(response) {
                console.log(response);
            }
        }
    );
}*/