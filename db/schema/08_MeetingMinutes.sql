USE SmartMeetingRoomDB;

CREATE TABLE MeetingMinutes(
    MeetingMinutesId INT IDENTITY(1, 1) NOT NULL,
    FK_MeetingId INT NOT NULL,
    FK_UserId INT NOT NULL,
    MeetingSummary VARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME NULL,
    PRIMARY KEY(MeetingMinutesId),
    FOREIGN KEY(FK_MeetingId) REFERENCES Meeting(MeetingId),
    FOREIGN KEY(FK_UserId) REFERENCES [User](UserId),
    CONSTRAINT CHK_Timestamps CHECK (LastUpdated >= CreatedAt)
);