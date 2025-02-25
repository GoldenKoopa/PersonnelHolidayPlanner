CREATE DATABASE PHP;
GO

USE PHP;
GO

-- create Employee table
CREATE TABLE Employee (
    EmployeeId INT IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK__Employee PRIMARY KEY (EmployeeId)
);

-- create Project table
CREATE TABLE Project (
    ProjectId INT IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CONSTRAINT PK__Project PRIMARY KEY (ProjectId)
);

-- create Leave table
CREATE TABLE Leave (
    LeaveId INT IDENTITY(1,1),
    EmployeeId INT,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    CONSTRAINT PK__Leave PRIMARY KEY (LeaveId),
    CONSTRAINT FK__Leave__Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId)
);

-- Create EmployeeProject table (for n:m relationship)
CREATE TABLE EmployeeProject (
    EmployeeId INT,
    ProjectId INT,
    CONSTRAINT PK__EmployeeProject PRIMARY KEY (EmployeeId, ProjectId),
    CONSTRAINT FK__EmployeeProject__Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
    CONSTRAINT FK__EmployeeProject__Project FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId)
);

