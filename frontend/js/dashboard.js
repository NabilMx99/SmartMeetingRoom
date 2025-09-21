function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('user');
    setTimeout(() => window.location.href = 'login.html', 1000); // Redirect to login page after 1 second delay
}

function formatDateTime(dt) {
    const d = new Date(dt);
    return d.toLocaleString();
}

function toggleDetails(button) {
    const detailsRow = button.closest('tr')?.nextElementSibling;
    if (!detailsRow) return;

    detailsRow.classList.toggle('show');
    button.classList.toggle('active');
    button.textContent = detailsRow.classList.contains('show') ? 'Hide Details' : 'View Details';
}

async function loadRooms() {
    const tableBody = document.querySelector("#roomTable tbody");
    if (!tableBody) return;

    try {
        const rooms = await apiGet("/rooms") || [];

        tableBody.innerHTML = "";

        let availableRoomsCount = 0;

        rooms.forEach(room => {
            if (room.isAvailable) availableRoomsCount++;

            const tr = document.createElement("tr");

            const tdName = document.createElement("td");
            tdName.textContent = room.roomName;
            tr.appendChild(tdName);

            const tdLocation = document.createElement("td");
            tdLocation.textContent = room.roomLocation;
            tr.appendChild(tdLocation);

            const tdCapacity = document.createElement("td");
            tdCapacity.textContent = room.roomCapacity;
            tr.appendChild(tdCapacity);

            const tdStatus = document.createElement("td");
            const statusBadge = document.createElement("span");
            statusBadge.className = `badge ${room.isAvailable ? "green" : "red"}`;
            statusBadge.textContent = room.isAvailable ? "Available" : "Occupied";
            tdStatus.appendChild(statusBadge);
            tr.appendChild(tdStatus);

            const tdFeatures = document.createElement("td");
            (room.features || []).forEach(feature => {  
                const badge = document.createElement("span");
                badge.className = "badge grey";
                badge.textContent = feature.featureName;  
                tdFeatures.appendChild(badge);
            });
            tr.appendChild(tdFeatures);

            tableBody.appendChild(tr);
        });
        
        document.getElementById("availableRoomsCount").textContent = availableRoomsCount;
    } catch (err) {
        console.error("Failed to load rooms:", err);
    }
}

async function createRoom(roomData) {
    try {
        const newRoom = await apiPost("/rooms", roomData);
        console.log("Room created:", newRoom);
        await loadRooms(); 
    } catch (err) {
        console.error("Failed to create room:", err);
    }
}

async function updateRoom(roomId, roomData) {
    try {
        await apiPut(`/rooms/${roomId}`, roomData);
        console.log("Room updated:", roomId);
        await loadRooms(); 
    } catch (err) {
        console.error("Failed to update room:", err);
    }
}

async function deleteRoom(roomId) {
    try {
        await apiDelete(`/rooms/${roomId}`);
        console.log("Room deleted:", roomId);
        await loadRooms(); 
    } catch (err) {
        console.error("Failed to delete room:", err);
    }
}

async function loadUpcomingMeetings() {
    const table = document.querySelector('.upcoming-meetings table tbody');
    if (!table) return;
    try {
        const meetings = await apiGet('/meetings') || [];
        const now = new Date();

        let todaysMeetings = 0;
        const today = new Date();
        today.setHours(0,0,0,0);
        const tomorrow = new Date(today);
        tomorrow.setDate(today.getDate() + 1);
        meetings.forEach(m => {
            if (m.meetingStatus !== 'Cancelled' && m.meetingStatus !== 'Completed') {
                const start = new Date(m.meetingStartTime);
                if (start >= today && start < tomorrow) {
                    todaysMeetings++;
                }
            }
        });
        document.getElementById('meetingsCount').textContent = todaysMeetings;

        const filtered = meetings.filter(m => {
            if (m.meetingStatus === 'Cancelled') return false;
            if (m.meetingStatus === 'Completed') {
                const end = new Date(m.meetingEndTime);
                return (now - end) < 24 * 60 * 60 * 1000;
            }
            return true;
        });

        table.innerHTML = '';
        if (!filtered.length) {
            table.innerHTML = '<tr><td colspan="4" style="text-align:center;color:#888;">No upcoming meetings.</td></tr>';
            return;
        }
        filtered.forEach(meeting => {

            const start = new Date(meeting.meetingStartTime);
            const end = new Date(meeting.meetingEndTime);
            let status = meeting.meetingStatus;
            if (status !== 'Cancelled') {
                if (now >= start && now <= end) status = 'Ongoing';
                else if (now > end) status = 'Completed';
                else if (now < start) status = 'Scheduled';
            }
            if (status !== meeting.meetingStatus) {
                apiPut(`/meetings/${meeting.meetingId}`, { meetingStatus: status }).catch(()=>{});
                meeting.meetingStatus = status;
            }

            const tr = document.createElement('tr');
            tr.className = status === 'Ongoing' ? 'next-meeting' : '';
            tr.innerHTML = `
                <td>${formatDateTime(meeting.meetingStartTime)}</td>
                <td>${formatDateTime(meeting.meetingEndTime)}</td>
                <td><span class="badge ${status === 'Ongoing' ? 'green' : status === 'Cancelled' ? 'red' : status === 'Completed' ? 'grey' : 'blue'}">${status}</span></td>
                <td><button class="view-details-btn">View Details</button></td>
            `;
            table.appendChild(tr);

            const detailsTr = document.createElement('tr');
            detailsTr.className = 'meeting-details';

            let organizerName = '';
            if (meeting.organizer) {
                if (meeting.organizer.firstName && meeting.organizer.lastName) {
                    organizerName = `${meeting.organizer.firstName} ${meeting.organizer.lastName}`;
                } else if (meeting.organizer.firstName) {
                    organizerName = meeting.organizer.firstName;
                } else if (meeting.organizer.userName) {
                    organizerName = meeting.organizer.userName;
                } else if (meeting.organizer.email) {
                    organizerName = meeting.organizer.email;
                }
            }
            detailsTr.innerHTML = `<td colspan="4">
                <strong>Organizer:</strong> ${organizerName}<br>
                <strong>Start Time:</strong> ${formatDateTime(meeting.meetingStartTime)}<br>
                <strong>End Time:</strong> ${formatDateTime(meeting.meetingEndTime)}<br>
                <strong>Title:</strong> ${meeting.meetingTitle}<br>
                <strong>Agenda:</strong> ${meeting.meetingAgenda || ''}<br>
                <strong>Room:</strong> ${meeting.room?.roomName || ''}<br>
                <strong>Location:</strong> ${meeting.room?.roomLocation || ''}<br>
                <strong>Status:</strong> ${status}
            </td>`;
            table.appendChild(detailsTr);
        });

        document.querySelectorAll('.view-details-btn').forEach((btn, idx) => {
            btn.addEventListener('click', function() {
                const detailsRow = btn.closest('tr')?.nextElementSibling;
                if (!detailsRow) return;
                detailsRow.classList.toggle('show');
                btn.classList.toggle('active');
                btn.textContent = detailsRow.classList.contains('show') ? 'Hide Details' : 'View Details';
            });
        });
    } catch (err) {
        table.innerHTML = `<tr><td colspan="4" style="color:red;">${err.message}</td></tr>`;
    }
}

document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');
document.querySelector('.quick-actions .schedule').addEventListener('click', () => window.location.href = 'booking.html');
document.querySelector('.quick-actions .join').addEventListener('click', async () => {
    try {
        const meetings = await apiGet('/meetings') || [];
        const now = new Date();
        const upcoming = meetings.filter(m => {
            if (m.meetingStatus === 'Cancelled' || m.meetingStatus === 'Completed') return false;
            const start = new Date(m.meetingStartTime);
            const end = new Date(m.meetingEndTime);
            return now <= end;
        });
        if (upcoming.length) {
            window.location.href = 'join-meeting.html';
        } else {
            alert('There are no upcoming meetings to join.');
        }
    } catch (err) {
        alert('Failed to check meetings.');
    }
});

window.addEventListener("DOMContentLoaded", () => {
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
    loadRooms();
    loadUpcomingMeetings();
});
