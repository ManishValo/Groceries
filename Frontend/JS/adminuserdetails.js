// Base URL for all user-related API endpoints
const BASE_URL = "http://localhost:58731/api/user"; 

// Navigates the user to the previous page
function goBack() {
    window.history.back();
}

// Logs out the user and redirects to the login/home page
function logout() {
    alert("You have been logged out.");
    window.location.href = "index.html";
}

// Loads all users from the API and displays them in a table
function loadUsers() {
    $.ajax({
        url: `${BASE_URL}/get-all`, // API endpoint to get all users
        method: "GET",
        success: function (data) {
            let rows = "";

            // Loop through the returned users and create table rows
            data.forEach(user => {
                rows += `
                <tr>
                  <td>${user.UserID}</td>
                  <td>${user.Name}</td>
                  <td>${user.Email}</td>
                  <td>${user.TypeId == 1 ? "Admin" : "Customer"}</td>
                  <td>
                    <!-- Pass entire user object to viewUser function -->
                    <button class="btn btn-sm btn-info me-1" onclick='viewUser(${JSON.stringify(user)})'>View</button>
                  </td>
                </tr>`;
            });

            // Insert rows into the table body
            $('#userTable tbody').html(rows);
        },
        error: function () {
            alert("Failed to load users.");
        }
    });
}

// Displays detailed info of a selected user inside a modal
function viewUser(user) {
    // Set modal fields with user details
    $('#viewUserID').text(user.UserID);
    $('#viewUserName').text(user.Name);
    $('#viewUserEmail').text(user.Email);
    $('#viewMobileNo').text(user.MobileNo || 'N/A'); // Use fallback if MobileNo is null/undefined
    $('#viewType').text(user.TypeId == 1 ? 'Admin' : 'Customer');
    $('#viewAddress').text(user.Address || 'N/A');
    $('#viewCity').text(user.City || 'N/A');
    $('#viewPincode').text(user.Pincode || 'N/A');

    // Show the user detail modal
    $('#viewUserModal').modal('show');
}


// Delete user
{/* <button class="btn btn-sm btn-danger" onclick="deleteUser(${user.UserID})">Delete</button> */}
// function deleteUser(UserID) {
//     if (confirm("Are you sure?")) {
//         $.ajax({
//             url: `${BASE_URL}/delete/${UserID}`,
//             type: "DELETE",
//             success: function () {
//                 alert("User deleted.");
//                 loadUsers();
//             }
//         });
//     }
// }

// Show/hide extra fields for customer
// $('#typeId').change(function () {
//     const typeId = $(this).val();
//     if (typeId === "2") {
//         $('#customerFields').show();
//     } else {
//         $('#customerFields').hide();
//     }
// });

// // Add user
// $('#userForm').submit(function (e) {
//     e.preventDefault();

//     const userData = {
//         Name: $('#userName').val(),
//         Email: $('#userEmail').val(),
//         Password: $('#userPassword').val(),
//         mobileNo: $('#mobileNo').val(),
//         TypeId: $('#typeId').val(),
//         Address: $('#typeId').val() === "2" ? $('#address').val() : null,
//         City: $('#typeId').val() === "2" ? $('#city').val() : null,
//         Pincode: $('#typeId').val() === "2" ? $('#pincode').val() : null
//     };

//     $.ajax({
//         url: `${BASE_URL}/add`,
//         method: "POST",
//         contentType: "application/json",
//         data: JSON.stringify(userData),
//         success: function () {
//             alert("User added successfully.");
//             $('#addUserModal').modal('hide');
//             $('#userForm')[0].reset();
//             $('#customerFields').hide();
//             loadUsers();
//         },
//         error: function () {
//             alert("Error adding user.");
//         }
//     });
// });

$(document).ready(function () {
    loadUsers();
});