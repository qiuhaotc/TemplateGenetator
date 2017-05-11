using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.Util
{
    public class JsonHelper
    {

        public static string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(string dataString)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(dataString);
        }
    }
}
