using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.GeneratorModel
{
    public class DataInfoForJson
    {
        public int Status { get; set; }
        public string Desc { get; set; }
        public List<DataItem> Data { get; set; }
        public List<DataItem> DataTable { get; set; }
    }
}
