window.addEventListener('DOMContentLoaded', () => {
  updateUserSection();
  loadUserOrders();
});

// Update user navbar section
function updateUserSection() {
  const userText = document.getElementById('user-text');
  const userJson = sessionStorage.getItem('loggedInUser');
  const user = userJson ? JSON.parse(userJson) : null;

  if (user && user.name) {
    userText.innerHTML = `
      Welcome, ${user.name} &nbsp;|&nbsp;
      <span id="spanLogout" class="logout" style="cursor:pointer; text-decoration:underline;">Logout</span>
    `;
    document.getElementById('spanLogout').onclick = logout;
  } else {
    userText.innerHTML = `
      <a href="signup.html" class="text-white text-decoration-none me-2">Sign up</a> /
      <a href="login.html" class="text-white text-decoration-none">Login</a>
    `;
  }
}

// Logout function
function logout() {
  sessionStorage.removeItem("loggedInUser");
  window.location.href = "login.html";
}

// Load user orders
function loadUserOrders() {
  const userJson = sessionStorage.getItem("loggedInUser");
  const user = userJson ? JSON.parse(userJson) : null;

  if (!user || !user.UserID) {
    alert("Please log in to view your orders.");
    window.location.href = "login.html";
    return;
  }

  $.ajax({
    url: `http://localhost:58731/api/bill/user/${user.UserID}`, 
    method: 'GET',
    success: function (orders) {
      console.log(orders)
      renderOrders(orders);
    },
    error: function (xhr, status, error) {
      console.error("Failed to load orders:", error);
      $('#orders-list').html("<p class='text-danger'>Failed to load order history.</p>");
    }
  });
}

// Render the orders
function renderOrders(orders) {
  const container = $('#orders-list');

  if (!orders || orders.length === 0) {
    container.html("<p>You have not placed any orders yet.</p>");
    return;
  }

  const html = orders.map(order => {
    const items = (order.Items || order.items || []).map(item => `
      <li class="list-group-item d-flex justify-content-between align-items-center">
        <div>
          <strong>${item.ProductName || item.productName}</strong><br>
          <small>${item.Description || item.description || 'No description'}</small>
        </div>
        <span>Qty: ${item.Quantity || item.quantity} | ₹${item.Price || item.price}</span>
      </li>
    `).join('');

    return `
      <div class="card mb-4 shadow">
        <div class="card-header bg-success text-white">
          <strong>Order ID:</strong> ${order.OrderID || order.orderId} &nbsp;&nbsp;
          <strong>Date:</strong> ${new Date(order.OrderDate || order.orderDate).toLocaleDateString()} &nbsp;&nbsp;
          <strong>Total:</strong> ₹${(order.OrderAmt || order.totalAmount).toFixed(2)}
        </div>
        <ul class="list-group list-group-flush">${items}</ul>
      </div>
    `;
  }).join('');

  container.html(html);
}
