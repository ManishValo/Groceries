// Base URL for the Bill API endpoint
const BASE_URL = "http://localhost:58731/api/bill";

// Function to load all bills from the server and populate them in a table
function loadBills() {
  $.ajax({
    url: BASE_URL, // API endpoint to fetch all bills
    method: "GET",
    success: function (data) {
      let rows = "";
      
      // Loop through each bill and generate table row HTML
      data.forEach(bill => {
        rows += `
          <tr>
            <td>${bill.OrderID}</td>
            <td>${bill.UserID}</td>
            <td>${bill.CustomerName}</td> 
            <td>${bill.OrderAmt}</td> 
            <td>
              <button class="btn btn-sm btn-info" onclick="viewBill(${bill.OrderID})">View</button>
            </td>
          </tr>
        `;
      });

      // Inject all the rows into the table body
      $("#billTable tbody").html(rows);
    },
    error: function () {
      alert("Error loading bill data."); // Show alert on failure
    }
  });
}

// Function to fetch and show the details of a specific bill
function viewBill(OrderID) {
  let totalAmount = 0; // Variable to accumulate the total amount of the bill

  $.ajax({
    url: `${BASE_URL}/details/${OrderID}`, // API endpoint to fetch bill details
    method: "GET",
    success: function (details) {
      let rows = "";

      // Loop through each item in the bill
      details.forEach(item => {
        totalAmount += item.TotalPrice; // Add up total amount

        // Create a row for each bill item
        rows += `
          <tr>
            <td>${item.ProductName}</td>
            <td>${item.Quantity}</td>
            <td>${item.UnitPrice}</td>
            <td>${item.TotalPrice}</td>
          </tr>
        `;
      });

      // Add the final row displaying the total amount
      rows += `
        <tr class="table-secondary fw-bold">
          <td colspan="3" class="text-end">Total Amount:</td>
          <td>₹${totalAmount}</td>
        </tr>
      `;

      // Inject the bill detail rows into the modal's table body
      $("#billDetailBody").html(rows);

      // Show the modal with bill details
      $("#viewBillModal").modal("show");
    },
    error: function () {
      alert("Unable to fetch bill details."); // Alert on failure
    }
  });
}

// Function to navigate back to the previous page
function goBack() {
  window.history.back();
}

// Function to handle user logout
function logout() {
  alert("Logged out."); // Show alert on logout
  window.location.href = "index.html"; // Redirect to home page
}

// Load all bills once the DOM is ready
$(document).ready(() => {
  loadBills(); // Initial call to populate bill table
});
