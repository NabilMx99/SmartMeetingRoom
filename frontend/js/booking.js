let attendeeSearchTimeout = null;
let attendeeSuggestions = [];
let selectedAttendees = [];

function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('user');
    setTimeout(() => window.location.href = 'login.html', 1000);
}

function formatDateTime(dt) {
    const d = new Date(dt);
    return d.toLocaleString();
}

function getCurrentUserId() {
    try {
        const user = JSON.parse(localStorage.getItem('user'));
        return user?.id || user?.Id;
    } catch {
        return null;
    }
}

function renderAttendeeSuggestions() {
    const attendeeSuggestionsDiv = document.getElementById('attendee-suggestions');
    const attendeeSearchInput = document.getElementById('attendeeSearch');
    attendeeSuggestionsDiv.innerHTML = '';
    if (!attendeeSuggestions.length) {
        attendeeSuggestionsDiv.style.display = 'none';
        return;
    }
    attendeeSuggestions.forEach(user => {
        const div = document.createElement('div');
        div.className = 'suggestion';
        div.textContent = user.fullName || (user.firstName + ' ' + user.lastName) || user.email;
        div.addEventListener('click', () => {
            addAttendee(user);
            attendeeSuggestionsDiv.style.display = 'none';
            attendeeSearchInput.value = '';
        });
        attendeeSuggestionsDiv.appendChild(div);
    });
    attendeeSuggestionsDiv.style.display = 'block';
}

function addAttendee(user) {
    if (selectedAttendees.some(a => a.userId === user.userId)) return;
    selectedAttendees.push(user);
    renderSelectedAttendees();
}

function removeAttendee(userId) {
    selectedAttendees = selectedAttendees.filter(a => a.userId !== userId);
    renderSelectedAttendees();
}

function renderSelectedAttendees() {
    const selectedAttendeesDiv = document.getElementById('selected-attendees');
    selectedAttendeesDiv.innerHTML = '';
    selectedAttendees.forEach(user => {
        const chip = document.createElement('span');
        chip.className = 'attendee-chip';
        chip.dataset.userid = user.userId;
        chip.textContent = user.fullName || (user.firstName + ' ' + user.lastName) || user.email;
        const removeBtn = document.createElement('button');
        removeBtn.className = 'remove-attendee';
        removeBtn.type = 'button';
        removeBtn.innerHTML = '&times;';
        removeBtn.addEventListener('click', () => removeAttendee(user.userId));
        chip.appendChild(removeBtn);
        selectedAttendeesDiv.appendChild(chip);
    });
}

function setupAttendeeSearch() {
    const attendeeSearchInput = document.getElementById('attendeeSearch');
    const attendeeSuggestionsDiv = document.getElementById('attendee-suggestions');
    if (!attendeeSearchInput) return;
    attendeeSearchInput.addEventListener('input', function() {
        const query = this.value.trim();
        if (attendeeSearchTimeout) clearTimeout(attendeeSearchTimeout);
        if (!query) {
            attendeeSuggestionsDiv.style.display = 'none';
            return;
        }
        attendeeSearchTimeout = setTimeout(async () => {
            try {
                const users = await apiGet(`/users/search?term=${encodeURIComponent(query)}`);
                attendeeSuggestions = (users || []).filter(u => !selectedAttendees.some(a => a.userId === u.userId));
                renderAttendeeSuggestions();
            } catch {
                attendeeSuggestions = [];
                renderAttendeeSuggestions();
            }
        }, 250);
    });
    document.getElementById('meetingBookingForm').addEventListener('reset', () => {
        selectedAttendees = [];
        renderSelectedAttendees();
        attendeeSuggestionsDiv.style.display = 'none';
    });
}

window.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('jwtToken');
    const userJson = localStorage.getItem('user');
    if (!token || !userJson) {
        window.location.href = 'login.html';
        return;
    }
    loadRooms();
    loadRoomOptions();
    loadMeetings();
    setupAttendeeSearch();
    document.getElementById("roomSearch")?.addEventListener("keyup", filterRooms);
    document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');
    // Only set up search-prefixes listeners once
    if (!window._searchPrefixesSetup) {
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
        window._searchPrefixesSetup = true;
    }
});

async function loadMeetings() {
    const tableBody = document.querySelector('#meetingInfoTable tbody');
    if (!tableBody) return;

    try {
        const meetings = await apiGet('/meetings') || [];
        const userId = getCurrentUserId();

        const filtered = meetings.filter(m => m.organizer?.id === userId || (m.attendees || []).some(a => a.user?.id === userId));

        const now = new Date();
        const validMeetings = filtered.filter(m => {
            if (m.meetingStatus === 'Completed') {
                const end = new Date(m.meetingEndTime);
                return (now - end) < 24 * 60 * 60 * 1000;
            }
            return true;
        });

        tableBody.innerHTML = '';
        if (!validMeetings.length) {
            tableBody.innerHTML = '<tr><td colspan="6" style="text-align:center;color:#888;">No meeting(s) booked yet.</td></tr>';
            return;
        }

        validMeetings.forEach(meeting => {

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
            tr.innerHTML = `
                <td>${meeting.meetingTitle}</td>
                <td>${formatDateTime(meeting.meetingStartTime)}</td>
                <td>${formatDateTime(meeting.meetingEndTime)}</td>
                <td>${meeting.room?.roomName || ''}</td>
                <td><span class="badge ${status === 'Ongoing' ? 'green' : status === 'Cancelled' ? 'red' : status === 'Completed' ? 'grey' : 'blue'}">${status}</span></td>
                <td>
                    <button class="cancel-btn" data-id="${meeting.meetingId}" ${status==='Cancelled'||status==='Completed'?'disabled':''}>Cancel</button>
                    <button class="reschedule-btn" data-id="${meeting.meetingId}" ${status==='Cancelled'||status==='Completed'?'':'disabled'}>Reschedule</button>
                </td>
            `;
            tableBody.appendChild(tr);
        });

        document.querySelectorAll('.cancel-btn').forEach(btn => {
            btn.addEventListener('click', async function() {
                const id = this.getAttribute('data-id');
                if (confirm('Cancel this meeting?')) {
                    await apiPut(`/meetings/${id}`, { meetingStatus: 'Cancelled' });
                    loadMeetings();
                }
            });
        });
        document.querySelectorAll('.reschedule-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = this.getAttribute('data-id');
                startReschedule(id);
            });
        });
    } catch (err) {
        tableBody.innerHTML = `<tr><td colspan="6" style="color:red;">${err.message}</td></tr>`;
    }
}

async function startReschedule(meetingId) {
    try {
        const meeting = (await apiGet(`/meetings/${meetingId}`));
        if (!meeting) return;
        document.getElementById('meetingDate').value = meeting.meetingStartTime.slice(0,10);
        document.getElementById('startTime').value = meeting.meetingStartTime.slice(11,16);
        document.getElementById('endTime').value = meeting.meetingEndTime.slice(11,16);
        document.getElementById('meetingTitle').value = meeting.meetingTitle;
        document.getElementById('agenda').value = meeting.meetingAgenda || '';
        document.getElementById('roomSelect').value = meeting.room?.roomId || '';
        document.getElementById('meetingBookingForm').setAttribute('data-reschedule-id', meetingId);
    } catch {}
}

document.getElementById('meetingBookingForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    const form = this;
    const rescheduleId = form.getAttribute('data-reschedule-id');
    const date = document.getElementById('meetingDate').value;
    const startTime = document.getElementById('startTime').value;
    const endTime = document.getElementById('endTime').value;
    const title = document.getElementById('meetingTitle').value;
    const agenda = document.getElementById('agenda').value;
    const roomId = document.getElementById('roomSelect').value;
    const attendees = selectedAttendees.map(u => u.userId);
    const startStr = `${date}T${startTime}`;
    const endStr = `${date}T${endTime}`;
    const start = new Date(startStr);
    const end = new Date(endStr);

    const errorDiv = document.getElementById('booking-error-message');
    if (errorDiv) {
        errorDiv.style.display = 'none';
        errorDiv.textContent = '';
    }

    if (end <= start) {
        if (errorDiv) {
            errorDiv.textContent = 'Meeting end time must be later than start time.';
            errorDiv.style.display = 'block';
        }
        return;
    }

    const meetings = await apiGet('/meetings');
    const conflict = meetings.some(m => m.fkRoomId == roomId && m.meetingStatus !== 'Cancelled' &&
        ((start < new Date(m.meetingEndTime)) && (end > new Date(m.meetingStartTime))) && (!rescheduleId || m.meetingId != rescheduleId));
    if (conflict) {
        if (errorDiv) {
            errorDiv.textContent = 'The selected room is already booked for the given time slot.';
            errorDiv.style.display = 'block';
        }
        return;
    }

    const payload = {
        fkRoomId: parseInt(roomId),
        meetingStartTime: startStr,
        meetingEndTime: endStr,
        meetingTitle: title,
        meetingAgenda: agenda,
        attendeeUserIds: attendees.map(Number).filter(Boolean)
    };

    try {
        if (rescheduleId) {
            await apiPut(`/meetings/${rescheduleId}`, payload);
            form.removeAttribute('data-reschedule-id');
        } else {
            await apiPost('/meetings', payload);
        }
        form.reset();
        loadMeetings();
    } catch (err) {
        if (errorDiv) {
            let msg = err.message;
            try {
                const errObj = JSON.parse(msg);
                if (errObj && errObj.errors) {
                    msg = Object.values(errObj.errors).flat().join(' ');
                } else if (errObj.title) {
                    msg = errObj.title;
                }
            } catch {}
            if (typeof msg === 'string' && msg.length > 1 && msg[0] === '"' && msg[msg.length - 1] === '"') {
                msg = msg.slice(1, -1);
            }
            errorDiv.textContent = msg;
            errorDiv.style.display = 'block';
        }
    }
});

async function loadRoomOptions() {
    const select = document.getElementById('roomSelect');
    if (!select) return;
    try {
        const rooms = await apiGet('/rooms') || [];
        select.innerHTML = '<option value="">Select a room</option>';
        rooms.filter(room => room.isAvailable).forEach(room => {
            const opt = document.createElement('option');
            opt.value = room.roomId;
            opt.textContent = `${room.roomName} (${room.roomLocation})`;
            select.appendChild(opt);
        });
    } catch {}
}

window.addEventListener('DOMContentLoaded', () => {
    
    const token = localStorage.getItem('jwtToken');
    const userJson = localStorage.getItem('user');
    if (!token || !userJson) {
        window.location.href = 'login.html';
        return;
    }
    loadRooms();
    loadRoomOptions();
    loadMeetings();
});

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

async function loadRooms() {
    const tableBody = document.querySelector("#roomTable tbody");
    if (!tableBody) return;

    try {
        const rooms = await apiGet("/rooms") || [];

        tableBody.innerHTML = "";

        rooms.forEach(room => {

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

        filterRooms();
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

document.getElementById("roomSearch")?.addEventListener("keyup", filterRooms);

document.querySelector(".user-actions img").addEventListener("click", () => window.location.href='profile.html');
