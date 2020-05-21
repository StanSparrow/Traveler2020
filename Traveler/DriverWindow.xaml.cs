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
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace NmspTraveler
{
    public partial class CDriverWindow : NmspTraveler.CBaseWindow
    {
        public CDriverWindow(ref NmspTraveler.CDataHub cDataHubObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;
            
            DriversDataRequest();   
        }

        private void AddListViewItem(
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData> lCVehicleTripDataObj, 
			string sFirstName_fp, string sLastName_fp, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture cPictureObj_fp,
			string sGender_fp, string sBirthDate_fp, string sDateOfEmployment_fp, 
			string sLenthOfService_fp, string sRating_fp)
        {
            lCVehicleTripDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData()
            {
                sFirstName = sFirstName_fp,
                sLastName = sLastName_fp,
                cPictureObj = cPictureObj_fp,
                sGender = sGender_fp,
                sBirthDate = sBirthDate_fp,
                sDateOfEmployment = sDateOfEmployment_fp,
                sLenthOfService = sLenthOfService_fp,
                sRating = sRating_fp,
            });
        }

        bool GetExperienceByEmploymentDate(string sEmploymentDate_fp, ref string sExperience, ref string sErrorMessage)
        {
            try
            {
                DateTime dtServiceLenth = DateTime.MinValue;
                TimeSpan tsServiceLenth = TimeSpan.MinValue;
                
                DateTime dtEmploymentDate = DateTime.MinValue;    
                DateTime dtNow = DateTime.MinValue;
                DateTime.TryParse(DateTime.Now.ToShortDateString(), out dtNow);

                var vEmploymentDate = sEmploymentDate_fp.Split('.');
                if (vEmploymentDate == null) return false;
                if (vEmploymentDate.Count() < 3) return false;

                string sEmploymentDate = vEmploymentDate[0];
                sEmploymentDate += "/";
                sEmploymentDate += vEmploymentDate[1];
                sEmploymentDate += "/";
                sEmploymentDate += vEmploymentDate[2];
                sEmploymentDate += " 12:00:00 AM";
        
                dtEmploymentDate = DateTime.MinValue;
                DateTime.TryParse(sEmploymentDate, out dtEmploymentDate);

                tsServiceLenth = dtNow.Subtract(dtEmploymentDate);
            
                dtServiceLenth = (new DateTime(1, 1, 1) + tsServiceLenth);
                if (dtServiceLenth == null) return false;

                sExperience = (dtServiceLenth.Month - 1).ToString();
                sExperience += ".";
                sExperience += (dtServiceLenth.Day - 1).ToString();
                sExperience += ".";
                sExperience += (dtServiceLenth.Year - 1).ToString();
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private bool AddDrivers
			(ref List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow> lDataBaseRows,
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData> lCVehicleDriverDataObj)
        {
			if (lCVehicleDriverDataObj != null) return false;
            lCVehicleDriverDataObj = new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData>();
            if (lCVehicleDriverDataObj == null) return false;

            string sErrorMessage = null;
            string sServiceLenth = null;
            ImageSource isDriverPictureObj = null;
            foreach (NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow dbRow in lDataBaseRows)
            {
                if (dbRow.lDataBaseColumnValue_ == null) continue;
                if (dbRow.lDataBaseColumnValue_.Count < 0) continue;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture cPictureObj = 
				new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture();
				if (cPictureObj == null) return false;

				isDriverPictureObj = null;
				if (NmspTraveler.WorkSources.CPicture.DataBasePictureBytesToImageSource(
					dbRow.lDataBaseColumnValue_[3], ref isDriverPictureObj, ref sErrorMessage) == false)
				{
					cPictureObj.bDefaultPicture = true;
					isDriverPictureObj = GetNoPicture();
				}

				cPictureObj.isPicture = isDriverPictureObj;

                if (GetExperienceByEmploymentDate(dbRow.lDataBaseColumnValue_[6], ref sServiceLenth, ref sErrorMessage) == false) continue;

                AddListViewItem(ref lCVehicleDriverDataObj, dbRow.lDataBaseColumnValue_[1],
                    dbRow.lDataBaseColumnValue_[2], ref cPictureObj, dbRow.lDataBaseColumnValue_[4], dbRow.lDataBaseColumnValue_[5],
                    dbRow.lDataBaseColumnValue_[6], sServiceLenth, dbRow.lDataBaseColumnValue_[7]);
            } 

            return true;
        }

        private bool DriversDataRequest()
        {
            string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseDriversTableObj = null;
            string sCondition = "where cast(vehicle_driver_number as integer) < 160";

			if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;
			if (Traveler.WorkSources.CDatabaseExchanger.DriverRequest(ref vcPostgresqlWorkerObj, 
				ref dataBaseDriversTableObj, sCondition, ref sErrorMessage) == false) return false;

			List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData> lCVehicleDriverDataObj = null;
			if (AddDrivers(ref dataBaseDriversTableObj.lDataBaseRows_, ref lCVehicleDriverDataObj) == false) return false;
                
			lvDrivers.ItemsSource = lCVehicleDriverDataObj;

            return true;
        }

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			string sErrorMessage = null;
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);

			BackgroundWorkerObj_.WorkerSupportsCancellation = true;
			BackgroundWorkerObj_.DoWork += DriverSearchKeyUpBeginThread;
            BackgroundWorkerObj_.RunWorkerCompleted += DriverSearchKeyUpEndThread;
		}

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            clmDriversGridViewListViewGenderColumn.Width = 100;
            clmDriversGridViewListViewBirthDateColumn.Width = 150;
            clmDriversGridViewListViewDateOfEmploymentColumn.Width = 250;
            clmDriversGridViewListViewLenthOfServiceColumn.Width = 200;
            clmDriversGridViewListViewRatingColumn.Width = 100;
			clmDriversGridViewListViewPictureColumn.Width = 136;

            double dblFirstNameLastNamePictureWidths = lvDrivers.ActualWidth - 
            (clmDriversGridViewListViewGenderColumn.ActualWidth +
            clmDriversGridViewListViewBirthDateColumn.ActualWidth +
            clmDriversGridViewListViewDateOfEmploymentColumn.ActualWidth +
            clmDriversGridViewListViewLenthOfServiceColumn.ActualWidth +
            clmDriversGridViewListViewRatingColumn.ActualWidth + 
			clmDriversGridViewListViewPictureColumn.ActualWidth);

            double dblBigColumnWidth = dblFirstNameLastNamePictureWidths / 2;

            clmDriversGridViewListViewFirstNameColumn.Width = dblBigColumnWidth;
            clmDriversGridViewListViewLastNameColumn.Width = dblBigColumnWidth;


			lccTextBoxesObj_.Clear();
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemFirstName", ctrlObject = null });
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemLastName", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemBirthDate", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemDateOfEmployment", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemGender", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemRating", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbDriversListViewItemLenthOfService", ctrlObject = null});
        }

        private void OnListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
			string sErrorMessage = null;
			SetListViewItemColor(ref sErrorMessage);
        }

		private void OnListViewMouseMove(object sender, MouseEventArgs e)
        {
            string sErrorMessage = null;
			SetListViewItemColor(ref sErrorMessage);
        }


		bool SetListViewItemColor(ref string sErrorMessage)
		{
			try
			{
				ListView lvListViewObj = lvDrivers as ListView;

			if (lvListViewObj == null) return false;

			if (SetListViewItemColor(ref lvListViewObj, ref lccTextBoxesObj_, 
				"imgDriverListViewItemPicture", 
				"brdDriverListViewItemPicture", ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}
        

        public string GetFirstName()
        {
            return sSelectedDriverFirstName_;
        }

        public string GetLastName()
        {
            return sSelectedDriverLastName_;
        }

        public string GetBirthDate()
        {
            return sSelectedDriverBirthDate_;
        }

        public string GetGender()
        {
            return sSelectedDriverGender_;
        }

        public string GetLenthOfService()
        {
            return sSelectedDriverLenthOfService_;
        }

        public string GetRating()
        {
            return sSelectedDriverRating_;
        }

        public string GetExperience()
        {
            return sSelectedDriverExperience_;
        }

        public ImageSource GetPicture()
        {
            return isSelectedDriverPicture_;
        }

        private void DriverSearchKeyUp(object sender, KeyEventArgs e)
        {
			if (BackgroundWorkerObj_ == null) return;

            string sInputedText = tbDriverSearch.Text;

			BackgroundWorkerObj_.CancelAsync();

			string sErrorMessage = null;
			while(BackgroundWorkerObj_.IsBusy == true)
			{
				Thread.Sleep(50);
				DoEvents(ref sErrorMessage); 
			}

            BackgroundWorkerObj_.RunWorkerAsync(sInputedText);
        }


		private void DriverSearchKeyUpBeginThread(object sender, DoWorkEventArgs e)
		{
			string sInputedText = (string)e.Argument;

			string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseDriversTableObj = null;
            string sCondition = "where ((position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_first_name) != 0) or ";
            
            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_last_name) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_gender) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_birth_date) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_employment_date) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_driver_rating) != 0)) and ";
            
            sCondition += "(cast(vehicle_driver_number as integer) < 160)";

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;
			
			List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData> lCVehicleDriverDataObj = null;
			if ((Traveler.WorkSources.CDatabaseExchanger.DriverRequest(ref vcPostgresqlWorkerObj, 
				ref dataBaseDriversTableObj, sCondition, ref sErrorMessage) == false) || 
				(AddDrivers(ref dataBaseDriversTableObj.lDataBaseRows_, ref lCVehicleDriverDataObj) == false))
			{
				e.Cancel = true;
			}
			else
			{
				if (BackgroundWorkerObj_.CancellationPending == true)
					e.Cancel = true;
				else
				{
					e.Cancel = false;
					e.Result = lCVehicleDriverDataObj;
				}
			}
		}

		private void DriverSearchKeyUpEndThread(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
                
            }
            else
            {
				List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData> lCVehicleDriverDataObj = 
					(List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDriverData>)e.Result;
				if (lCVehicleDriverDataObj == null) return;

				lvDrivers.ItemsSource = lCVehicleDriverDataObj;
            }		
		}


        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            ListView lvListViewObj = lvDrivers as ListView;
            if (lvListViewObj == null) return;

            if ((lvListViewObj.SelectedIndex < 0) || (lvListViewObj.SelectedIndex >= lvListViewObj.Items.Count)) return;



			NmspTraveler.CGui.GetElementListObj(ref lvListViewObj, ref lccTextBoxesObj_, lvListViewObj.SelectedIndex);

			foreach (NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lccTextBoxesObj_)
			{
				if (cControlObj == null) continue;

				if(cControlObj.sControlName == "tbDriversListViewItemFirstName")
					sSelectedDriverFirstName_ = ((TextBox)cControlObj.ctrlObject).Text;	
				else if(cControlObj.sControlName == "tbDriversListViewItemLastName")
					sSelectedDriverLastName_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbDriversListViewItemBirthDate")
					sSelectedDriverBirthDate_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbDriversListViewItemDateOfEmployment")
					sSelectedDriverGender_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbDriversListViewItemGender")
					sSelectedDriverLenthOfService_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbDriversListViewItemRating")
					sSelectedDriverRating_ = ((TextBox)cControlObj.ctrlObject).Text;	
				else if(cControlObj.sControlName == "tbDriversListViewItemLenthOfService")
					sSelectedDriverExperience_ = ((TextBox)cControlObj.ctrlObject).Text;
			}

            isSelectedDriverPicture_ = NmspTraveler.CGui.GetImageElement(ref lvListViewObj, 
				"imgDriverListViewItemPicture", lvListViewObj.SelectedIndex).Source;
            if (isSelectedDriverPicture_ == null) return;

            if (Keyboard.IsKeyDown(Key.Enter) == true)
            {
				if (cDataHubObj_ == null) return;
				var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
				if (vcAccessManagerObj == null) return;
				if (vcAccessManagerObj.GetOperationPermissionByArea("4", 
					"appointment of one of the drivers for the trip") == false)
				{
					NmspTraveler.CGui.ShowMessageBox(this, 
						"You don't have permission to perform this operation!", 
						"Traveler - Driver");
					return;
				}

                this.DialogResult = true;
                this.Close();
            }
        }

       


		private string sSelectedDriverFirstName_ = null;
        private string sSelectedDriverLastName_ = null;
        private string sSelectedDriverBirthDate_ = null;
        private string sSelectedDriverLenthOfService_ = null;
        private string sSelectedDriverGender_ = null;
        private string sSelectedDriverRating_ = null;
        private string sSelectedDriverExperience_ = null;
        private ImageSource isSelectedDriverPicture_ = null;

		private BackgroundWorker BackgroundWorkerObj_ = new BackgroundWorker();

        private NmspTraveler.CDataHub cDataHubObj_ = null;

		List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj_ = 
			new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox>();
    }
}
