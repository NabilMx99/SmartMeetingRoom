document.getElementById('registerForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const signUpData = {
        firstName: document.getElementById('firstName').value.trim(),
        lastName: document.getElementById('lastName').value.trim(),
        email: document.getElementById('email').value.trim(),
        phoneNumber: document.getElementById('phone').value.trim(),
        password: document.getElementById('password').value,
        confirmPassword: document.getElementById('confirmPassword').value
    };

    try {
        await apiPost("/auth/register", signUpData); // POST to /auth/register
        alert("Registered successfully! Please login.");
        window.location.href = "login.html"; // redirect to login page
    } catch (err) {
        alert(err.message); // show any API error
    }
});

