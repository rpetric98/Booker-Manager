-- 1-to-N
CREATE TABLE UserRole (
    UserRoleID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserRoleName NVARCHAR(100) NULL
);
-- 1-to-N
-- User
CREATE TABLE UserDetail (
    UserDetailID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Username NVARCHAR(100) NULL,
    PasswordHash NVARCHAR(256) NULL,
    PasswordSalt NVARCHAR(256) NULL,
    FirstName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NULL,
    Email NVARCHAR(100) NULL,
    Phone NVARCHAR(100) NULL,
    UserRoleID INT NULL,
    CreatedAt DATETIME NULL,
    CONSTRAINT FK_UserDetail_UserRole FOREIGN KEY (UserRoleID) REFERENCES UserRole(UserRoleID)
);

CREATE TABLE UserMessage ( 
	UserMessageId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY, 
	Username NVARCHAR(100) NULL, 
	Message NVARCHAR(MAX) NULL, 
	CreatedAt DATETIME NULL 
) 

SET IDENTITY_INSERT UserRole ON;

INSERT INTO UserRole (UserRoleID, UserRoleName) VALUES (1, 'User');
INSERT INTO UserRole (UserRoleID, UserRoleName) VALUES (2, 'Admin');

SET IDENTITY_INSERT UserRole OFF;

CREATE TABLE Log (
    LogId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Timestamp DATETIME NULL,
    Level NVARCHAR(100) NULL,
    Message NVARCHAR(100) NULL
);
-- 1-to-N
CREATE TABLE PropertyType (
    PropertyTypeID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PropertyTypeName NVARCHAR(100) NULL
);
-- Primary
CREATE TABLE Property (
    PropertyID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PropertyTypeID INT NULL,
    Name NVARCHAR(100) NULL,
    Description NVARCHAR(max) NULL,
    Address NVARCHAR(255) NULL,
    City NVARCHAR(100) NULL,
    ZipCode NVARCHAR(100) NULL, 
	Country NVARCHAR(100) NULL, 
    PricePerNight INT NULL, 
    MaxGuests INT NULL, 
    CreatedAt DATETIME NULL,
    CONSTRAINT FK_Property_PropertyType FOREIGN KEY (PropertyTypeID) REFERENCES PropertyType(PropertyTypeID) ON DELETE CASCADE,
); 
-- 1-to-N
CREATE TABLE Reservation (
    ReservationId INT PRIMARY KEY IDENTITY(1,1),  
    PropertyId INT NULL,                        
    Username NVARCHAR(100) NULL,                
    CheckInDate DATE NULL,                      
	CheckOutDate DATE NULL, 
	TotalPrice INT NULL, 
    NumberOfDays INT NULL, 
	CreatedAt DATETIME NULL, 
    CONSTRAINT FK_Reservation_Property FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId) ON DELETE CASCADE 
); 
-- M-to-N
CREATE TABLE Amenity (
    AmenityID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AmenityName NVARCHAR(255) NULL
);
-- M-to-N
CREATE TABLE PropertyAmenity (
    PropertyAmenityID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PropertyID INT NULL,
    AmenityID INT NULL,
    CONSTRAINT FK_PropertyAmenity_Property FOREIGN KEY (PropertyID) REFERENCES Property(PropertyID) ON DELETE CASCADE,
    CONSTRAINT FK_PropertyAmenity_Amenity FOREIGN KEY (AmenityID) REFERENCES Amenity(AmenityID) ON DELETE CASCADE
);






