function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('user');
    setTimeout(() => window.location.href = 'login.html', 1000); // Redirect to login page after 1 second delay
}

function formatDateTime(dt) {
    if (!dt) return '';
    const d = new Date(dt);
    return d.toLocaleString();
}

async function loadMeetings() {
    const meetingsList = document.getElementById('meetingsList');
    meetingsList.innerHTML = '<div class="loading">Loading meetings...</div>';
    try {
        const meetings = await apiGet('/meetings') || [];
        if (!meetings.length) {
            meetingsList.innerHTML = '<div class="no-meetings">No meetings found.</div>';
            return;
        }
        meetingsList.innerHTML = '';
        meetings
            .filter(meeting => meeting.meetingStatus !== 'Cancelled' && meeting.meetingStatus !== 'Completed')
            .forEach(meeting => {
                const attendees = (meeting.attendees || []).map(a => {
                    const u = a.user || a;
                    if (u.fullName) return u.fullName;
                    if (u.firstName && u.lastName) return `${u.firstName} ${u.lastName}`;
                    if (u.firstName) return u.firstName;
                    if (u.userName) return u.userName;
                    if (u.email) return u.email;
                    return '';
                }).filter(Boolean).join(', ');

                const zoomUrl = meeting.zoomJoinUrl || '';
                const zoomLink = zoomUrl ? `<a href="${zoomUrl}" target="_blank">${zoomUrl}</a>` : 'N/A';

                const meetingDiv = document.createElement('div');
                meetingDiv.className = 'meeting-details-box';
                meetingDiv.innerHTML = `
                    <div class="meeting-detail-row"><strong>Title:</strong> ${meeting.meetingTitle || ''}</div>
                    <div class="meeting-detail-row"><strong>Agenda:</strong> ${meeting.meetingAgenda || ''}</div>
                    <div class="meeting-detail-row"><strong>Start time:</strong> ${formatDateTime(meeting.meetingStartTime)}</div>
                    <div class="meeting-detail-row"><strong>End time:</strong> ${formatDateTime(meeting.meetingEndTime)}</div>
                    <div class="meeting-detail-row"><strong>Attendees:</strong> ${attendees || 'N/A'}</div>
                    <div class="meeting-detail-row"><strong>Zoom link:</strong> ${zoomLink}</div>
                `;
                meetingsList.appendChild(meetingDiv);
            });
    } catch (err) {
        meetingsList.innerHTML = `<div class="error">${err.message}</div>`;
    }
}

document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');

window.addEventListener('DOMContentLoaded', async () => {
    const token = localStorage.getItem('jwtToken');
    const userJson = localStorage.getItem('user');
    if (!token || !userJson) {
        window.location.href = 'login.html';
        return;
    }
    try {
        await loadMeetings();
    } catch (err) {
        document.getElementById('meetingsList').innerHTML = `<div class="error">${err.message}</div>`;
    }
});
