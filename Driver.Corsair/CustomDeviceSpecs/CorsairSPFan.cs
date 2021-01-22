using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Driver.Corsair.CustomDeviceSpecs
{
    public class CorsairCustomDeviceSpecification : CustomDeviceSpecification
    {
        public CorsairCustomDeviceSpecification()
        {
            this.MadeByName = "Corsair";
            this.RGBOrder = RGBOrder.RGB;
            this.MapperName = null;
        }
    }
    public class CorsairSPFan : CorsairCustomDeviceSpecification   
    {
        public CorsairSPFan() : this(1){}

        public CorsairSPFan(int leds=1)
        {
            LedCount = leds;
            Name = "SP Fan";
            PngData = ImageHelper.ReadImageStream("SPFan.png");
        }
    }

    public class CorsairMLFan : CorsairCustomDeviceSpecification
    {
        public CorsairMLFan() : this(4) { }
        public CorsairMLFan(int leds = 4)
        {
            LedCount = leds;
            Name = "ML Fan";
            PngData = ImageHelper.ReadImageStream("MLFan.png");
        }
    }

    public class CorsairHDFan : CorsairCustomDeviceSpecification
    {
        public CorsairHDFan() : this(12) { }
        public CorsairHDFan(int leds = 12)
        {
            LedCount = leds;
            Name = "HD Fan";
            PngData = ImageHelper.ReadImageStream("HDFan.png");
        }
    }

    public class CorsairLLFan : CorsairCustomDeviceSpecification
    {
        public CorsairLLFan() : this(16)
        {
        }

        public CorsairLLFan(int leds = 16)
        {
            LedCount = leds;
            Name = "LL Fan";
            PngData = ImageHelper.ReadImageStream("LLFan.png");
        }
    }

    public class CorsairQLFan : CorsairCustomDeviceSpecification
    {
        public CorsairQLFan() : this(34)
        {
        }

        public CorsairQLFan(int leds = 34)
        {
            LedCount = leds;
            Name = "QL Fan";
            PngData = ImageHelper.ReadImageStream("QLFan.png");
        }
    }

    public class CorsairSPProFan : CorsairCustomDeviceSpecification
    {
        public CorsairSPProFan() : this(8)
        {
        }

        public CorsairSPProFan(int leds = 8)
        {
            LedCount = leds;
            Name = "SP/ML Pro Fan";
            PngData = ImageHelper.ReadImageStream("SPProFan.png");
        }
    }

    public class CorsairInternalLEDStrip : CorsairCustomDeviceSpecification
    {
        public CorsairInternalLEDStrip() : this(10)
        {
        }

        public CorsairInternalLEDStrip(int leds = 10)
        {
            LedCount = leds;
            Name = "Internal LED Strip";
            PngData = ImageHelper.ReadImageStream("LedStrip.png");
        }
    }

    public class CorsairLS100_250mm : CorsairCustomDeviceSpecification
    {
        public CorsairLS100_250mm() : this(15)
        {
        }

        public CorsairLS100_250mm(int leds = 15)
        {
            LedCount = leds;
            Name = "250mm LED Strip";
            PngData = ImageHelper.ReadImageStream("LS100-250mm.png");
        }
    }

    public class CorsairLS100_350mm : CorsairCustomDeviceSpecification
    {
        public CorsairLS100_350mm():this(21){}
        public CorsairLS100_350mm(int leds = 21)
        {
            LedCount = leds;
            Name = "350mm LED Strip";
            PngData = ImageHelper.ReadImageStream("LS100-350mm.png");
        }
    }

    public class CorsairLS100_450mm : CorsairCustomDeviceSpecification
    {
        public CorsairLS100_450mm() : this(27)
        {
        }

        public CorsairLS100_450mm(int leds = 27)
        {
            LedCount = leds;
            Name = "450mm LED Strip";
            PngData = ImageHelper.ReadImageStream("LS100-450mm.png");
        }
    }

    public class CorsairLS100_1M : CorsairCustomDeviceSpecification
{
    public CorsairLS100_1M():this(82){}
        public CorsairLS100_1M(int leds = 82)
        {
            LedCount = leds;
            Name = "1.4M LED Strip";
            PngData = ImageHelper.ReadImageStream("LS100-1M.png");
        }
    }
}
