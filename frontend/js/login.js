document.getElementById('loginForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();

    try {
        const data = await apiPost("/auth/login", { email, password });

        setToken(data.token);
        localStorage.setItem("user", JSON.stringify(data.user));

        window.location.href = "dashboard.html"; // redirect to dashboard page

    } catch (err) {
        alert(err.message);
    }
});
