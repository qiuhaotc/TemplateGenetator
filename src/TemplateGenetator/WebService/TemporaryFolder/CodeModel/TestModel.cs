using TemplateGenerator.CodeFirstGenerator.ModelAttributes;
using TemplateGenerator.GeneratorModel.EnumData;

namespace WebService.TemporaryFolder.CodeModel
{
    [ItemDisplayName("测试模型")]
    public class TestModel
    {
        [KeyProperty]
        [DisplayType(DisplayTypeEnum.Hidden)]
        public int ID { get; set; }

        [ItemDisplayName("名称")]
        [DisplayType(DisplayTypeEnum.Input)]
        public string Name { get; set; }
    }
}