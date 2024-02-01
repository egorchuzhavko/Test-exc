# Test exc
Entity framework + Postgresql + jwt token + serilog (logs are created in the logs folder in the root folder of the project) 
(connection string in appsettings.json in project)

Before first use api you should make few steps:
1) Open project in VS;
2) Open package manager console;
3) 3) Write 'Update-Database'
4) Then open terminal;
5) Write in terminal 'cd vebTechTest';
6) Then write in terminal 'dotnet run filldata'
7) Press the keyboard shortcut ctrl+c.
This is necessary to populate the database with data.

To Get JWT token you have to make post request 'LoginJwt' where you have to write in name 'Log' and in password 'Pas'. 
Then you have to get your token in write him in authorization panel.
