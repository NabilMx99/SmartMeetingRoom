const API_BASE_URL = "https://localhost:7085/api";

// Store JWT in localStorage
function setToken(token) {
    localStorage.setItem("jwtToken", token);
}

function getToken() {
    return localStorage.getItem("jwtToken");
}

// Generic GET request
async function apiGet(endpoint) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: "GET",
        headers: buildHeaders()
    });
    return handleResponse(response);
}

// Generic POST request
async function apiPost(endpoint, data) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: "POST",
        headers: buildHeaders(),
        body: JSON.stringify(data)
    });
    return handleResponse(response);
}

// Generic PUT request
async function apiPut(endpoint, data) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: "PUT",
        headers: buildHeaders(),
        body: JSON.stringify(data)
    });
    return handleResponse(response);
}

// Generic DELETE request
async function apiDelete(endpoint) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: "DELETE",
        headers: buildHeaders()
    });
    return handleResponse(response);
}

// Build headers with Content-Type, Accept, and Authorization
function buildHeaders() {
    const headers = {
        "Content-Type": "application/json",
        "Accept": "application/json"
    };
    const token = getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
    return headers;
}

// Handle API responses
async function handleResponse(response) {
    let data = null;

    try {
        if (response.status !== 204) {
            data = await response.json();
        }
    } catch (err) {
        console.error("Failed to parse JSON:", err);
        data = null;
    }

    if (!response.ok) {
        let message = "Unknown error";
        if (data) {
            message = data.message || data.Message || JSON.stringify(data);
        }
        throw new Error(message);
    }

    return data;
}
