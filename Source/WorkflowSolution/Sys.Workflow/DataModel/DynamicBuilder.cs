using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sys.Workflow.DataModel
{
    /// <summary>
    /// 数据存储属性
    /// </summary>
    public class DataStorage : System.Attribute
    {
        public string TableName;
        public DataStorage(string tblName)
        {
            TableName = tblName;
        }
    }

    /// <summary>
    /// 运用 DynamicMethod and ILGenerator 创建实体
    /// CPOL开源协议
    /// 作者:Herbrandson
    ///changed:add a dicationary as a cache to improve speed. 2012-6-9  qinshichuan
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicBuilder<T>
    {
        // http://www.codeproject.com/info/cpol10.aspx

        private static readonly MethodInfo getValueMethod =
            typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });

        private static readonly MethodInfo isDBNullMethod =
            typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

        /// <summary>
        /// 该委托由动态IL调用
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        private delegate T Load(IDataRecord dataRecord);
        private Load handler;

        private DynamicBuilder() { }

        /// <summary>
        /// 执行CreateBuilder里创建的DynamicCreate动态方法的委托
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        private static object _LOCK = new object();
        private static IDictionary<int, object> _FULLENTITY_MAP = new Dictionary<int, object>();
            
        /// <summary>
        /// 得到一个创建者
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public static DynamicBuilder<T> CreateBuilder(IDataRecord dataRecord)
        {
            int entityTypeHashCode = typeof(T).GetHashCode();
            lock(_LOCK){
                if (_FULLENTITY_MAP.ContainsKey(entityTypeHashCode)){
                    return _FULLENTITY_MAP[entityTypeHashCode] as DynamicBuilder<T>;
                }
            }
            
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();

            //定义一个名为'DynamicCreate'的动态方法，返回值typof(T)，参数typeof(IDataRecord),与typeof(IDataRecord)逻辑关联{动态方法可以访问类型的所有成员。 },
            DynamicMethod method = new DynamicMethod("DynamicCreate", typeof(T),
                new Type[] { typeof(IDataRecord) }, typeof(T), true);

            //创建一个MSIL生成器，为动态方法生成代码
            ILGenerator generator = method.GetILGenerator();
            //为动态方法声明指定类型T的局部变量 T result;
            LocalBuilder result = generator.DeclareLocal(typeof(T));
            //The next piece of code instantiates the requested type of object and stores it in the local variable.
			//可以t=new T();这么理解
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(dataRecord.GetName(i));//根据列名取属性  原则上属性和列是一一对应的关系
                Label endIfLabel = generator.DefineLabel();

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)//实体存在该属性 且该属性有SetMethod方法
                {
                    /*The code then loops through the fields in the data reader, finding matching properties on the type passed in.
					 * When a match is found, the code checks to see if the value from the data reader is null.
					 */
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);//就知道这里要调用IsDBNull方法 如果IsDBNull==true contine
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    /*If the value in the data reader is not null, the code sets the value on the object.*/
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);//调用get_Item方法

                    //处理为空数据列
                    Type dataType = dataRecord.GetFieldType(i);
                    bool isNullable = false;
                    if (propertyInfo.PropertyType.Name.ToLower().Contains("nullable"))
                        isNullable = true;
                    if (isNullable)
                        generator.Emit(OpCodes.Unbox_Any, GetNullableType(dataType));
                    else
                        generator.Emit(OpCodes.Unbox_Any, dataType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());//给该属性设置对应值

                    generator.MarkLabel(endIfLabel);
                }
            }

            /*The last part of the code returns the value of the local variable*/
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);//方法结束，返回

            //完成动态方法的创建，并且创建执行该动态方法的委托，赋值到全局变量handler,handler在Build方法里Invoke
            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            lock(_LOCK){
                if (_FULLENTITY_MAP.ContainsKey(entityTypeHashCode)==false){
                    _FULLENTITY_MAP.Add(entityTypeHashCode, dynamicBuilder);
                }
            }
            return dynamicBuilder;
        }

        private static Type GetNullableType(Type type)
        {
            Type result = null;

            if (type == typeof(bool))
                result = typeof(Nullable<bool>);

            if (type == typeof(byte))
                result = typeof(Nullable<byte>);

            if (type == typeof(DateTime))
                result = typeof(Nullable<DateTime>);

            if (type == typeof(decimal))
                result = typeof(Nullable<decimal>);

            if (type == typeof(double))
                result = typeof(Nullable<double>);

            if (type == typeof(float))
                result = typeof(Nullable<float>);

            if (type == typeof(Guid))
                result = typeof(Nullable<Guid>);

            if (type == typeof(Int16))
                result = typeof(Nullable<Int16>);

            if (type == typeof(Int32))
                result = typeof(Nullable<Int32>);

            if (type == typeof(Int64))
                result = typeof(Nullable<Int64>);

            return result;
        }
    }
}
