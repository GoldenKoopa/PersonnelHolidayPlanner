# PersonnelHolidayPlanner (PHP)

## Configuration:
- clone repository
- setup a mssql server
- run sql create (and optionally fill) script[^1]
- copy .env.example to .env
- fill .env with the correct details
- `dotnet run` to run

## Headless:
To use the program headless you can also run `dotnet run` and specify the arguments.
To be able to access the help page you must first build the project (`dotnet build`)
and execute it via `bin/Debug/net9.0/PersonnelHolidayPlanner --help`, otherwise the
help page for the dotnet command will be displayed.

Examples for the headless usage can be found in `headless_examples.txt`.


[^1]: sample data from the fill script is setup that it shows all functionality
for employee with id 1
