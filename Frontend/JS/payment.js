$(document).ready(function () {
  const loggedInUser = JSON.parse(sessionStorage.getItem("loggedInUser"));

  if (!loggedInUser || !loggedInUser.UserID) {
    alert("User not logged in.");
    window.location.href = "login.html";
    return;
  }

  const userId = loggedInUser.UserID;
  $("#full-name").val(loggedInUser.name);
  $("#email").val(loggedInUser.email);

  let cartItems = [];
  let totalAmount = 0;

  $.ajax({
    url: `http://localhost:58731/api/cart/user/${userId}`,
    type: "GET",
    success: function (data) {
      cartItems = data;
      if (cartItems.length === 0) {
        alert("Your cart is empty.");
        return;
      }

      totalAmount = 0;

      cartItems.forEach(item => {
        totalAmount += item.TotalPrice;
      });

      // $("#cartSummary").html(html);
      $("#totalAmount").text(`Total: ₹${totalAmount}`);
    },
    error: function () {
      alert("Failed to load cart.");
    }
  });

  $("#pay-btn").click(function () {
    if (cartItems.length === 0) {
      alert("Cart is empty.");
      return;
    }

    const updatedUser = {
      UserID: userId,
      Address: $("#address").val().trim(),
      City: $("#city").val().trim(),
      Pincode: $("#pincode").val().trim(),
      MobileNo: $("#contact").val().trim()
    };

    if (!updatedUser.Address || !updatedUser.City || !updatedUser.Pincode || !updatedUser.MobileNo) {
      alert("Please fill in all address and contact details.");
      return;
    }
    
    const payment={
      CardNumber:$('#cardnumber').val(),
      Cvv:$('#cvv').val(),
      CardExpiry:$('#cardexpiry').val(),
      otp:$('#otp').val(),  
    }
    if (!payment.CardNumber || !payment.Cvv || !payment.CardExpiry || !payment.otp) {
      alert("Please fill in card details.");
      return;
    }
    if(payment.CardNumber.length!=16)
    {
      alert("Enter 16 digit card number");
      return;
    }
    if(payment.Cvv.length!=3)
    {
      alert("Enter correct cvv");
      return;
    }
    // Update user info
    $.ajax({
      url: `http://localhost:58731/api/user/update-contact/${userId}`,
      type: "PUT",
      contentType: "application/json",
      data: JSON.stringify(updatedUser),
      success: function () {
        const billDto = {
          UserID: parseInt(userId),
          BillAmt: totalAmount,
          Details: cartItems.map(item => ({
            ProductID: item.ProductID,
            Quantity: item.CartQty,
            UnitPrice: item.ProductPrice,
            TotalPrice: item.TotalPrice
          }))
        };

        // Create bill
        $.ajax({
          url: "http://localhost:58731/api/bill/add",
          type: "POST",
          contentType: "application/json",
          data: JSON.stringify(billDto),
          success: function (response) {
            // Update product stock
            const stockUpdateData = cartItems.map(item => ({
              ProductID: item.ProductID,
              Quantity: item.CartQty
            }));

            $.ajax({
              url: "http://localhost:58731/api/products/update-stock",
              type: "POST",
              contentType: "application/json",
              data: JSON.stringify(stockUpdateData),
              success: function () {
                // Clear cart
                $.ajax({
                  url: `http://localhost:58731/api/cart/clear/user/${userId}`,
                  type: "DELETE",
                  success: function () {
                    alert("Payment successful! Bill ID: " + response.Order);
                    sessionStorage.setItem("billId", response.Order);
                    window.location.href = "/bill.html?billId=" + response.Order;
                  },
                  error: function () {
                    alert("Payment successful, but failed to clear cart.");
                  }
                });
              },
              error: function () {
                alert("Stock update failed. Please check inventory.");
              }
            });
          },
          error: function () {
            alert("Payment failed. Please try again.");
          }
        });
      },
      error: function () {
        alert("Failed to update user information.");
      }
    });
  });
});
