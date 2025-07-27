USE SmartMeetingRoomDB;

CREATE TABLE [User](
    UserId INT IDENTITY(1, 1) NOT NULL,
    FK_RoleId TINYINT NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    PhoneNumber VARCHAR(25) UNIQUE NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    UserStatus BIT NOT NULL DEFAULT 0, -- 1 = Online and 0 = Offline
    PRIMARY KEY(UserId),
    FOREIGN KEY(FK_RoleId) REFERENCES [Role](RoleId),
    CONSTRAINT CHK_PhoneNumberFormat CHECK (PhoneNumber LIKE '+%' OR PhoneNumber LIKE '[0-9]%'),
    CONSTRAINT CHK_EmailFormat CHECK (Email LIKE '%_@_%._%'),
    CONSTRAINT CHK_UserStatus CHECK (UserStatus IN (0, 1))
);