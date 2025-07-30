USE SmartMeetingRoomDB;

CREATE TABLE Room(
    RoomId INT IDENTITY(1, 1) NOT NULL,
    RoomName VARCHAR(50) UNIQUE NOT NULL,
    RoomLocation VARCHAR(100) NOT NULL,
    RoomCapacity TINYINT NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    PRIMARY KEY(RoomId),
    CONSTRAINT CHK_RoomCapacity CHECK (RoomCapacity > 0),
    CONSTRAINT CHK_RoomStatus CHECK (IsAvailable IN (0, 1))
);