function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('user');
    setTimeout(() => window.location.href = 'login.html', 1000); // Redirect to login page after 1 second delay
}

(function () {
    const token = localStorage.getItem('jwtToken');
    const userJson = localStorage.getItem('user');

    if (!token || !userJson) {
        window.location.href = 'login.html';
        return;
    }

    try {
        const user = JSON.parse(userJson);
        const first = user?.firstName || user?.userName || user?.email?.split('@')[0] || '';
        const el = document.getElementById('welcomeName');
        if (el) el.textContent = first ? `Welcome, ${first}` : 'Welcome';
    } catch (err) {
        console.error('Error parsing user data:', err);
        window.location.href = 'login.html';
    }
})();

function toggleDetails(button) {
    const detailsRow = button.closest('tr')?.nextElementSibling;
    if (!detailsRow) return;

    detailsRow.classList.toggle('show');
    button.classList.toggle('active');
    button.textContent = detailsRow.classList.contains('show') ? 'Hide Details' : 'View Details';
}

function filterRooms() {
    const input = document.getElementById("roomSearch")?.value.toLowerCase().trim() || '';
    const table = document.getElementById("roomTable");
    if (!table) return;

    const tbody = table.querySelector("tbody");
    if (!tbody) return;

    const rows = Array.from(table.querySelectorAll("tbody tr"));
    const availableRows = [];
    const occupiedRows = [];

    rows.forEach(row => {
        const cells = row.querySelectorAll("td");
        if (cells.length < 4) return;

        const rowText = Array.from(cells).slice(0, 4).map(cell => cell.textContent.toLowerCase()).join(' ');
        const matchesSearch = !input || rowText.includes(input);

        if (cells[3].textContent.toLowerCase() === 'available') availableRows.push({ row, show: matchesSearch });
        else occupiedRows.push({ row, show: matchesSearch });
    });

    tbody.innerHTML = '';
    [...availableRows, ...occupiedRows].forEach(({ row, show }) => {
        row.style.display = show ? '' : 'none';
        tbody.appendChild(row);
    });
}

document.getElementById("roomSearch")?.addEventListener("keyup", filterRooms);
document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');

window.addEventListener("DOMContentLoaded", filterRooms);
