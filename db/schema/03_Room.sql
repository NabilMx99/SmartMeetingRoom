USE SmartMeetingRoomDB;

CREATE TABLE Room(
    RoomId INT IDENTITY(1, 1) NOT NULL,
    RoomName VARCHAR(50) UNIQUE NOT NULL,
    RoomLocation VARCHAR(100) NOT NULL,
    RoomCapacity TINYINT NOT NULL,
    RoomStatus BIT NOT NULL DEFAULT 1, -- 1 = Available, 0 = Not Available
    PRIMARY KEY(RoomId),
    CONSTRAINT CHK_RoomCapacity CHECK (RoomCapacity > 0),
    CONSTRAINT CHK_RoomStatus CHECK (RoomStatus IN (0, 1))
);