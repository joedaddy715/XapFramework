using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Services;

namespace Xap.Data.Factory.Poco {
    internal class XapPoco : IXapPoco {
        #region "Constructors"
        private XapPoco(IXapDbConnectionContext dbConnectionContext) {
            _dbConnectionContext = dbConnectionContext;
        }

        internal static IXapPoco Create(IXapDbConnectionContext dbConnectionContext) {
            return new XapPoco(dbConnectionContext);
        }
        #endregion

        #region "Properties"
        private IXapDbConnectionContext _dbConnectionContext;
        private IXapDataProvider _dataProvider;
        private IXapPocoMap pocoMap;
        #endregion

        #region "CRUD Operations"
        T IXapPoco.Insert<T>(T obj) {
            try {
                pocoMap = PocoMapService.Instance.GetPocoMap(obj);

                if (string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    _dbConnectionContext.TSql = pocoMap.InsertProcedure;
                }

                _dataProvider = DbFactory.Instance.Db(_dbConnectionContext);
                DoInsertUpdate(obj, PocoOperationType.Insert);
                int idField = _dataProvider.ExecuteNonQuery();
                obj.GetType()?.GetProperty(pocoMap.GetIdentityField()).SetValue(obj, idField);
                return obj;
            } catch (Exception ex) {
                throw new XapException($"Error performing insert for {typeof(T).FullName}");
            }
        }

        async Task<T> IXapPoco.InsertAsync<T>(T obj) {
            try {
                Func<T> function = new Func<T>(() => ((IXapPoco)this).Insert<T>(obj));
                return await Task.Run(function);
            } catch (Exception ex) {
                throw new XapException($"Error performing async insert for {typeof(T).FullName}",ex);
            }
        }

        T IXapPoco.Select<T>(T obj) {
            try {
                pocoMap = PocoMapService.Instance.GetPocoMap(obj);

                if (string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    _dbConnectionContext.TSql = pocoMap.SelectProcedure;
                }
                _dataProvider = DbFactory.Instance.Db(_dbConnectionContext);

                DoInsertUpdate(obj, PocoOperationType.Select);

                XapDataReader dr = _dataProvider.ExecuteReader();

                SetPropertiesFromDataReader<T>(obj, dr);

                return obj;
            } catch (Exception ex) {
                throw new XapException($"Error performing select for {typeof(T).FullName}",ex);
            } finally {
                _dataProvider.CloseConnection();
            }
        }

        async Task<T> IXapPoco.SelectAsync<T>(T obj) {
            try {
                Func<T> function = new Func<T>(() => ((IXapPoco)this).Select<T>(obj));
                return await Task.Run<T>(function);
            } catch (Exception ex) {
                throw new XapException($"Error performing async select for {typeof(T).FullName}",ex);
            }
        }

        List<T> IXapPoco.SelectList<T>(T obj) {
            try {
                List<T> lst = new List<T>();

                pocoMap = PocoMapService.Instance.GetPocoMap(obj);
                if (string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    _dbConnectionContext.TSql = pocoMap.SelectListProcedure;
                }
                _dataProvider = DbFactory.Instance.Db(_dbConnectionContext);

                DoInsertUpdate(obj, PocoOperationType.SelectList);

                XapDataReader dr = _dataProvider.ExecuteReader();

                while (dr.Read()) {
                    T newObj = Activator.CreateInstance<T>();

                    SetListPropertiesFromDataReader<T>(newObj, dr);

                    lst.Add(newObj);
                }
                return lst;
            } catch (Exception ex) {
                throw new XapException($"Error performing select list for {typeof(T).FullName}",ex);
            } finally {
                _dataProvider.CloseConnection();
            }
        }

        async Task<List<T>> IXapPoco.SelectListAsync<T>(T obj) {
            try {
                Func<List<T>> function = new Func<List<T>>(() => ((IXapPoco)this).SelectList<T>(obj));
                return await Task.Run<List<T>>(function);
            } catch (Exception ex) {
                throw new XapException($"Error performing async select list for {typeof(T).FullName}",ex);
            }

        }
        void IXapPoco.Update<T>(T obj) {
            try {
                pocoMap = PocoMapService.Instance.GetPocoMap(obj);
                if (string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    _dbConnectionContext.TSql = pocoMap.UpdateProcedure;
                }
                _dataProvider = DbFactory.Instance.Db(_dbConnectionContext);

                DoInsertUpdate(obj, PocoOperationType.Update);

                _dataProvider.ExecuteNonQuery();

            } catch (Exception ex) {
                throw new XapException($"Error performing update for {typeof(T).FullName}",ex);
            }
        }

        async Task IXapPoco.UpdateAsync<T>(T obj) {
            try {
                Action function = new Action(() => ((IXapPoco)this).Update<T>(obj));
                await Task.Run(function);
            } catch (Exception ex) {
                throw new XapException($"Error performing async update for {typeof(T).FullName}",ex);
            }
        }

        void IXapPoco.Delete<T>(T obj) {
            try {
                pocoMap = PocoMapService.Instance.GetPocoMap(obj);
                if (string.IsNullOrWhiteSpace(_dbConnectionContext.TSql)) {
                    _dbConnectionContext.TSql = pocoMap.DeleteProcedure;
                }
                _dataProvider = DbFactory.Instance.Db(_dbConnectionContext);

                DoInsertUpdate(obj, PocoOperationType.Delete);

                _dataProvider.ExecuteNonQuery();
            } catch (Exception ex) {
                throw new XapException($"Error performing delete for {typeof(T).FullName}",ex);
            }
        }

        async Task IXapPoco.DeleteAsync<T>(T obj) {
            try {
                Action function = new Action(() => ((IXapPoco)this).Delete<T>(obj));
                await Task.Run(function);
            } catch (Exception ex) {
                throw new XapException($"Error performing async delete for {typeof(T).FullName}",ex);
            }
        }
        #endregion


        #region "Data Helper Methods"

        private void DoInsertUpdate<T>(T obj, PocoOperationType operationType) {

            bool setParameter = false;
            IXapPocoField pocoField;
            try {
                foreach (PropertyInfo prop in PropertyService.Instance.GetInterfaceProperties<T>(obj).GetProperties()) {
                    pocoField = pocoMap.GetField(prop.ShortName());
                    switch (operationType) {
                        case PocoOperationType.Insert:
                            setParameter = pocoField == null ? false : pocoField.DoesInsert;
                            break;
                        case PocoOperationType.Select:
                            setParameter = pocoField == null ? false : pocoField.DoesSelect;
                            break;
                        case PocoOperationType.Update:
                            setParameter = pocoField == null ? false : pocoField.DoesUpdate;
                            break;
                        case PocoOperationType.Delete:
                            setParameter = pocoField == null ? false : pocoField.DoesDelete;
                            break;
                        case PocoOperationType.SelectList:
                            setParameter = pocoField == null ? false : pocoField.DoesSelectList;
                            break;
                    }

                    if (setParameter) {
                        object objValue = prop.GetValue(obj, null);

                        if (prop.PropertyType == typeof(string)) {
                            if (pocoMap.GetField(prop.ShortName()).DataType == "SmartDate") {
                                _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, SmartDate.Parse(objValue.ToString()).DBValue));
                            } else {
                                _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString()));
                            }
                        } else if (prop.PropertyType == typeof(SmartDate)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, SmartDate.Parse(objValue.ToString()).DBValue));
                        } else if (prop.PropertyType == typeof(bool)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<bool>()));
                        } else if (prop.PropertyType == typeof(int)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<int>()));
                        } else if (prop.PropertyType == typeof(long)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<long>()));
                        } else if (prop.PropertyType == typeof(short)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<short>()));
                        } else if (prop.PropertyType == typeof(double)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<double>()));
                        } else if (prop.PropertyType == typeof(decimal)) {
                            _dataProvider.AddParameter(DbFactory.Instance.DbParameter(pocoMap.GetField(prop.ShortName()).DbColumn, objValue.ToString().ConvertValue<decimal>()));
                        }
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error setting parameter values {obj.GetType().FullName}",ex);
            }
        }

        private void SetPropertiesFromDataReader<T>(T obj, XapDataReader dr) {
            IXapPocoField pocoField;
            try {
                if (dr.HasRows()) {
                    foreach (PropertyInfo prop in PropertyService.Instance.GetInterfaceProperties<T>(obj).GetProperties()) {
                        pocoField = pocoMap.GetField(prop.ShortName());
                        if (pocoField != null) {
                            string columnName = pocoMap.GetField(prop.ShortName()).DbColumn;
                            if (prop.PropertyType == typeof(string)) {
                                if (pocoMap.GetField(prop.ShortName()).DataType == "SmartDate") {
                                    prop.SetValue(obj, dr.GetSmartDate(columnName).Text, null);
                                } else {
                                    prop.SetValue(obj, dr.GetString(columnName).Trim(), null);
                                }
                            } else if (prop.PropertyType == typeof(SmartDate)) {
                                prop.SetValue(obj, dr.GetSmartDate(columnName), null);
                            } else if (prop.PropertyType == typeof(bool)) {
                                prop.SetValue(obj, dr.GetBoolean(columnName), null);
                            } else if (prop.PropertyType == typeof(int)) {
                                prop.SetValue(obj, dr.GetInt32(columnName), null);
                            } else if (prop.PropertyType == typeof(long)) {
                                prop.SetValue(obj, dr.GetInt64(columnName), null);
                            } else if (prop.PropertyType == typeof(short)) {
                                prop.SetValue(obj, dr.GetInt16(columnName), null);
                            } else if (prop.PropertyType == typeof(double)) {
                                prop.SetValue(obj, dr.GetDouble(columnName), null);
                            } else if (prop.PropertyType == typeof(decimal)) {
                                prop.SetValue(obj, dr.GetDecimal(columnName), null);
                            } else if (prop.PropertyType == typeof(float)) {
                                prop.SetValue(obj, dr.GetFloat(columnName), null);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
               throw new XapException($"Error setting properties for {obj.GetType().FullName}",ex);
            }
        }

        private void SetListPropertiesFromDataReader<T>(T obj, XapDataReader dr) {
            IXapPocoField pocoField;
            try {
                foreach (PropertyInfo prop in PropertyService.Instance.GetInterfaceProperties<T>(obj).GetProperties()) {
                    pocoField = pocoMap.GetField(prop.ShortName());
                    if (pocoField != null) {
                        string columnName = pocoMap.GetField(prop.ShortName()).DbColumn;
                        if (prop.PropertyType == typeof(string)) {
                            if (pocoMap.GetField(prop.ShortName()).DataType == "SmartDate") {
                                prop.SetValue(obj, dr.GetSmartDate(columnName).Text, null);
                            } else {
                                prop.SetValue(obj, dr.GetString(columnName).Trim(), null);
                            }
                        } else if (prop.PropertyType == typeof(SmartDate)) {
                            prop.SetValue(obj, dr.GetSmartDate(columnName), null);
                        } else if (prop.PropertyType == typeof(bool)) {
                            prop.SetValue(obj, dr.GetBoolean(columnName), null);
                        } else if (prop.PropertyType == typeof(int)) {
                            prop.SetValue(obj, dr.GetInt32(columnName), null);
                        } else if (prop.PropertyType == typeof(long)) {
                            prop.SetValue(obj, dr.GetInt64(columnName), null);
                        } else if (prop.PropertyType == typeof(short)) {
                            prop.SetValue(obj, dr.GetInt16(columnName), null);
                        } else if (prop.PropertyType == typeof(double)) {
                            prop.SetValue(obj, dr.GetDouble(columnName), null);
                        } else if (prop.PropertyType == typeof(decimal)) {
                            prop.SetValue(obj, dr.GetDecimal(columnName), null);
                        } else if (prop.PropertyType == typeof(float)) {
                            prop.SetValue(obj, dr.GetFloat(columnName), null);
                        }
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error setting properties for {obj.GetType().FullName}",ex);
            }
        }
        #endregion
    }
}
