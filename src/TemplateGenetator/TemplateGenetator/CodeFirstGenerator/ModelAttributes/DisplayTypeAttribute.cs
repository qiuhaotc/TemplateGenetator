using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.CodeFirstGenerator.ModelAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DisplayTypeAttribute:Attribute
    {
        public GeneratorModel.EnumData.DisplayTypeEnum DisplayType { get; set; }

        public DisplayTypeAttribute(GeneratorModel.EnumData.DisplayTypeEnum displayType)
        {
            this.DisplayType = displayType;
        }
    }
}
