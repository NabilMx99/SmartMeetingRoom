(function () {
  const msg = document.getElementById('profileMessage');
  const form = document.getElementById('profileForm');
  const deleteBtn = document.getElementById('deleteAccountBtn');

  const firstName = document.getElementById('firstName');
  const lastName = document.getElementById('lastName');
  const email = document.getElementById('email');
  const phoneNumber = document.getElementById('phoneNumber');

  const showMessage = (text, isError = false) => {
    if (!msg) return;
    msg.textContent = text;
    msg.style.color = isError ? 'crimson' : 'inherit';
  };

  const storedToken = localStorage.getItem('jwtToken');
  const storedUserJson = localStorage.getItem('user');

  if (!storedToken || !storedUserJson) {
    // Not logged in
    window.location.href = 'login.html';
    return;
  }

  let user = null;
  try {
    user = JSON.parse(storedUserJson);
  } catch {
    window.location.href = 'login.html';
    return;
  }

  const userId = user?.userId;
  if (!userId) {
    window.location.href = 'login.html';
    return;
  }

  // Prefill from localStorage immediately
  const prefill = (u) => {
    if (!u) return;
    firstName.value = u.firstName || '';
    lastName.value = u.lastName || '';
    email.value = u.email || '';
    phoneNumber.value = u.phoneNumber || '';
  };
  prefill(user);

  (async function refreshFromApi() {
    try {
      const latest = await apiGet(`/users/${userId}`);
      if (latest) {
        prefill(latest);
        localStorage.setItem('user', JSON.stringify(latest));
      }
    } catch (err) {
      showMessage(err.message || 'Failed to load latest profile.', true);
    }
  })();

  // Handle Update
  form.addEventListener('submit', async function (e) {
    e.preventDefault();

    const payload = {
      firstName: firstName.value.trim(),
      lastName: lastName.value.trim(),
      email: email.value.trim(),
      phoneNumber: phoneNumber.value.trim()
    };

    try {
      await apiPut(`/users/${userId}`, payload);
      // Re-fetch user to keep localStorage accurate
      const latest = await apiGet(`/users/${userId}`);
      if (latest) {
        localStorage.setItem('user', JSON.stringify(latest));
        prefill(latest);
      }
      showMessage('Profile updated successfully.');
    } catch (err) {
      showMessage(err.message || 'Failed to update profile.', true);
    }
  });

  // Handle Delete
  deleteBtn.addEventListener('click', async function () {
    const confirmed = confirm('Are you sure you want to delete your account? This cannot be undone.');
    if (!confirmed) return;

    try {
      await apiDelete(`/users/${userId}`);
      localStorage.removeItem('jwtToken');
      localStorage.removeItem('user');
      showMessage('Account deleted. Redirecting to login...');
      window.location.href = 'login.html';
    } catch (err) {
      showMessage(err.message || 'Failed to delete account.', true);
    }
  });
})();
