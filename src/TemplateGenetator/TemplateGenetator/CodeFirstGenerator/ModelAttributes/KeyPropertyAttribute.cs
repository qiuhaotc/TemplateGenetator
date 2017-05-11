using System;

namespace TemplateGenerator.CodeFirstGenerator.ModelAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class KeyPropertyAttribute:Attribute
    {
        public KeyPropertyAttribute()
        {

        }
    }
}
