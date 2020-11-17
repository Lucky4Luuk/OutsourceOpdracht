using System;
using System.Threading;
using System.Globalization;

using MySql.Data.MySqlClient;

namespace Database
{
    public class MySQLDatabaseConnection : IDatabaseConnection, IDisposable
    {
        private MySqlConnection conn { get; }

        private readonly string connStr;

        public MySQLDatabaseConnection(string address, string databaseName, string password)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            connStr = $"Server = {address};Uid = {databaseName};Database = {databaseName};Pwd = {password};";
            conn = new MySqlConnection(connStr);

            Console.WriteLine("Connecting to MySQL database...");
            conn.Open();
            Console.WriteLine("Connection to MySQL Database completed!");
        }

        public void Dispose()
        {
            CloseConnection();
            conn.Dispose();
        }

        public void CloseConnection() => conn.Close();

        public bool IsOpen()
        {
            if (conn.State == System.Data.ConnectionState.Closed) return false;
            return conn.Ping();
        }

        public Pdf GetPdf(int pdf_id)
        {
            string sql = $"SELECT * from `pdf` WHERE ID = {pdf_id}";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                byte[] data = (byte[])rdr[1];
                Pdf result = new Pdf(pdf_id, data);
                rdr.Close();
                return result;
            }
            rdr.Close();

            //Throw error
            throw new ApplicationException("Failed to get PDF!");
        }

        public void AddPDF(Pdf pdf)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO pdf(ID, Data) VALUES(?ID, ?Data)";
            cmd.Parameters.AddWithValue("?ID", pdf.id);
            cmd.Parameters.AddWithValue("?Data", pdf.data);
            cmd.ExecuteNonQuery();
        }

        public void UploadPDF(Pdf pdf)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `pdf` SET `ID`=?ID,`Data`=?Data WHERE ID=?ID";
            cmd.Parameters.AddWithValue("?ID", pdf.id);
            cmd.Parameters.AddWithValue("?Data", pdf.data);
            cmd.ExecuteNonQuery();
        }
    }
}
