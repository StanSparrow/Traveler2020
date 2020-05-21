using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NmspTraveler.WorkSources
{
    public class CPicture
    {
        public static bool PictureFileNameToPictureByteString(string sPictureFileName, 
            ref string sPictureByteString, ref string sErrorMessage)
        {
            try
            {
                var vFileStream = new FileStream(sPictureFileName, FileMode.Open, FileAccess.Read);
                if (vFileStream == null) return false;
                if (vFileStream.Length == 0) return false;

                byte[] baPicture = new byte[vFileStream.Length];
                if (baPicture == null) return false;

                vFileStream.Read(baPicture, 0, (int)vFileStream.Length);

                for (int i = 0; i < baPicture.Count(); i++)
                {
                    sPictureByteString += baPicture[i].ToString();
                    if (i < baPicture.Count() - 1) sPictureByteString += "\\";
                }
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

		public static bool DataBasePictureBytesToImageSource(string sPictureByteString, 
            ref ImageSource isPicture, ref string sErrorMessage)
        {
            try
            {
                var vBytes = sPictureByteString.Split('\\');
                if (vBytes == null) return false;
                if (vBytes.Count() == 0) return false;

                byte[] baPictureByteArray = new byte[vBytes.Count()];

                for (int i = 0; i < vBytes.Count(); i++)
                    baPictureByteArray[i] = byte.Parse(vBytes[i]);

                if (baPictureByteArray == null) return false;
                if (baPictureByteArray.Count() == 0) return false;

                MemoryStream msPictureByteArray = new MemoryStream(baPictureByteArray);
                if (msPictureByteArray == null) return false;

                isPicture = BitmapFrame.Create(msPictureByteArray,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                if (isPicture == null) return false;
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }
    }
}
