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

    const regex = /(\w+):\s*([^:]+)(?=\s+\w+:|$)/g;
    const terms = [];
    let match;
    while ((match = regex.exec(input)) !== null) {
        terms.push({ key: match[1].trim(), value: match[2].trim() });
    }

    const noPrefixWords = input.replace(regex, '').split(/[\s,]+/).filter(Boolean);

    rows.forEach(row => {
        const cells = row.querySelectorAll("td");
        if (cells.length < 5) {
            row.style.display = '';
            return;
        }

        const room = cells[0].textContent.toLowerCase();
        const location = cells[1].textContent.toLowerCase();
        const capacity = cells[2].textContent.toLowerCase();
        const status = cells[3].textContent.toLowerCase();
        const features = Array.from(cells[4].querySelectorAll('.badge')).map(b => b.textContent.toLowerCase()).join(' ');

        let matches = true;
        for (const term of terms) {
            switch (term.key) {
                case 'room':
                    if (!room.includes(term.value)) matches = false;
                    break;
                case 'location':
                    if (!location.includes(term.value)) matches = false;
                    break;
                case 'capacity':
                    if (!capacity.includes(term.value)) matches = false;
                    break;
                case 'status':
                    if (!status.includes(term.value)) matches = false;
                    break;
                case 'features':
                case 'feature':
                    
                    const featureTerms = term.value.split(/[\s,]+/).filter(Boolean);
                    if (!featureTerms.every(f => features.includes(f))) matches = false;
                    break;
                default:
                    
                    break;
            }
            if (!matches) break;
        }

        if (matches && noPrefixWords.length > 0) {
            const searchable = [room, location, capacity, status, features];
            matches = noPrefixWords.every(word =>
                searchable.some(field => field.includes(word))
            );
        }

        row.style.display = matches ? '' : 'none';
    });
}

document.getElementById("roomSearch")?.addEventListener("keyup", filterRooms);
document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');

window.addEventListener("DOMContentLoaded", filterRooms);

document.querySelectorAll('.search-prefixes button').forEach(btn => {
    btn.addEventListener('click', function () {
        const input = document.getElementById('roomSearch');
        if (!input) return;
        const prefix = btn.getAttribute('data-prefix');
        
        const start = input.selectionStart;
        const end = input.selectionEnd;
        const value = input.value;
        input.value = value.slice(0, start) + prefix + value.slice(end);
        
        const cursorPos = start + prefix.length;
        input.setSelectionRange(cursorPos, cursorPos);
        input.focus();
        
        filterRooms();
    });
});
