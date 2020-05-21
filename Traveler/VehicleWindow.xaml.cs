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
    public partial class CVehicleWindow : NmspTraveler.CBaseWindow
    {
        public CVehicleWindow(ref NmspTraveler.CDataHub cDataHubObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;

            VehiclesDataRequest();
        }

        private void AddListViewItem(
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleTripDataObj, 
			string sVehicleType_fp, string sVehicleModel_fp, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture cPictureObj_fp,
			string sVehicleLicencePlate_fp, string sVehicleManufactureYear_fp, string sVehicleCountryOfOrigin_fp,
            string sVehicleTechnicalState_fp, string sVehicleRating_fp, string sPassengerCapacity_fp)
        {
            lCVehicleTripDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData()
            {
                sVehicleType = sVehicleType_fp,
                sVehicleModel = sVehicleModel_fp,
                cPictureObj = cPictureObj_fp,
                sVehicleLicencePlate = sVehicleLicencePlate_fp,
                sVehicleManufactureYear = sVehicleManufactureYear_fp,
                sVehicleCountryOfOrigin = sVehicleCountryOfOrigin_fp,
                sVehicleTechnicalState = sVehicleTechnicalState_fp,
                sVehicleRating = sVehicleRating_fp,
				sPassengerCapacity = sPassengerCapacity_fp
            });
        }

        private bool VehiclesDataRequest()
        {
            string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseVehiclesTableObj = null;
            string sCondition = "where cast(vehicle_number as integer) < 160";

			if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;
			if (Traveler.WorkSources.CDatabaseExchanger.VehicleRequest(ref vcPostgresqlWorkerObj,
				ref dataBaseVehiclesTableObj, sCondition, ref sErrorMessage) == false) return false;
			List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleDataObj = null;
            if (AddVehicles(ref dataBaseVehiclesTableObj.lDataBaseRows_, ref lCVehicleDataObj) == false) return false;
			SetVehicleList(ref lCVehicleDataObj);

            return true;
        }

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			string sErrorMessage = null;
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);
		
            BackgroundWorkerObj_.WorkerSupportsCancellation = true;
            BackgroundWorkerObj_.DoWork += VehicleSearchKeyUpBeginThread;
            BackgroundWorkerObj_.RunWorkerCompleted += VehicleSearchKeyUpEndThread;
		}

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
			clmVehiclesGridViewListViewLicensePlateColumn.Width = 160;
            clmVehiclesGridViewListViewYearOfManufactureColumn.Width = 230;
            clmVehiclesGridViewListViewTechnicalStateColumn.Width = 180;
            clmVehiclesGridViewListViewRatingColumn.Width = 100;
			clmVehiclesGridViewListViewPictureColumn.Width = 136;
			clmVehiclesGridViewListViewPassengerCapacityColumn.Width = 220;

            double dblVehicleTypeVehicleModelPictureLicencePlateWidths = lvVehicles.ActualWidth -
            (clmVehiclesGridViewListViewYearOfManufactureColumn.ActualWidth +
            clmVehiclesGridViewListViewTechnicalStateColumn.ActualWidth +
            clmVehiclesGridViewListViewRatingColumn.ActualWidth + 
			clmVehiclesGridViewListViewLicensePlateColumn.ActualWidth + 
			clmVehiclesGridViewListViewPictureColumn.ActualWidth + 
			clmVehiclesGridViewListViewPassengerCapacityColumn.ActualWidth);

            double dblBigColumnWidth = dblVehicleTypeVehicleModelPictureLicencePlateWidths / 3;

            clmVehiclesGridViewListViewVehicleTypeColumn.Width = dblBigColumnWidth;
            clmVehiclesGridViewListViewsVehicleModelColumn.Width = dblBigColumnWidth;
            clmVehiclesGridViewListViewCountryOfOriginColumn.Width = dblBigColumnWidth;



			lccTextBoxesObj_.Clear();
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemVehicleType", ctrlObject = null });
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemsVehicleModel", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemLicensePlate", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemYearOfManufacture", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemCountryOfOrigin", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemTechnicalState", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemRating", ctrlObject = null});
			
			lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
				sControlName = "tbVehiclesListViewItemPassengerCapacity", ctrlObject = null});
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
				ListView lvListViewObj = lvVehicles as ListView;

			if (lvListViewObj == null) return false;

			if (SetListViewItemColor(ref lvListViewObj, ref lccTextBoxesObj_, 
				"iVehiclesListViewItemPicture", 
				"iVehiclesListViewItemPictureBorder", ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}
        

        public string GetVehicleType()
        {
            return sSelectedVehicleType_;
        }

        public string GetVehicleModel()
        {
            return sSelectedVehicleModel_;
        }

        public ImageSource GetVehiclePicture()
        {
            return isSelectedVehiclePicture_;
        }

        public string GetVehicleLicensePlate()
        {
            return sSelectedVehicleLicensePlate_;
        }

        public string GetVehicleYearOfManufacture()
        {
            return sSelectedVehicleYearOfManufacture_;
        }

        public string GetVehicleCountryOfOrigin()
        {
            return sSelectedVehicleCountryOfOrigin_;
        }

        public string GetVehicleTechnicalState()
        {
            return sSelectedVehicleTechnicalState_;
        }

        public string GetVehicleRating()
        {
            return sSelectedVehicleRating_;
        }

		public string GetVehiclePassengerCapacity()
		{
			return sSelectedVehiclePassengerCapacity_;
		}

        private void VehicleSearchKeyUp(object sender, KeyEventArgs e)
        {
			if (BackgroundWorkerObj_ == null) return;

            string sInputedText = tbVehicleSearch.Text;

			BackgroundWorkerObj_.CancelAsync();

			string sErrorMessage = null;
			while(BackgroundWorkerObj_.IsBusy == true)
			{
				Thread.Sleep(50);
				DoEvents(ref sErrorMessage); 
			}

            BackgroundWorkerObj_.RunWorkerAsync(sInputedText);
        }



		private void VehicleSearchKeyUpBeginThread(object sender, DoWorkEventArgs e)
		{
			string sInputedText = (string)e.Argument;

			string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseVehiclesTableObj = null;
            string sCondition = "where ((position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_type) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_model) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_licence_plate) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_manufacture_year) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_manufacture_country) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_technical_state) != 0) or ";

			sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_rating) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_number_of_seats) != 0)) and ";

            sCondition += "(cast(vehicle_number as integer) < 160)";


			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;
			
			List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleDataObj = null;
			if ((Traveler.WorkSources.CDatabaseExchanger.VehicleRequest(ref vcPostgresqlWorkerObj,
				ref dataBaseVehiclesTableObj, sCondition, ref sErrorMessage) == false) || 
				(AddVehicles(ref dataBaseVehiclesTableObj.lDataBaseRows_, ref lCVehicleDataObj) == false))
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
					e.Result = lCVehicleDataObj;
				}
			}
		}

		private bool SetVehicleList(ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleDataObj)
		{
			lvVehicles.ItemsSource = lCVehicleDataObj;

			return true;
		}

		private void ClearVehicleList()
		{
			lvVehicles.ItemsSource = new List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData>();
		}

		private void VehicleSearchKeyUpEndThread(object sender, RunWorkerCompletedEventArgs e)
		{
			ClearVehicleList();

			if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
                
            }
            else
            {
				List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleDataObj = 
					(List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData>)e.Result;
				if (lCVehicleDataObj == null) return;

				SetVehicleList(ref lCVehicleDataObj);
            }		
		}

        private bool AddVehicles(
			ref List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow> lDataBaseRows,
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData> lCVehicleDataObj)
        {
			if (lCVehicleDataObj != null) return false;
            lCVehicleDataObj = new List<NmspTraveler.WorkSources.NmspWorkingStructures.CTripVehicleData>();
            if (lCVehicleDataObj == null) return false;

            string sErrorMessage = null;
            ImageSource isVehiclePictureObj = null;

            foreach (NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow dbRow in lDataBaseRows)
            {
                if (dbRow.lDataBaseColumnValue_ == null) continue;
                if (dbRow.lDataBaseColumnValue_.Count < 0) continue;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture cPictureObj = 
				new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture();
				if (cPictureObj == null) return false;

				isVehiclePictureObj = null;
				if(NmspTraveler.WorkSources.CPicture.DataBasePictureBytesToImageSource(
					dbRow.lDataBaseColumnValue_[3], ref isVehiclePictureObj, ref sErrorMessage) == false)
				{
					cPictureObj.bDefaultPicture = true;
					isVehiclePictureObj = GetNoPicture();
				}

				cPictureObj.isPicture = isVehiclePictureObj;
				
                AddListViewItem(ref lCVehicleDataObj, dbRow.lDataBaseColumnValue_[1], dbRow.lDataBaseColumnValue_[2],
                    ref cPictureObj, dbRow.lDataBaseColumnValue_[4], dbRow.lDataBaseColumnValue_[5], 
                    dbRow.lDataBaseColumnValue_[6], dbRow.lDataBaseColumnValue_[7], dbRow.lDataBaseColumnValue_[8],
					dbRow.lDataBaseColumnValue_[9]);		
			}

            return true;
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            ListView lvListViewObj = lvVehicles as ListView;
            if (lvListViewObj == null) return;

            if ((lvListViewObj.SelectedIndex < 0) || (lvListViewObj.SelectedIndex >= lvListViewObj.Items.Count)) return;

			NmspTraveler.CGui.GetElementListObj(ref lvListViewObj, ref lccTextBoxesObj_, lvListViewObj.SelectedIndex);

			foreach (NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lccTextBoxesObj_)
			{
				if (cControlObj == null) continue;

				if(cControlObj.sControlName == "tbVehiclesListViewItemVehicleType")
					sSelectedVehicleType_ = ((TextBox)cControlObj.ctrlObject).Text;	
				else if(cControlObj.sControlName == "tbVehiclesListViewItemsVehicleModel")
					sSelectedVehicleModel_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemLicensePlate")
					sSelectedVehicleLicensePlate_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemYearOfManufacture")
					sSelectedVehicleYearOfManufacture_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemCountryOfOrigin")
					sSelectedVehicleCountryOfOrigin_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemTechnicalState")
					sSelectedVehicleTechnicalState_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemRating")
					sSelectedVehicleRating_ = ((TextBox)cControlObj.ctrlObject).Text;
				else if(cControlObj.sControlName == "tbVehiclesListViewItemPassengerCapacity")
					sSelectedVehiclePassengerCapacity_ = ((TextBox)cControlObj.ctrlObject).Text;
			}

            isSelectedVehiclePicture_ = NmspTraveler.CGui.GetImageElement(ref lvListViewObj, 
				"iVehiclesListViewItemPicture", lvListViewObj.SelectedIndex).Source;

            if (isSelectedVehiclePicture_ == null) return;
            
            if (Keyboard.IsKeyDown(Key.Enter) == true)
            {
				if (cDataHubObj_ == null) return;
				var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
				if (vcAccessManagerObj == null) return;
				if (vcAccessManagerObj.GetOperationPermissionByArea("5", 
					"appointment of one of the vehicle for the trip") == false)
				{
					NmspTraveler.CGui.ShowMessageBox(this, 
						"You don't have permission to perform this operation!", 
						"Traveler - Vehicle");
					return;
				}

                this.DialogResult = true;
                this.Close();
            }
        }




		private string sSelectedVehicleType_ = null;
        private string sSelectedVehicleModel_ = null;
        private ImageSource isSelectedVehiclePicture_ = null;
        private string sSelectedVehicleLicensePlate_ = null;
        private string sSelectedVehicleYearOfManufacture_ = null;
        private string sSelectedVehicleCountryOfOrigin_ = null;
        private string sSelectedVehicleTechnicalState_ = null;
        private string sSelectedVehicleRating_ = null;
		private string sSelectedVehiclePassengerCapacity_ = null;


		private BackgroundWorker BackgroundWorkerObj_ = new BackgroundWorker();

        private NmspTraveler.CDataHub cDataHubObj_ = null;

		List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj_ = 
			new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox>();
    }
}
