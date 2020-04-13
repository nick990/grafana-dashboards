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

       

        static void ConvertCsvToSqlCsv(string inputFilePath, String outputFilePath){
            InitFile(outputFilePath);
            StreamWriter fileOutput = new StreamWriter(outputFilePath, true);
            fileOutput.WriteLine("timestamp,country,geoId,cases,deaths"); //header      
            StreamReader fileInput = new StreamReader(inputFilePath);
            string line = fileInput.ReadLine(); //Skip first line
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            while((line = fileInput.ReadLine()) != null)  
            {  
                //var fields = line.Split(',');
                var fields = CSVParser.Split(line);    
                var date = DateTime.ParseExact(fields[0], "dd/MM/yyyy", null);
                var ts = ConvertToUnixTime(date);
                string country = fields[6].Replace(@"""",String.Empty).Replace(",",@"\,").Replace(" ",@"\ ");
                string cases = fields[4];
                string deaths = fields[5];
                string geoId = fields[7];
                string new_line = $"{ts},\"{country}\",{geoId},{cases},{deaths}";
                fileOutput.WriteLine(new_line);
            }  
            fileInput.Close();
            fileOutput.Close();
        }

        static void InsertFromEcdcCsv(String inputFilePath, SqlConnection connection){
            new SqlCommand("DELETE FROM daily_global",connection).ExecuteNonQuery();
            StreamReader fileInput = new StreamReader(inputFilePath);
            string line = fileInput.ReadLine(); //Skip first line
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            while((line = fileInput.ReadLine()) != null)  
            {  
                //var fields = line.Split(',');
                var fields = CSVParser.Split(line);    
                var date = DateTime.ParseExact(fields[0], "dd/MM/yyyy", null);
                var ts = ConvertToUnixTime(date);
                string country = fields[6].Replace(@"""",String.Empty).Replace(",",@"\,").Replace(" ",@"\ ");
                string cases = fields[4];
                string deaths = fields[5];
                string geoId = fields[7];
                string popData = fields[9];
                if(String.IsNullOrEmpty(popData))
                    popData="0";
                string query = $"INSERT INTO daily_global (ts,country,geoId,cases,deaths) VALUES ({ts},'{country}','{geoId}',{cases},{deaths})";
                //string query = $"EXEC sp_insert_ecdc @ts={ts}, @geoId='{geoId}', @country='{country}', @cases={cases}, @deaths={deaths}, @popData={popData}";
                Console.WriteLine(query);
                new SqlCommand(query,connection).ExecuteNonQuery();
            }  
            fileInput.Close();
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
        

        static void Main(string[] args)
        {   
            string csvInputFilePath = @"./data.csv";
            downloadFile("https://opendata.ecdc.europa.eu/covid19/casedistribution/csv", csvInputFilePath);
            String command = args[0];
            switch(command){
                case "sql":
                    String sqlServer = args[1];
                    String sqlPort = args[2];
                    String sqlUser = args[3];
                    String sqlPassword = args[4];
                    var conn = GetSQLConnection(sqlServer, sqlPort, sqlUser, sqlPassword);
                    conn.Open();                   
                    InsertFromEcdcCsv(csvInputFilePath, conn);
                    conn.Close();
                break;
                case "csv":
                    string csvOutputFilePath = args[1];
                    ConvertCsvToSqlCsv(csvInputFilePath,csvOutputFilePath);                
                break;
            }
            File.Delete(csvInputFilePath);           
        }
    }
}
