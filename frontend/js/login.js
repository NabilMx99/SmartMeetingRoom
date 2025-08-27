document.getElementById('loginForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();

    try {
        const data = await apiPost("/auth/login", { email, password });

        setToken(data.token);
        localStorage.setItem("user", JSON.stringify(data.user));

        setTimeout(function() {
            window.location.href = "dashboard.html"; // redirect to dashboard page after 1 second delay
        }, 1000);

    } catch (err) {
        alert(err.message);
    }
});
