# Dashboard Covid-19
This dashboard shows data about Covid-19 using the daily report published by [European Centre for Disease Prevention and Control (ECDC)](https://www.ecdc.europa.eu/en/publications-data/download-todays-data-geographic-distribution-covid-19-cases-worldwide).
 
 It shows worlwide information and it allows to select a country to view its details.

 ## Data Source
 The source data is the daily report published by [European Centre for Disease Prevention and Control (ECDC)](https://www.ecdc.europa.eu/en/publications-data/download-todays-data-geographic-distribution-covid-19-cases-worldwide).
## Data Source
The dashboard uses SQL Server as Data Source.
In folder `SQL` you'll find the scirpt `create-db.sql` to create the DB `covid_data` and the table `daily_global`.

The folder `DataFiller` contains a dotnet console application useful to populate the DB.

You have to build it with:
```
dotnet clean
dotnet publish -c Release
```
This app downloads the latest report from ECDC web site and then it can works in two different ways: it can generate a well formatted CSV or it can insert data into the SQL table `covid_data.dbo.daily_global`.
### CSV
In this mode, the DataFiller will download the latest ECDC report and it will generate a CSV file. You can use this file to import the data into the SQL DB.
```
# USAGE: 
bin/Release/netcoreapp3.1/publish/DataFiller.dll csv [OUTPUT_FILE]

# EXAMPLE:
bin/Release/netcoreapp3.1/publish/DataFiller.dll csv report.csv
```
Now you can manually import into SQL table `daily_global` the generated file `report.csv`.
### SQL
In this mode, the DataFiller will download the latest ECDC report and it will insert the downloaded data into the SQL table `covid_data.dbo.daily_global`. Please note that all previous data in this table will be deleted before the insert.
You can  run it with the following:
```
# USAGE
dotnet bin/Release/netcoreapp3.1/publish/DataFiller.dll sql [SQL_HOST] [SQL_PORT] [SQL_USER] [SQL_PASSWORD]

# EXAMPLE:
dotnet bin/Release/netcoreapp3.1/publish/DataFiller.dll sql localhost 1433 SA SQLserver2020 
```
