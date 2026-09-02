using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Biblioteca.Datos
{
    public sealed class SqlDatabase
    {
        private readonly string _connectionString;

        public SqlDatabase()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["BibliotecaDb"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new ConfigurationErrorsException("No se encontró la cadena de conexión BibliotecaDb en App.config.");
            _connectionString = settings.ConnectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public DataTable Query(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand(commandText, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.CommandType = commandType;
                if (parameters != null) command.Parameters.AddRange(parameters);
                adapter.Fill(table);
            }
            return table;
        }

        public int Execute(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.CommandType = commandType;
                if (parameters != null) command.Parameters.AddRange(parameters);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public object Scalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.CommandType = commandType;
                if (parameters != null) command.Parameters.AddRange(parameters);
                connection.Open();
                return command.ExecuteScalar();
            }
        }

        public static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }
}

