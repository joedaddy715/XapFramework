using Xap.Evaluation.Engine.Parser;
using Xap.Infrastructure.Caches;

namespace Xap.Evaluation.Engine.Cache {
    public class TokenCache  {
        private XapCache<string, Token> tokens = new XapCache<string, Token>();

        #region "Constructors"

        private static readonly TokenCache instance = new TokenCache();

        static TokenCache() { }

        private TokenCache() { }

        public static TokenCache Instance {
            get { return instance; }
        }
        #endregion

        #region "public Methods"
        public Token GetToken(string ruleName) {
            return tokens.GetItem(ruleName);
        }

        public void AddToken(string ruleName,Token token) {
            tokens.AddItem(ruleName,token);
        }
        #endregion
    }
}
