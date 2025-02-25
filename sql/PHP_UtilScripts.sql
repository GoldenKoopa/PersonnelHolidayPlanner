-- get employees of each project
select
	p."name",
	e.firstname,
	e.lastname
from
	project p
inner join employeeproject ep on
	p.projectid = ep.projectid
inner join employee e on
	e.employeeid = ep.employeeid
order by
	p.projectid;

-- get leaves for each employee
select
	e.firstname,
	e.lastname,
	ld.LeaveDayId,
	ld.StartDate,
	ld.EndDate,
	ld.LeaveType
from
	Employee e
inner join Leave ld on
	e.EmployeeId = ld.EmployeeId
order by
	e.EmployeeId;
