using System;
using System.Collections.Generic;
using System.Text;
using Xap.Evaluation.Engine.Evaluate;
using Xap.Evaluation.Engine.Parser;
using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Interfaces.Data;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Logging;

namespace Xap.Evaluation.Engine.SyntaxBuilder {
    public class SyntaxBuilder {
        #region "Constructors"
        private SyntaxBuilder() { }
        public static SyntaxBuilder Create() {
            return new SyntaxBuilder();
        }
        #endregion

        #region "Properties"
        private StringBuilder syntaxBuilder = new StringBuilder();
        private string _finalSyntax = string.Empty;

        public string Syntax {
            get => _finalSyntax;
        }

        private string _syntaxError = string.Empty;
        public string SyntaxError {
            get => _syntaxError;
        }
        #endregion

        #region "Public Methods"
        public SyntaxBuilder ClearSyntax() {
            syntaxBuilder.Clear();
            _finalSyntax = string.Empty;
            return this;
        }

        public SyntaxBuilder AddRaw(string syntax) {
            syntaxBuilder.Append(syntax);
            return this;
        }

        public SyntaxBuilder Build() {
            _finalSyntax = syntaxBuilder.ToString();
            syntaxBuilder.Clear();
            return this;
        }

        public T Execute<T>() {
            IXapRule rule = XapRule.Create();
            try {
                rule.RuleSyntax = _finalSyntax;
                return rule.EvaluateRule<T>();
            } catch  {
                XapLogger.Instance.Error("Error executing Syntax");
                throw new Exception(rule.SyntaxError);
            }
        }

        public T Execute<T>(XapObjectCore obj) {
            IXapRule rule = XapRule.Create();
            try {
                rule.RuleSyntax = _finalSyntax;
                rule.RuleSyntax = XapRuleSyntax.PrepareRuleSyntax(obj, rule, null);
                return rule.EvaluateRule<T>();
            } catch {
                XapLogger.Instance.Error("Error executing Syntax");
                throw new Exception(rule.SyntaxError);
            }
        }

        public bool SyntaxIsValid() {
            try {
                Token token = new Token(_finalSyntax);
                Evaluator eval = new Evaluator(token);

                string value = string.Empty;
                return eval.Evaluate(out value, out _syntaxError);

            } catch(Exception ex) {
                XapLogger.Instance.Error("Error validating Syntax");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "Comparison Operators"
        public SyntaxBuilder GreaterThan(int value1,int value2) {
            syntaxBuilder.Append($"{value1.ToString()} > {value2.ToString()}");
            return this;
        }

        public SyntaxBuilder LessThan(int value1,int value2) {
            syntaxBuilder.Append($"{value1.ToString()} < {value2.ToString()}");
            return this;
        }

        public SyntaxBuilder GreaterThanOrEqual(int value1, int value2) {
            syntaxBuilder.Append($"{value1.ToString()} >= {value2.ToString()}");
            return this;
        }

        public SyntaxBuilder LessThanOrEqual(int value1, int value2) {
            syntaxBuilder.Append($"{value1.ToString()} <= {value2.ToString()}");
            return this;
        }

        public SyntaxBuilder NotEqual(int value1, int value2) {
            syntaxBuilder.Append($"{value1.ToString()} <> {value2.ToString()}");
            return this;
        }

        public SyntaxBuilder Equal(int value1, int value2) {
            syntaxBuilder.Append($"{value1.ToString()} = {value2.ToString()}");
            return this;
        }
        #endregion

        #region "Logical Operators"
        public SyntaxBuilder And() {
            syntaxBuilder.Append(" and ");
            return this;
        }

        public SyntaxBuilder Or() {
            syntaxBuilder.Append(" or ");
            return this;
        }
        #endregion

        #region "Math Operators"
        public SyntaxBuilder Caret(int value1,int value2) {
            syntaxBuilder.Append($" {value1}^{value2} ");
            return this;
        }

        public SyntaxBuilder Multiply(int value1, int value2) {
            syntaxBuilder.Append($" {value1}*{value2} ");
            return this;
        }

        public SyntaxBuilder Divide(int value1, int value2) {
            syntaxBuilder.Append($" {value1}/{value2} ");
            return this;
        }

        public SyntaxBuilder Plus(int value1, int value2) {
            syntaxBuilder.Append($" {value1}+{value2} ");
            return this;
        }

        public SyntaxBuilder Subtract(int value1, int value2) {
            syntaxBuilder.Append($" {value1}-{value2} ");
            return this;
        }

        public SyntaxBuilder Modulus(int value1, int value2) {
            syntaxBuilder.Append($" {value1}%{value2} ");
            return this;
        }
        #endregion

        #region "Syntax Operands"
        public SyntaxBuilder IsTrue(bool value) {
            syntaxBuilder.Append($" '{value.ToString()}' = 'True'");
            return this;
        }

        public SyntaxBuilder NotTrue(bool value) {
            syntaxBuilder.Append($" '{value.ToString()}' = 'False'");
            return this;
        }

        public SyntaxBuilder DaysBetween(DateTime startDate,DateTime endDate,string calculationType,string holidays) {
            if (string.IsNullOrWhiteSpace(holidays)) {
                syntaxBuilder.Append($"DaysBetween[{startDate.Date.Month.ToString()}.{startDate.Date.Day.ToString()}.{startDate.Date.Year.ToString()},{endDate.Date.Month.ToString()}.{endDate.Date.Day.ToString()}.{endDate.Date.Year.ToString()},'{calculationType}']");
            } else {
                syntaxBuilder.Append($"DaysBetween[{startDate.Date.Month.ToString()}.{startDate.Date.Day.ToString()}.{startDate.Date.Year.ToString()},{endDate.Date.Month.ToString()}.{endDate.Date.Day.ToString()}.{endDate.Date.Year.ToString()},'{calculationType}',{holidays}]");
            }
            return this;
        }
        public SyntaxBuilder DateAdd(DateTime startDate, int days, string calculationType) {
            return DateAdd(startDate, days, calculationType, string.Empty);
        }

        public SyntaxBuilder DateAdd(DateTime startDate, int days, string calculationType, string holidays) {
            if (!string.IsNullOrEmpty(holidays)) {
                syntaxBuilder.Append($" DateAdd[{startDate.Date.Month.ToString()}.{startDate.Date.Day.ToString()}.{startDate.Date.Year.ToString()},'{calculationType}',{days},'{holidays}']");
                return this;
            }
            syntaxBuilder.Append($" DateAdd[{startDate.Date.Month.ToString()}.{startDate.Date.Day.ToString()}.{startDate.Date.Year.ToString()},'{calculationType}',{days}]");
            return this;
        }

        public SyntaxBuilder DateDiff(DateTime startDate, DateTime endDate) {
            syntaxBuilder.Append($" DateDiff[{startDate.Date.Month.ToString()}.{startDate.Date.Day.ToString()}.{startDate.Date.Year.ToString()},{endDate.Date.Month.ToString()}.{endDate.Date.Day.ToString()}.{endDate.Date.Year.ToString()}]");
            return this;
        }

        public SyntaxBuilder IsValidDate(DateTime dateValue) {
            syntaxBuilder.Append($" IsValidDate[{dateValue.Date.Month.ToString()}.{dateValue.Date.Day.ToString()}.{dateValue.Date.Year.ToString()}]");
            return this;
        }

        public SyntaxBuilder Len(string value) {
            syntaxBuilder.Append($" Len['{value}']");
            return this;
        }

        public SyntaxBuilder IsEmptyString(string value) {
            syntaxBuilder.Append($" Len['{value.Trim()}'] = 0");
            return this;
        }

        public SyntaxBuilder IsNotEmptyString(string value) {
            syntaxBuilder.Append($" Len['{value.Trim()}'] > 0");
            return this;
        }

        public SyntaxBuilder MinLen(string value,int length) {
            syntaxBuilder.Append($" Len['{value}']  > {length.ToString()}");
            return this;
        }

        public SyntaxBuilder MaxLen(string value, int length) {
            syntaxBuilder.Append($" Len['{value}']  < {length.ToString()}");
            return this;
        }

        public SyntaxBuilder IIF(string condition, string returnValue1, string returnValue2) {
            syntaxBuilder.Append($" IIF[{condition},{returnValue1},{returnValue2}]");
            return this;
        }

        public SyntaxBuilder EndsWith(string value,string criteria) {
            syntaxBuilder.Append($" EndsWith['{value}','{criteria}']");
            return this;
        }

        public SyntaxBuilder SubString(string value,int start,int end) {
            syntaxBuilder.Append($" SubString['{value}',{start.ToString()},{end.ToString()}]");
            return this;
        }

        /// <summary>
        /// converts a string to upper case
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns></returns>
        public SyntaxBuilder UCase(string value) {
            syntaxBuilder.Append($" UCase['{value.Trim()}']");
            return this;
        }

        /// <summary>
        /// converts a string to lower case
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns></returns>
        public SyntaxBuilder LCase(string value) {
            syntaxBuilder.Append($" LCase['{value.Trim()}']");
            return this;
        }

        public SyntaxBuilder Abs(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder Avg(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// indicates if a value is between the other values.  cmparison is inclusive
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <returns></returns>
        public SyntaxBuilder Between(int value, int minValue,int maxValue) {
            syntaxBuilder.Append($" Between[{value.ToString()},{minValue.ToString()},{maxValue.ToString()}]");
            return this;
        }

        public SyntaxBuilder ConCat(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// returns the left number of characters from a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfCharacters"></param>
        /// <returns></returns>
        public SyntaxBuilder Left(string value,int numberOfCharacters) {
            syntaxBuilder.Append($" Left['{value.Trim()}',{numberOfCharacters.ToString()}]");
            return this;
        }

        public SyntaxBuilder Mid(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// returns the right number of characters from a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfCharacters"></param>
        /// <returns></returns>
        public SyntaxBuilder Right(string value,int numberOfCharacters) {
            syntaxBuilder.Append($" Right['{value.Trim()}',{numberOfCharacters.ToString()}]");
            return this;
        }

        public SyntaxBuilder Round(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// Trims white space from a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder Trim(string value) {
            syntaxBuilder.Append($" Trim['{value}']");
            return this;
        }

        /// <summary>
        /// Trims white space from the right side of a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder RTrim(string value) {
            syntaxBuilder.Append($" RTrim['{value}']");
            return this;
        }

        /// <summary>
        /// Trims white space from the left side of a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder LTrim(string value) {
            syntaxBuilder.Append($" LTrim['{value}']");
            return this;
        }


        /// <summary>
        /// Pads a string on the right with new values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <example>rpad[a, b, n]  where a and b are string values and n is numeric.  The parameter p will be appended to the right of parameter a, n times.</example>
        public SyntaxBuilder RPad(string value,string appendValue,int amount) {
            syntaxBuilder.Append($" RPad['{value.Trim()}','{appendValue.Trim()}',{amount.ToString()}]");
            return this;
        }

        /// <summary>
        /// Pads a string on the left with new values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="appendValue"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <example>lpad[a, b, n]  where a and b are string values and n is numeric.  The parameter p will be appended to the left of parameter a, n times.</example>
        public SyntaxBuilder LPad(string value, string appendValue, int amount) {
            syntaxBuilder.Append($" LPad['{value.Trim()}','{appendValue.Trim()}',{amount.ToString()}]");
            return this;
        }

        /// <summary>
        /// Joins a list of items together using a delimiter
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        /// <example>join[a, b1, ..., bn] where a is the delimiter and b1, ..., bn are the items to be joined.</example>
        public SyntaxBuilder Join(string delimiter,string valuesList) {
            syntaxBuilder.Append($" Join['{delimiter}','{valuesList}']");
            return this;
        }

        /// <summary>
        /// Searchs for a string within another string at a specified starting position
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <example>SearchString[a, n, b] where a is the string that is being searched, b is the string that is being sought, and n is the start position in a</example>
        public SyntaxBuilder SearchString(string searchString,string value,int position) {
            syntaxBuilder.Append($" SearchString['{searchString.Trim()}','{value.ToString()}',{position.ToString()}]");
            return this;
        }

        /// <summary>
        /// returns the day from a date
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder Day(DateTime value) {
            syntaxBuilder.Append($" Day[{value.Date.Month.ToString()}.{value.Date.Day.ToString()}.{value.Date.Year.ToString()}]");
            return this;
        }

        /// <summary>
        /// returns the month from a date value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder Month(DateTime value) {
            syntaxBuilder.Append($" Month[{value.Date.Month.ToString()}.{value.Date.Day.ToString()}.{value.Date.Year.ToString()}]");
            return this;
        }

        /// <summary>
        /// returns the year from a date value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder Year(DateTime value) {
            syntaxBuilder.Append($" Year[{value.Date.Month.ToString()}.{value.Date.Day.ToString()}.{value.Date.Year.ToString()}]");
            return this;
        }

        public SyntaxBuilder NumericMin(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder NumericMax(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder DateMax(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder DateMin(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder StringMax(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        public SyntaxBuilder StringMin(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// Indicates if the item is contained in the list.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        /// <example>contains[p1, p2, ...., pn]   If p1 in in the list p2, ..., pn, this function returns \"true\" otherwise, this function returns \"false\".</example>
        public SyntaxBuilder Contains(string value,string valuesList) {
            syntaxBuilder.Append($" Contains['{value}','{valuesList}'");
            return this;
        }

        public SyntaxBuilder IndexOf(string value) {
            syntaxBuilder.Append($"");
            return this;
        }

        /// <summary>
        /// returns the current date
        /// </summary>
        /// <returns></returns>
        public SyntaxBuilder Now() {
            syntaxBuilder.Append($" Now[]");
            return this;
        }

        /// <summary>
        /// replaces one string within another
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="replaceValue"></param>
        /// <param name="insertValue"></param>
        /// <returns></returns>
        /// <example>Replace[a, b, c] where a is the search string, b is the value being replaced, and c is the value that is being inserted</example>
        public SyntaxBuilder Replace(string searchValue,string replaceValue,string insertValue) {
            syntaxBuilder.Append($" Replace['{searchValue.Trim()}','{replaceValue.Trim()}','{insertValue.Trim()}']");
            return this;
        }

        /// <summary>
        /// converts a string to proper case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder PCase(string value) {
            syntaxBuilder.Append($" PCase['{value.Trim()}']");
            return this;
        }

        /// <summary>
        /// performs a not on a boolean operator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <example>not[p1] where p1 is a boolean parameter</example>
        public SyntaxBuilder Not(string value) {
            syntaxBuilder.Append($" Not['{value}']");
            return this;
        }

        /// <summary>
        /// checks if a string is all numeric characters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SyntaxBuilder IsAllDigits(string value) {
            syntaxBuilder.Append($" IsAllDigits['{value}']");
            return this;
        }

        /// <summary>
        /// Checks to see if a pipe delimited list contains the specified value
        /// </summary>
        /// <param name="value">values to check for</param>
        /// <param name="valuesList">list of possible values</param>
        /// <returns>SyntaxBuilder instance</returns>
        /// <example>ListContains['value','value1|value2|values3']</example>
        public SyntaxBuilder ListContains(string delimeter,string value,string valuesList) {
            syntaxBuilder.Append($" ListContains['{delimeter}','{valuesList}','{value}']");
            return this;
        }

        /// <summary>
        /// Compares two strings for equality
        /// </summary>
        /// <param name="value1">string 1</param>
        /// <param name="value2">string 2</param>
        /// <returns>SyntaxBuilder Instance</returns>
        /// <example>Requal['value1','value2]</example>
        public SyntaxBuilder Reqaul(string value1,string value2) {
            syntaxBuilder.Append($" Requal['{value1.Trim().ToLower()}' = '{value2.Trim().ToLower()}']");
            return this;
        }

        public SyntaxBuilder SqlScalarFunction(IXapDbContext dbContext,List<IXapDbParameter> parameters) {
            syntaxBuilder.Append($"SqlScalarFunction['{dbContext.DbEnvironment}','{dbContext.TSql}',");
            foreach(IXapDbParameter xapDbParameter in parameters) {
                syntaxBuilder.Append($"{xapDbParameter.ParameterName}|{xapDbParameter.ParameterValue}");
            }
            syntaxBuilder.Append("']");
            return this;
        }
        #endregion

    }
}
