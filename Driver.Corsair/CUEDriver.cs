using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleLed;
using SimpleLed.RawInput;

namespace Driver.Corsair
{
    public class CUEDriver : ISimpleLed
    {
        public bool IsInitialized { get; private set; }

        public event EventHandler DeviceRescanRequired;

        /// <summary>
        /// Gets the loaded architecture (x64/x86).than
        /// </summary>
        public string LoadedArchitecture => _CUESDK.LoadedArchitecture;

        /// <summary>
        /// Gets the protocol details for the current SDK-connection.
        /// </summary>
        public CorsairProtocolDetails ProtocolDetails { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets whether the application has exclusive access to the SDK or not.
        /// </summary>
        public bool HasExclusiveAccess { get; private set; }

        /// <summary>
        /// Gets the last error documented by CUE.
        /// </summary>
        public CorsairError LastError => _CUESDK.CorsairGetLastError();

        public void Dispose()
        {
        }

        public void Configure(DriverDetails driverDetails)
        {
            _CUESDK.HomePath = driverDetails.HomeFolder;
            _CUESDK.Reload();

            ProtocolDetails = new CorsairProtocolDetails(_CUESDK.CorsairPerformProtocolHandshake());

            CorsairError error = LastError;
            if (error != CorsairError.Success)
                throw new Exception(error.ToString());

            if (ProtocolDetails.BreakingChanges)
                throw new Exception("The SDK currently used isn't compatible with the installed version of CUE.\r\n"
                                    + $"CUE-Version: {ProtocolDetails.ServerVersion} (Protocol {ProtocolDetails.ServerProtocolVersion})\r\n"
                                    + $"SDK-Version: {ProtocolDetails.SdkVersion} (Protocol {ProtocolDetails.SdkProtocolVersion})");


            //if (!_CUESDK.CorsairRequestControl(CorsairAccessMode.ExclusiveLightingControl))
            //{
            //    throw new Exception(LastError.ToString());
            //    HasExclusiveAccess = true;
            //}
            //else
            //{
            HasExclusiveAccess = false;
            //}

            if (!_CUESDK.CorsairSetLayerPriority(127))
            {
                throw new Exception(LastError.ToString());
            }

            // DarthAffe 07.07.2018: 127 is CUE, we want to directly compete with it as in older versions.
        }


        public List<ControlDevice> GetDevices()
        {
            List<ControlDevice> devices = new List<ControlDevice>();
            Dictionary<string, int> modelCounter = new Dictionary<string, int>();

            int deviceCount = _CUESDK.CorsairGetDeviceCount();

            var imgDict = new Dictionary<string, string>()
            {
                //Keyboards
                {"Corsair K65 RGB", "K65"},
                {"Corsair K65 LUX RGB", "K65"},

                {"Corsair K68 RGB", "K68"},

                {"Corsair STRAFE RGB", "Strafe"},

                {"Corsair K70 RGB", "K70"},
                {"Corsair K70 LUX RGB", "K70"},

                {"Corsair K95 RGB", "K95"},

                {"Corsair K70 RGB MK.2", "K70v2"},
                {"Corsair K70 RGB MK.2 LP", "K70v2"},
                {"Corsair K70 RGB MK.2 SE", "K70v2SE"},

                {"Corsair STRAFE RGB MK.2", "Strafev2"},

                {"Corsair K95 RGB PLATINUM", "K95Plat"},
                {"Corsair K95 RGB PLATINUM XT", "K95Plat"},

                //Mice
                {"Corsair HARPOON RGB", "Harpoon"},
                {"Corsair HARPOON RGB PRO", "Harpoon"},
                {"Corsair HARPOON RGB WIRELESS", "Harpoon"},

                {"Corsair M55 RGB", "M55"},

                {"Corsair M65 RGB", "M65"},
                {"Corsair M65 PRO RGB", "M65"},
                {"Corsair M65 RGB ELITE", "M65"},

                {"Corsair SCIMITAR RGB", "Scimitar"},
                {"Corsair SCIMITAR PRO RGB", "Scimitar"},
                {"Corsair SCIMITAR ELITE RGB", "Scimitar"},

                {"Corsair IRONCLAW RGB", "Ironclaw"},
                {"Corsair IRONCLAW RGB WIRELESS", "Ironclaw"},

                {"Corsair GLAIVE RGB", "Glaive"},
                {"Corsair GLAIVE RGB PRO", "Glaive"},

                {"Corsair NIGHTSWORD RGB", "Nightsword"},

                {"Corsair DARK CORE RGB", "DarkCore"},
                {"Corsair DARK CORE RGB SE", "DarkCore"},
                {"Corsair DARK CORE PRO RGB", "DarkCore"},
                {"Corsair DARK CORE PRO RGB SE", "DarkCore"},

                //Mousepads
                {"Corsair MM800RGB", "MM800"},
                {"Corsair MM800CRGB", "MM800"},

                //Headset Stands
                {"Corsair ST100RGB", "ST100"},

                //Headsets
                {"Corsair VOID Wireless", "Void"},
                {"Corsair VOID PRO Wireless", "Void"},
                {"Corsair VOID ELITE Wireless", "Void"},

                {"Corsair VIRTUOSO RGB Wireless", "Virtuoso"},
                {"Corsair VIRTUOSO SE RGB Wireless", "Virtuoso"},

                //DRAM
                {"Corsair VENGEANCE RGB PRO", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 2", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 3", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 4", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 5", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 6", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 7", "VengeancePro"},
                {"Corsair VENGEANCE RGB PRO 8", "VengeancePro"},

                {"Corsair DOMINATOR PLATINUM RGB", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 2", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 3", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 4", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 5", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 6", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 7", "DomPlat"},
                {"Corsair DOMINATOR PLATINUM RGB 8", "DomPlat"}
            };

            for (int i = 0; i < deviceCount; i++)
            {
                var tst = _CUESDK.CorsairGetDeviceInfo(i);
                _CorsairDeviceInfo nativeDeviceInfo =
                    (_CorsairDeviceInfo) Marshal.PtrToStructure(tst, typeof(_CorsairDeviceInfo));
                CorsairRGBDeviceInfo info =
                    new CorsairRGBDeviceInfo(i, DeviceTypes.Other, nativeDeviceInfo, modelCounter);
                string friendlyName = info.DeviceName.Replace("Corsair", "").Trim();

                if (!info.CapsMask.HasFlag(CorsairDeviceCaps.Lighting))
                {
                    continue; // Everything that doesn't support lighting control is useless
                }


                var nativeLedPositions = (_CorsairLedPositions) Marshal.PtrToStructure(
                    _CUESDK.CorsairGetLedPositionsByDeviceIndex(info.CorsairDeviceIndex), typeof(_CorsairLedPositions));

                int structSize = Marshal.SizeOf(typeof(_CorsairLedPosition));
                IntPtr ptr = nativeLedPositions.pLedPosition;

                List<_CorsairLedPosition> positions = new List<_CorsairLedPosition>();
                for (int ii = 0; ii < nativeLedPositions.numberOfLed; ii++)
                {
                    _CorsairLedPosition ledPosition =
                        (_CorsairLedPosition) Marshal.PtrToStructure(ptr, typeof(_CorsairLedPosition));
                    ptr = new IntPtr(ptr.ToInt64() + structSize);
                    positions.Add(ledPosition);
                }

                /*using (StreamWriter sw = File.AppendText((Path.Combine(docPath, "Devices.txt"))))
                {
                    sw.WriteLine("name: " + info.DeviceName);
                }*/

                string imageKey;

                if (imgDict.ContainsKey(info.DeviceName))
                {
                    imageKey = imgDict[info.DeviceName];
                }
                else
                {
                    switch (GetDeviceType(info.CorsairDeviceType))
                    {
                        case DeviceTypes.Keyboard:
                            imageKey = "K95Plat";
                            break;
                        case DeviceTypes.Mouse:
                            imageKey = "Scimitar";
                            break;
                        case DeviceTypes.MousePad:
                            imageKey = "MM800";
                            break;
                        case DeviceTypes.Headset:
                            imageKey = "Void";
                            break;
                        case DeviceTypes.HeadsetStand:
                            imageKey = "ST100";
                            break;
                        case DeviceTypes.MotherBoard:
                            imageKey = "Motherboard";
                            break;
                        case DeviceTypes.GPU:
                            imageKey = "GPU";
                            break;
                        case DeviceTypes.Cooler:
                            imageKey = "AIO";
                            break;
                        case DeviceTypes.Fan:
                            imageKey = "QLFan";
                            break;
                        case DeviceTypes.LedStrip:
                            imageKey = "LedStrip";
                            break;
                        case DeviceTypes.Memory:
                            imageKey = "VengeancePro";
                            break;
                        default:
                            imageKey = "CorsairPlaceholder";
                            break;
                    }
                }

                CorsairDevice device = new CorsairDevice
                {
                    Driver = this,
                    Name = friendlyName,
                    ProductImage = GetImage(imageKey),
                    CorsairDeviceIndex = info.CorsairDeviceIndex,
                    DeviceType = GetDeviceType(info.CorsairDeviceType)
                };


                var channelsInfo = (nativeDeviceInfo.channels);

                if (channelsInfo != null)
                {
                    IntPtr channelInfoPtr = channelsInfo.channels;

                    if (channelsInfo.channelsCount > 0)
                    {
                        for (int channel = 0; channel < channelsInfo.channelsCount; channel++)
                        {
                            _CorsairChannelInfo channelInfo =
                                (_CorsairChannelInfo) Marshal.PtrToStructure(channelInfoPtr,
                                    typeof(_CorsairChannelInfo));

                            int channelDeviceInfoStructSize = Marshal.SizeOf(typeof(_CorsairChannelDeviceInfo));
                            IntPtr channelDeviceInfoPtr = channelInfo.devices;
                            CorsairLedId channelReferenceLed = GetChannelReferenceId(info.CorsairDeviceType, channel);
                            _CorsairChannelDeviceInfo channelDeviceInfo =
                                (_CorsairChannelDeviceInfo) Marshal.PtrToStructure(channelDeviceInfoPtr,
                                    typeof(_CorsairChannelDeviceInfo));
                            if (info.CorsairDeviceType == CorsairDeviceType.Cooler && channel == 0)
                            {
                                //aio pump device
                                CorsairDevice aioPumpDevice = new CorsairDevice
                                {
                                    Driver = this,
                                    Name = friendlyName,
                                    ConnectedTo = "Channel "+(channel+1),
                                    TitleOverride = info.DeviceName,
                                    ProductImage = GetImage(imageKey),
                                    CorsairDeviceIndex = info.CorsairDeviceIndex,
                                    DeviceType = GetDeviceType(info.CorsairDeviceType)
                                };

                                List<ControlDevice.LedUnit> leds = new List<ControlDevice.LedUnit>();

                                for (int devLed = 0; devLed < channelDeviceInfo.deviceLedCount; devLed++)
                                {
                                    CorsairLedId corsairLedId = channelReferenceLed + devLed;
                                    leds.Add(new ControlDevice.LedUnit()
                                    {
                                        Data = new CorsairLedData
                                        {
                                            LEDNumber = devLed,
                                            CorsairLedId = (int)corsairLedId
                                        },
                                        LEDName = "Pump " + devLed
                                    });
                                }

                                aioPumpDevice.LEDs = leds.ToArray();
                                devices.Add(aioPumpDevice);
                            }
                            else
                            {
                                for (int dev = 0; dev < channelInfo.devicesCount; dev++)
                                {
                                    CorsairLedId referenceLed =
                                        channelReferenceLed + (dev * channelDeviceInfo.deviceLedCount);

                                    List<ControlDevice.LedUnit> leds = new List<ControlDevice.LedUnit>();

                                    string subDeviceName = "Invalid";
                                    string subDeviceType = DeviceTypes.Other;
                                    string subImageKey = "CorsairPlaceholder";

                                    switch (channelDeviceInfo.type)
                                    {
                                        case CorsairChannelDeviceType.Invalid:
                                            if (channelDeviceInfo.deviceLedCount == 27)
                                            {
                                                subDeviceName = "LT100RGB";
                                                subDeviceType = DeviceTypes.LedStrip;
                                                subImageKey = "LT100";
                                            }
                                            else
                                            {
                                                subDeviceName = "Unknown";
                                                subDeviceType = DeviceTypes.Other;
                                            }

                                            break;
                                        case CorsairChannelDeviceType.FanHD:
                                            subDeviceName = "HD Fan";
                                            subDeviceType = DeviceTypes.Fan;
                                            subImageKey = "HDFan";
                                            break;
                                        case CorsairChannelDeviceType.FanSP:
                                            subDeviceName = "SP Fan";
                                            subDeviceType = DeviceTypes.Fan;
                                            subImageKey = "SPFan";
                                            break;
                                        case CorsairChannelDeviceType.FanML:
                                            subDeviceName = "ML Fan";
                                            subDeviceType = DeviceTypes.Fan;
                                            subImageKey = "MLFan";
                                            break;
                                        case CorsairChannelDeviceType.FanLL:
                                            subDeviceName = "LL Fan";
                                            subDeviceType = DeviceTypes.Fan;
                                            subImageKey = "LLFan";
                                            break;
                                        case CorsairChannelDeviceType.Strip:
                                            subDeviceType = DeviceTypes.LedStrip;
                                            if (channelDeviceInfo.deviceLedCount > 10)
                                            {
                                                subDeviceName = "LS100 LED Strip";
                                                subImageKey = "LS100";
                                            }
                                            else
                                            {
                                                subDeviceName = "Internal LED Strip";
                                                subImageKey = "LedStrip";
                                            }

                                            break;
                                        case CorsairChannelDeviceType.DAP:
                                            subDeviceName = "DAP??";
                                            subDeviceType = DeviceTypes.Other;
                                            break;
                                        case CorsairChannelDeviceType.FanQL:
                                            subDeviceName = "QL Fan";
                                            subDeviceType = DeviceTypes.Fan;
                                            subImageKey = "QLFan";
                                            break;
                                        default:
                                            subDeviceName = "Unknown";
                                            break;
                                    }


                                    CorsairDevice subDevice = new CorsairDevice
                                    {
                                        Driver = this,
                                        Name = subDeviceName + " " +
                                               (dev + 1)
                                               .ToString(), //make device id start at 1 not 0 because normal people use this program
                                        ConnectedTo = "Channel " + (channel + 1),
                                        TitleOverride = info.DeviceName,
                                        ProductImage = GetImage(subImageKey),
                                        CorsairDeviceIndex = info.CorsairDeviceIndex,
                                        DeviceType = subDeviceType
                                    };

                                    for (int devLed = 0; devLed < channelDeviceInfo.deviceLedCount; devLed++)
                                    {
                                        //Fanman's ugly code for LED mapping. Abandon hope all ye who have to troublshoot this dumpster-fire of magic numbers.
                                        CorsairLedId corsairLedId;
                                        if (channelDeviceInfo.deviceLedCount > 30)
                                        {
                                            if ((int) referenceLed > 369 && (int) referenceLed != 350 &&
                                                (int) referenceLed != 384 && (int) referenceLed != 418 &&
                                                (int) referenceLed != 452 && (int) referenceLed != 486)
                                            {
                                                corsairLedId = referenceLed + 562 + devLed;
                                            }
                                            else if ((int) referenceLed > 335 && (int) referenceLed != 350 &&
                                                     (int) referenceLed != 384 && (int) referenceLed != 418 &&
                                                     (int) referenceLed != 452 && (int) referenceLed != 486)
                                            {
                                                if (devLed < 14)
                                                {
                                                    corsairLedId = referenceLed + devLed;
                                                }
                                                else
                                                {
                                                    corsairLedId = referenceLed + 562 + devLed;
                                                }
                                            }
                                            //ch2
                                            else if ((int) referenceLed >= 486)
                                            {
                                                if (devLed < 14)
                                                {
                                                    corsairLedId = referenceLed + devLed;
                                                }
                                                else
                                                {
                                                    corsairLedId = referenceLed + 562 + devLed;
                                                }
                                            }
                                            else
                                            {
                                                corsairLedId = referenceLed + devLed;
                                            }
                                        }
                                        else
                                        {
                                            corsairLedId = referenceLed + devLed;
                                        }

                                        leds.Add(new ControlDevice.LedUnit()
                                        {
                                            Data = new CorsairLedData
                                            {
                                                LEDNumber = devLed,
                                                CorsairLedId = (int) corsairLedId
                                            },
                                            LEDName = device.Name + " " + devLed
                                        });
                                    }

                                    subDevice.LEDs = leds.ToArray();
                                    devices.Add(subDevice);
                                }
                            }
                        }
                    }
                    else if (info.CorsairDeviceType == CorsairDeviceType.Keyboard)
                    {
                        List<ControlDevice.LedUnit> leds = new List<ControlDevice.LedUnit>();

                        int ctr = 0;
                        foreach (var lp in positions.OrderBy(x => x.LedId))
                        {
                            leds.Add(new ControlDevice.LedUnit()
                            {
                                Data = new CorsairPositionalLEDData()
                                {
                                    LEDNumber = ctr,
                                    CorsairLedId = lp.LedId,
                                    X = (int) lp.left,
                                    Y = Math.Max(0, ((int)lp.top - 38)),
                                },
                                LEDName = device.Name + " " + ctr
                            });
                            ctr++;
                        }

                        if (positions.Any())
                        {
                            int largestX = (int)positions.Max(x => x.left);
                            int largestY = (int)positions.Max(x => x.top);
                            device.Has2DSupport = true;
                            device.GridHeight = largestY;
                            device.GridWidth = largestX;
                            device.LEDs = leds.ToArray();
                        }


                    }
                    else
                    {
                        List<ControlDevice.LedUnit> leds = new List<ControlDevice.LedUnit>();

                        int ctr = 0;
                        foreach (var lp in positions.OrderBy(x => x.top + x.left))
                        {
                            leds.Add(new ControlDevice.LedUnit()
                            {
                                Data = new CorsairLedData()
                                {
                                    LEDNumber = ctr,
                                    CorsairLedId = lp.LedId
                                },
                                LEDName = device.Name + " " + ctr
                            });
                            ctr++;
                        }

                        device.LEDs = leds.ToArray();

                    }

                    if (info.CorsairDeviceType == CorsairDeviceType.CommanderPro ||
                        info.CorsairDeviceType == CorsairDeviceType.LightningNodePro
                        || info.CorsairDeviceType == CorsairDeviceType.Cooler) //filter out pointless devices
                    {
                        continue;
                    }
                    else
                    {
                        devices.Add(device);
                    }
                }
            }

            var gp = devices.GroupBy(x => x.Name);

            foreach (var gpx in gp)
            {
                if (gpx.ToList().Count > 1)
                {
                    int ct = 0;
                    foreach (ControlDevice controlDevice in gpx)
                    {
                        ct++;
                        controlDevice.Name = controlDevice.Name + " #" + ct;
                    }
                }
            }

            Debug.WriteLine("Done : " + LastError);
            return devices;
        }

        public Bitmap GetImage(string image)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();

            try
            {
                Stream imageStream =
                    myAssembly.GetManifestResourceStream("Driver.Corsair.ProductImages." + image + ".png");
                return (Bitmap) Image.FromStream(imageStream);
            }
            catch
            {
                Stream placeholder = myAssembly.GetManifestResourceStream("Driver.Corsair.CorsairPlaceholder.png");
                return (Bitmap) Image.FromStream(placeholder);
            }
        }


        private string GetDeviceType(CorsairDeviceType t)
        {
            switch (t)
            {
                case CorsairDeviceType.Cooler: return DeviceTypes.Cooler;
                case CorsairDeviceType.CommanderPro: return DeviceTypes.Other;
                case CorsairDeviceType.Headset: return DeviceTypes.Headset;
                case CorsairDeviceType.HeadsetStand: return DeviceTypes.HeadsetStand;
                case CorsairDeviceType.Keyboard: return DeviceTypes.Keyboard;
                case CorsairDeviceType.MemoryModule: return DeviceTypes.Memory;
                case CorsairDeviceType.Motherboard: return DeviceTypes.MotherBoard;
                case CorsairDeviceType.GraphicsCard: return DeviceTypes.GPU;
                case CorsairDeviceType.Unknown:
                    return DeviceTypes.Other;
                case CorsairDeviceType.Mouse:
                    return DeviceTypes.Mouse;
                case CorsairDeviceType.Mousepad:
                    return DeviceTypes.MousePad;
                case CorsairDeviceType.LightningNodePro:
                    return DeviceTypes.Other;
                default:
                    return DeviceTypes.Other;
            }
        }

        private static CorsairLedId GetChannelReferenceId(CorsairDeviceType deviceType, int channel)
        {
            if (deviceType == CorsairDeviceType.Cooler)
                return CorsairLedId.CustomLiquidCoolerChannel1Led1;
            else
            {
                switch (channel)
                {
                    case 0: return CorsairLedId.CustomDeviceChannel1Led1;
                    case 1: return CorsairLedId.CustomDeviceChannel2Led1;
                    case 2: return CorsairLedId.CustomDeviceChannel3Led1;
                }
            }

            return CorsairLedId.Invalid;
        }

        public void Push(ControlDevice controlDevice)
        {
            int deviceIndex = ((CorsairDevice) controlDevice).CorsairDeviceIndex;

            int numberOfLedsToUpdate = controlDevice.LEDs.Length;
            int structSize = Marshal.SizeOf(typeof(_CorsairLedColor));
            int ptrSize = structSize * numberOfLedsToUpdate;
            IntPtr ptr = Marshal.AllocHGlobal(ptrSize);
            IntPtr addPtr = new IntPtr(ptr.ToInt64());


            foreach (var led in controlDevice.LEDs)
            {
                _CorsairLedColor color = new _CorsairLedColor
                    {
                        ledId = led.Data is CorsairLedData cld ? cld.CorsairLedId : ((CorsairPositionalLEDData)led.Data).CorsairLedId,
                        r = (byte) led.Color.Red,
                        g = (byte) led.Color.Green,
                        b = (byte) led.Color.Blue
                    };



                Marshal.StructureToPtr(color, addPtr, false);
                addPtr = new IntPtr(addPtr.ToInt64() + structSize);
            }


            _CUESDK.CorsairSetLedsColorsBufferByDeviceIndex(deviceIndex, numberOfLedsToUpdate, ptr);

            _CUESDK.CorsairSetLedsColorsFlushBuffer();


            Marshal.FreeHGlobal(ptr);
        }

        public void Pull(ControlDevice controlDevice)
        {
            int structSize = Marshal.SizeOf(typeof(_CorsairLedColor));
            IntPtr ptr = Marshal.AllocHGlobal(structSize * controlDevice.LEDs.Count());
            IntPtr addPtr = new IntPtr(ptr.ToInt64());
            foreach (var led in controlDevice.LEDs)
            {
                _CorsairLedColor color = new _CorsairLedColor {ledId = led.Data is CorsairLedData cld ? cld.CorsairLedId : ((CorsairPositionalLEDData)led.Data).CorsairLedId };
                Marshal.StructureToPtr(color, addPtr, false);
                addPtr = new IntPtr(addPtr.ToInt64() + structSize);
            }

            _CUESDK.CorsairGetLedsColorsByDeviceIndex(((CorsairDevice) controlDevice).CorsairDeviceIndex,
                controlDevice.LEDs.Count(), ptr);

            IntPtr readPtr = ptr;
            for (int i = 0; i < controlDevice.LEDs.Count(); i++)
            {
                _CorsairLedColor ledColor =
                    (_CorsairLedColor) Marshal.PtrToStructure(readPtr, typeof(_CorsairLedColor));
                try
                {
                    var setme = controlDevice.LEDs.FirstOrDefault(x =>
                        ((CorsairLedData) x.Data).CorsairLedId == ledColor.ledId);
                    if (setme != null)
                    {
                        setme.Color = new LEDColor(ledColor.r, ledColor.g, ledColor.b);
                    }
                }
                catch
                {
                    var setme = controlDevice.LEDs.FirstOrDefault(x =>
                        ((CorsairPositionalLEDData)x.Data).CorsairLedId == ledColor.ledId);
                    if (setme != null)
                    {
                        setme.Color = new LEDColor(ledColor.r, ledColor.g, ledColor.b);
                    } 
                }


                readPtr = new IntPtr(readPtr.ToInt64() + structSize);
            }

            Marshal.FreeHGlobal(ptr);
        }

        static unsafe T[] MakeArray<T>(int t, int length) where T : struct
        {
            int tSizeInBytes = Marshal.SizeOf(typeof(T));
            T[] result = new T[length];
            for (int i = 0; i < length; i++)
            {
                IntPtr p = new IntPtr((byte*) t + (i * tSizeInBytes));
                result[i] = (T) System.Runtime.InteropServices.Marshal.PtrToStructure(p, typeof(T));
            }

            return result;
        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                SupportsPull = true,
                SupportsPush = true,
                IsSource = false,
                Id = Guid.Parse("59440d02-8ca3-4e35-a9a3-88b024cc0e2d"),
                Author = "Fanman03",
                Blurb = "Driver for all devices compatible with the iCUE SDK.",
                CurrentVersion = new ReleaseNumber(1,0,0,11),
                GitHubLink = "https://github.com/SimpleLed/Driver.Corsair",
                IsPublicRelease = true
            };
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            //TODO throw new NotImplementedException();
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            //TODO throw new NotImplementedException();
        }

        public string Name()
        {
            return "Corsair";
        }

        public class CorsairDevice : ControlDevice
        {
            public int CorsairDeviceIndex { get; set; }
        }

        public class CorsairLedData : ControlDevice.LEDData
        {
            public int CorsairLedId { get; set; }
        }

        public class CorsairPositionalLEDData : ControlDevice.PositionalLEDData
        {
            public int CorsairLedId { get; set; }
        }

        /*public class CorsairKeyboardControlDevice : ControlDevice
        {
            public void HandleInput(KeyPressEvent e)
            {
                var derp = HyperXSupport.humm.FirstOrDefault(x => x.DebugName == e.VKeyName);

                if (derp != null)
                {

                    TriggerAllMapped(new TriggerEventArgs
                    {
                        FloatX = (derp.X / (float)GridWidth),
                        FloatY = (derp.Y / (float)GridHeight),
                        X = derp.X,
                        Y = derp.Y
                    });

                }
                else
                {
                    Debug.WriteLine("Key not found: " + e.VKeyName + " - " + e.VKey);
                }
            }
        }*/
    }
}