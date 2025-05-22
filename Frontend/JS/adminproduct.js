// Base URL for API endpoints
const apiBase = "http://localhost:58731/api";

// Logout function to alert user and redirect to login page
function logout() {
  alert("You have been logged out.");
  window.location.href = "index.html";
}

// Function to fetch and display all products in the table
function loadProducts() {
  $.ajax({
    url: `${apiBase}/products`,
    method: "GET",
    success: function (data) {
      let rows = "";
      // Loop through each product and build HTML row
      data.forEach(p => {
        rows += `<tr>
          <td>${p.ProductID}</td>
          <td>${p.ProductName}</td>
          <td><img src="Images/${p.ProductImg}" class="thumb"/></td>
          <td>${p.ProductPrice}</td>
          <td>${p.ProductQuantity}</td>
          <td>${p.ProductDescription}</td>
          <td>${p.ProductCatID}</td>
          <td>
            <button class="btn btn-warning btn-sm" onclick='showEditForm(${JSON.stringify(p)})'>Edit</button>
            <button class="btn btn-danger btn-sm" onclick="deleteProduct(${p.ProductID})">Delete</button>
          </td>
        </tr>`;
      });
      // Inject rows into table body
      $("#ProductTable tbody").html(rows);
    },
    error: function () {
      alert("Error loading Products.");
    }
  });
}

// Function to fetch and populate category options in dropdowns
function loadCategories() {
  $.ajax({
    url: `${apiBase}/category`,
    method: "GET",
    success: function (data) {
      let options = "<option value=''>Select Category</option>";
      // Loop through categories and create option elements
      data.forEach(c => {
        options += `<option value='${c.CategoryID}'>${c.CategoryName}</option>`;
      });
      // Set options for both add and edit dropdowns
      $("#categorySelect").html(options);
      $("#editCategorySelect").html(options); 
    },
    error: function () {
      alert("Error loading Categories.");
    }
  });
}

// Function to handle adding a new product
function addProduct() {
  const ProductName = $("#ProductName").val().trim();
  let ProductImg = $("#ProductImg").val();
  // Extract filename only from file path
  ProductImg = ProductImg.split('\\').pop().split('/').pop();

  const ProductPrice = $("#ProductPrice").val().trim();
  const ProductQty = $("#ProductQty").val().trim();
  const ProductDesc = $("#ProductDesc").val().trim();
  const categoryID = $("#categorySelect").val();

  // Basic form validation
  if (!ProductName || !ProductImg || !ProductPrice || !ProductQty || !ProductDesc || !categoryID || ProductPrice < 0 || ProductQty < 0) {
    alert("Enter All Fields Correctly");
    return;
  }

  // Create product object to send
  const Product = {
    ProductName,
    ProductImg,
    ProductPrice: parseFloat(ProductPrice),
    ProductQuantity: parseInt(ProductQty),
    ProductDescription: ProductDesc,
    ProductCatID: parseInt(categoryID)
  };

  // AJAX POST to add product
  $.ajax({
    url: `${apiBase}/products/add`,
    type: "POST",
    data: JSON.stringify(Product),
    contentType: "application/json",
    success: function () {
      alert("Product added.");
      loadProducts(); // Refresh product list

      // Clear input fields
      $("#ProductName, #ProductImg, #ProductPrice, #ProductQty, #ProductDesc").val("");
      $("#categorySelect").val("");
    },
    error: function (err) {
      console.log(err);
      alert("Error adding Product.");
    }
  });
}

// Function to delete a product
function deleteProduct(id) {
  if (confirm("Are you sure you want to delete this Product?")) {
    $.ajax({
      url: `${apiBase}/products/delete/${id}`,
      type: "DELETE",
      success: function () {
        alert("Deleted.");
        loadProducts(); // Refresh list after deletion
      },
      error: function () {
        alert("Delete failed.");
      }
    });
  }
}

// Show product data in the edit modal for updating
function showEditForm(Product) {
  $("#editProductId").val(Product.ProductID);
  $("#editProductName").val(Product.ProductName);
  $("#editProductImg").val(""); // Clear file input field
  $("#oldProductImgName").val(Product.ProductImg); // Save current image name
  $("#editProductPrice").val(Product.ProductPrice);
  $("#editProductQty").val(Product.ProductQuantity);
  $("#editProductDesc").val(Product.ProductDescription);
  $("#editCategorySelect").val(Product.ProductCatID);
  $("#editProductModal").modal("show"); // Open modal
}

// Function to update a product
function updateProduct() {
  // Get image name; use old if not selected
  let ProductImg = $("#editProductImg").val();
  ProductImg = ProductImg.split('\\').pop().split('/').pop();
  if (!ProductImg) {
    ProductImg = $("#oldProductImgName").val();
  }

  // Build product object from form inputs
  const Product = {
    ProductID: parseInt($("#editProductId").val()),
    ProductName: $("#editProductName").val(),
    ProductImg: ProductImg,
    ProductPrice: parseFloat($("#editProductPrice").val()),
    ProductQuantity: parseInt($("#editProductQty").val()),
    ProductDescription: $("#editProductDesc").val(),
    ProductCatID: parseInt($("#editCategorySelect").val())
  };

  // AJAX PUT request to update product
  $.ajax({
    url: `${apiBase}/products/update`,
    type: "PUT",
    data: JSON.stringify(Product),
    contentType: "application/json",
    success: function () {
      alert("Product updated successfully.");
      $("#editProductModal").modal("hide"); // Close modal
      loadProducts(); // Refresh list
    },
    error: function () {
      alert("Error updating Product.");
    }
  });
}

// Run when the DOM is fully loaded
$(document).ready(function () {
  loadProducts();   // Load all products into table
  loadCategories(); // Populate category dropdowns
});
