namespace A.UserAccessManage
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    public class ExtensionUtils
    {
        public static string GetAuthenticationMethod(string extension)
        {
            XElement extensionElement = GetExtensionElement(extension);
            if (extensionElement != null)
            {
                XElement element2 = extensionElement.Element("authentication_method");
                if (element2 != null)
                {
                    return element2.Value;
                }
            }
            return null;
        }

        public static XElement GetExtensionElement(string extension)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(extension)))
                    {
                        return XDocument.Load(stream).Element("extension");
                    }
                }
                catch (Exception exception)
                {
                    throw new InvalidExtensionFormatException(extension, exception);
                }
            }
            return null;
        }
    }
}

