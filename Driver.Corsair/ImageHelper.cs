using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Driver.Corsair
{
    internal static class ImageHelper
    {
        internal static byte[] ReadImageStream(string name)
        {
            Stream imgStream = System.Reflection.Assembly.GetAssembly(typeof(CUEDriver)).GetManifestResourceStream("Driver.Corsair.ProductImages." + name);
            var temp = new byte[imgStream.Length];
            imgStream.Read(temp, 0, (int)imgStream.Length);

            return temp;
        }

        public static Type[] GetInheritedClasses(Type MyType)
        {
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
        }
    }

    
}
