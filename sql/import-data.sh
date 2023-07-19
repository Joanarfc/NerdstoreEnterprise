# The script will wait for 90 seconds to allow for the provisioning and start of a database
sleep 90s
# Command to create the database
/opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U SA -P "MeuDB@123" -i DatabaseCreation.sql