// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleConfirmDeleteUserButton(uniqueId, isDeleteClicked) {
    console.log("Entered function toggleConfirmDeleteUserButton with uniqueId:" + uniqueId + "and isDeleteClicked: " + isDeleteClicked+"\n");

    var deleteSpan = "deleteSpan_" + uniqueId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + uniqueId;
    var animationTime = 300;

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
    console.log("Entered function confirmDeleteUserAjax with:"+userId+" "+userNum+" "+userEmail+"\n");
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


$("#searchSubmit").click(function () {
    var searchString = $("#searchUserString").val();
    searchUsers(searchString);
});

function searchUsers(searchUserString) {
    var usersHtml = "";
    $.ajax({
            type: "POST",
        url: "Administration/ListUsersBySearch",
            data: {
                searchUserString: searchUserString
            },
        success: function (data, textStatus, jqXhr) {
            /*var dvUsers = $("#users");
            dvUsers.empty();
            $.each(data, function(i, user) {
                    var $tr
                });*/
                for (var userIndex = 0; userIndex < Object.keys(data).length; userIndex++) {
                    usersHtml += '<div class="card mb-3 user'+ userIndex+'">\n';
                    usersHtml +=     '<div class="card-header">\n';
                    usersHtml +=        'User Id: '+data[userIndex].id;
                    usersHtml +=     '</div>\n';
                    usersHtml +=     '<div class="card-body">\n';
                    usersHtml +=        '<h5 class="card-title">\n' + data[userIndex].email+'</h5>\n';
                    usersHtml +=     '</div>\n';
                    usersHtml +=     '<div class="card-footer">\n';
                    usersHtml +=        '<a href="/Administration/EditUser/'+data[userIndex].id+'" class="btn btn-primary">\nEdit</a>\n';
                    usersHtml +=        '<span id="confirmDeleteSpan_'+data[userIndex].id+'" style="display: none">\n';
                    usersHtml +=        '<span>\nAre you sure you want to delete?</span>\n';
                    usersHtml +=          '<input type="button" onclick="confirmDeleteUserAjax(' + "'" + data[userIndex].id + "'" + ',' + "'" + (userIndex + 1) + "'" + ',' + "'" + data[userIndex].email+ "'" +')" class="btn btn-danger" value="Yes">\n';
                    usersHtml +=          '<a href="javascript:void(0)" class="btn btn-primary" onclick="toggleConfirmDeleteUserButton('+"'"+data[userIndex].id+"'"+', false)">\n No </a>\n';
                    usersHtml +=        '</span>\n';
                    usersHtml +=        '<span id="deleteSpan_'+data[userIndex].id+'">\n';
                    usersHtml +=          '<a href="javascript:void(0)" class="btn btn-danger" onclick="toggleConfirmDeleteUserButton(' + "'" + data[userIndex].id + "'" +', true)">\n Delete </a>\n';
                    usersHtml +=        '</span>\n';
                    usersHtml +=     '</div>\n';
                    usersHtml += '</div>\n';
                }
            $("#users").html(usersHtml);
            console.log("success: ");

        },
            complete: function(response) {
                console.log("completed: " + response);
            },
            failure: function(response) {
                console.log("failure: "+response);
            },
            error: function(response) {
                console.log("error: "+response);
            }
        }
    );
 }