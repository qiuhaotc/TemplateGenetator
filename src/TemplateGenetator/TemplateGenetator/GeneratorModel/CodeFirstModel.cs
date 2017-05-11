using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.GeneratorModel
{
    [Serializable]
    public class CodeFirstModel
    {
        public string ClassName { get; set; }
        public string Title { get; set; }
        public List<CodeFirstItem> CodeFirstItemList { get; set; }
    }
}
