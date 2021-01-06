using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DxDataGridDapper.DevExtreme
{
    public class DevExtremeHelper : IDevExtremeHelper
    {
        private string AND_OP = "AND";
        private string OR_OP = "OR";
        private string LIKE_OP = "LIKE";
        private string NOT_OP = "NOT";
        private string IS_OP = "IS";

        private string NULL_VAL = "NULL";
        private string QuoteStringValue(dynamic value, bool isFieldName = true)
        {

            //if (!isFieldName) {
            //     value =   value ;
            //}string.Format
            string resultPattern = isFieldName ? "`{0}`" : ((value is bool) || string.IsNullOrEmpty(value) ? "{0}" : "'{0}'");
            string stringValue = (value is bool) ? (value ? "1" : "0") : (string.IsNullOrEmpty(value) ? NULL_VAL : value.ToString());
            string result = string.Format(resultPattern, stringValue);
            return result;
        }

        private string addcslashes(string str, string str2)
        {
            string ret = "";
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\'': ret += "\\\'"; break;
                    case '\"': ret += "\\\""; break;
                    case '\0': ret += "\\0"; break;
                    case '\\': ret += "\\\\"; break;
                    default: ret += c.ToString(); break;
                }
            }
            return ret;
        }

        private string _GetSqlFieldName(string field)
        {
            string result = field;

            return result;
        }

        private string _GetSimpleSqlExpr(IList expression)
        {
            string result = "";
            int itemsCount = expression.Count;
            string clause = "";
            string fieldName = _GetSqlFieldName((expression[0]).ToString().Trim());
            if (itemsCount == 2)
            {
                string val = expression[1].ToString();
                result = string.Format("{0} = {1}", fieldName, QuoteStringValue(val, false));
            }
            else if (itemsCount == 3)
            {
                clause = expression[1].ToString().Trim();
                string val = expression[2].ToString().Trim();
                string pattern = "";
                if (string.IsNullOrEmpty(val))
                {
                    val = QuoteStringValue(val, false);
                    pattern = "{0} {1} {2}";
                    switch (clause)
                    {
                        case "=":
                            {
                                clause = IS_OP;
                                break;
                            }
                        case "<>":
                            {
                                clause = IS_OP + " " + NOT_OP;
                                break;
                            }
                    }
                }
                else
                {
                    switch (clause)
                    {
                        case "=":
                        case "<>":
                        case ">":
                        case ">=":
                        case "<":
                        case "<=":
                            {
                                pattern = "{0} {1} {2}";
                                val = QuoteStringValue(val, false);
                                break;
                            }
                        case "startswith":
                            {
                                pattern = "{0} {1} '{2}%'";
                                clause = LIKE_OP;
                                val = addcslashes(val, "%_");
                                break;
                            }
                        case "endswith":
                            {
                                pattern = "{0} {1} '%{2}'";
                                val = addcslashes(val, "%_");
                                clause = LIKE_OP;
                                break;
                            }
                        case "contains":
                            {
                                pattern = "{0} {1} '%{2}%'";
                                val = addcslashes(val, "%_");
                                clause = LIKE_OP;
                                break;
                            }
                        case "notcontains":
                            {
                                pattern = "{0} {1} '%{2}%'";
                                val = addcslashes(val, "%_");
                                clause = string.Format("{0} {1}", NOT_OP, LIKE_OP);
                                break;
                            }
                        default:
                            {
                                clause = "";

                            }
                            break;
                    }
                }
                result = string.Format(pattern, fieldName, clause, val);
            }
            return result;
        }

        private bool IsString(object item)
        {
            if ((item.GetType().FullName.StartsWith("System.Collections.Generic.List")))
            {
                return false;
            }
            else if (item.GetType().FullName.StartsWith("System.Text.Json.JsonElement"))
            {
                var m = (System.Text.Json.JsonElement)item;
                if (m.ValueKind == System.Text.Json.JsonValueKind.Array) // if (m.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    return false;
                }
            }
            //if ((item.GetType() == typeof(JValue)) || (item.GetType() == typeof(String)) || (item.GetType().FullName.StartsWith("System.Text.Json.JsonElement")))

            return true;
        }

        private bool IsArray(object item)
        {
            //if ((item.GetType() == typeof(JArray)) || (item.GetType().FullName.StartsWith("System.Collections.Generic.List")))

            if ((item.GetType().FullName.StartsWith("System.Collections.Generic.List")))
            {
                return true;
            }
            else if (item.GetType().FullName.StartsWith("System.Text.Json.JsonElement"))
            {
                var m = (System.Text.Json.JsonElement)item;
                if (m.ValueKind == System.Text.Json.JsonValueKind.Array) // if (m.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    return true;
                }
            }
            return false;
        }
        private string GetSqlExprByKey(string field)
        {
            string result = "";

            return result;
        }

        public string GetFilterSqlExprByArray(IList expression)
        {
            string result = "(";
            bool prevItemWasArray = false;
            int index = -1;
            string strItem = "";

            foreach (var item in expression)
            {
                index++;
                if (IsString(item))
                {
                    prevItemWasArray = false;
                    if (index == 0)
                    {
                        if (item.ToString() == "!")
                        {
                            result += string.Format("{0) ", NOT_OP);
                            continue;
                        }
                        result += ((expression != null) && (IsArray(expression))) ? _GetSimpleSqlExpr(expression) : "";
                        break;
                    }
                    strItem = item.ToString().Trim().ToUpper();
                    if (strItem == AND_OP || strItem == OR_OP)
                    {
                        result += string.Format(" {0} ", strItem);
                    }
                    continue;
                }
                else if (IsArray(item))
                {
                    if (prevItemWasArray)
                    {
                        result += string.Format(" {0} ", AND_OP);
                    }

                    var m = (System.Text.Json.JsonElement)item;
                    var list = new List<object>();
                    for (int i = 0; i < m.GetArrayLength(); i++)
                    {
                        list.Add(m[i]);
                    }
                    result += GetFilterSqlExprByArray(list);
                    prevItemWasArray = true;
                }
            }
            result += ")";
            return result;
        }

        public string GetSortSqlExprByArray(IList expression)
        {
            string result = "";

            foreach (var item in (DevExtremeSortingInfo[])expression)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " ,";
                }

                if (item.Desc)
                {
                    result += string.Format(" {0} {1} ", item.Selector, "DESC");
                }
                else
                {
                    result += string.Format(" {0} {1} ", item.Selector, "ASC");
                }
            }

            return result;
        }
    }
}
