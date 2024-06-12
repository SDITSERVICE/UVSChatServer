using System;
using System.Collections.Generic;
using System.Text;

namespace UVSMedia.Media
{
    public class Utils
    {
        /// <summary>
        /// Readable array format
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder hexString = new StringBuilder(byteArray.Length * 4);
            foreach (byte b in byteArray)
            {
                hexString.AppendFormat("0x{0:X2} ", b);
            }
            return hexString.ToString().Trim();
        }
    }
}
