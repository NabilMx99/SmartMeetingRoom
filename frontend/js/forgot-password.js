document.getElementById('forgotPasswordForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    const email = document.getElementById('email').value.trim();

    try {
        const data = await apiPost('/auth/forgot-password', { email });
        alert(data.message || 'If that email is registered, a reset link has been sent.');
    } catch (err) {
        console.error(err);
        alert('Failed to send password reset email.');
    }
});