namespace SDCard;

using System;
using System.Runtime.InteropServices;
using System.Text;

public class SdCardSerialNumber
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        uint nInBufferSize,
        [Out] byte[] lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped);

    private const uint IOCTL_DISK_GET_STORAGEID = 0x002D1400;
    private const uint GENERIC_READ = 0x80000000;
    private const uint OPEN_EXISTING = 3;

    public static string GetSdCardSerialNumber(string driveName)
    {
        IntPtr hFile = CreateFile(
            $@"\\.\{driveName}",
            GENERIC_READ,
            0,
            IntPtr.Zero,
            OPEN_EXISTING,
            0,
            IntPtr.Zero);

        if (hFile == IntPtr.Zero)
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        var data = new byte[512];
        uint returned;
        var result = DeviceIoControl(
            hFile,
            IOCTL_DISK_GET_STORAGEID,
            IntPtr.Zero,
            0,
            data,
            (uint)data.Length,
            out returned,
            IntPtr.Zero);

        if (!result)
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        var serialOffset = BitConverter.ToInt32(data, 12);
        var serialNumber = Encoding.ASCII.GetString(data, serialOffset, data.Length - serialOffset).TrimEnd('\0');

        return serialNumber;
    }
}
