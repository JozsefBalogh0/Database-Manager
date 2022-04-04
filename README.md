# Database-Manager
C# based SQL database manager app for Windows

Documentation only available in Hungarian


Program setup:
1. Import included SQL database template (The program will manage this)
2. Import user profile: 

CREATE USER 'leltarrendszer'@'localhost' IDENTIFIED BY 'Jelszo123!';

GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, DROP, ALTER, SHOW VIEW ON *.* TO 'leltarrendszer'@'localhost';
3. At first startup the program will ask for a password, this password is "root" without the quotation marks. This password can be changed in the settings.
