using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.SqlCe;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System.Data.SqlServerCe;
using System.Data.SqlClient;

namespace RDCore
{
    /// <summary>
    /// The Base class that is a wrapper for the Database. It makes a Database call
    /// Logs if there is error and returns DataSet object. All component call to the 
    /// Database is routed through this class
    /// </summary>
    public class DataBaseManager
    {
        private string CONNECTION_STRING = "ConnectionString";
        public Database db;

        /// <summary>
        /// The Base DAC class that calls the Database and in case of sql exception logs its else
        /// returns the dataset
        /// </summary>
        public DataBaseManager()
        {
            db = EnterpriseLibraryContainer.Current.GetInstance<Database>(CONNECTION_STRING);
        }

        /// <summary>
        /// Gets databse connection of given connectionstring
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <remarks>Gets databse connection of given connectionstring</remarks>
        public DataBaseManager(string connectionStringKey)
        {
            CONNECTION_STRING = connectionStringKey;
            db = EnterpriseLibraryContainer.Current.GetInstance<Database>(CONNECTION_STRING);
        }

        /// <summary>
        /// Gets databse connection of given connectionstring and specific provider
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="Provider"></param>
        /// <example>new DataBaseManager("Initial Catalog=(local);User Id=sa;Password=sa","System.Data.SqlClient")</example>
        /// <remarks>Gets databse connection of given connectionstring and specific provider</remarks>
        public DataBaseManager(string connectionString, string Provider)
        {
            CONNECTION_STRING = connectionString;
            db = new GenericDatabase(CONNECTION_STRING, System.Data.Common.DbProviderFactories.GetFactory(Provider));
        }

        /// <summary>
        /// Executes the stored proceedure and returns a DataSet
        /// </summary>
        /// <param name="dbCommand">DbCommand</param>
        /// <returns>Dataset</returns>
        public DataSet ExecuteDataSet(DbCommand dbCommand)
        {
            DataSet returnObject = null;
            try
            {
                //returnObject = Tracert.Trace<DataSet>(() => db.ExecuteDataSet(dbCommand), this.GetType());                
                returnObject = db.ExecuteDataSet(dbCommand);
            }
            catch
            {
                if (dbCommand.Connection != null
                    && dbCommand.Connection.State == ConnectionState.Open)
                {
                    dbCommand.Connection.Close();
                }
                throw;
            }

            return returnObject;
        }

        /// <summary>
        /// Executes the stored proceedure and returns a DataReader
        /// </summary>
        /// <param name="dbCommand">DbCommand</param>
        /// <returns>Dataset</returns>
        public IDataReader ExecuteReader(DbCommand dbCommand)
        {
            IDataReader dr = null;
            try
            {
                //returnObject = Tracert.Trace<DataSet>(() => db.ExecuteDataSet(dbCommand), this.GetType());            
                dr = db.ExecuteReader(dbCommand);
            }
            catch
            {
                if (dbCommand.Connection != null
                    && dbCommand.Connection.State == ConnectionState.Open)
                {
                    dbCommand.Connection.Close();
                }
                throw;
            }

            return dr;
        }

        /// <summary>
        /// Executes the stored proceedure and returns effected rows
        /// </summary>
        /// <param name="dbCommand">DbCommand</param>
        /// <returns>int</returns>
        public int ExecuteNonQuery(DbCommand dbCommand)
        {
            int returnObject = 0;
            try
            {
                //returnObject = Tracert.Trace<int>(() => db.ExecuteNonQuery(dbCommand), this.GetType());
                returnObject = db.ExecuteNonQuery(dbCommand);
            }
            catch
            {
                if (dbCommand.Connection != null
                    && dbCommand.Connection.State == ConnectionState.Open)
                {
                    dbCommand.Connection.Close();
                }
                throw;
            }
            return returnObject;
        }

        /// <summary>
        /// Executes the stored proceedure and returns 
        /// </summary>
        /// <param name="dbCommand">DbCommand</param>
        /// <returns>object</returns>
        public object ExecuteScalar(DbCommand dbCommand)
        {
            object returnObject = null;
            try
            {
                //returnObject = Tracert.Trace<object>(() => db.ExecuteScalar(dbCommand), this.GetType());            
                returnObject = db.ExecuteScalar(dbCommand);
            }
            catch
            {
                if (dbCommand.Connection != null
                    && dbCommand.Connection.State == ConnectionState.Open)
                {
                    dbCommand.Connection.Close();
                }
                throw;
            }
            return returnObject;
        }

        /// <summary>
        /// Checks if data exists
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private bool IsDataExist(DataSet dataSet)
        {
            int tableCount = 0;
            bool returnValue = true;
            foreach (DataTable dt in dataSet.Tables)
            {
                if (dt.Rows.Count == 0)
                    tableCount = tableCount + 1;
            }
            if (tableCount == dataSet.Tables.Count)
            {
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            this.AddInParameter(command, name, dbType, value, true);
        }

        /// <summary>
        /// Adding optional parameters
        /// </summary>
        /// <param name="command">command object</param>
        /// <param name="name">parameter name</param>
        /// <param name="dbType">DB Type</param>
        /// <param name="value">value</param>
        /// <param name="isDefaultValueIsNotNull">set to true if for any data type should go as null, if the value has default value</param>
        public void AddInParameter(DbCommand command, string name, DbType dbType, object value, bool isDefaultValueIsNotNull)
        {
            if (value != null && isDefaultValueIsNotNull)
            {
                if ((value.GetType() == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                    || (value.GetType() == typeof(Int64) && (Int64)value == 0)
                    || (value.GetType() == typeof(Int32) && (Int32)value == 0)
                    || (value.GetType() == typeof(double) && (double)value == 0)
                    || (value.GetType() == typeof(string) && value.ToString().Trim() == string.Empty)
                   )
                {
                    value = DBNull.Value;
                }
            }
            db.AddInParameter(command, name, dbType, value);
        }

        public void AddOutParameter(DbCommand command, string name, DbType dbType)
        {
            db.AddParameter(command, name, dbType, int.MaxValue, ParameterDirection.InputOutput, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        public void AddOutParameter(DbCommand command, string name, DbType dbType, object value)
        {
            db.AddParameter(command, name, dbType, int.MaxValue, ParameterDirection.InputOutput, true, 0, 0, String.Empty, DataRowVersion.Default, value);
        }

        public void AddTableValueParameter(DbCommand command, string name, object value)
        {
            SqlParameter parm = new SqlParameter(name, value);
            parm.SqlDbType = SqlDbType.Structured;
            command.Parameters.Add(parm);
        }

        public object GetParameterValue(DbCommand command, string name)
        {
            return db.GetParameterValue(command, name);
        }

        public void SetParameterValue(DbCommand command, string paramName, object value)
        {
            if ((value.GetType() == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                    || (value.GetType() == typeof(Int64) && (Int64)value == 0)
                    || (value.GetType() == typeof(Int32) && (Int32)value == 0)
                    || (value.GetType() == typeof(double) && (double)value == 0)
                    || (value.GetType() == typeof(string) && value.ToString().Trim() == string.Empty)
                   )
            {
                value = DBNull.Value;
            }
            db.SetParameterValue(command, paramName, value);
        }

        /// <summary>
        /// Get stored procedure command
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            return db.GetStoredProcCommand(storedProcedureName);
        }

        public DbCommand GetSqlStringCommand(string query)
        {
            return db.GetSqlStringCommand(query);
        }

        public void CloseConnection(DbCommand dbCommand, IDataReader reader)
        {
            if (reader != null || !reader.IsClosed)
            {
                reader.Close();
                reader.Dispose();
            }

            if (dbCommand.Connection != null
                    && dbCommand.Connection.State == ConnectionState.Open)
            {
                dbCommand.Connection.Close();
            }
            dbCommand.Dispose();
        }

        public DataSet ExecuteDataSet(String Query)
        {
            DataSet returnObject = null;
            SqlCeDatabase sqlCEdb = new SqlCeDatabase(db.ConnectionString);
            try
            {
                returnObject = sqlCEdb.ExecuteDataSetSql(Query);
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlCEdb.CloseSharedConnection();
            }

            return returnObject;
        }

        public int ExecuteNonQuery(String Query)
        {
            int returnObject = 0;
            SqlCeDatabase sqlCEdb = new SqlCeDatabase(db.ConnectionString);
            try
            {
                returnObject = sqlCEdb.ExecuteNonQuerySql(Query);
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlCEdb.CloseSharedConnection();
            }
            return returnObject;
        }

        public object ExecuteScalar(String Query)
        {
            object returnObject = null;
            SqlCeDatabase sqlCEdb = new SqlCeDatabase(db.ConnectionString);
            try
            {
                returnObject = sqlCEdb.ExecuteScalarSql(Query);
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlCEdb.CloseSharedConnection();
            }
            return returnObject;
        }
    }
}
