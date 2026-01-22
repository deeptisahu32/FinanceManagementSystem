


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

CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    ProductName VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl VARCHAR(255) NULL,
    IsActive BIT DEFAULT 1
);

CREATE TABLE Purchases (
    PurchaseId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
    CardId INT NOT NULL FOREIGN KEY REFERENCES EMICards(CardId),
    EMIMonths INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    PurchaseDate DATETIME DEFAULT GETDATE()
);


CREATE TABLE EMIPayments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    PurchaseId INT NOT NULL FOREIGN KEY REFERENCES Purchases(PurchaseId),
    EMIAmount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME NULL,
    PaymentStatus VARCHAR(20) DEFAULT 'Pending'
);


CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    Amount DECIMAL(10,2) NOT NULL,
    TransactionType VARCHAR(20) NULL,
    TransactionDate DATETIME DEFAULT GETDATE()
);

select * from CardTypes

select * from Products


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

 