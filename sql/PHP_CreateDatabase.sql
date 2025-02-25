CREATE DATABASE PHP;
GO

USE PHP;
GO

-- create Employee table
CREATE TABLE Employee (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL
);

-- create Project table
CREATE TABLE Project (
    ProjectId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE
);

-- create Leave table
CREATE TABLE Leave (
    LeaveId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT FOREIGN KEY REFERENCES Employee(EmployeeId),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL
);

-- Create EmployeeProject table (for n:m relationship)
CREATE TABLE EmployeeProject (
    EmployeeId INT FOREIGN KEY REFERENCES Employee(EmployeeId),
    ProjectId INT FOREIGN KEY REFERENCES Project(ProjectId),
    PRIMARY KEY (EmployeeId, ProjectId)
);
