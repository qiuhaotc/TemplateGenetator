using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace TemplateGenerator.Util
{
    internal class ModuleHelp
    {
        /// <summary>
        /// 根据一行数据填充实体类
        /// </summary>
        /// <typeparam name="mouob">需要一个实体类对象</typeparam>
        /// <param name="Tr">table数据源，注意只取第一行</param>
        /// <returns>返回填充后的实体类</returns>
        public static object GetEntity(object mouob, DataRow Tr)
        {
            PropertyInfo[] Fields = mouob.GetType().GetProperties();
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                if (DBNull.Value != Tr[PI.Name])
                {
                    PI.SetValue(mouob, Tr[PI.Name], null);
                }
                i++;
            }
            Tr.Delete();
            return mouob;
        }

        /// <summary>
        /// 根据一行数据填充实体类
        /// </summary>
        /// <typeparam name="mouob">需要一个实体类对象</typeparam>
        /// <param name="Tr">table数据源，注意只取第一行</param>
        /// <returns>返回填充后的实体类</returns>
        public static object GetEntity(object mouob, DataRow Tr, PropertyInfo[] Fields)
        {
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                if (DBNull.Value != Tr[PI.Name])
                {
                    PI.SetValue(mouob, Tr[PI.Name], null);
                }
                i++;
            }
            Tr.Delete();
            return mouob;
        }

        /// <summary>
        /// 给实体类初始化数据
        /// </summary>
        /// <param name="mouob">需要一个实体类对象</param>
        /// <returns>初始后的实体类</returns>
        public static object GetEntity(object mouob)
        {
            PropertyInfo[] Fields = mouob.GetType().GetProperties();
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                if (PI.PropertyType == typeof(System.Int32))
                {
                    PI.SetValue(mouob, 0, null);
                }
                else if (PI.PropertyType == typeof(System.String))
                {
                    PI.SetValue(mouob, "", null);
                }
                else if (PI.PropertyType == typeof(System.DateTime))
                {
                    PI.SetValue(mouob, Convert.ToDateTime("2000-1-1"), null);
                }
                else if (PI.PropertyType == typeof(System.Double))
                {
                    PI.SetValue(mouob, 0, null);
                }
                else if (PI.PropertyType == typeof(System.Boolean))
                {
                    PI.SetValue(mouob, false, null);
                }
                i++;
            }
            return mouob;
        }

        /// <summary>
        /// DataTable转化实体类
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="model">实体类</param>
        /// <returns>已经填充的实体类集合</returns>
        public static List<T> GetModelByDT<T>(DataTable dt, PropertyInfo[] pInfoList) where T : class,new()
        {
            return ToList<T>(dt, pInfoList);
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(DataTable dt) where TResult : class, new()
        {
            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例
                TResult ob = new TResult();
                //找到对应的数据  并赋值
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.
                oblist.Add(ob);
            }
            return oblist;
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="propArray">PropertyInfo[]</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(DataTable dt, PropertyInfo[] propArray) where TResult : class, new()
        {
            List<PropertyInfo> prlist = new List<PropertyInfo>();

            //获取TResult的类型实例  反射的入口
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表
            Array.ForEach<PropertyInfo>(propArray, p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例
                TResult ob = new TResult();
                //找到对应的数据  并赋值
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.
                oblist.Add(ob);
            }
            return oblist;
        }

        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if (value == null)
                {
                    return null;
                } // end if

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(conversionType);

                conversionType = nullableConverter.UnderlyingType;
            } // end if

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }
    }
}