using System;
using System.Collections.Generic;
using Xap.Data.Factory;
using Xap.Infrastructure.Data;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Data;
using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Interfaces.Shared;
using Xap.Infrastructure.Logging;
using Xap.Infrastructure.Shared;

namespace Xap.Security.Default {
    internal class XapUser : IXapUser {
        #region "Constructors"
        private XapUser(string userName,string password) {
                _userName = userName;
                _password = password;

                LoadUserData();
                LoadUserLobs();
        }

        internal static IXapUser Create(string userName,string password) {
            return new XapUser(userName,password);
        }
        #endregion

        #region "Properties"
        private List<IXapGenericData> _userLobs = new List<IXapGenericData>();
        private List<IXapGenericData> _userRoles = new List<IXapGenericData>();

        private int _userId;
        int IXapUser.UserId {
            get => _userId;
            set => _userId = value;
        }

        private string _userName = string.Empty;
        string IXapUser.UserName {
            get => _userName;
            set => _userName = value;
        }

        private string _password = string.Empty;
        string IXapUser.Password {
            get => _password;
            set => _password = value;
        }

        private string _firstName = string.Empty;
        string IXapUser.FirstName {
            get => _firstName;
            set => _firstName = value;
        }

        private string _lastName = string.Empty;
        string IXapUser.LastName {
            get => _lastName;
            set => _lastName = value;
        }

        private string _fullName = string.Empty;
        string IXapUser.FullName {
            get => _fullName;
            set => _fullName = value;
        }

        private string _fullNameReverse = string.Empty;
        string IXapUser.FullNameReverse {
            get => _fullNameReverse;
            set => _fullNameReverse = value;
        }

        private bool _isAuthenticated;
        bool IXapUser.IsAuthenticated {
            get => _isAuthenticated;
            set => _isAuthenticated = value;
        }

        private string _emailAddress = string.Empty;
        string IXapUser.EmailAddress {
            get => _emailAddress;
            set => _emailAddress = value;
        }

        private bool _isActive;
        bool IXapUser.IsActive {
            get => _isActive;
            set => _isActive = value;
        }

        private string _currentLob = string.Empty;
        string IXapUser.CurrentLob {
            get => _currentLob;
        }
        #endregion

        #region "Interface Methods"
        string IXapUser.UserRolesList {
            get {
                System.Text.StringBuilder tmp = new System.Text.StringBuilder();
                foreach (IXapGenericData r in _userRoles) {
                    tmp.Append(r.ValueMember + "|");
                }
                string _retVal = tmp.ToString();
                if (!string.IsNullOrEmpty(_retVal)) {
                    _retVal = _retVal.Remove(_retVal.LastIndexOf("|"), 1);
                }
                return _retVal;
            }
        }

        IEnumerable<IXapGenericData> IXapUser.UserLobs() {
            foreach(IXapGenericData lob in _userLobs) {
                yield return lob;
            }
        }

        IEnumerable<IXapGenericData> IXapUser.UserRoles() {
            foreach(IXapGenericData role in _userRoles) {
                yield return role;
            }
        }

        void IXapUser.Login(string lobName) {
            IXapDataProvider db = null;
            try {
                _currentLob = lobName;

                db = DbFactory.Instance.XapDb("acts", _currentLob, "SECURITY.RecordUserLogin");
                db.AddParameter(DbFactory.Instance.XapDbParameter("LobName", _currentLob))
                    .AddParameter(DbFactory.Instance.XapDbParameter("UserName", _userName))
                    .ExecuteNonQuery();


                _isAuthenticated = true;

                LoadUserRoles();
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error logging in user", ex);
            }
        }

        void IXapUser.LogOut() {
            IXapDataProvider db = null;
            try {
                db = DbFactory.Instance.XapDb("acts", _currentLob, "SECURITY.RecordUserLogout");
                db.AddParameter(DbFactory.Instance.XapDbParameter("LobName", _currentLob))
                    .AddParameter(DbFactory.Instance.XapDbParameter("UserName", _userName))
                    .ExecuteNonQuery();
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error logging in user", ex);
            }
        }
        #endregion

        #region "Private Methods"

        private void LoadUserData() {
            IXapDataProvider db = null;
            try {
                db = DbFactory.Instance.XapDb("acts", _userName, "SECURITY.SelectUser");

                XapDataReader dr = db.AddParameter(DbFactory.Instance.XapDbParameter("UserName", _userName)).ExecuteReader();

                while (dr.Read()) {
                    _userId = dr.GetInt32("UserId");
                    _userName = dr.GetString("UserName");
                    _firstName = dr.GetString("FirstName");
                    _lastName = dr.GetString("LastName");
                    _emailAddress = dr.GetString("EmailAddress");
                    _fullName = dr.GetString("FullName");
                    _fullNameReverse = dr.GetString("FullNameReverse");
                    _isActive = dr.GetBoolean("IsActive");
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error loading useruser data", ex);
            } finally {
                db.CloseConnection();
            }
        }

        private void LoadUserRoles() {
            IXapDataProvider db = null;
            try {
                db = DbFactory.Instance.XapDb("acts", _currentLob, "SECURITY.SelectUserRoles");

                XapDataReader dr = db.AddParameter(DbFactory.Instance.XapDbParameter("UserName", _userName))
                    .AddParameter(DbFactory.Instance.XapDbParameter("LobName", _currentLob)).ExecuteReader();

                while (dr.Read()) {
                    IXapGenericData role = XapGenericData.Create(dr.GetString("RoleDescription"), dr.GetString("RoleName"));
                    _userRoles.Add(role);
                }
            } catch(Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error loading user roles for {_currentLob}",ex);
            } finally {
                db.CloseConnection();
            }
        }

        private void LoadUserLobs() {
            IXapDataProvider db = null;
            try {
                db = DbFactory.Instance.XapDb("acts", string.Empty, "SECURITY.SelectUserLobs");

                XapDataReader dr = db.AddParameter(DbFactory.Instance.XapDbParameter("UserName", _userName)).ExecuteReader();

                while (dr.Read()) {
                    IXapGenericData lob = XapGenericData.Create(dr.GetString("LobDescription"), dr.GetString("LobName"));
                    _userLobs.Add(lob);
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error loading user lobs", ex);
            } finally {
                db.CloseConnection();
            }
        }
        #endregion
    }
}
