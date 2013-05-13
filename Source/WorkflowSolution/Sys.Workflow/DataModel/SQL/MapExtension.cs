using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;

namespace Sys.Workflow.DataModel.SQL
{
    /// <summary>
    /// 映射扩展类
    /// </summary>
    public class MapExtension
    {
        private Type _defaultMapper;
        private SqlGenerator _sqlGenerator;
        private List<Type> _simpleTypes;
        public MapExtension(Type defaultMapper, SqlGenerator sqlGenerator)
        {
            _defaultMapper = defaultMapper;
            _sqlGenerator = sqlGenerator;
            _simpleTypes = new List<Type>
                            {
                                typeof(byte),
                                typeof(sbyte),
                                typeof(short),
                                typeof(ushort),
                                typeof(int),
                                typeof(uint),
                                typeof(long),
                                typeof(ulong),
                                typeof(float),
                                typeof(double),
                                typeof(decimal),
                                typeof(bool),
                                typeof(string),
                                typeof(char),
                                typeof(string),
                                typeof(DateTime),
                                typeof(DateTimeOffset),
                                typeof(byte[])
                            };
        }

        /// <summary>
        /// 获取映射类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEntityMapper GetMap<T>() where T : class
        {
            Type entityType = typeof(T);
            var mapType = _defaultMapper.MakeGenericType(typeof(T));
            var map = Activator.CreateInstance(mapType) as IEntityMapper;

            return map;
        }
    }
}
