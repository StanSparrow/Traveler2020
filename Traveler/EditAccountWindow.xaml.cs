using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NmspTraveler
{
	public partial class CEditAccountWindow : NmspTraveler.CBaseWindow
	{
		public CEditAccountWindow(ref NmspTraveler.CDataHub cDataHubObj, string sOldUserName)
		{
			InitializeComponent();

			cDataHubObj_ = cDataHubObj;

			sOldUserName_ = sOldUserName;
		}

		public NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData GetAccount()
		{
			return cAccountObj_;
		}

		bool InitControl(ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj, ref string sErrorMessage)
		{
			try
			{
				cbRoles.Items.Add("Admin");
				cbRoles.Items.Add("Manager");
				cbRoles.Items.Add("Cashier");
				cbRoles.Items.Add("Driver");

				if(cAccountObj.sUserRole == "Admin")
					cbRoles.SelectedIndex = 0;
				else if(cAccountObj.sUserRole == "Manager")
					cbRoles.SelectedIndex = 1;
				else if(cAccountObj.sUserRole == "Cashier")
					cbRoles.SelectedIndex = 2;
				else if(cAccountObj.sUserRole == "Driver")
					cbRoles.SelectedIndex = 3;

				tbUserName.Text = cAccountObj.sUserName;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			if (cAccountObj_ == null) return;
			cAccountObj_.sUserName = sOldUserName_;
			if (NmspTraveler.CString.IsStringEmpty(cAccountObj_.sUserName) == true) return;

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

			string sErrorMessage = null;
			if (Traveler.WorkSources.CDatabaseExchanger.AccountRequest(
				ref vcPostgresqlWorkerObj, ref cAccountObj_, ref sErrorMessage) == false) return;
		
			if (NmspTraveler.CString.IsStringEmpty(cAccountObj_.sPassword) == true) return;
			if (NmspTraveler.CString.IsStringEmpty(cAccountObj_.sUserAccessLevel) == true) return;
			if (NmspTraveler.CString.IsStringEmpty(cAccountObj_.sUserRole) == true) return;

			if (InitControl(ref cAccountObj_, ref sErrorMessage) == false) return;	
		}

		private bool GetAccessLevelByRole(string sUserRole, ref string sAccessLevel)
		{
			if (NmspTraveler.CString.IsStringEmpty(sUserRole) == true) return false;

			if(sUserRole == "Admin")
				sAccessLevel = "4";
			else if(sUserRole == "Manager")
				sAccessLevel = "3";
			else if(sUserRole == "Cashier")
				sAccessLevel = "2";
			else if(sUserRole == "Driver")
				sAccessLevel = "1";

			return true;
		}

		private void OnOkButton(object sender, RoutedEventArgs e)
        {
			string sOldUserPassword = pbOldPassword.Password;
			if (NmspTraveler.CString.IsStringEmpty(sOldUserPassword) == true) return;

			string sOldPasswordHash = null;
			if (NmspTraveler.WorkSources.CEncryption.GetMd5Hash(sOldUserPassword, ref sOldPasswordHash) == false)return;
			if (NmspTraveler.CString.IsStringEmpty(sOldPasswordHash) == true) return;

			string sOldUserPasswordHashFromDatabase = cAccountObj_.sPassword;
			if (NmspTraveler.CString.IsStringEmpty(sOldUserPasswordHashFromDatabase) == true) return;
					
			if (sOldPasswordHash != sOldUserPasswordHashFromDatabase)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"The old password is not correct! Retype it again!", 
					"Traveler - Account Editin");
				return;
			}



			string sNewUserRole = cbRoles.Text;
			if (NmspTraveler.CString.IsStringEmpty(sNewUserRole) == true) return;

			string sNewUserAccessLevel = null;
			if (GetAccessLevelByRole(sNewUserRole, ref sNewUserAccessLevel) == false) return;
			if (NmspTraveler.CString.IsStringEmpty(sNewUserAccessLevel) == true) return;

			string sNewUserName = tbUserName.Text;
			if (NmspTraveler.CString.IsStringEmpty(sNewUserName) == true) return;

			if(pbPassword.Password != pbPasswordConfirmation.Password)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"The password and confirmation password do not match! Try again!", 
					"Traveler - Account Editin");
				return;
			}

			string sNewUserPassword = pbPassword.Password;
			if (NmspTraveler.CString.IsStringEmpty(sNewUserPassword) == true) return;

			
			cAccountObj_.sUserRole = sNewUserRole;
			cAccountObj_.sUserName = sNewUserName;
			cAccountObj_.sUserAccessLevel = sNewUserAccessLevel;
			cAccountObj_.sPassword = sNewUserPassword;

            SetDialogResult(true);
            this.Close();        
        }

		private void OnCancelButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(false);
            this.Close();      
        }


		string sOldUserName_ = null;

		private NmspTraveler.CDataHub cDataHubObj_ = null;

		private NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj_ = 
			new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData();
	}
}
