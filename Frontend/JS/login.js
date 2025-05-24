$(document).ready(function () {
    // Event handler for login form submission
    $('#login-form').submit(function (e) {
        e.preventDefault(); // Prevent default form submission behavior (page reload)

        // Get email and password input values and trim whitespace
        const Email = $('#email').val().trim();
        const Password = $('#password').val().trim();

        // Check if any of the fields are empty
        if (!Email || !Password) {
            $('#login-message').html('<span style="color:red">Both fields are required.</span>');
            return; // Exit the function early
        }

        // Send login credentials to backend using AJAX POST request
        $.ajax({
            type: "POST",
            url: "http://localhost:58731/api/user/login",  // Replace with your backend API endpoint
            data: JSON.stringify({ Email, Password }), // Convert user input to JSON format
            contentType: "application/json", // Set content type to JSON

            // Handle successful response from the API
            success: function (response) {
                // If response indicates an admin user
                if (response && response.TypeID === 1) {
                    window.location.href = "adminpanel.html"; // Redirect to admin panel
                }
                // If response indicates a normal user
                else if (response && response.TypeID === 2) {
                    sessionStorage.setItem("loggedInUser", JSON.stringify(response)); // Store user data in sessionStorage
                    window.location.href = "index.html"; // Redirect to homepage
                }
                // If credentials are incorrect
                else {
                    $('#login-message').html('<span style="color:red">Invalid login credentials.</span>');
                }
            },

            // Handle errors from the server
            error: function (err) {
                if (err.status === 404) {
                    $('#login-message').html('<span style="color:red">Email or password incorrect.</span>');
                } else {
                    $('#login-message').html('<span style="color:red">Server error. Please try again later.</span>');
                    console.error(err); // Log error to console for debugging
                }
            }
        });
    });

    // Toggle password visibility when eye icon or button is clicked
    $('#toggle-password').on('click', function () {
        const passwordField = $('#password');
        const type = passwordField.attr('type') === 'password' ? 'text' : 'password';
        passwordField.attr('type', type); // Toggle between 'password' and 'text'
    });
});
