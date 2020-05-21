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
using Newtonsoft.Json;

namespace NmspTraveler
{
    public partial class CSettingsWindow : NmspTraveler.CBaseWindow
    {
        public CSettingsWindow(ref NmspTraveler.CDataHub cDataHubObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;
        }

		private bool UsersListFilling(ref List<string> lsUserNames, ref string sErrorMessage)
		{
			try
			{
				if (lsUserNames == null) return false;
				if (lsUserNames.Count() == 0) return false;

				foreach (string sUserName in lsUserNames)
					cbUsers.Items.Add(sUserName);

				cbUsers.Text = "Users";
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private bool UsersListClearing(ref string sErrorMessage)
		{
			try
			{
				cbUsers.Items.Clear();
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool UsersListGetting(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> laAccounts, 
			ref List<string> lsUserNames, 
			ref string sErrorMessage)
		{
			try
			{
				if (laAccounts == null) return false;
				if (laAccounts.Count() <= 0) return false;

				if (lsUserNames != null) return false;
				lsUserNames = new List<string>();
				if (lsUserNames == null) return false;

				foreach (NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj in laAccounts)
				{
					if (cAccountObj == null) return false;
					
					if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserRole) == true) return false;
					if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserName) == true) return false;
					if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserAccessLevel) == true) return false;

					lsUserNames.Add(cAccountObj.sUserName);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private bool UsersListFilling(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> laAccounts, 
			ref string sErrorMessage)
        {
			List<string> lsUserNames = null;
			if(UsersListGetting(ref laAccounts, ref lsUserNames, ref sErrorMessage))
            if(UsersListFilling(ref lsUserNames, ref sErrorMessage) == false) return false;

            return true;
        }

        public bool SettingsWindowLoaded()
        {
			if (cDataHubObj_ == null) return false;
            var vcSettingsDataObj = cDataHubObj_.GetSettingsDataObject();
            if (vcSettingsDataObj == null) return false;

            tbDataBaseIp.Text = vcSettingsDataObj.GetIp();
            tbDataBasePort.Text = vcSettingsDataObj.GetPort();
            tbDataBaseName.Text = vcSettingsDataObj.GetDBName();
            pbDataBaseLogin.Password = vcSettingsDataObj.GetUser();
            pbDataBasePassword.Password = vcSettingsDataObj.GetPassword();

            CheckDataBaseConnectionButton(vcSettingsDataObj.GetIp(), vcSettingsDataObj.GetPort(),
                vcSettingsDataObj.GetDBName(), vcSettingsDataObj.GetUser(), vcSettingsDataObj.GetPassword());




			string sErrorMessage = null;
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);

            return true;
        }

        private void OnOkButton(object sender, RoutedEventArgs e)
        {
            if (cDataHubObj_ == null) return;
            var vcSettingsDataObj = cDataHubObj_.GetSettingsDataObject();
            if (vcSettingsDataObj == null) return;

            vcSettingsDataObj.SetIp(tbDataBaseIp.Text);
            vcSettingsDataObj.SetPort(tbDataBasePort.Text);
            vcSettingsDataObj.SetDBName(tbDataBaseName.Text);
            vcSettingsDataObj.SetUser(pbDataBaseLogin.Password);
            vcSettingsDataObj.SetPassword(pbDataBasePassword.Password);

            string sErrorMessage = null;
            vcSettingsDataObj.WriteSettingsToFile(@"Settings.txt", ref sErrorMessage);

			SetDialogResult(true);
            this.Close();
        }
  

        bool CheckDataBaseConnectionButton(string sIp, string sPort, string sDataBaseName, string sLogin, string sPassword)
        {
            ImageSource isConnectionStatusPicture = null;
            string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj = null;

            if (NmspTraveler.WorkSources.CPostgresqlWorker.Connect(sIp, sPort, sDataBaseName, sLogin,
                sPassword, ref cPostgresqlWorkerObj, ref sErrorMessage) == false)
            {
                isConnectionStatusPicture = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resource/Pictures/Forbidden.bmp"));
            }
            else
            {
                isConnectionStatusPicture = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resource/Pictures/GreenCheckMark.bmp"));
            }

            iConnectionStatusPicture.Source = isConnectionStatusPicture;

            return true;
        }

		private void OnDatabaseConnectionTabItemWindowLoaded(object sender, RoutedEventArgs e)
        {
                   
        }

		private bool AccountsListClearing(ref string sErrorMessage)
		{
			try
			{
				if (laAccounts_ == null) return false;
				if (laAccounts_.Count() == 0) return false;

				laAccounts_.Clear();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void OnAccountsTabItemWindowLoaded(object sender, RoutedEventArgs e)
        {
			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

			string sErrorMessage = null;
            if (Traveler.WorkSources.CDatabaseExchanger.AccountRequest(ref vcPostgresqlWorkerObj,
				ref laAccounts_, ref sErrorMessage) == false) return;	

            UsersListFilling(ref laAccounts_, ref sErrorMessage); 
        }

        private void OnCheckDataBaseConnectionButton(object sender, RoutedEventArgs e)
        {
            CheckDataBaseConnectionButton(tbDataBaseIp.Text, tbDataBasePort.Text, 
                tbDataBaseName.Text, pbDataBaseLogin.Password, pbDataBasePassword.Password);
        }

        private void OnCancelButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(false);
            this.Close();
        }

		private void OnAddAccountButton(object sender, RoutedEventArgs e)
        {
			NmspTraveler.CAddAccountWindow cAccountWindowObj = new NmspTraveler.CAddAccountWindow();
			if (cAccountWindowObj == null) return;
			cAccountWindowObj.Owner = this;
			cAccountWindowObj.ShowDialog();

			if (cAccountWindowObj.GetDialogResult() == false) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj = cAccountWindowObj.GetAccount();

			string sErrorMessage = null;
			if (AccountsListClearing(ref sErrorMessage) == false) return;
			if (UsersListClearing(ref sErrorMessage) == false) return;

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

			if (Traveler.WorkSources.CDatabaseExchanger.AccountAddingRequest(ref vcPostgresqlWorkerObj, 
				ref cAccountObj, ref sErrorMessage) == false) return;

            if (Traveler.WorkSources.CDatabaseExchanger.AccountRequest(ref vcPostgresqlWorkerObj,
				ref laAccounts_, ref sErrorMessage) == false) return;

			List<string> lsUserNames = null;
			if (UsersListGetting(ref laAccounts_, ref lsUserNames, ref sErrorMessage) == false) return;
            if(UsersListFilling(ref lsUserNames, ref sErrorMessage) == false) return;
        }

		private void OnEditAccountButton(object sender, RoutedEventArgs e)
        {
			if (cbUsers.SelectedIndex < 0)
			{ 
				NmspTraveler.CGui.ShowMessageBox(this, 
					"Choose a username first!", 
					"Traveler - Account Settings");
				return; 
			}
			
			string sOldUserName = cbUsers.Text;
			if (NmspTraveler.CString.IsStringEmpty(sOldUserName) == true) return;

            NmspTraveler.CEditAccountWindow cAccountWindowObj = new NmspTraveler.CEditAccountWindow(
				ref cDataHubObj_, sOldUserName);
			if (cAccountWindowObj == null) return;
			cAccountWindowObj.Owner = this;
			cAccountWindowObj.ShowDialog();

			if (cAccountWindowObj.GetDialogResult() == false) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj = cAccountWindowObj.GetAccount();

			string sErrorMessage = null;
			if (AccountsListClearing(ref sErrorMessage) == false) return;
			if (UsersListClearing(ref sErrorMessage) == false) return;

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

			if (Traveler.WorkSources.CDatabaseExchanger.AccountEditingRequest(ref vcPostgresqlWorkerObj, 
				ref cAccountObj, sOldUserName, ref sErrorMessage) == false) return;

            if (Traveler.WorkSources.CDatabaseExchanger.AccountRequest(ref vcPostgresqlWorkerObj,
				ref laAccounts_, ref sErrorMessage) == false) return;

			List<string> lsUserNames = null;
			if (UsersListGetting(ref laAccounts_, ref lsUserNames, ref sErrorMessage) == false) return;
            if(UsersListFilling(ref lsUserNames, ref sErrorMessage) == false) return;
        }

		private void OnDeleteAccountButton(object sender, RoutedEventArgs e)
        {
			if (cbUsers.SelectedIndex < 0)
			{ 
				NmspTraveler.CGui.ShowMessageBox(this, 
					"Choose a username first!", 
					"Traveler - Account Settings");
				return; 
			}

			string sSelectedUserName = cbUsers.Text;
			if (NmspTraveler.CString.IsStringEmpty(sSelectedUserName) == true) return;


			var vCommonCurrentAccountInformationObj = cDataHubObj_.GetCommonCurrentAccountInformationObject();
			if (vCommonCurrentAccountInformationObj == null) return;
			string sCurrentUserName = vCommonCurrentAccountInformationObj.GetUserName();
			if (NmspTraveler.CString.IsStringEmpty(sCurrentUserName) == true) return;

			if(sSelectedUserName == sCurrentUserName)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You can't remove this account from this account!", 
					"Traveler - Account Settings");
				return; 
			}

			if (NmspTraveler.CGui.ShowQuestionMessageBox(this, 
				"Do you really want to remove this account?", 
				"Traveler - Account Settings") == false)
                return;

			string sErrorMessage = null;
			if (AccountsListClearing(ref sErrorMessage) == false) return;
			if (UsersListClearing(ref sErrorMessage) == false) return;

			if (cDataHubObj_ == null) return;
			var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
			if (vcPostgresqlWorkerObj == null) return;
			
			if (Traveler.WorkSources.CDatabaseExchanger.AccountDeletionRequest(ref vcPostgresqlWorkerObj, 
				sSelectedUserName, ref sErrorMessage) == false) return;

            if (Traveler.WorkSources.CDatabaseExchanger.AccountRequest(ref vcPostgresqlWorkerObj,
				ref laAccounts_, ref sErrorMessage) == false) return;

			List<string> lsUserNames = null;
			if (UsersListGetting(ref laAccounts_, ref lsUserNames, ref sErrorMessage) == false) return;
            if(UsersListFilling(ref lsUserNames, ref sErrorMessage) == false) return;
        }


        private NmspTraveler.CDataHub cDataHubObj_;
		private List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> laAccounts_ = 
			new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData>();
    }
}
