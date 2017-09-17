using System.IO;
using System.Xml.Serialization;

namespace HousingBuyingHelper.Extension
{
    public static class StringExtension
    {
        public static T Deserialize<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }

            var serializer = new XmlSerializer(typeof(T));
            var returnObject = (T)serializer.Deserialize(new StringReader(str));

            return returnObject;
        }
    }
}
