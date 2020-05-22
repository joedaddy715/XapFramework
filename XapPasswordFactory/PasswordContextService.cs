using System;
using System.Collections.Generic;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;
using Xap.Password.Factory.Interfaces;

namespace Xap.Password.Factory {
    public class PasswordContextService{
        #region "Constructors"
        private static readonly PasswordContextService instance = new PasswordContextService();

        static PasswordContextService() {
        }

        private PasswordContextService() {
        }

        public static PasswordContextService Instance {
            get {
                return instance;
            }
        }
        #endregion

        private XapCache<string, IXapPasswordContext> pwdContexts = new XapCache<string, IXapPasswordContext>();

        public void AddPasswordContext(string configurationKey,IXapPasswordContext passwordContext) {
            pwdContexts.AddItem(configurationKey, passwordContext);
        }

        public void RemovePasswordContext(string configurationKey) {
            pwdContexts.RemoveItem(configurationKey);
        }

        public void Clear() {
            pwdContexts.ClearCache();
        }

        public int Count {
            get => pwdContexts.Count;
        }

        public IEnumerable<IXapPasswordContext> GetPasswordContexts() {
            foreach (KeyValuePair<string, IXapPasswordContext> kvp in pwdContexts.GetItems()) {
                yield return kvp.Value;
            }
        }

        /// <summary>
        /// Will return password context if cached, if not will try and load from configuration
        /// </summary>
        /// <param name="configurationKey"></param>
        /// <returns></returns>
        public IXapPasswordContext GetPasswordContext(string configurationKey) {
            try {
                IXapPasswordContext pwdContext = pwdContexts.GetItem(configurationKey);
                if(pwdContext != null) {
                    return pwdContext;
                }

                return null;
            } catch (Exception ex) {
                throw new XapException($"Error retrieving password context for {configurationKey}", ex);
            }
        }
    }
}
