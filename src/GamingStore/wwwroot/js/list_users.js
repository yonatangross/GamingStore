
function toggleConfirmDeleteUserButton(uniqueId, isDeleteClicked) {
    console.log(
        `Entered function toggleConfirmDeleteUserButton with uniqueId:${uniqueId}and isDeleteClicked: ${isDeleteClicked
        }\n`);

    const deleteSpan = `deleteSpan_${uniqueId}`;
    const confirmDeleteSpan = `confirmDeleteSpan_${uniqueId}`;
    const animationTime = 300;

    $("[id^=confirmDeleteSpan_]").each(function() {
        $("[id^=deleteSpan_]").show();
        $("[id^=confirmDeleteSpan_]").hide();
    });

    if (isDeleteClicked) {
        $(`#${deleteSpan}`).hide(animationTime);
        $(`#${confirmDeleteSpan}`).show(animationTime);
    } else {
        $(`#${deleteSpan}`).show(animationTime);
        $(`#${confirmDeleteSpan}`).hide(animationTime);
    }
}

function confirmDeleteUserAjax(userId, userNum, userEmail) {
    console.log(`Entered function confirmDeleteUserAjax with:${userId} ${userNum} ${userEmail}\n`);
    $.ajax({
            type: "POST",
            url: "/Administration/DeleteUser",
            data: {
                id: userId
            },
            success: function() {
                $.notify(userEmail + " deleted", { color: "#fff", background: "#D44950", position: "top center" });

                $(`.user${userNum}`).hide("slow", function() { $(`.user${userNum}`).remove(); });
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

$("#searchSubmit").click(function() {
    const searchString = $("#searchUserString").val();
    searchUsers(searchString);
});


const card = (id, email, userIndex) => `
 <div class="card mb-3 user${userIndex}">
                <div class="card-header">
                    User Id : ${id}
                </div>
                <div class="card-body">
                    <h5 class="card-title">${email}</h5>
                </div>
                <div class="card-footer">
                    <a href="/Administration/EditUser/${id}" class="btn btn-primary">
                        Edit
                    </a>
                    <span id="confirmDeleteSpan_${id}" style="display: none">
                        <span>Are you sure you want to delete?</span>
                        <input type="button" onclick="confirmDeleteUserAjax('${id}','${userIndex}','${email
    }')" class="btn btn-danger" value="Yes">
                        <a href="javascript:void(0)" class="btn btn-primary"
                           onclick="toggleConfirmDeleteUserButton('${id}', false)">
                            No
                        </a>
                    </span>
                    <span id="deleteSpan_${id}">
                        <a href="javascript:void(0)" class="btn btn-danger"
                           onclick="toggleConfirmDeleteUserButton('${id}', true)">
                            Delete
                        </a>
                    </span>
                </div>
            </div>
`;

//todo: fix edituser link after search

function searchUsers(searchUserString) {
    $.ajax({
            type: "POST",
            url: "/Administration/ListUsersBySearch",
            data: {
                searchUserString: searchUserString
            },
            success: function(data, textStatus, jqXhr) {

                $("#users").empty();
                for (let userIndex = 0; userIndex < Object.keys(data).length; userIndex++) {
                    $("#users").append(card(data[userIndex].id, data[userIndex].email, userIndex + 1));
                }
                console.log("success: ");
            },
            complete: function(response) {
                console.log(`completed: ${response}`);
            },
            failure: function(response) {
                console.log(`failure: ${response}`);
            },
            error: function(response) {
                console.log(`error: ${response}`);
            }
        }
    );
}
