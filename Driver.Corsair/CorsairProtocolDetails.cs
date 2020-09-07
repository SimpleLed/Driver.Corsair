using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Driver.Corsair
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// CUE-SDK: contains information about SDK and CUE versions
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairProtocolDetails
    {
        /// <summary>
        /// CUE-SDK: null - terminated string containing version of SDK(like “1.0.0.1”). Always contains valid value even if there was no CUE found
        /// </summary>
        internal IntPtr sdkVersion;

        /// <summary>
        /// CUE-SDK: null - terminated string containing version of CUE(like “1.0.0.1”) or NULL if CUE was not found.
        /// </summary>
        internal IntPtr serverVersion;

        /// <summary>
        /// CUE-SDK: integer number that specifies version of protocol that is implemented by current SDK.
        /// Numbering starts from 1. Always contains valid value even if there was no CUE found
        /// </summary>
        internal int sdkProtocolVersion;

        /// <summary>
        /// CUE-SDK: integer number that specifies version of protocol that is implemented by CUE.
        /// Numbering starts from 1. If CUE was not found then this value will be 0
        /// </summary>
        internal int serverProtocolVersion;

        /// <summary>
        /// CUE-SDK: boolean value that specifies if there were breaking changes between version of protocol implemented by server and client
        /// </summary>
        internal byte breakingChanges;
    };

    public class CorsairProtocolDetails
    {
        #region Properties & Fields

        /// <summary>
        /// String containing version of SDK(like "1.0.0.1").
        /// Always contains valid value even if there was no CUE found.
        /// </summary>
        public string SdkVersion { get; }

        /// <summary>
        /// String containing version of CUE(like "1.0.0.1") or NULL if CUE was not found.
        /// </summary>
        public string ServerVersion { get; }

        /// <summary>
        /// Integer that specifies version of protocol that is implemented by current SDK.
        /// Numbering starts from 1.
        /// Always contains valid value even if there was no CUE found.
        /// </summary>
        public int SdkProtocolVersion { get; }

        /// <summary>
        /// Integer that specifies version of protocol that is implemented by CUE.
        /// Numbering starts from 1.
        /// If CUE was not found then this value will be 0.
        /// </summary>
        public int ServerProtocolVersion { get; }

        /// <summary>
        /// Boolean that specifies if there were breaking changes between version of protocol implemented by server and client.
        /// </summary>
        public bool BreakingChanges { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor of managed CorsairProtocolDetails.
        /// </summary>
        /// <param name="nativeDetails">The native CorsairProtocolDetails-struct</param>
        internal CorsairProtocolDetails(_CorsairProtocolDetails nativeDetails)
        {
            this.SdkVersion = nativeDetails.sdkVersion == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(nativeDetails.sdkVersion);
            this.ServerVersion = nativeDetails.serverVersion == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(nativeDetails.serverVersion);
            this.SdkProtocolVersion = nativeDetails.sdkProtocolVersion;
            this.ServerProtocolVersion = nativeDetails.serverProtocolVersion;
            this.BreakingChanges = nativeDetails.breakingChanges != 0;
        }

        #endregion
    }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// CUE-SDK: contains information about separate LED-device connected to the channel controlled by the DIY-device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class _CorsairChannelDeviceInfo
    {
        /// <summary>
        /// CUE-SDK: type of the LED-device
        /// </summary>
        internal CorsairChannelDeviceType type;

        /// <summary>
        /// CUE-SDK: number of LEDs controlled by LED-device.
        /// </summary>
        internal int deviceLedCount;
    }

    /// <summary>
    /// Contains list of available corsair channel device types.
    /// </summary>
    public enum CorsairChannelDeviceType
    {
        Invalid = 0,
        FanHD = 1,
        FanSP = 2,
        FanLL = 3,
        FanML = 4,
        Strip = 5,
        DAP = 6,
        Pump = 7,
        FanQL = 8
    };
}
