using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TemplateGenerator.RazorGenerator
{
    /// <summary>
    /// 通过应用程序域获取Razor对象 方便模板更新
    /// </summary>
    public class DomainHelper
    {
        static AppDomain appDomain = null;
        static GeneratorHelper helper = null;

        static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 获取GeneratorHelper
        /// </summary>
        /// <param name="forceChange">强制刷新</param>
        /// <param name="dllLocation">dll的文件夹位置</param>
        /// <returns></returns>
        internal static GeneratorHelper GetGeneratorHelper(bool forceChange,string dllLocation)
        {
            try
            {
                helper = null;

                if (appDomain != null)
                {
                    AppDomain.Unload(appDomain);
                    appDomain = null;
                }

                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                setup.CachePath = AppDomain.CurrentDomain.BaseDirectory;
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = AppDomain.CurrentDomain.BaseDirectory;
                appDomain = AppDomain.CreateDomain("DomainRazor", null, setup);
                //appDomain = AppDomain.CreateDomain("DomainRazor", null, AppDomain.CurrentDomain.BaseDirectory, "bin", true);


                //helper = appDomain.CreateInstanceAndUnwrap("TemplateGenerator", "TemplateGenerator.RazorGenerator.GeneratorHelper") as GeneratorHelper;
                helper = appDomain.CreateInstanceFromAndUnwrap(dllLocation+"/TemplateGenerator.dll", "TemplateGenerator.RazorGenerator.GeneratorHelper") as GeneratorHelper;
            }
            catch
            {
                throw;
            }

            return helper;
        }

        /// <summary>
        /// 根据模板生成代码
        /// </summary>
        /// <param name="forceChange"></param>
        /// <param name="dllLocation"></param>
        /// <param name="tempURL"></param>
        /// <param name="tablename"></param>
        /// <param name="connectionStr"></param>
        /// <param name="nameSpaceStr"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string GenetatorTemp(bool forceChange, string dllLocation,string tempURL, string tablename,string connectionStr, string nameSpaceStr,string dbType)
        {
            if(forceChange||helper==null||appDomain==null)
            {
                try
                {
                    rwLock.EnterWriteLock();
                    helper = GetGeneratorHelper(forceChange, dllLocation);

                    return helper.GenetatorTemp(tempURL, tablename, connectionStr, nameSpaceStr, dbType);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }
            else
            {
                try
                {
                    rwLock.EnterReadLock();

                    if (helper == null || appDomain == null)
                    {
                        throw new NullReferenceException("生成器还未准备好,请稍后再试！");
                    }

                    return helper.GenetatorTemp(tempURL, tablename, connectionStr, nameSpaceStr,dbType);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 根据定义的模型类生成代码
        /// </summary>
        /// <param name="tempURL"></param>
        /// <param name="forceChange"></param>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static string GenetatorCodeFirst(string dllLocation,string tempURL, bool forceChange, string dataString)
        {

            if (forceChange || helper == null || appDomain == null)
            {
                try
                {
                    rwLock.EnterWriteLock();
                    helper = GetGeneratorHelper(forceChange, dllLocation);

                    return helper.GenetatorCodeFirst(tempURL, dataString);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }
            else
            {
                try
                {
                    rwLock.EnterReadLock();

                    if (helper == null || appDomain == null)
                    {
                        throw new NullReferenceException("生成器还未准备好,请稍后再试！");
                    }

                    return helper.GenetatorCodeFirst(tempURL, dataString);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }
        }
    }
}
