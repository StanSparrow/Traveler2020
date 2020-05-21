using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler
{
    public class CDataHub
    {
        public bool CreateSettingsDataObject(ref string sErrorMessage)
        {
            if (cSettingsDataObj_ != null) return false;
            if (NmspTraveler.WorkSources.CSettingsData.Create(ref cSettingsDataObj_, 
                ref sErrorMessage) == false) return false;    
            if (cSettingsDataObj_ == null) return false;

            return true;
        }

        public bool CreatePostgresqlWorkerObject(
            NmspTraveler.WorkSources.CSettingsData cSettingsDataObj, 
            ref string sErrorMessage)
        {
            if (cSettingsDataObj == null) return false;

            if (NmspTraveler.WorkSources.CPostgresqlWorker.Connect(cSettingsDataObj.GetIp(), cSettingsDataObj.GetPort(),
                cSettingsDataObj.GetDBName(), cSettingsDataObj.GetUser(), cSettingsDataObj.GetPassword(), 
                ref cPostgresqlWorkerObj_, ref sErrorMessage) == false) return false;

            if (cPostgresqlWorkerObj_ == null) return false;

            return true;
        }

        public bool CreateAccessObject(ref string sErrorMessage)
        {
            if (cAccessManagerObj_ != null) return false;
            cAccessManagerObj_ = new NmspTraveler.WorkSources.CAccessManager();
            if (cAccessManagerObj_ == null) return false;

            return true;
        }

		public bool CreateCommonCurrentAccountInformationObject()
		{
			if (cCommonCurrentAccountInformationObj_ != null) return false;
			cCommonCurrentAccountInformationObj_ = new NmspTraveler.WorkSources.CCommonCurrentAccountInformation(); 
			if (cCommonCurrentAccountInformationObj_ == null) return false;

			return true;
		}

        public NmspTraveler.WorkSources.CPostgresqlWorker GetPostgresqlWorkerObject()
        {
            return cPostgresqlWorkerObj_;
        }

        public NmspTraveler.WorkSources.CSettingsData GetSettingsDataObject()
        {
            return cSettingsDataObj_;
        }

        public NmspTraveler.WorkSources.CAccessManager GetAccessManagerObject()
        {
            return cAccessManagerObj_;
        }

		public NmspTraveler.WorkSources.CCommonCurrentAccountInformation GetCommonCurrentAccountInformationObject()
		{
			return cCommonCurrentAccountInformationObj_;
		}


        private NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj_ = null;
        private NmspTraveler.WorkSources.CSettingsData cSettingsDataObj_ = null;
        private NmspTraveler.WorkSources.CAccessManager cAccessManagerObj_ = null;
		private NmspTraveler.WorkSources.CCommonCurrentAccountInformation cCommonCurrentAccountInformationObj_ = null;
    }
}
