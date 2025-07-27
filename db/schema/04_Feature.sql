USE SmartMeetingRoomDB;

CREATE TABLE Feature(
    FeatureId TINYINT IDENTITY(1, 1) NOT NULL,
    FeatureName VARCHAR(50) UNIQUE NOT NULL,
    FeatureDescription VARCHAR(255) NULL DEFAULT 'No description provided',
    PRIMARY KEY(FeatureId)
);