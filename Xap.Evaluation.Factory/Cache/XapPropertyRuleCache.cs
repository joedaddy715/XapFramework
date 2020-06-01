using System;

namespace Xap.Evaluation.Factory.Cache {
    public class XapPropertyRuleCache : XapCache<string, XapRuleCache> {
        private readonly object propItUpIllTakeIt = new object();

        #region "Constructors"

        private static readonly XapPropertyRuleCache instance = new XapPropertyRuleCache();

        static XapPropertyRuleCache() { }

        private XapPropertyRuleCache() { }

        public static XapPropertyRuleCache Instance {
            get { return instance; }
        }
        #endregion

        public XapRuleCache GetRuleCache(IXapRuleSearch ruleSearch, IXapDbConnectionContext dbContext) {
            XapRuleCache ruleCache = null;
            ruleCache = this.GetItem($"{ruleSearch.RuleType}.{ruleSearch.NameSpace}.{ruleSearch.PropertyName}");

            if (ruleCache != null) {
                return ruleCache.Clone();
            } else {
                ruleCache = XapRuleCache.Create();
            }

            dbContext.TSql = "CORE.SelectPropertyRules";

            IXapDataProvider db = DbFactory.Instance.Db(dbContext);

            string ruleDependents = string.Empty;

            try {
                XapDataReader dr = db.AddParameter(DbFactory.Instance.DbParameter("RuleType", ruleSearch.RuleType))
                   .AddParameter(DbFactory.Instance.DbParameter("PropertyName", ruleSearch.PropertyName))
                   .AddParameter(DbFactory.Instance.DbParameter("LobName", ruleSearch.LobName))
                   .AddParameter(DbFactory.Instance.DbParameter("NameSpace", ruleSearch.NameSpace))
                   .ExecuteReader();

                while (dr.Read()) {
                    IXapRule _rule = XapRule.Create(dr.GetString("RuleName"));
                    _rule.RuleType = dr.GetString("RuleType");
                    _rule.RuleSyntax = dr.GetString("RuleSyntax");
                    _rule.RuleDescription = dr.GetString("RuleDesc");
                    _rule.RuleMessage = dr.GetString("RuleMessage");
                    _rule.PropertyName = dr.GetString("PropertyName");

                    ruleDependents = dr.GetString("Dependencies");

                    string[] dependents = ruleDependents.Split(',');
                    for (int i = 0; i < dependents.Length; i++) {
                        if (!_rule.HasDependent(dependents[i])) {
                            _rule.AddDependent(XapRuleDependent.Create(dependents[i]));
                        }
                    }

                    lock (propItUpIllTakeIt) {
                        ruleCache.AddRule(_rule.RuleName, _rule);
                    }
                }

                lock (propItUpIllTakeIt) {
                    this.AddItem($"{ruleSearch.RuleType}.{ruleSearch.NameSpace}.{ruleSearch.PropertyName}", ruleCache);
                }

                return ruleCache.Clone();
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error loading rules for {ruleSearch.NameSpace}.{ruleSearch.PropertyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            } finally {
                db.CloseConnection();
            }
        }
    }
}
