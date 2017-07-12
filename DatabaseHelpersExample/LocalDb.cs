﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace DatabaseHelpersExample
{
    public class LocalDb : IDisposable
    {
        bool _disposed;

        public static string DatabaseDirectory = "Data";

        public string ConnectionStringName { get; private set; }
        public string DatabaseName { get; private set; }
        public string OutputFolder { get; private set; }
        public string DatabaseMdfPath { get; private set; }
        public string DatabaseLogPath { get; private set; }

        public LocalDb(string databaseName = null)
        {
            DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? Guid.NewGuid().ToString("N") : databaseName;
            CreateDatabase();
        }

        public IDbConnection OpenConnection()
        {
            return new SqlConnection(ConnectionStringName);
        }

        private void CreateDatabase()
        {
            OutputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DatabaseDirectory);
            var mdfFilename = string.Format("{0}.mdf", DatabaseName);
            DatabaseMdfPath = Path.Combine(OutputFolder, mdfFilename);
            DatabaseLogPath = Path.Combine(OutputFolder, String.Format("{0}_log.ldf", DatabaseName));

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            // If the database does not already exist, create it.
            var connectionString = String.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                DetachDatabase();
                cmd.CommandText = String.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", DatabaseName, DatabaseMdfPath);
                cmd.ExecuteNonQuery();
            }

            // Open newly created, or old database.
            ConnectionStringName = String.Format(@"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", DatabaseName, DatabaseMdfPath);
        }

        void DetachDatabase()
        {
            try
            {
                var connectionString = String.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'", DatabaseName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            finally
            {
                if (File.Exists(DatabaseMdfPath)) File.Delete(DatabaseMdfPath);
                if (File.Exists(DatabaseLogPath)) File.Delete(DatabaseLogPath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LocalDb()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null
            DetachDatabase();

            _disposed = true;
        }
    }
}
