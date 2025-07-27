USE SmartMeetingRoomDB;

CREATE TABLE Attendee(
    AttendeeId INT IDENTITY(1, 1) NOT NULL,
    FK_UserId INT NOT NULL,
    FK_MeetingId INT NOT NULL,
    AttendeeStatus VARCHAR(20) NOT NULL DEFAULT 'Invited',
    PRIMARY KEY(AttendeeId),
    FOREIGN KEY(FK_UserId) REFERENCES [User](UserId),
    FOREIGN KEY(FK_MeetingId) REFERENCES Meeting(MeetingId),
    CONSTRAINT CHK_AttendeeStatus CHECK (AttendeeStatus IN ('Invited', 'Accepted', 'Declined', 'Attended', 'Absent')),
    CONSTRAINT UQ_UserMeeting UNIQUE(FK_UserId, FK_MeetingId)
);