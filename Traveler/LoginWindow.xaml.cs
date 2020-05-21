using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace NmspTraveler
{
    public partial class CLoginWindow : CBaseWindow
    {
        public CLoginWindow(ref NmspTraveler.CDataHub cDataHubObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;

            ClearAccounts();

            AccountsDataRequest();
        }

        private bool ClearAccounts()
        {
			laAccounts_.Clear();
            cbLogin.Items.Clear();

            return true;
        }

        private bool AddAccounts(ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> lcAccountsObj)
        {
            foreach (var vcAccountObj in lcAccountsObj)
            {
                if (vcAccountObj == null) continue;
                if (NmspTraveler.CString.IsStringEmpty(vcAccountObj.sUserName) == true) return false;

                cbLogin.Items.Add(vcAccountObj.sUserName);
            }  

            return true;
        }

        private bool AccountsDataRequest()
        {
			if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;

			if (ClearAccounts() == false) return false;

            string sErrorMessage = null;
            if (Traveler.WorkSources.CDatabaseExchanger.FullAccountRequest(
				ref vcPostgresqlWorkerObj, ref laAccounts_, ref sErrorMessage) == false) return false;
            
            if (AddAccounts(ref laAccounts_) == false) return false;

            return true;
        }

		void SetControlsByDefault()
		{
			cbLogin.Text = "Users";
			pbPassword.Password = "";
		}


        private void OnSettingsButton(object sender, RoutedEventArgs e)
        {
			int iNumberOfAccounts = laAccounts_.Count();
			
			if(iNumberOfAccounts != 0)
			{
				if (Login() == false)
				{
					NmspTraveler.CGui.ShowMessageBox(this, 
						"The username or password that you have entered is invalid!", 
						"Traveler - Login");
					return;
				}

				if (cDataHubObj_ == null) return;
				var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
				if (vcAccessManagerObj == null) return;
				if (vcAccessManagerObj.GetOperationPermissionByArea("10", "settings") == false)
				{
					NmspTraveler.CGui.ShowMessageBox(this, 
						"You don't have permission to perform this operation!", 
						"Traveler - Vehicle");
					return;
				}
			}

			NmspTraveler.CSettingsWindow cSettingsWindowObj = new NmspTraveler.CSettingsWindow(ref cDataHubObj_);
            if (cSettingsWindowObj == null) return;

            if (cSettingsWindowObj == null) return;
            cSettingsWindowObj.SettingsWindowLoaded();
            cSettingsWindowObj.Owner = this;
            cSettingsWindowObj.ShowDialog();

			ClearAccounts();
			SetControlsByDefault();

			if (cSettingsWindowObj.GetDialogResult() == false)
				goto gt;
			
			string sErrorMessage = null;
			if (cDataHubObj_.CreatePostgresqlWorkerObject(cDataHubObj_.GetSettingsDataObject(), ref sErrorMessage) == false)
				return;
			
			gt:
				AccountsDataRequest();
        }

        bool Login()
        {
            if (cDataHubObj_ == null) return false;
            var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
            if (vcAccessManagerObj == null) return false;

			var vCommonCurrentAccountInformationObj = cDataHubObj_.GetCommonCurrentAccountInformationObject();
			if (vCommonCurrentAccountInformationObj == null) return false;

            if (laAccounts_ == null) return false;
            if (laAccounts_.Count == 0) return false;

			string sHashedPassword = null;
			if (NmspTraveler.WorkSources.CEncryption.GetMd5Hash(pbPassword.Password, ref sHashedPassword) == false) return false;
            if (sHashedPassword == null) return false;
            if (sHashedPassword.Length == 0) return false;

			string sUserName = cbLogin.Text;
			if (NmspTraveler.CString.IsStringEmpty(sUserName) == true) return false;

			string sUserAccessLevel = null;
			string sUserRole = null;

            foreach (var vcAccountObj in laAccounts_)
            {
                if (vcAccountObj == null) continue;

				if (NmspTraveler.CString.IsStringEmpty(vcAccountObj.sUserName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(vcAccountObj.sPassword) == true) return false;

                if ((vcAccountObj.sUserName == sUserName) &&
                    (vcAccountObj.sPassword == sHashedPassword))
                {
					sUserAccessLevel = vcAccountObj.sUserAccessLevel;
					sUserRole = vcAccountObj.sUserRole;

					if (NmspTraveler.CString.IsStringEmpty(sUserAccessLevel) == true) return false;
					if (NmspTraveler.CString.IsStringEmpty(sUserRole) == true) return false;

					if (vcAccessManagerObj.LoadFreeAccessAreas(sUserAccessLevel,
						sUserRole) == false) return false;

					vCommonCurrentAccountInformationObj.SetUserName(sUserName);

                    return true;
                }
            }

            return false;
        }

        private void OnLoginButton(object sender, RoutedEventArgs e)
        {
            if (Login() == false)
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"The username or password that you have entered is invalid!", 
					"Traveler - Login");
                return;
            }

			SetDialogResult(true);
            this.Hide();     
        }


		List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> laAccounts_ = 
			new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData>();

        private NmspTraveler.CDataHub cDataHubObj_ = null;
    }
}
