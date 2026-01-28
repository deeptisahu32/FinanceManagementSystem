select * from Users

CREATE TABLE UserCardApplications (
    ApplicationId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    CardTypeId INT NOT NULL,
    Status VARCHAR(20) DEFAULT 'Pending',
    AppliedOn DATETIME DEFAULT GETDATE()
);

ALTER TABLE UserCardApplications
ADD CONSTRAINT FK_UserCardApplications_User
FOREIGN KEY (UserId)
REFERENCES [Users](UserId);


ALTER TABLE UserCardApplications
ADD CONSTRAINT FK_UserCardApplications_CardType
FOREIGN KEY (CardTypeId)
REFERENCES CardTypes(CardTypeId);



select * from UserCardApplications

select * from EMICards