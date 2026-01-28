


Create database Finance_Management_System

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    DOB DATE NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Phone VARCHAR(15) NOT NULL UNIQUE,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Address VARCHAR(255) NULL,
    IsActive BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);
 
 
 CREATE TABLE UserBankDetails (
    BankDetailId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserId),
    BankName VARCHAR(100) NOT NULL,
    AccountNumber VARCHAR(30) NOT NULL,
    IFSCCode VARCHAR(20) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);


CREATE TABLE CardTypes (
    CardTypeId INT PRIMARY KEY IDENTITY(1,1),
    CardTypeName VARCHAR(50) NOT NULL,
    CardLimit DECIMAL(10,2) NOT NULL,
    JoiningFee DECIMAL(10,2) NOT NULL
);


CREATE TABLE EMICards (
    CardId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserId),
    CardTypeId INT NOT NULL FOREIGN KEY REFERENCES CardTypes(CardTypeId),
    CardNumber VARCHAR(20) NOT NULL UNIQUE,
    UsedLimit DECIMAL(10,2) DEFAULT 0,
    ValidTill DATE NOT NULL,
    IsActivated BIT DEFAULT 0
);


ALTER TABLE EMICards
ADD 
    CreditLimit DECIMAL(10,2) NOT NULL DEFAULT 0,
    AvailableLimit DECIMAL(10,2) NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE();

   ALTER TABLE EMICards
ALTER COLUMN CardNumber VARCHAR(100);


    CREATE TABLE Products (
        ProductId INT PRIMARY KEY IDENTITY(1,1),
        ProductName VARCHAR(100) NOT NULL,
        Description VARCHAR(255) NULL,
        Price DECIMAL(10,2) NOT NULL,
        ImageUrl VARCHAR(255) NULL,
        IsActive BIT DEFAULT 1
    );
    ALTER TABLE Products
add  Category VARCHAR(50);



CREATE TABLE Purchases (
    PurchaseId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
    CardId INT NOT NULL FOREIGN KEY REFERENCES EMICards(CardId),
    EMIMonths INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    PurchaseDate DATETIME DEFAULT GETDATE()
);

ALTER TABLE Purchases
ADD PaymentMode VARCHAR(20)  -- FULL / EMI
 

CREATE TABLE EMIPayments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    PurchaseId INT NOT NULL FOREIGN KEY REFERENCES Purchases(PurchaseId),
    EMIAmount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME NULL,
    PaymentStatus VARCHAR(20) DEFAULT 'Pending'
);
ALTER TABLE EMIPayments
ADD DueDate DATETIME NOT NULL DEFAULT GETDATE();

ALTER TABLE EMIPayments
ADD EMISequence INT NOT NULL DEFAULT 1;

UPDATE EMIPayments
SET EMISequence = 1,
    DueDate = DATEADD(MONTH, 0, GETDATE());

    ALTER TABLE EMIPayments
ADD IsPaid BIT NOT NULL DEFAULT 0;

ALTER TABLE EMIPayments
ADD PaymentMode VARCHAR(20) NOT NULL DEFAULT 'EMI';

 
CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    Amount DECIMAL(10,2) NOT NULL,
    TransactionType VARCHAR(20) NULL,
    TransactionDate DATETIME DEFAULT GETDATE()
);


CREATE TABLE Orders
(
    OrderId INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,
    ProductId INT NOT NULL,

    OrderAmount DECIMAL(10,2) NOT NULL,

    PaymentMode VARCHAR(20) NOT NULL, -- EMI / FULL
    EmiDuration INT NULL,              -- months (3,6,9)

    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),

    OrderStatus VARCHAR(20) DEFAULT 'Completed',

    CONSTRAINT FK_Orders_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId),

    CONSTRAINT FK_Orders_Products
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
 
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

INSERT INTO CardTypes (CardTypeName, CardLimit, JoiningFee)

VALUES

('Gold', 40000, 1000),

('Titanium', 80000, 2000);

 
INSERT INTO Products (ProductName, Description, Price, ImageUrl, IsActive)

VALUES

('Samsung Mobile', 'Samsung Galaxy smartphone with 128GB storage', 30000, 'samsung.jpg', 1),
 
('Dell Laptop', 'Dell Inspiron i5 laptop with 16GB RAM', 55000, 'dell_laptop.jpg', 1),
 
('LG Smart TV', '55-inch 4K Ultra HD Smart TV', 60000, 'lg_tv.jpg', 1),
 
('Washing Machine', 'Fully automatic front load washing machine', 28000, 'washing_machine.jpg', 1),
 
('Refrigerator', 'Double door refrigerator with inverter technology', 32000, 'refrigerator.jpg', 1);


INSERT INTO Products (ProductName, Description, Price, ImageUrl, IsActive, Category)
VALUES 
('HP Laptop', 'HP 15s Intel Core i5 Laptop', 55000.00, 'Hp.jpg', 1, 'Laptop'),

('Apple iPhone', 'iPhone 14 with A15 Bionic Chip', 69999.00, 'Iphone.jpg', 1, 'Mobile'),

('Lenovo Laptop', 'Lenovo IdeaPad Slim 3 Laptop', 48000.00, 'lonevo.jpg', 1, 'Laptop'),

('Redmi Mobile', 'Redmi Note 12 Pro Smartphone', 22999.00, 'redmi.jpg', 1, 'Mobile'),

('Sony TV', 'Sony Bravia 43 inch Smart LED TV', 42000.00, 'sony.jpg', 1, 'Television'),

('TCL TV', 'TCL 50 inch 4K Smart Android TV', 36000.00, 'tcl.jpg', 1, 'Television'),

('Vivo Mobile', 'Vivo V27 5G Smartphone', 32999.00, 'vivo.jpg', 1, 'Mobile');

 
   select * from Users 

    select * from Admins

    select * from CardTypes

    select * from EMICards  

    select * from EMIPayments

    select * from Orders  

    select * from Products

    select * from Purchases 

    select * from Transactions 

    select * from UserBankDetails 

    select * from UserCardApplications

 
 
                alter table Orders add Constraint FK_Orders_UserId foreign key (UserId )references Users (UserId) on delete cascade
                alter table EMICards add Constraint FK_EMICards_UserId foreign key (UserId )references Users (UserId) on delete cascade
                alter table  Purchases add Constraint FK_Purchases_UserId foreign key (UserId )references Users (UserId) on delete cascade
                alter table  Transactions add Constraint FK_Transactions_UserId foreign key (UserId )references Users (UserId) on delete cascade
                alter table  UserBankDetails add Constraint FK_UserBankDetails_UserId foreign key (UserId )references Users (UserId) on delete cascade
                alter table  UserCardApplications add Constraint FK_UserCardApplications_UserId foreign key (UserId )references Users (UserId) on delete cascade

                delete from EMIPayments
                delete from Orders
                delete from Purchases
                delete from Transactions
                delete from UserBankDetails
                delete from UserCardApplications
                delete from Users

                select * from EMIPayments
                select * from Users
                select * from UserCardApplications
                select * from UserBankDetails
                select * from Transactions
                select * from Purchases
                select * from Orders
                select * from Admins
                select * from CardTypes
                select * from Products
                select * from EMICards

                
