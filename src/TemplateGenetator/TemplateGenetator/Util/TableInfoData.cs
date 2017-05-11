using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.Util
{
    public class TableInfoData : BaseDAL<GeneratorModel.TableInfoModel>
    {
        public override void SetParameter()
        {
            TableName = @"TableInfo";
        }
    }
}
