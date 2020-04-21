using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace DataFiller
{
    class Program
    {
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalMilliseconds;
        }

        public static void InitFile(string fileName){
            if (!File.Exists(fileName))    
            {    
                File.Create(fileName).Close();
            }
            File.WriteAllText(fileName, String.Empty);
        }
        
        static void downloadFile(String url, String path){
            using (var client = new WebClient())
            {
                client.DownloadFile(url, path);
            }
        }
      
        
        static SqlConnection GetSQLConnection(String sqlServer, String sqlPort, String sqlUser, String sqlPassword){
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["server"] = $"{sqlServer},{sqlPort}"; 
            builder.UserID = sqlUser;            
            builder.Password = sqlPassword;     
            builder.InitialCatalog = "covid_data";
            builder["Trusted_Connection"] = true;
            builder.ConnectTimeout = 10;
            builder.IntegratedSecurity = false;
            return new SqlConnection(builder.ConnectionString);
        }
        
        static void InsertFromDpcItalyCsv(SqlConnection connection){
            string csvInputFilePath = @"./data.csv";
            downloadFile("https://raw.githubusercontent.com/pcm-dpc/COVID-19/master/dati-andamento-nazionale/dpc-covid19-ita-andamento-nazionale.csv", csvInputFilePath);

            new SqlCommand("DELETE FROM covid_data.dbo.pcm_dpc_italy",connection).ExecuteNonQuery();

            StreamReader csvInputFile = new StreamReader(csvInputFilePath);
            string line = csvInputFile.ReadLine(); //Skip first line
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
               while((line = csvInputFile.ReadLine()) != null)  
            {  
                var fields = CSVParser.Split(line); 
                var dateFields = fields[0].Substring(0,10).Split("-");
                var date = DateTime.ParseExact($"{dateFields[2]}/{dateFields[1]}/{dateFields[0]}", "dd/MM/yyyy", null);
                var ts = ConvertToUnixTime(date);
                String hospitalisedWithSymptoms = fields[2];
                String intensiveCare = fields[3];
                String totalHospitalised = fields[4];
                String homeConfinement = fields[5];
                String currentPositiveCases = fields[6];
                String newCurrentPositiveCases = fields[7];
                String newPositiveCases = fields[8];
                String recovered = fields[9];
                String deaths = fields[10];
                String totalPositiveCases = fields[11];
                String testsPerformed = fields[12];

                string query = $"INSERT INTO covid_data.dbo.pcm_dpc_italy (ts, hospitalisedWithSymptoms, intensiveCare, totalHospitalised, homeConfinement, currentPositiveCases, newCurrentPositiveCases, newPositiveCases, recovered, deaths, totalPositiveCases, testsPerformed) VALUES({ts}, {hospitalisedWithSymptoms}, {intensiveCare}, {totalHospitalised}, {homeConfinement}, {currentPositiveCases}, {newCurrentPositiveCases}, {newPositiveCases}, {recovered}, {deaths}, {totalPositiveCases}, {testsPerformed});";
                Console.WriteLine(query);
                new SqlCommand(query,connection).ExecuteNonQuery();
            }  
            csvInputFile.Close();
            File.Delete(csvInputFilePath);
        }

        static void InsertFromDpcRegionsCsv(SqlConnection connection){
            string csvInputFilePath = @"./data.csv";
            downloadFile("https://raw.githubusercontent.com/pcm-dpc/COVID-19/master/dati-regioni/dpc-covid19-ita-regioni.csv", csvInputFilePath);

            new SqlCommand("DELETE FROM covid_data.dbo.pcm_dpc_regions",connection).ExecuteNonQuery();
              
            StreamReader csvInputFile = new StreamReader(csvInputFilePath);
            string line = csvInputFile.ReadLine(); //Skip first line
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
               while((line = csvInputFile.ReadLine()) != null)  
            {  
                var fields = CSVParser.Split(line); 
                var dateFields = fields[0].Substring(0,10).Split("-");
                var date = DateTime.ParseExact($"{dateFields[2]}/{dateFields[1]}/{dateFields[0]}", "dd/MM/yyyy", null);
                var ts = ConvertToUnixTime(date);
                String regionCode = fields[2];
                String regionName = fields[3];
                String hospitalisedWithSymptoms = fields[6];
                String intensiveCare = fields[7];
                String totalHospitalised = fields[8];
                String homeConfinement = fields[9];
                String currentPositiveCases = fields[10];
                String newCurrentPositiveCases = fields[11];
                String newPositiveCases = fields[12];
                String recovered = fields[13];
                String deaths = fields[14];
                String totalPositiveCases = fields[15];
                String testsPerformed = fields[16];

                string query = $"INSERT INTO covid_data.dbo.pcm_dpc_regions (ts, regionCode, regionName, hospitalisedWithSymptoms, intensiveCare, totalHospitalised, homeConfinement, currentPositiveCases, newCurrentPositiveCases, newPositiveCases, recovered, deaths, totalPositiveCases, testsPerformed) VALUES({ts}, {regionCode}, '{regionName.Replace("'", "''")}', {hospitalisedWithSymptoms}, {intensiveCare}, {totalHospitalised}, {homeConfinement}, {currentPositiveCases}, {newCurrentPositiveCases}, {newPositiveCases}, {recovered}, {deaths}, {totalPositiveCases}, {testsPerformed});";
                Console.WriteLine(query);
                new SqlCommand(query,connection).ExecuteNonQuery();
            }  
            csvInputFile.Close();
            File.Delete(csvInputFilePath);
        }

        static void InsertFromDpcProvincesCsv(SqlConnection connection){
            string csvInputFilePath = @"./data.csv";
            downloadFile("https://raw.githubusercontent.com/pcm-dpc/COVID-19/master/dati-province/dpc-covid19-ita-province.csv", csvInputFilePath);

            new SqlCommand("DELETE FROM covid_data.dbo.pcm_dpc_provinces",connection).ExecuteNonQuery();

            StreamReader csvInputFile = new StreamReader(csvInputFilePath);
            string line = csvInputFile.ReadLine(); //Skip first line
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            while((line = csvInputFile.ReadLine()) != null)  
            {  
                var fields = CSVParser.Split(line); 
                var dateFields = fields[0].Substring(0,10).Split("-");
                var date = DateTime.ParseExact($"{dateFields[2]}/{dateFields[1]}/{dateFields[0]}", "dd/MM/yyyy", null);
                var ts = ConvertToUnixTime(date);
                String provinceCode = fields[4];
                String provinceName = fields[5];
                String totalPositiveCases = fields[9];
                int provinceCodeInt = Int32.Parse(provinceCode);
                if(provinceCodeInt >= 979 && provinceCodeInt <= 999)
                    continue;
                string query = $"INSERT INTO  covid_data.dbo.pcm_dpc_provinces (ts, provinceCode, provinceName, totalPositiveCases) VALUES ({ts}, {provinceCode}, '{provinceName.Replace("'","''")}', {totalPositiveCases})";
                Console.WriteLine(query);
                new SqlCommand(query,connection).ExecuteNonQuery();
            }  
            csvInputFile.Close();
            File.Delete(csvInputFilePath);
        }


        static void Main(string[] args)
        {   
            
            String command = args[0];
            switch(command){
                case "sql":
                    String sqlServer = args[1];
                    String sqlPort = args[2];
                    String sqlUser = args[3];
                    String sqlPassword = args[4];
                    var conn = GetSQLConnection(sqlServer, sqlPort, sqlUser, sqlPassword);
                    conn.Open();                    
                    InsertFromDpcItalyCsv(conn);
                    InsertFromDpcRegionsCsv(conn);
                    InsertFromDpcProvincesCsv(conn);
                    conn.Close();
                break;
                case "csv":
                    string csvOutputFilePath = args[1];         
                break;
            }
        }
    }
}
