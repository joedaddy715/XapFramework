using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Extensions;

namespace Xap.Data.Factory {
    internal class XapDb:IXapDataProvider {
        #region "Constructors"
        private XapDb(IXapDbConnectionContext dbConnectionContext) {
            _dbConnectionContext = dbConnectionContext;
        }

        internal static IXapDataProvider Create(IXapDbConnectionContext dbConnectionContext) {
            return new XapDb(dbConnectionContext);
        }
        #endregion

        #region "Properties"
        private IDbConnection _connection;
        private IDbCommand _dbCommand;

        private IXapDataConnectionProvider _connectionProvider;
        private IXapDbConnectionContext _dbConnectionContext;
        private List<IXapDbParameter> _parameterList = new List<IXapDbParameter>();
        #endregion

        #region "Connection Methods"

        private void OpenConnection() {
            try {
                _connectionProvider = DbFactory.Instance.LoadDataConnectionProvider(_dbConnectionContext.DataProvider);
                _connection = _connectionProvider.GetConnection(_dbConnectionContext.ConnectionString);

                if (!string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    CreateCommand();
                } else {
                    throw new XapException("TSQL must be provided");
                }
            } catch (Exception ex) {
                DbConnectionContextService.Instance.Clear();
                throw new XapException($"Error opening data connection for {_dbConnectionContext.DbKey}",ex);
            }
        }

        void IXapDataProvider.CloseConnection() {
            try {
                _parameterList.Clear();
                if (_connection != null) {
                    if (_connection.State != ConnectionState.Closed) {
                        _connection.Close();
                        _connection = null;
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error closing data connection for {_dbConnectionContext.DbKey}", ex);
            }
        }

        bool IXapDataProvider.IsConnectionValid() {
            try {
                _connectionProvider = DbFactory.Instance.LoadDataConnectionProvider(_dbConnectionContext.DataProvider);
                _connection = _connectionProvider.GetConnection(_dbConnectionContext.ConnectionString);

                if (_connection != null) {
                    return true;
                }

                return false;
            }catch(Exception ex) {
                return false;
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }
        #endregion

        #region "Command methods"
        private void CreateCommand() {
            try {
                _dbCommand = _connection.CreateCommand();
                if (_dbConnectionContext.TSql.Substring(0, 6).ToUpper() == "SELECT" | _dbConnectionContext.TSql.Substring(0, 6).ToUpper() == "UPDATE" | _dbConnectionContext.TSql.Substring(0, 6).ToUpper() == "DELETE" | _dbConnectionContext.TSql.Substring(0, 6).ToUpper() == "INSERT") {
                    _dbCommand.CommandType = CommandType.Text;
                    _dbCommand.CommandText = _dbConnectionContext.TSql;
                } else {
                    string tmpSql = ReadSqlFile(_dbConnectionContext.TSql);
                    if (string.IsNullOrEmpty(tmpSql)) {
                        _dbCommand.CommandType = CommandType.StoredProcedure;
                        _dbCommand.CommandText = _dbConnectionContext.TSql;
                    } else {
                        _dbCommand.CommandType = CommandType.Text;
                        _dbCommand.CommandText = tmpSql;
                    }
                }

                AddCommandParameters();
            } catch (Exception ex) {
                throw new XapException($"Error creating data command for {_dbConnectionContext.DbKey}", ex);
            }
        }
        #endregion

        #region "Parameter Methods"
        IXapDataProvider IXapDataProvider.AddParameter(IXapDbParameter param) {
            _parameterList.Add(param);
            return this;
        }

        private void AddCommandParameters() {
            try {
                foreach (IXapDbParameter p in _parameterList) {
                    IDataParameter param = _dbCommand.CreateParameter();
                    if (_dbCommand.CommandType == CommandType.Text) {
                        if (p.ParameterValue is string) {
                            _dbCommand.CommandText = _dbCommand.CommandText.Replace(p.ParameterName, "'" + PrepSql(p.ParameterValue.ToString()) + "'");
                        } else if (p.ParameterValue is bool) {
                            if (p.ParameterValue.ToString().ToLower() == "true") {
                                _dbCommand.CommandText = _dbCommand.CommandText.Replace(p.ParameterName, "1");
                            } else {
                                _dbCommand.CommandText = _dbCommand.CommandText.Replace(p.ParameterName, "0");
                            }
                        } else {
                            _dbCommand.CommandText = _dbCommand.CommandText.Replace(p.ParameterName, p.ParameterValue.ToString());
                        }
                    } else {
                        param.ParameterName = p.ParameterName;
                        param.Value = p.ParameterValue;
                        param.Direction = p.ParameterDirection;
                        _dbCommand.Parameters.Add(param);
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error adding command parameters for {_dbConnectionContext.DbKey}", ex);
            }
        }
        #endregion

        #region "Helper methods"
        private string PrepSql(string sql) {
            return sql.Replace("'", "''");
        }

        private string ReadSqlFile(string fileName) {
            System.IO.StreamReader sr = null;
            try {
                if (!string.IsNullOrWhiteSpace(_dbConnectionContext.SqlLocation)) {
                    if (System.IO.File.Exists($"{_dbConnectionContext.SqlLocation}{fileName}.sql")) {
                        sr = new System.IO.StreamReader($"{_dbConnectionContext.SqlLocation}{fileName}.sql");
                        return sr.ReadToEnd();
                    }
                }
                return string.Empty;
            } catch (Exception ex) {
                throw new XapException($"Error reading in tsql file for {_dbConnectionContext.DbKey}", ex);
            } finally {
                if (sr != null) {
                    sr.Close();
                }
            }
        }
        #endregion

        #region "Data access methods"
        int IXapDataProvider.ExecuteNonQuery() {
            try {
                OpenConnection();
                if (_dbCommand.CommandText.ToLower().Contains("insert")) {
                    object retVal = _dbCommand.ExecuteScalar();
                    if (retVal != null) {
                        return int.Parse(retVal.ToString());
                    }
                } else {
                    _dbCommand.ExecuteNonQuery();
                }
                return 0;
            } catch (Exception ex) {
                throw new XapException($"Error executing non query for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<int> IXapDataProvider.ExecuteNonQueryAsync() {
            try {
                Func<int> function = new Func<int>(() => ((IXapDataProvider)this).ExecuteNonQuery());
                return await Task.Run<int>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing non query async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        public XapDataReader ExecuteReader() {
            try {
                OpenConnection();
                return new XapDataReader(_dbCommand.ExecuteReader());
            } catch (Exception ex) {
                throw new XapException($"Error executing reader for {_dbConnectionContext.DbKey}", ex);
            }
        }

        async Task<XapDataReader> IXapDataProvider.ExecuteReaderAsync() {
            try {
                Func<XapDataReader> function = new Func<XapDataReader>(() => ExecuteReader());
                return await Task.Run<XapDataReader>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing reader async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        object IXapDataProvider.ExecuteScalar() {
            try {
                OpenConnection();
                return _dbCommand.ExecuteScalar();
            } catch (Exception ex) {
                throw new XapException($"Error executing scalar for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<object> IXapDataProvider.ExecuteScalarAsync() {
            try {
                Func<object> function = new Func<object>(() => ((IXapDataProvider)this).ExecuteScalar());
                return await Task.Run<object>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing scalar async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        DataTable IXapDataProvider.ExecuteDataTable() {
            try {
                OpenConnection();

                XapDataReader dr = new XapDataReader(_dbCommand.ExecuteReader());
                DataTable dataTable = new DataTable();

                AddDataTableColumns(dataTable, dr);

                while (dr.Read()) {
                    AddDataRow(dataTable, dr);
                }
                return dataTable;
            } catch (Exception ex) {
                throw new XapException($"Error executing datatable for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<DataTable> IXapDataProvider.ExecuteDataTableAsync() {
            try {
                Func<DataTable> function = new Func<DataTable>(() => ((IXapDataProvider)this).ExecuteDataTable());
                return await Task.Run<DataTable>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing datatable async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        DataSet IXapDataProvider.ExecuteDataSet() {
            try {
                OpenConnection();
                XapDataReader dr = new XapDataReader(_dbCommand.ExecuteReader());
                DataSet dataSet = new DataSet();
                DataTable dataTable = null;

                do {
                    dataTable = new DataTable();
                    AddDataTableColumns(dataTable, dr);

                    dataSet.Tables.Add(dataTable);

                    while (dr.Read()) {
                        AddDataRow(dataTable, dr);
                    }
                } while (dr.NextResult());
                return dataSet;
            } catch (Exception ex) {
                throw new XapException($"Error executing dataset for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<DataSet> IXapDataProvider.ExecuteDataSetAsync() {
            try {
                Func<DataSet> function = new Func<DataSet>(() => ((IXapDataProvider)this).ExecuteDataSet());
                return await Task.Run<DataSet>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing dataset async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        private void AddDataTableColumns(DataTable dataTable, XapDataReader dr) {
            for (int x = 0; x <= dr.FieldCount - 1; x++) {
                DataColumn column = new DataColumn(dr.GetName(x), dr.GetFieldType(x));
                dataTable.Columns.Add(column);
            }
        }

        private void AddDataRow(DataTable dataTable, XapDataReader dr) {
            object obj = null;
            Type t = null;
            DataRow dataRow = dataTable.NewRow();
            for (int i = 0; i <= dr.FieldCount - 1; i++) {
                obj = dr.GetValue(i);
                t = dr.GetFieldType(i);

                if (obj == null || string.IsNullOrEmpty(obj.ToString())) {
                    dataRow[i] = t.GetDefaultValue();
                } else {
                    dataRow[i] = dr.GetValue(i);
                }
            }
            dataTable.Rows.Add(dataRow);
        }

        XmlDocument IXapDataProvider.ExecuteXml() {
            ArrayList columnNames = null;
            try {
                OpenConnection();

                XapDataReader dr = new XapDataReader(_dbCommand.ExecuteReader());
                XmlDocument xDoc = new XmlDocument();
                XmlElement rootNode = xDoc.CreateElement("ResultSets");
                xDoc.AppendChild(rootNode);
                do {
                    columnNames = new ArrayList(dr.FieldCount);
                    for (int x = 0; x <= dr.FieldCount - 1; x++) {
                        columnNames.Add(dr.GetName(x));
                    }

                    string obj = string.Empty;

                    XmlElement resultNode = xDoc.CreateElement("ResultSet");
                    XmlElement rowNode = null;

                    XmlElement columnNode = null;

                    while (dr.Read()) {
                        rowNode = xDoc.CreateElement("Row");

                        for (int i = 0; i <= dr.FieldCount - 1; i++) {
                            columnNode = xDoc.CreateElement(columnNames[i].ToString());
                            obj = dr.GetValue(i).ToString().Trim();

                            if (obj == null || string.IsNullOrEmpty(obj.ToString())) {
                                obj = string.Empty;
                            }
                            columnNode.InnerText = obj.ToString();
                            rowNode.AppendChild(columnNode);
                        }
                        resultNode.AppendChild(rowNode);
                    }
                    rootNode.AppendChild(resultNode);
                } while (dr.NextResult());
                return xDoc;
            } catch (Exception ex) {
                throw new XapException($"Error executing xml for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<XmlDocument> IXapDataProvider.ExecuteXmlAsync() {
            try {
                Func<XmlDocument> function = new Func<XmlDocument>(() => ((IXapDataProvider)this).ExecuteXml());
                return await Task.Run<XmlDocument>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing xml async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        //TODO:  Look at injecting XapEnvironment class into DbProvider
        void IXapDataProvider.ExecuteCsv(string filePath) {
            StringBuilder sb = new StringBuilder();
            StreamWriter _logger = null;
            string tmp = string.Empty;
            try {
                _logger = new StreamWriter(XapEnvironment.Instance.MapFolderPath(filePath), false);

                OpenConnection();

                XapDataReader dr = new XapDataReader(_dbCommand.ExecuteReader());

                for (int x = 0; x < dr.FieldCount; x++) {
                    if (x + 1 < dr.FieldCount) {
                        _logger.Write("\"" + dr.GetName(x) + "\"" + ",");
                    } else {
                        _logger.Write("\"" + dr.GetName(x) + "\"\n");
                    }
                }

                string obj = string.Empty;
                while (dr.Read()) {
                    for (int i = 0; i < dr.FieldCount; i++) {
                        obj = dr.GetValue(i).ToString();

                        if (obj == null || string.IsNullOrEmpty(obj.ToString())) {
                            if (i + 1 < dr.FieldCount) {
                                _logger.Write("\"" + string.Empty + "\"" + ",");
                            } else {
                                _logger.Write("\"" + string.Empty + "\"\n");
                            }
                        } else {
                            if (i + 1 < dr.FieldCount) {
                                _logger.Write("\"" + dr.GetValue(i).ToString().TrimEnd() + "\"" + ",");
                            } else {
                                _logger.Write("\"" + dr.GetValue(i).ToString().TrimEnd() + "\"\n");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error executing csv for {_dbConnectionContext.DbKey}", ex);
            } finally {
                _logger.Close();
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task IXapDataProvider.ExecuteCsvAsync(string filePath) {
            try {
                Action function = new Action(() => ((IXapDataProvider)this).ExecuteCsv(filePath));
                await Task.Run(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing csv async for {_dbConnectionContext.DbKey}", ex);
            }
        }

        string IXapDataProvider.ExecuteJson() {
            StringBuilder json = new StringBuilder();
            string retVal = string.Empty;

            try {
                OpenConnection();

                XapDataReader dr = new XapDataReader(_dbCommand.ExecuteReader());

                json.AppendLine("{\"data\":[");
                do {
                    string obj = string.Empty;
                    string colName = string.Empty;

                    while (dr.Read()) {
                        json.AppendLine("{");

                        for (int i = 0; i <= dr.FieldCount - 1; i++) {
                            obj = dr.GetValue(i).ToString().Trim();
                            colName = dr.GetName(i);
                            if (obj == null || string.IsNullOrEmpty(obj.ToString())) {
                                obj = string.Empty;
                            }
                            json.AppendLine(string.Format("\"{0}\":\"{1}\",", colName, obj.ToString()));
                        }
                        json = json.RemoveLast(",");
                        json.AppendLine("},");
                    }
                } while (dr.NextResult());
                json = json.RemoveLast(",");
                json.AppendLine("]}");
                retVal = json.ToString();
                return retVal;
            } catch (Exception ex) {
                throw new XapException($"Error executing json for {_dbConnectionContext.DbKey}", ex);
            } finally {
                ((IXapDataProvider)this).CloseConnection();
            }
        }

        async Task<string> IXapDataProvider.ExecuteJsonAsync() {
            try {
                Func<string> function = new Func<string>(() => ((IXapDataProvider)this).ExecuteJson());
                return await Task.Run<string>(function);
            } catch (Exception ex) {
                throw new XapException($"Error executing json async for {_dbConnectionContext.DbKey}", ex);
            }
        }
        #endregion
    }
}
