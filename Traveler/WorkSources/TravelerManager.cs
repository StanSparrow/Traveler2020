using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler.WorkSources
{
    public class CTravelerManager
    {
        public bool Init(ref string sErrorMessage)
        {
            if (cDataHubObj_ != null) return false;
            cDataHubObj_ = new NmspTraveler.CDataHub();
            if (cDataHubObj_ == null) return false;

			bool bRetVal = false;
            bRetVal = cDataHubObj_.CreateSettingsDataObject(ref sErrorMessage);
            bRetVal = cDataHubObj_.CreatePostgresqlWorkerObject(cDataHubObj_.GetSettingsDataObject(), ref sErrorMessage);
            bRetVal = cDataHubObj_.CreateAccessObject(ref sErrorMessage);
			bRetVal = cDataHubObj_.CreateCommonCurrentAccountInformationObject();

            return true;
        }

        public bool Run()
        { 
            while(true)
            {
                NmspTraveler.CLoginWindow cLoginWindowObj = new NmspTraveler.CLoginWindow(ref cDataHubObj_);
                if (cLoginWindowObj == null) return false;
                cLoginWindowObj.ShowDialog();

                if (cLoginWindowObj.GetDialogResult() == false)
                {
                    System.Windows.Application.Current.Shutdown();
                    return true;
                }

                NmspTraveler.CRouteWindow cRouteWindowObj = new NmspTraveler.CRouteWindow(ref cDataHubObj_);
                if (cRouteWindowObj == null) return false;
                cRouteWindowObj.ShowDialog();
            }
        }

        private NmspTraveler.CDataHub cDataHubObj_ = null;
    }
}
