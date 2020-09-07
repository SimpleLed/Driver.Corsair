using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Driver.Corsair
{
    [StructLayout(LayoutKind.Sequential)]
    internal class _CorsairDeviceInfo
    {
        /// <summary>
        /// CUE-SDK: enum describing device type
        /// </summary>
        internal CorsairDeviceType type;

        /// <summary>
        /// CUE-SDK: null - terminated device model(like “K95RGB”)
        /// </summary>
        internal IntPtr model;

        /// <summary>
        /// CUE-SDK: enum describing physical layout of the keyboard or mouse
        /// </summary>
        internal int physicalLayout;

        /// <summary>
        /// CUE-SDK: enum describing logical layout of the keyboard as set in CUE settings
        /// </summary>
        internal int logicalLayout;

        /// <summary>
        /// CUE-SDK: mask that describes device capabilities, formed as logical “or” of CorsairDeviceCaps enum values
        /// </summary>
        internal int capsMask;

        /// <summary>
        /// CUE-SDK: number of controllable LEDs on the device
        /// </summary>
        internal int ledsCount;

        /// <summary>
        /// CUE-SDK: structure that describes channels of the DIY-devices
        /// </summary>
        internal _CorsairChannelsInfo channels;
    }

    /// <summary>
    /// Contains list of available corsair device types.
    /// </summary>
    public enum CorsairDeviceType
    {
        Unknown = 0,
        Mouse = 1,
        Keyboard = 2,
        Headset = 3,
        Mousepad = 4,
        HeadsetStand = 5,
        CommanderPro = 6,
        LightningNodePro = 7,
        MemoryModule = 8,
        Cooler = 9,
        Unknown2 = 10,
        Motherboard = 11,
        GraphicsCard = 12
    };

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// CUE-SDK: contains information about channels of the DIY-devices.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class _CorsairChannelsInfo
    {
        /// <summary>
        /// CUE-SDK: number of channels controlled by the device
        /// </summary>
        internal int channelsCount;

        /// <summary>
        /// CUE-SDK: array containing information about each separate channel of the DIY-device.
        /// Index of the channel in the array is same as index of the channel on the DIY-device.
        /// </summary>
        internal IntPtr channels;
    }
}
