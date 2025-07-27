USE SmartMeetingRoomDB;

CREATE TABLE Meeting(
    MeetingId INT IDENTITY(1, 1) NOT NULL,
    FK_UserId INT NOT NULL,
    FK_RoomId INT NOT NULL,
    MeetingStartTime DATETIME NOT NULL,
    MeetingEndTime DATETIME NOT NULL,
    MeetingAgenda VARCHAR(MAX) NULL,
    MeetingTitle VARCHAR(100) NOT NULL,
    MeetingStatus VARCHAR(20) NOT NULL DEFAULT 'Scheduled',
    PRIMARY KEY(MeetingId),
    FOREIGN KEY(FK_UserId) REFERENCES [User](UserId),
    FOREIGN KEY(FK_RoomId) REFERENCES Room(RoomId),
    CONSTRAINT CHK_MeetingTime CHECK (MeetingEndTime > MeetingStartTime),
    CONSTRAINT CHK_MeetingStatus CHECK (MeetingStatus IN ('Scheduled', 'Ongoing', 'Completed', 'Cancelled'))
);