using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NmspTraveler.WorkSources
{
    public class CSettingsData
    {
        public static bool Create(ref CSettingsData cSettingsDataObj, ref string sErrorMessage)
        {
            if (cSettingsDataObj != null) return false;
            cSettingsDataObj = new NmspTraveler.WorkSources.CSettingsData();
            if (cSettingsDataObj == null) return false;
            if (cSettingsDataObj.ReadSettingsFromFile(cSettingsDataObj.sJsonSettingsFileName, ref sErrorMessage) == false) return false;
            return true;
        }

        public bool ReadSettingsFromFile(string sFileName, ref string sErrorMessage)
        {
            try
            {
                string sJson = System.IO.File.ReadAllText(sFileName);
                if (sJson == null) return false;

                NmspTraveler.WorkSources.NmspWorkingStructures.NmspSettingWindow.CSettingsJsonData cJsonDataObj = 
					JsonConvert.DeserializeObject<NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspSettingWindow.CSettingsJsonData>(sJson);
                if (cJsonDataObj == null) return false;

				if (NmspTraveler.CString.IsStringEmpty(cJsonDataObj.sIp) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cJsonDataObj.sPort) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cJsonDataObj.sDatabaseName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cJsonDataObj.sUser) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cJsonDataObj.sPassword) == true) return false;

				string sDecryptedPassword = null;
				if (NmspTraveler.WorkSources.CEncryption.StringXor(cJsonDataObj.sUser, 
					cJsonDataObj.sPassword, ref sDecryptedPassword) == false) return false;

				sDecryptedPassword = NmspTraveler.WorkSources.CEncryption.Base64Decode(sDecryptedPassword);

				if (NmspTraveler.CString.IsStringEmpty(sDecryptedPassword) == true) return false;

                sIp_ = cJsonDataObj.sIp;
                sPort_ = cJsonDataObj.sPort;
                sDatabaseName_ = cJsonDataObj.sDatabaseName;
                sUser_ = cJsonDataObj.sUser;
				sPassword_ = sDecryptedPassword;
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public bool WriteSettingsToFile(string sFileName, ref string sErrorMessage)
        {
            try
            {
                NmspTraveler.WorkSources.NmspWorkingStructures.NmspSettingWindow.CSettingsJsonData cJsonDataObj = 
					new NmspTraveler.WorkSources.NmspWorkingStructures.NmspSettingWindow.CSettingsJsonData();
                if (cJsonDataObj == null) return false;

				if (NmspTraveler.CString.IsStringEmpty(sIp_) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(sPort_) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(sDatabaseName_) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(sUser_) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(sPassword_) == true) return false;

				string sEncryptedPassword = NmspTraveler.WorkSources.CEncryption.Base64Encode(sPassword_);

				if (NmspTraveler.WorkSources.CEncryption.StringXor(sUser_, 
					sEncryptedPassword, ref sEncryptedPassword) == false) return false;

				if (NmspTraveler.CString.IsStringEmpty(sEncryptedPassword) == true) return false;

                cJsonDataObj.sIp = sIp_;
                cJsonDataObj.sPort = sPort_;
                cJsonDataObj.sDatabaseName = sDatabaseName_;
                cJsonDataObj.sUser = sUser_;
                cJsonDataObj.sPassword = sEncryptedPassword;

                string sJson = JsonConvert.SerializeObject(cJsonDataObj);
                if (sJson == null) return false;

                System.IO.File.WriteAllText(sFileName, sJson);
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public void SetIp(string sIp)
        {
            sIp_ = sIp;
        }

        public string GetIp()
        {
            return sIp_;
        }

        public void SetPort(string sPort)
        {
            sPort_ = sPort;
        }

        public string GetPort()
        {
            return sPort_;
        }

        public void SetDBName(string sDBName)
        {
            sDatabaseName_ = sDBName;
        }

        public string GetDBName()
        {
            return sDatabaseName_;
        }

        public void SetUser(string sUser)
        {
            sUser_ = sUser;
        }

        public string GetUser()
        {
            return sUser_;
        }

        public void SetPassword(string sPassword)
        {
            sPassword_ = sPassword;
        }

        public string GetPassword()
        {
            return sPassword_;
        }

        private string sIp_ = null;
        private string sPort_ = null;
        private string sDatabaseName_ = null;
        private string sUser_ = null;
        private string sPassword_ = null;
        private string sJsonSettingsFileName = "Settings.txt";
    }
}
