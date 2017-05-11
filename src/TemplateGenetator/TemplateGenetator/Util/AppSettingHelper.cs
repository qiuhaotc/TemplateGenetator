using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace TemplateGenerator.Util
{
    public  class AppSettingHelper
    {
        public static bool UpdateWebConfigAppSetting(string key, string value)
        {
            var _config = WebConfigurationManager.OpenWebConfiguration("~");
            if (!_config.HasFile)
            {
                throw new ArgumentException("程序配置文件缺失！");
            }
            KeyValueConfigurationElement _key = _config.AppSettings.Settings[key];
            if (_key == null)
                _config.AppSettings.Settings.Add(key, value);
            else
                _config.AppSettings.Settings[key].Value = value;
            _config.Save(ConfigurationSaveMode.Modified);
            return true;
        }

        public static string GetAppSetting(string keyName)
        {
            return System.Configuration.ConfigurationManager.AppSettings[keyName];
        }
    }
}
