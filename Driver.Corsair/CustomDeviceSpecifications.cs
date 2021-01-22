using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Driver.Corsair
{
    public class CustomDevices
    {
        public static byte[] GetImage(string image)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            byte[] result;
            try
            {
                Stream imageStream = myAssembly.GetManifestResourceStream("Driver.Corsair.ProductImages." + image + ".png");
                result = new byte[imageStream.Length];
                imageStream.Read(result, 0, (int)imageStream.Length);

            }
            catch
            {
                Stream placeholder = myAssembly.GetManifestResourceStream("Driver.Corsair.CorsairPlaceholder.png");
                result = new byte[placeholder.Length];
                placeholder.Read(result, 0, (int)placeholder.Length);
            }

            return result;
        }
        public class MM800RGBPolaris : CustomDeviceSpecification
        {
            public MM800RGBPolaris()
            {
                this.Name = "MM800 RGB Polaris";
                this.LedCount = 15;
                this.PngData = GetImage("MM800");
                this.MapperName = ""; //no mapper needed
            }
        }

        public class LT100 : CustomDeviceSpecification
        {
            public LT100()
            {
                this.Name = "LT100";
                this.LedCount = 46;
                this.PngData = GetImage("LT100");
                this.MapperName = ""; //no mapper needed
            }
        }

        public static List<Type> GetDeviceTypes => new List<Type>
        {
            typeof(MM800RGBPolaris),
            typeof(LT100)
        }.ToList();
    }
}
