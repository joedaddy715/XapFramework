using System;
using System.Collections.Generic;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;

namespace Xap.Data.Factory {
    public class DbConnectionContextService {
        #region "Constructors"
        private static readonly DbConnectionContextService instance = new DbConnectionContextService();

        static DbConnectionContextService() {
        }

        private DbConnectionContextService() {
        }

        public static DbConnectionContextService Instance {
            get {
                return instance;
            }
        }
        #endregion

        private XapCache<string, IXapDbConnectionContext> dbConnectionContexts = new XapCache<string, IXapDbConnectionContext>();

        public void AddDbConnectionContext(string dbKey, IXapDbConnectionContext dbConnectionContext) {
            dbConnectionContexts.AddItem(dbKey, dbConnectionContext);
        }

        public void RemoveDbConnectionContext(string dbKey) {
            dbConnectionContexts.RemoveItem(dbKey);
        }

        public int Count {
            get => dbConnectionContexts.Count;
        }

        public IEnumerable<IXapDbConnectionContext> GetDbConnectionContexts() {
            foreach(KeyValuePair<string, IXapDbConnectionContext> kvp in dbConnectionContexts.GetItems()) {
                yield return kvp.Value;
            }
        }

        public IXapDbConnectionContext GetDbConnectionContext(string dbKey) {
            try {
                IXapDbConnectionContext dbConnectionContext = dbConnectionContexts.GetItem(dbKey);
                if (dbConnectionContext != null) {
                    return dbConnectionContext;
                }

                return null;
            } catch (Exception ex) {
                throw new XapException($"Error retrieving database context for {dbKey}",ex);
            }
        }
               
        public void Clear() {
            dbConnectionContexts.ClearCache();
        }
    }
}
