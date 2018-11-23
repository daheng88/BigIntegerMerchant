using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigIntegerMerchant
{
    public class ApiUtil
    {

        //merchantId和memberId最多16位
        public static string encryptMemberId(long merchantId, long memberId)
        {
            var merHexStr = merchantId + "" + GetMd5Hex(merchantId + "").Substring(16);
            if (merHexStr[0] > 8)
            {
                merHexStr = "0" + merHexStr;
            }
            BigInteger mer=BigInteger.Parse(merHexStr, NumberStyles.HexNumber);
            var hexStr = GetMd5Hex(memberId + "").Substring(0, 16) + memberId + "";
            if (hexStr[0] > 8)
            {
                hexStr = "0" + hexStr;
            }
            BigInteger mem = BigInteger.Parse(hexStr, NumberStyles.HexNumber);
            var newBig = (mem ^ mer);
            sbyte[] bts = ToSByte(newBig.ToByteArray());
            return encode(bts);
        }

        public static long decryptMemberId(long merchantId, string memberId)
        {
            try
            {
                BigInteger mer = new BigInteger(strToToHexByte(merchantId + "" + GetMd5Hex(merchantId + "").Substring(16)));
                BigInteger mem = new BigInteger(Convert.FromBase64String(memberId));
                string src = Convert.ToString((mem ^ mer).ToByteArray()).PadLeft(32, ' ');
                return Convert.ToInt64(src.Substring(16));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return 0;
            }
        }

        public static string GetMd5Hex(string content)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(content);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static sbyte[] ToSByte(byte[] bContent)
        {
            sbyte[] signed = Array.ConvertAll(bContent, b => unchecked((sbyte)b));
            return signed.Reverse().ToArray(); 
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string StrToHex(string mStr) //返回处理后的十六进制字符串
        {
            return BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");
        }

        public static string HexToStr(string mHex) // 返回十六进制代表的字符串
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);
        }

        public static System.String encode(sbyte[] inputBytes)
        {
            /// <summary>Conversion table for encoding to base64.
            /// 
            /// emap is a six-bit value to base64 (8-bit) converstion table.
            /// For example, the value of the 6-bit value 15
            /// is mapped to 0x50 which is the ASCII letter 'P', i.e. the letter P
            /// is the base64 encoded character that represents the 6-bit value 15.
            /// </summary>
            /*
            * 8-bit base64 encoded character                 base64       6-bit
            *                                                encoded      original
            *                                                character    binary value
            */
            char[] emap = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' }; // 4-9, + /;  56-63



            int i, j, k;
            int t, t1, t2;
            int ntb; // number of three-bytes in inputBytes
            bool onePadding = false, twoPaddings = false;
            char[] encodedChars; // base64 encoded chars
            int len = inputBytes.Length;

            if (len == 0)
            {
                // No data, return no data.
                return new System.Text.StringBuilder("").ToString();
            }

            // every three bytes will be encoded into four bytes
            if (len % 3 == 0)
            {
                ntb = len / 3;
            }
            // the last one or two bytes will be encoded into
            // four bytes with one or two paddings
            else
            {
                ntb = len / 3 + 1;
            }

            // need two paddings
            if ((len % 3) == 1)
            {
                twoPaddings = true;
            }
            // need one padding
            else if ((len % 3) == 2)
            {
                onePadding = true;
            }

            encodedChars = new char[ntb * 4];

            // map of decoded and encoded bits
            //     bits in 3 decoded bytes:   765432  107654  321076  543210
            //     bits in 4 encoded bytes: 76543210765432107654321076543210
            //       plain           "AAA":   010000  010100  000101  000001
            //       base64 encoded "QUFB": 00010000000101000000010100000001
            // one padding:
            //     bits in 2 decoded bytes:   765432  10 7654  3210
            //     bits in 4 encoded bytes: 765432107654 321076543210 '='
            //       plain            "AA":   010000  010100  0001
            //       base64 encoded "QUE=": 00010000000101000000010000111101
            // two paddings:
            //     bits in 1 decoded bytes:   765432  10
            //     bits in 4 encoded bytes: 7654321076543210 '=' '='
            //       plain             "A":   010000  01
            //       base64 encoded "QQ==": 00010000000100000011110100111101
            //
            // note: the encoded bits which have no corresponding decoded bits
            // are filled with zeros; '=' = 00111101.
            for (i = 0, j = 0, k = 1; i < len; i += 3, j += 4, k++)
            {

                // build encodedChars[j]
                t = 0x00ff & inputBytes[i];
                encodedChars[j] = emap[t >> 2];

                // build encodedChars[j+1]
                if ((k == ntb) && twoPaddings)
                {
                    encodedChars[j + 1] = emap[(t & 0x03) << 4];
                    encodedChars[j + 2] = '=';
                    encodedChars[j + 3] = '=';
                    break;
                }
                else
                {
                    t1 = 0x00ff & inputBytes[i + 1];
                    encodedChars[j + 1] = emap[((t & 0x03) << 4) + ((t1 & 0xf0) >> 4)];
                }

                // build encodedChars[j+2]
                if ((k == ntb) && onePadding)
                {
                    encodedChars[j + 2] = emap[(t1 & 0x0f) << 2];
                    encodedChars[j + 3] = '=';
                    break;
                }
                else
                {
                    t2 = 0x00ff & inputBytes[i + 2];
                    encodedChars[j + 2] = (emap[(t1 & 0x0f) << 2 | (t2 & 0xc0) >> 6]);
                }

                // build encodedChars[j+3]
                encodedChars[j + 3] = (emap[(t2 & 0x3f)]);
            }
            return new System.String(encodedChars);
        }
    
}
}
