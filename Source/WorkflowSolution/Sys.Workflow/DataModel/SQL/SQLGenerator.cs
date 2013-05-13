using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Sys.Workflow.Utility;

namespace Sys.Workflow.DataModel.SQL
{
    /// <summary>
    /// SQL 语句生成
    /// </summary>
    public class SqlGenerator
    {
        public string Get(IEntityMapper classMap)
        {
            var columns = GetSelectColumns(classMap);
            var tableName = GetTableName(classMap);
            var whereSql = GetWhere(classMap);

            return string.Format("SELECT {0} FROM {1} WHERE {2}",
                   columns, tableName, whereSql);
        }

        public string GetSelectFrom(IEntityMapper classMap)
        {
            var columns = GetSelectColumns(classMap);
            var tableName = GetTableName(classMap);

            return string.Format("SELECT {0} FROM {1}",
                   columns, tableName);
        }

        public string GetInSql(IEntityMapper classMap)
        {
            var columns = GetSelectColumns(classMap);
            var tableName = GetTableName(classMap);
            var whereSql = GetWhereIn(classMap);

            return string.Format("SELECT {0} FROM {1} WHERE {2}",
                columns, tableName, whereSql);
        }

        public string GetTableName(IEntityMapper map)
        {
            var schemaName = map.SchemaName;
            var tableName = map.TableName;
            return schemaName + "." + tableName;
        }

        public string GetColumnName(IEntityMapper map, string propertyName)
        {
            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("未找到列名: {0}", propertyName));
            }
            return propertyMap.ColumnName;

        }

        private string GetSelectColumns(IEntityMapper classMap)
        {
            var columns = classMap.Properties
                .Select(p => GetColumnName(classMap, p.ColumnName));

            return columns.AppendStrings();
        }

        private string GetWhere(IEntityMapper classMap)
        {
            var where = classMap.Properties
                .Where(p => p.ColumnType != ColumnType.NotAKey)
                .Select(p => GetColumnName(classMap, p.ColumnName) + " = @" + p.Name);

            return where.AppendStrings(" AND ");
        }

        private string GetWhereIn(IEntityMapper classMap)
        {
            var where = classMap.Properties
                .Where(p => p.ColumnType != ColumnType.NotAKey)
                .Select(p => GetColumnName(classMap, p.ColumnName) + " in(@" + p.Name + ")");

            return where.AppendStrings(" AND ");
        }

        public string GetInsertSql(IEntityMapper classMap)
        {
            var columns = classMap.Properties.Where(p => (p.ColumnType != ColumnType.Identity));
            var columnNames = columns.Select(p => GetColumnName(classMap, p.ColumnName));
            var parameters = columns.Select(p => "@" + p.Name);

            var insSql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                GetTableName(classMap),
                columnNames.AppendStrings(),
                parameters.AppendStrings());

            return insSql;
        }

        public string GetUpdateSql(IEntityMapper classMap)
        {
            var columns = classMap.Properties.Where(p=> p.ColumnType != ColumnType.Identity);
            var setSql = columns.Select(p => GetColumnName(classMap, p.ColumnName) + " = @" + p.Name);

            var updateSql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                GetTableName(classMap),
                setSql.AppendStrings(),
                GetWhere(classMap));

            return updateSql;
        }

        public string GetDeleteSql(IEntityMapper classMap)
        {
            var deleteSql = string.Format("DELETE FROM {0} WHERE {1}",
                GetTableName(classMap),
                GetWhere(classMap));

            return deleteSql;
        }

        #region 分页SQL语句
        public char OpenQuote
        {
            get { return '"'; }
        }

        public char CloseQuote
        {
            get { return '"'; }
        }

        public bool IsQuoted(string value)
        {
            if (value.Trim()[0] == OpenQuote)
            {
                return value.Trim().Last() == CloseQuote;
            }

            return false;
        }

        public string QuoteString(string value)
        {
            return IsQuoted(value) ? value : string.Format("{0}{1}{2}", OpenQuote, value.Trim(), CloseQuote);
        }

        public virtual string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException(columnName, "columnName cannot be null or empty.");
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                result.AppendFormat(QuoteString(prefix) + ".");
            }

            result.AppendFormat(QuoteString(columnName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }

            return result.ToString();
        }

        protected string GetOrderByClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            if (orderByIndex > 0)
            {
                return sql.Substring(orderByIndex);
            }

            return null;
        }

        protected string GetWhereClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            int whereIndex = sql.LastIndexOf(" WHERE ", StringComparison.InvariantCultureIgnoreCase);
            if (whereIndex > 0)
            {
                return sql.Substring(whereIndex, orderByIndex - whereIndex);
            }
            return null;
        }

        protected int GetFromStart(string sql)
        {
            int selectCount = 0;
            string[] words = sql.Split(' ');
            int fromIndex = 0;
            foreach (var word in words)
            {
                if (word.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        protected int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", "sql");
        }

        protected IList<string> GetColumnNames(string sql)
        {
            int start = GetSelectEnd(sql);
            int stop = GetFromStart(sql);
            string[] columnSql = sql.Substring(start, stop - start).Split(',');
            List<string> result = new List<string>();
            foreach (string c in columnSql)
            {
                int index = c.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                string[] colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }
            return result;
        }

        /// <summary>
        /// 获取分页语句SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public string GetPagingSql(string sql, int pageIndex, int pageSize)
        {
            int selectIndex = GetSelectEnd(sql) + 1;
            string orderByClause = GetOrderByClause(sql);
            if (orderByClause == null)
            {
                orderByClause = " ORDER BY CURRENT_TIMESTAMP";
            }

            string projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_proj", s, null)), sb => sb.ToString());
            string newSql = sql
                .Replace(orderByClause, string.Empty)
                .Insert(selectIndex, string.Format("ROW_NUMBER() OVER(ORDER BY {0}) AS {1}, ", orderByClause.Substring(10), GetColumnName(null, "_row_number", null)));

            int startValue = (pageIndex * pageSize) + 1;
            string result = string.Format("SELECT TOP({0}) {1} FROM ({2}) [_proj] WHERE {3} >= {4} ORDER BY {3}",
                pageSize, projectedColumns.Trim(), newSql, GetColumnName("_proj", "_row_number", null), startValue);

            return result;
        }

        /// <summary>
        /// 获取查询记录总数SQL语句
        /// </summary>
        /// <param name="classMap"></param>
        /// <param name="whereSQL"></param>
        /// <returns></returns>
        public string GetCountSql(IEntityMapper classMap, string whereSQL)
        {
            StringBuilder countSql = new StringBuilder(string.Format("SELECT COUNT(*) AS [Total] FROM {0}",
                                GetTableName(classMap)));
            if (!string.IsNullOrEmpty(whereSQL))
            {
                countSql.Append(" ")
                    .Append(whereSQL);
            }

            return countSql.ToString();
        }
        #endregion
    }
}