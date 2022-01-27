// Decompiled with JetBrains decompiler
// Type: libzkfpcsharp.zkfp
// Assembly: libzkfpcsharp, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null
// MVID: 580F0D87-3F9A-4976-AC88-EA78E56F4B1A
// Assembly location: D:\QTech\J (2)\J\J\bin\Debug\libzkfpcsharp.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace libzkfpcsharp
{
  public class zkfp
  {
    public static int ZKFP_ERR_ALREADY_INIT = 1;
    public static int ZKFP_ERR_OK = 0;
    public static int ZKFP_ERR_INITLIB = -1;
    public static int ZKFP_ERR_INIT = -2;
    public static int ZKFP_ERR_NO_DEVICE = -3;
    public static int ZKFP_ERR_NOT_SUPPORT = -4;
    public static int ZKFP_ERR_INVALID_PARAM = -5;
    public static int ZKFP_ERR_OPEN = -6;
    public static int ZKFP_ERR_INVALID_HANDLE = -7;
    public static int ZKFP_ERR_CAPTURE = -8;
    public static int ZKFP_ERR_EXTRACT_FP = -9;
    public static int ZKFP_ERR_ABSORT = -10;
    public static int ZKFP_ERR_MEMORY_NOT_ENOUGH = -11;
    public static int ZKFP_ERR_BUSY = -12;
    public static int ZKFP_ERR_ADD_FINGER = -13;
    public static int ZKFP_ERR_DEL_FINGER = -14;
    public static int ZKFP_ERR_FAIL = -17;
    public static int ZKFP_ERR_CANCEL = -18;
    public static int ZKFP_ERR_VERIFY_FP = -20;
    public static int ZKFP_ERR_MERGE = -22;
    public static int ZKFP_ERR_NOT_OPENED = -23;
    public static int ZKFP_ERR_NOT_INIT = -24;
    public static int ZKFP_ERR_ALREADY_OPENED = -25;
    public static int ZKFP_ERR_LOADIMAGE = -26;
    public static int ZKFP_ERR_ANALYSE_IMG = -27;
    public static int ZKFP_ERR_TIMEOUT = -28;
    private IntPtr hDevice = IntPtr.Zero;
    private IntPtr hDBCache = IntPtr.Zero;
    public int imageWidth;
    public int imageHeight;
    public int imageDPI;
    public string devSn = "";

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_Init();

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_Terminate();

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetDeviceCount();

    [DllImport("libzkfp.dll")]
    private static extern IntPtr ZKFPM_OpenDevice(int index);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_CloseDevice(IntPtr handle);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetCaptureParamsEx(
      IntPtr handle,
      ref int width,
      ref int height,
      ref int dpi);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_SetParameters(
      IntPtr handle,
      int nParamCode,
      IntPtr paramValue,
      int cbParamValue);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetParameters(
      IntPtr handle,
      int nParamCode,
      IntPtr paramValue,
      ref int cbParamValue);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_AcquireFingerprint(
      IntPtr handle,
      IntPtr fpImage,
      uint cbFPImage,
      IntPtr fpTemplate,
      ref int cbTemplate);

    [DllImport("libzkfp.dll")]
    private static extern IntPtr ZKFPM_CreateDBCache();

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_CloseDBCache(IntPtr hDBCache);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GenRegTemplate(
      IntPtr hDBCache,
      IntPtr temp1,
      IntPtr temp2,
      IntPtr temp3,
      IntPtr regTemp,
      ref int cbRegTemp);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_AddRegTemplateToDBCache(
      IntPtr hDBCache,
      uint fid,
      IntPtr fpTemplate,
      uint cbTemplate);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_DelRegTemplateFromDBCache(IntPtr hDBCache, uint fid);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_ClearDBCache(IntPtr hDBCache);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetDBCacheCount(IntPtr hDBCache, IntPtr count);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_Identify(
      IntPtr hDBCache,
      IntPtr fpTemplate,
      uint cbTemplate,
      ref int FID,
      ref int score);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_VerifyByID(
      IntPtr hDBCache,
      uint fid,
      IntPtr fpTemplate,
      uint cbTemplate);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_MatchFinger(
      IntPtr hDBCache,
      IntPtr fpTemplate1,
      uint cbTemplate1,
      IntPtr fpTemplate2,
      uint cbTemplate2);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_ExtractFromImage(
      IntPtr hDBCache,
      string FilePathName,
      int DPI,
      IntPtr fpTemplate,
      ref int cbTemplate);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_AcquireFingerprintImage(
      IntPtr hDBCache,
      IntPtr fpImage,
      uint cbFPImage);

    public int Initialize()
    {
      int zkfpErrOk = zkfperrdef.ZKFP_ERR_OK;
      int num;
      if (zkfperrdef.ZKFP_ERR_OK != (num = zkfp.ZKFPM_Init()))
        return num;
      this.hDBCache = zkfp.ZKFPM_CreateDBCache();
      if (!(IntPtr.Zero == this.hDBCache))
        return num;
      zkfp.ZKFPM_Terminate();
      return zkfperrdef.ZKFP_ERR_INITLIB;
    }

    public int Finalize()
    {
      this.CloseDevice();
      zkfp.ZKFPM_CloseDBCache(this.hDBCache);
      zkfp.ZKFPM_Terminate();
      return zkfperrdef.ZKFP_ERR_OK;
    }

    public int GetDeviceCount() => zkfp.ZKFPM_GetDeviceCount();

    public int OpenDevice(int index)
    {
      if (IntPtr.Zero != this.hDevice)
        return zkfperrdef.ZKFP_ERR_ALREADY_OPENED;
      this.hDevice = zkfp.ZKFPM_OpenDevice(index);
      if (IntPtr.Zero == this.hDevice)
        return zkfperrdef.ZKFP_ERR_OPEN;
      byte[] numArray = new byte[64];
      int cbParamValue = 64;
      zkfp.ZKFPM_GetParameters(this.hDevice, 1103, Marshal.UnsafeAddrOfPinnedArrayElement((Array) numArray, 0), ref cbParamValue);
      this.devSn = Encoding.Default.GetString(numArray);
      zkfp.ZKFPM_GetCaptureParamsEx(this.hDevice, ref this.imageWidth, ref this.imageHeight, ref this.imageDPI);
      return zkfperrdef.ZKFP_ERR_OK;
    }

    public int CloseDevice()
    {
      this.devSn = "";
      if (!(IntPtr.Zero != this.hDevice))
        return zkfperrdef.ZKFP_ERR_NOT_OPENED;
      zkfp.ZKFPM_CloseDevice(this.hDevice);
      this.hDevice = IntPtr.Zero;
      return zkfperrdef.ZKFP_ERR_OK;
    }

    public int SetParameters(int code, byte[] pramValue, int size)
    {
      if (IntPtr.Zero == this.hDevice)
        return zkfperrdef.ZKFP_ERR_NOT_OPENED;
      if (pramValue == null || pramValue.Length < size || size <= 0)
        return zkfperrdef.ZKFP_ERR_INVALID_PARAM;
      int num = zkfp.ZKFPM_SetParameters(this.hDevice, code, Marshal.UnsafeAddrOfPinnedArrayElement((Array) pramValue, 0), size);
      if (num == 0 && 3 == code)
        zkfp.ZKFPM_GetCaptureParamsEx(this.hDevice, ref this.imageWidth, ref this.imageHeight, ref this.imageDPI);
      return num;
    }

    public int GetParameters(int code, byte[] paramValue, ref int size)
    {
      if (IntPtr.Zero == this.hDevice)
        return zkfperrdef.ZKFP_ERR_NOT_OPENED;
      size = paramValue.Length;
      return zkfp.ZKFPM_GetParameters(this.hDevice, code, Marshal.UnsafeAddrOfPinnedArrayElement((Array) paramValue, 0), ref size);
    }

    public int AcquireFingerprint(byte[] imgBuffer, byte[] template, ref int size)
    {
      if (IntPtr.Zero == this.hDevice)
        return zkfperrdef.ZKFP_ERR_NOT_OPENED;
      size = template.Length;
      return zkfp.ZKFPM_AcquireFingerprint(this.hDevice, Marshal.UnsafeAddrOfPinnedArrayElement((Array) imgBuffer, 0), (uint) imgBuffer.Length, Marshal.UnsafeAddrOfPinnedArrayElement((Array) template, 0), ref size);
    }

    public int AcquireFingerprintImage(byte[] imgBuffer) => IntPtr.Zero == this.hDevice ? zkfperrdef.ZKFP_ERR_NOT_OPENED : zkfp.ZKFPM_AcquireFingerprintImage(this.hDevice, Marshal.UnsafeAddrOfPinnedArrayElement((Array) imgBuffer, 0), (uint) imgBuffer.Length);

    public int GenerateRegTemplate(
      byte[] temp1,
      byte[] temp2,
      byte[] temp3,
      byte[] regTemp,
      ref int regTempLen)
    {
      if (IntPtr.Zero == this.hDBCache)
        return zkfperrdef.ZKFP_ERR_NOT_INIT;
      regTempLen = regTemp.Length;
      return zkfp.ZKFPM_GenRegTemplate(this.hDBCache, Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp1, 0), Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp2, 0), Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp3, 0), Marshal.UnsafeAddrOfPinnedArrayElement((Array) regTemp, 0), ref regTempLen);
    }

    public int AddRegTemplate(int fid, byte[] regTemp) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_AddRegTemplateToDBCache(this.hDBCache, (uint) fid, Marshal.UnsafeAddrOfPinnedArrayElement((Array) regTemp, 0), (uint) regTemp.Length);

    public int DelRegTemplate(int fid) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_DelRegTemplateFromDBCache(this.hDBCache, (uint) fid);

    public int Clear() => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_ClearDBCache(this.hDBCache);

    public int Identify(byte[] temp, ref int fid, ref int score) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_Identify(this.hDBCache, Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp, 0), (uint) temp.Length, ref fid, ref score);

    public int VerifyByID(int fid, byte[] temp) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_VerifyByID(this.hDBCache, (uint) fid, Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp, 0), (uint) temp.Length);

    public int Match(byte[] temp1, byte[] temp2) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_MatchFinger(this.hDBCache, Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp1, 0), (uint) temp1.Length, Marshal.UnsafeAddrOfPinnedArrayElement((Array) temp2, 0), (uint) temp2.Length);

    public int ExtractFromImage(string FileName, int DPI, byte[] template, ref int size) => IntPtr.Zero == this.hDBCache ? zkfperrdef.ZKFP_ERR_NOT_INIT : zkfp.ZKFPM_ExtractFromImage(this.hDBCache, FileName, DPI, Marshal.UnsafeAddrOfPinnedArrayElement((Array) template, 0), ref size);

    public static int Blob2Base64String(byte[] buf, int len, ref string strBase64)
    {
      strBase64 = Convert.ToBase64String(buf, 0, len);
      return strBase64.Length;
    }

    public static byte[] Base64String2Blob(string strBase64) => Convert.FromBase64String(strBase64);

    public static bool ByteArray2Int(byte[] buf, ref int value)
    {
      if (buf.Length < 4)
        return false;
      value = BitConverter.ToInt32(buf, 0);
      return true;
    }

    public static bool Int2ByteArray(int value, byte[] buf)
    {
      if (buf == null || buf.Length < 4)
        return false;
      buf[0] = (byte) (value & (int) byte.MaxValue);
      buf[1] = (byte) ((value & 65280) >> 8);
      buf[2] = (byte) ((value & 16711680) >> 16);
      buf[3] = (byte) (value >> 24 & (int) byte.MaxValue);
      return true;
    }
  }
}
