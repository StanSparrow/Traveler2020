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
	public partial class CAddAccountWindow : NmspTraveler.CBaseWindow
	{
		public CAddAccountWindow()
		{
			InitializeComponent();
		}

		public NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData GetAccount()
		{
			return cAccountObj_;
		}

		bool InitControl(ref string sErrorMessage)
		{
			try
			{
				cbRoles.Items.Add("Admin");
				cbRoles.Items.Add("Manager");
				cbRoles.Items.Add("Cashier");
				cbRoles.Items.Add("Driver");

				cbRoles.SelectedIndex = 0;
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
			string sErrorMessage = null;
			if (InitControl(ref sErrorMessage) == false) return;	
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
			cAccountObj_.sUserRole = cbRoles.Text;

			if (GetAccessLevelByRole(cAccountObj_.sUserRole, ref cAccountObj_.sUserAccessLevel) == false) return;

			cAccountObj_.sUserName = tbUserName.Text;

			if(pbPassword.Password != pbPasswordConfirmation.Password)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"The password and confirmation password do not match! Try again!", 
					"Traveler - Account Adding");
				return;
			}

			cAccountObj_.sPassword = pbPassword.Password;

            SetDialogResult(true);
            this.Close();        
        }

		private void OnCancelButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(false);
            this.Close();      
        }


		NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj_ = 
			new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData();
	}
}
