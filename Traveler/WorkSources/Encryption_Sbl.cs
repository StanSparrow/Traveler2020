using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace NmspTraveler.WorkSources
{
	public class CEncryption
	{
		public static bool GetMd5Hash(string sInput, ref string sOutput)
        {
			if (NmspTraveler.CString.IsStringEmpty(sInput) == true) return false;
			MD5 md5HashObj = MD5.Create();
            if (md5HashObj == null) return false;

            byte[] data = md5HashObj.ComputeHash(Encoding.UTF8.GetBytes(sInput));
            StringBuilder sBuilder = new StringBuilder();
            
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

			if (sBuilder.Length == 0) return false;
            sOutput = sBuilder.ToString();

			if (NmspTraveler.CString.IsStringEmpty(sOutput) == true) return false;

			return true;
        }

		public static bool StringXor(string sKey, string sInput, ref string sOutput)
		{
			if (NmspTraveler.CString.IsStringEmpty(sKey) == true) return false;
			if (NmspTraveler.CString.IsStringEmpty(sInput) == true) return false;

			StringBuilder sbObj = new StringBuilder();
			if (sbObj == null) return false;

			for(int i=0; i < sInput.Length; i++)
				sbObj.Append((char)(sInput[i] ^ sKey[(i % sKey.Length)]));
			sOutput = sbObj.ToString ();

			if (NmspTraveler.CString.IsStringEmpty(sOutput) == true) return false;

			return true;
		}

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData) 
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
