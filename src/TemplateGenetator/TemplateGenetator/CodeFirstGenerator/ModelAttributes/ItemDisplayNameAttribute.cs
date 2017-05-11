using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.CodeFirstGenerator.ModelAttributes
{
    /// <summary>
    /// 名称Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ItemDisplayNameAttribute : Attribute
    {
        readonly string displayName;
        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public ItemDisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
