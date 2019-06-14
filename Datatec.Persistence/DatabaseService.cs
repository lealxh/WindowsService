using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;

namespace Datatec.Persistence
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly string _connectionName;
        private readonly ILogService _logService;

        public DatabaseService(ILogService logService)
        {
            _connectionName = ConfigurationManager.AppSettings["ConnectionName"];
            _connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            this._logService = logService;
            
        }

        private SqlParameter CreateParam(object value,string paramName, System.Data.SqlDbType type,int length)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = type;
            parameter.ParameterName = paramName;
            parameter.Size = length;
            parameter.Value = value;

            return parameter;
        }
        private SqlParameter CreateParam(object value, string paramName, System.Data.SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = type;
            parameter.ParameterName = paramName;
            parameter.Value = value;
            return parameter;
        }

        public IEnumerable<object> CreateParameters(string data)
        {

            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(CreateParam(DateTime.Now, "@fecha",System.Data.SqlDbType.DateTime));
            parametros.Add(CreateParam("DOLAR", "@moneda", System.Data.SqlDbType.Char,6));
            string valorStr = data.Substring(10, 9).Trim().Replace('.', ',');
            decimal valorDec = 0;
            decimal.TryParse(valorStr, out valorDec);
            parametros.Add(CreateParam(valorDec, "@precio", System.Data.SqlDbType.Decimal));
            string factorStr = ConfigurationManager.AppSettings["Factor"];
            decimal factorDec = 0;
            decimal.TryParse(factorStr, out factorDec);
            parametros.Add(CreateParam(factorDec, "@factor", System.Data.SqlDbType.Decimal));

            return parametros;
        }

        public void ExecuteQuery(string query, IEnumerable<object> Params)
        {

            SqlConnection sqlConnection= new SqlConnection(_connectionString);
            try
            {
                    sqlConnection.Open();
                    using (SqlCommand queryCommand = new SqlCommand(query, sqlConnection))
                    {
                        foreach (var item in Params)
                        {
                            queryCommand.Parameters.Add(item);
                        }
                        queryCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        queryCommand.ExecuteNonQuery();
                    }
            }
            catch (Exception ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());

            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();

            }
                
        

        }
    }
}
