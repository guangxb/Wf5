using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sys.Workflow.Utility;

namespace Sys.Workflow.DataModel.SQL
{
    /// <summary>
    /// 列字段类型
    /// </summary>
    public enum ColumnType
    {
        NotAKey,
        Identity,
        Guid,
        Assigned
    }

    public interface IPropertyMap
    {
        string Name { get; }
        string ColumnName { get; }
        ColumnType ColumnType { get; }
        PropertyInfo PropertyInfo { get; }
    }

    /// <summary>
    /// 属性映射类
    /// </summary>
    public class PropertyMap : IPropertyMap
    {
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        public string ColumnName
        {
            get;
            private set;
        }

        public ColumnType ColumnType
        {
            get;
            private set;
        }

        public PropertyMap(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            ColumnName = PropertyInfo.Name;
        }

        public string Name
        {
            get
            {
                return PropertyInfo.Name;
            }
        }

        public PropertyMap Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }

        public PropertyMap Key(ColumnType columnType)
        {
            ColumnType = columnType;
            return this;
        }
    }

    public interface IEntityMapper
    {
        string SchemaName { get; }
        string TableName { get; }
        IList<IPropertyMap> Properties { get; }
    }

    /// <summary>
    /// 通用实体Mapper类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityMapper<T> : IEntityMapper where T : class
    {
        public string SchemaName { get; protected set; }
        public string TableName { get; protected set; }
        public IList<IPropertyMap> Properties { get; private set; }

        public EntityMapper()
        {
            Properties = new List<IPropertyMap>();
        }

        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }

        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// 构造列信息
        /// </summary>
        protected virtual void AutoMap()
        {
            Type type = typeof(T);
            bool keyFound = Properties.Any(p => p.ColumnType != ColumnType.NotAKey);

            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                PropertyMap map = Map(propertyInfo);
                if (!keyFound && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                {
                    if (map.PropertyInfo.PropertyType == typeof(int) || map.PropertyInfo.PropertyType == typeof(int?))
                    {
                        map.Key(ColumnType.Identity);
                    }
                    else if (map.PropertyInfo.PropertyType == typeof(long) || map.PropertyInfo.PropertyType == typeof(long?))
                    {
                        map.Key(ColumnType.Identity);
                    }
                    else if (map.PropertyInfo.PropertyType == typeof(Guid) || map.PropertyInfo.PropertyType == typeof(Guid?))
                    {
                        map.Key(ColumnType.Guid);
                    }
                    else
                    {
                        map.Key(ColumnType.Assigned);
                    }
                    keyFound = true;
                }
            }
        }

        protected PropertyMap Map(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Map(propertyInfo);
        }

        protected PropertyMap Map(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo);
            Properties.Add(result);
            return result;
        } 
    }

    /// <summary>
    /// 具体实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoEntityMapper<T> : EntityMapper<T> where T : class
    {
        public AutoEntityMapper()
        {
            System.Attribute attr = System.Attribute.GetCustomAttributes(typeof(T))[0];
            Schema("dbo");
            Table((attr as DataStorage).TableName);
            AutoMap();
        }
    }
}