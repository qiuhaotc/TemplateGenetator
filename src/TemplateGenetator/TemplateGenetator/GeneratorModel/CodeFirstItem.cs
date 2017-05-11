using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.GeneratorModel.EnumData;

namespace TemplateGenerator.GeneratorModel
{
    [Serializable]
    public class CodeFirstItem
    {
        public int Order { get; set; }
        public string ItemName { get; set; }
        public string ItemDisplayName { get; set; }
        public DisplayTypeEnum DisplayType { get; set; }
        public bool IsKey { get; set; }
    }
}
