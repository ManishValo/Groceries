// Wait until the DOM is fully loaded
$(document).ready(function () {
  
  // Handle form submission for the signup form
  $('#signup-form').submit(function (e) {
    e.preventDefault(); // Prevent the default form submission (page reload)

    // Get values from input fields and trim extra whitespace
    const fullName = $('#full-name').val().trim();
    const email = $('#email').val().trim();
    const password = $('#password').val().trim();
    const confirmPassword = $('#confirm-password').val().trim();

    // Validate that no fields are left empty
    if (!fullName || !email || !password || !confirmPassword) {
      alert("All fields are required.");
      return; // Stop further execution if validation fails
    }

    // Check if password and confirm password fields match
    if (password !== confirmPassword) {
      alert("Passwords do not match.");
      return; // Stop further execution if passwords don't match
    }

    // Construct the user object with default null values for optional fields
    const userData = {
      Name: fullName,           // User's full name
      Email: email,             // User's email address
      Password: password,       // User's password
      TypeId: 2,                // 2 represents a customer
      Address: null,            // Optional fields initially set as null
      City: null,
      Pincode: null,
      MobileNo: null
    };

    // Send a POST request to the API to register the user
    $.ajax({
      url: 'http://localhost:58731/api/user/register', // API endpoint for registration
      method: 'POST',
      contentType: 'application/json',                 // Set request content type
      data: JSON.stringify(userData),                  // Convert JS object to JSON string
      success: function (response) {
        // Show success message and redirect to login page
        alert("Signup successful! Please log in.");
        window.location.href = "login.html";
      },
      error: function (xhr) {
        // Handle registration failure and display specific error if available
        if (xhr.responseText) {
          alert("Signup failed: " + xhr.responseText);
        } else {
          alert("Signup failed.");
        }
      }
    });
  });

});
