using System;

namespace TemplateGenerator.GeneratorModel.EnumData
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumNameAttribute : Attribute
    {
        readonly string enumName;
        
        public EnumNameAttribute(string enumName)
        {
            this.enumName = enumName;
        }

        public string EnumName
        {
            get { return enumName; }
        }
    }
}
