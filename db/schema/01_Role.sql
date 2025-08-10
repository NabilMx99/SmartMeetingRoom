USE SmartMeetingRoomDB;

CREATE TABLE [Role](
    RoleId INT IDENTITY(1, 1) NOT NULL,
    RoleName VARCHAR(20) UNIQUE NOT NULL,
    RoleDescription VARCHAR(255) NULL DEFAULT 'No description provided',
    PRIMARY KEY(RoleId),
    CONSTRAINT CHK_RoleName CHECK (RoleName IN ('Admin', 'Employee', 'Guest'))
);