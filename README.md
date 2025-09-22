## 📅 Smart Meeting Room & Minutes Management System

An **ASP.NET Core web application** developed as my **internship project**, designed to streamline **meeting room bookings**, **attendee management**, and **meeting minutes recording**, with **Zoom API integration** for virtual meetings. Built with **.NET Core backend** and **HTML/CSS/JS frontend**.


## 📋 Table of Contents
- [Features]()
- [Technologies Used]()
- [Database Design]()
- [API Endpoints]()
- [Recordings]()
- [Setup Instructions]()
- [Future Enhancements]()

## ⚙️ Features

- **User Management**
  - Register and login.  
  - Profile management.  
  - Role-based access (Admin, Employee, Guest). 

- **Room Management**
  - Create, update, or delete rooms (Admin only).
  - Assign features to rooms (e.g., projector, video 
conferencing).
  - View room capacity, location, status, and features.

- **Booking System**
  - Book a room for a time slot.
  - Check room availability via a list view.
  - Cancel or reschedule bookings.

- **Meeting Setup**
  - Schedule a meeting (tied to a booking).
  - Add title, agenda, and attendees (internal users).
  - Join Zoom meetings via a link generated through **Zoom API**.

- **Minutes of Meeting (MoM)** *(not implemented ⚠️)*
  - Add action items, discussion points, decisions.
  - Assign action items to attendees.

- **Notifications** *(not implemented ⚠️)*
  - Notify users of upcoming meetings, booking confirmations, and assigned action items.  

- **Dashboard**
  - Overview of upcoming meetings, rooms, and notifications.

## 🛠️ Technologies Used

- **Frontend:** HTML, CSS, JavaScript
- **Backend:** ASP.NET Core Web API, .NET 9.0
- **Database:** SQL Server Express
- **Authentication:** JWT & ASP.NET Core Identity
- **External APIs:** Zoom API (https://api.zoom.us/v2/)
- **Version Control:** Git & GitHub

## 🛢️ Database Design

**ER diagram** :

<img width="2963" height="1113" alt="SmartMeetingRoom-ERD (draw io)" src="https://github.com/user-attachments/assets/89b317e9-9c57-4540-94e2-ec6960dfcfa5" />

**Tables** :

- **Role**  
  - `RoleId` (PK), `RoleName`, `RoleDescription`

- **User**  
  - `UserId` (PK), `RoleId` (FK), `FirstName`, `LastName`, `PhoneNumber`, `Email`, `PasswordHash`, `IsOnline`

- **Room**  
  - `RoomId` (PK), `RoomName`, `RoomLocation`, `RoomCapacity`, `RoomStatus`, `IsAvailable`

- **Feature**  
  - `FeatureId` (PK), `FeatureName`, `FeatureDescription`

- **RoomFeature**  
  - `RoomFeatureId` (PK), `RoomId` (FK), `FeatureId` (FK)

- **Meeting**  
  - `MeetingId` (PK), `UserId` (FK), `RoomId` (FK), `MeetingStartTime`, `MeetingEndTime`, `MeetingTitle`, `MeetingAgenda`, `MeetingStatus`

- **Attendee**  
  - `AttendeeId` (PK), `MeetingId` (FK), `UserId` (FK), `AttendeeStatus`

- **MeetingMinutes**  
  - `MeetingMinutesId` (PK), `MeetingId` (FK), `UserId` (FK), `MeetingSummary`, `LastUpdated`, `CreatedAt`

- **ActionItem**  
  - `ActionItemId` (PK), `MeetingMinutesId` (FK), `UserId` (FK), `ActionItemDueDate`, `ActionItemDescription`, `ActionItemStatus`

## 🌐 API Endpoints

Some important API endpoints :

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - User login (JWT issued)
- `PUT /api/users/{id}` - Update a user profile
- `DELETE /api/users/{id}` - Delete a user account
- `GET /api/rooms` – List all rooms
- `POST /api/rooms` – Create a room (Admin only)
- `POST /api/meetings` - Schedule a new meeting (integrates with **Zoom API**)

## 🎥 Recordings

https://github.com/user-attachments/assets/2231d9c6-40f7-49d5-b54b-f132eda52561

https://github.com/user-attachments/assets/822ddc40-63ee-4df3-af40-d319cebedf9a

https://github.com/user-attachments/assets/7c9b1a6b-c96f-41ff-84f8-3eb463233775

https://github.com/user-attachments/assets/d030256e-ebf1-4b35-a019-6b2bf6501e94

## 🛠 Setup Instructions

> Will be added later

## 🔜 Future Enhancements

- Full real-time notifications for upcoming meetings.
- Calendar integration with Outlook/Google Calendar.
- Analytics & reporting for meeting efficiency.
- Mobile-friendly design for on-the-go usage.
