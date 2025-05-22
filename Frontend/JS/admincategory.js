// API base URL for category operations
const apiUrl = "http://localhost:58731/api/category";

// Wait for the document to be fully loaded
$(document).ready(function () {
  loadCategories(); // Load categories when page loads

  // Add category event handler
  $("#categoryForm").on("submit", function (e) {
    e.preventDefault(); // Prevent default form submission

    const name = $("#categoryName").val().trim(); // Get the trimmed category name
    if (name === "") return; // Do not proceed if input is empty

    // Make a POST request to add a new category
    $.ajax({
      url: apiUrl + "/add",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({ CategoryName: name }), // Convert JS object to JSON
      success: function (res) {
        alert(res); // Show success message
        $("#categoryName").val(""); // Clear the input field
        loadCategories(); // Reload category list
      },
      error: function (xhr) {
        alert(xhr.responseText || "Failed to add category."); // Show error message
      }
    });
  });
});

// Function to load and display all categories
function loadCategories() {
  $.ajax({
    url: apiUrl, // GET all categories
    type: "GET",
    success: function (data) {
      const tbody = $("#categoryTable tbody");
      tbody.empty(); // Clear existing rows

      // Loop through categories and create table rows
      data.forEach(cat => {
        tbody.append(`
          <tr>
            <td>${cat.CategoryID}</td>
            <td><input type="text" class="form-control" value="${cat.CategoryName}" id="cat-${cat.CategoryID}"/></td>
            <td>
              <button class="btn btn-sm btn-primary" onclick="updateCategory(${cat.CategoryID})">Update</button>
              <button class="btn btn-sm btn-danger" onclick="deleteCategory(${cat.CategoryID})">Delete</button>
            </td>
          </tr>
        `);
      });
    },
    error: function () {
      alert("Error loading categories."); // Alert if loading fails
    }
  });
}

// Function to update a category by ID
function updateCategory(id) {
  const updatedName = $(`#cat-${id}`).val().trim(); // Get the updated name
  if (updatedName === "") {
    alert("Category name cannot be empty."); // Prevent update if name is empty
    return;
  }

  // Make PUT request to update category
  $.ajax({
    url: apiUrl + "/update",
    type: "PUT",
    contentType: "application/json",
    data: JSON.stringify({ CategoryID: id, CategoryName: updatedName }),
    success: function (res) {
      alert(res); // Show success message
      loadCategories(); // Refresh the list
    },
    error: function () {
      alert("Failed to update category."); // Show error on failure
    }
  });
}

// Function to delete a category by ID
function deleteCategory(id) {
  // Confirm before deleting
  if (!confirm("Are you sure you want to delete this category?")) return;

  // Make DELETE request to remove the category
  $.ajax({
    url: apiUrl + "/delete/" + id,
    type: "DELETE",
    success: function (res) {
      alert(res); // Show confirmation
      loadCategories(); // Reload list
    },
    error: function () {
      alert("Failed to delete category."); // Show error message
    }
  });
}

// Navigate back to the previous page
function goBack() {
  window.history.back();
}

// Handle logout action
function logout() {
  alert("You have been logged out.");
  window.location.href = "index.html"; // Redirect to homepage/login
}
