using TemplateGenerator.GeneratorModel.EnumData;
namespace WebService.Data
{
    public enum TemplateType
    {
        [EnumName("数据表生成模板")]
        DataFirst=5,
        [EnumName("模型生成模板")]
        ModelFirst =10,
    }
}
