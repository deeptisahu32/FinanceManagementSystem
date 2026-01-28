select * from Users

CREATE TABLE UserCardApplications (
    ApplicationId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    CardTypeId INT NOT NULL,
    Status VARCHAR(20) DEFAULT 'Pending',
    AppliedOn DATETIME DEFAULT GETDATE()
);
