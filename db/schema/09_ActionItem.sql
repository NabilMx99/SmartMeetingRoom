USE SmartMeetingRoomDB;

CREATE TABLE ActionItem(
    ActionItemId INT IDENTITY(1, 1) NOT NULL,
    FK_MeetingMinutesId INT NOT NULL,
    FK_UserId INT NOT NULL,
    ActionItemDueDate DATETIME NOT NULL,
    ActionItemDescription VARCHAR(MAX) NOT NULL,
    ActionItemStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',
    PRIMARY KEY(ActionItemId),
    FOREIGN KEY(FK_MeetingMinutesId) REFERENCES MeetingMinutes(MeetingMinutesId),
    FOREIGN KEY(FK_UserId) REFERENCES [User](UserId),
    CONSTRAINT CHK_ActionItemStatus CHECK (ActionItemStatus IN ('Pending', 'In Progress', 'Completed'))
);