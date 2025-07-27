USE SmartMeetingRoomDB;

CREATE TABLE RoomFeature(
    RoomFeatureId INT IDENTITY(1, 1) NOT NULL,
    FK_RoomId INT NOT NULL,
    FK_FeatureId TINYINT NOT NULL,
    PRIMARY KEY(RoomFeatureId),
    FOREIGN KEY(FK_RoomId) REFERENCES Room(RoomId),
    FOREIGN KEY(FK_FeatureId) REFERENCES Feature(FeatureId)
);