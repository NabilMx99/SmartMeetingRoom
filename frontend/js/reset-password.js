const params = new URLSearchParams(window.location.search);
const emailField = document.getElementById('email');
const tokenField = document.getElementById('token');

emailField.value = params.get('email') || '';
tokenField.value = params.get('token') || '';

document.getElementById('resetPasswordForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const payload = {
        email: emailField.value,
        token: tokenField.value,
        newPassword: document.getElementById('newPassword').value,
        confirmPassword: document.getElementById('confirmPassword').value
    };

    try {
        const data = await apiPost('/auth/reset-password', payload);
        alert(data.message);
        window.location.href = 'login.html';
    } catch (err) {
        console.error(err);
        alert('Failed to reset password: ' + err.message);
    }
});