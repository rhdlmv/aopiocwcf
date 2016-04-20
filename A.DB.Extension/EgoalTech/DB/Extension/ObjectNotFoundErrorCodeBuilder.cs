namespace EgoalTech.DB.Extension
{
    using EgoalTech.DB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class ObjectNotFoundErrorCodeBuilder
    {
        public static string GenNotFoundErrorCodes(string prefix, params Module[] module)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = new List<string>();
            foreach (Module module2 in module.Distinct<Module>())
            {
                List<string> modelNames = GetModelNames(module2);
                list.AddRange(modelNames);
            }
            for (int i = 0; i < list.Count; i++)
            {
                int num2 = i + 1;
                builder.AppendFormat("public const string {0}_NOT_EXIST = \"{1}-{2}\";\r\n", list[i].ToUpper(), prefix, num2.ToString().PadLeft(3, '0'));
            }
            return builder.ToString();
        }

        public static List<string> GetModelNames(Module module)
        {
            List<string> list = new List<string>();
            foreach (Type type in module.GetTypes())
            {
                DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(type);
                list.Add(dbObjectInfo.TableName);
            }
            return list;
        }
    }
}

