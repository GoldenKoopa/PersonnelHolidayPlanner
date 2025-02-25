-- ai generated
-- prompt: <PHP_CreateDatabase.sql> create some sample data, at least
-- 			5 employees, 3 projects and 3 leaves per employee between
-- 			january 2025 and april 2025


-- Insert sample employees
INSERT INTO Employee (FirstName, LastName)
VALUES
    ('John', 'Doe'),
    ('Jane', 'Smith'),
    ('Michael', 'Johnson'),
    ('Emily', 'Brown'),
    ('David', 'Wilson');

-- Insert sample projects
INSERT INTO Project (Name, StartDate, EndDate)
VALUES
    ('Project Alpha', '2025-01-01', '2025-06-30'),
    ('Project Beta', '2025-02-15', '2025-08-31'),
    ('Project Gamma', '2025-03-01', '2025-12-31');

-- Insert sample leave days for each employee
INSERT INTO Leave (EmployeeId, StartDate, EndDate, LeaveType)
VALUES
    -- John Doe's leaves
    (1, '2025-01-15', '2025-01-17', 'Vacation'),
    (1, '2025-02-20', '2025-02-21', 'Personal'),
    (1, '2025-03-10', '2025-03-12', 'Sick'),

    -- Jane Smith's leaves
    (2, '2025-01-22', '2025-01-24', 'Vacation'),
    (2, '2025-02-28', '2025-02-28', 'Personal'),
    (2, '2025-04-05', '2025-04-07', 'Sick'),

    -- Michael Johnson's leaves
    (3, '2025-02-01', '2025-02-05', 'Vacation'),
    (3, '2025-03-15', '2025-03-15', 'Personal'),
    (3, '2025-04-10', '2025-04-11', 'Sick'),

    -- Emily Brown's leaves
    (4, '2025-01-05', '2025-01-09', 'Vacation'),
    (4, '2025-02-14', '2025-02-14', 'Personal'),
    (4, '2025-03-20', '2025-03-22', 'Sick'),

    -- David Wilson's leaves
    (5, '2025-03-01', '2025-03-05', 'Vacation'),
    (5, '2025-04-01', '2025-04-01', 'Personal'),
    (5, '2025-04-15', '2025-04-16', 'Sick');

-- Assign employees to projects
INSERT INTO EmployeeProject (EmployeeId, ProjectId)
VALUES
    (1, 1), (1, 2),
    (2, 1), (2, 3),
    (3, 2), (3, 3),
    (4, 1), (4, 3),
    (5, 2);
