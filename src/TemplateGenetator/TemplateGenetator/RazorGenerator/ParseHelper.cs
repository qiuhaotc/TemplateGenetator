using System;
using RazorEngine;
using RazorEngine.Templating;

namespace TemplateGenerator.RazorGenerator
{
    public class ParseHelper : MarshalByRefObject
    {
        static ParseHelper()
        {

        }
        

        public string ParseData(string keyName, string tempUrl, object modelData, DynamicViewBag viewBag)
        {
            ITemplateKey key = Engine.Razor.GetKey(keyName, ResolveType.Global);

            if (!Engine.Razor.IsTemplateCached(key, null))
            {
                string content = System.IO.File.ReadAllText(tempUrl);
                return Engine.Razor.RunCompile(content, key, null, modelData, viewBag);
            }
            else
            {
                //if(forceChange)
                //{
                //    key = Engine.Razor.GetKey(keyName+DateTime.Now.Ticks.ToString(), ResolveType.Global);

                //    string content = System.IO.File.ReadAllText(tempUrl);
                //    return Engine.Razor.RunCompile(content, key, null, modelData, viewBag);
                //}
                //else
                //{
                return Engine.Razor.RunCompile(key, null, modelData, viewBag);
                //}
            }
        }

        public string ParseDataWithString(string keyName, string content, object modelData, DynamicViewBag viewBag)
        {
            ITemplateKey key = Engine.Razor.GetKey(keyName, ResolveType.Global);

            if (!Engine.Razor.IsTemplateCached(key, null))
            {
                return Engine.Razor.RunCompile(content, key, null, modelData, viewBag);
            }
            else
            {
                return Engine.Razor.RunCompile(key, null, modelData, viewBag);
            }
        }
    }

}
