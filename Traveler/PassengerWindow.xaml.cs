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
using System.ComponentModel;
using System.Threading;


namespace NmspTraveler
{
	public partial class CPassengerWindow : NmspTraveler.CBaseWindow
	{
		public CPassengerWindow(
			ref NmspTraveler.CDataHub cDataHubObj, 
			NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo cTripInfoObj)
		{
			InitializeComponent();

			cDataHubObj_ = cDataHubObj;
			cTripInfoObj_ = cTripInfoObj;
			
			string sErrorMessage = null;
			PassengersDataRequest(ref sErrorMessage);
		}

		private bool PassengersDataRequest(ref string sErrorMessage)
        {    
			try
			{
				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBasePassengerTableObj = null;
            
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;
				if (Traveler.WorkSources.CDatabaseExchanger.PassengerRequest(ref vcPostgresqlWorkerObj,
					ref dataBasePassengerTableObj, "", ref sErrorMessage) == false) return false;
				
				List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCPassengerDataObj = null;
				if (AddPassengers(ref dataBasePassengerTableObj.lDataBaseRows_, 
					ref lCPassengerDataObj, ref sErrorMessage) == false) return false;
				SetPassengerList(ref lCPassengerDataObj, ref sErrorMessage);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		private bool AddPassengers(
			ref List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow> lDataBaseRows,
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lcPassengerDataObj,
			ref string sErrorMessage)
        {
			try
			{
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				if (lcPassengerDataObj != null) return false;
				lcPassengerDataObj = new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData>();
				if (lcPassengerDataObj == null) return false;

				ImageSource isPassengerPictureObj = null;

				foreach (NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow dbRow in lDataBaseRows)
				{
					if (dbRow.lDataBaseColumnValue_ == null) continue;
					if (dbRow.lDataBaseColumnValue_.Count < 0) continue;

					NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture cPictureObj = 
					new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CPicture();
					if (cPictureObj == null) return false;

					isPassengerPictureObj = null;
					if (NmspTraveler.WorkSources.CPicture.DataBasePictureBytesToImageSource(
						dbRow.lDataBaseColumnValue_[4], ref isPassengerPictureObj, ref sErrorMessage) == false)
					{
						cPictureObj.bDefaultPicture = true;
						isPassengerPictureObj = GetNoPicture();
					}	

					cPictureObj.isPicture = isPassengerPictureObj;



					NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData cTripDatabaseDataObj = 
						new NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData();
					if (cTripDatabaseDataObj == null) return false;

					cTripDatabaseDataObj.lsTripNumber.Add(dbRow.lDataBaseColumnValue_[6]);

					if (Traveler.WorkSources.CDatabaseExchanger.TripRequest(ref vcPostgresqlWorkerObj,
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) return false;



					NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData cRouteDatabaseDataObj = 
						new NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData();
					if (cRouteDatabaseDataObj == null) return false;

					cRouteDatabaseDataObj.sRouteNumber = cTripDatabaseDataObj.sRouteNumber;

					if (Traveler.WorkSources.CDatabaseExchanger.RouteRequest(ref vcPostgresqlWorkerObj,
					ref cRouteDatabaseDataObj, ref sErrorMessage) == false) return false;



					NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData cTicketDatabaseDataObj = 
						new NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData();
					if (cTicketDatabaseDataObj == null) return false;


					cTicketDatabaseDataObj.sTicketNumber = dbRow.lDataBaseColumnValue_[0];
					cTicketDatabaseDataObj.sPassengerFirstname = dbRow.lDataBaseColumnValue_[1];
					cTicketDatabaseDataObj.sPassengerLastname = dbRow.lDataBaseColumnValue_[2];
					cTicketDatabaseDataObj.sPassengerDriverLicenseNumber = dbRow.lDataBaseColumnValue_[3];
					cTicketDatabaseDataObj.cPictureObj = cPictureObj;
					cTicketDatabaseDataObj.sPassengerSeatNumber = dbRow.lDataBaseColumnValue_[5];
					cTicketDatabaseDataObj.sPassengerDepartureTime = cTripDatabaseDataObj.sDepartureTime;
					cTicketDatabaseDataObj.sPassengerRoute = cRouteDatabaseDataObj.sRouteName;
					cTicketDatabaseDataObj.sTicketSaleDate = dbRow.lDataBaseColumnValue_[7];
					cTicketDatabaseDataObj.sDepartureDate = dbRow.lDataBaseColumnValue_[8];
				
					AddListViewItem(ref lcPassengerDataObj, ref cTicketDatabaseDataObj, ref sErrorMessage);	
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		private bool AddListViewItem(
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCVehicleTripDataObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData cTicketDatabaseDataObj,
			ref string sErrorMessage)
        {
			try 
			{
				lCVehicleTripDataObj.Add(cTicketDatabaseDataObj);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
        }

		private bool SetPassengerList(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lcTripPassengerDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				lvPassengers.ItemsSource = lcTripPassengerDataObj;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private bool ClearPassengerList(ref string sErrorMessage)
		{
			try
			{
				lvPassengers.ItemsSource = new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData>();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void PassengerFilterWorkingThreadBegin(object sender, DoWorkEventArgs e)
		{
			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch 
				cPassengerFilterSearchObj = (NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch)e.Argument;

			if (cPassengerFilterSearchObj == null)
			{
				e.Cancel = true;
				return;
			}

			if (cDataHubObj_ == null)
			{
				e.Cancel = true;
				return;
			}

            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
			if (vcPostgresqlWorkerObj == null)
			{
				e.Cancel = true;
				return;
			}

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData cTripDatabaseDataObj = 
						new NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData();

			if (cTripDatabaseDataObj == null)
			{
				e.Cancel = true;
				return;
			}

			cTripDatabaseDataObj.sDepartureTime = cPassengerFilterSearchObj.sDepartureTime;
			cTripDatabaseDataObj.sRouteNumber = cPassengerFilterSearchObj.sRouteNumber;

			string sErrorMessage = null;
			if (Traveler.WorkSources.CDatabaseExchanger.TripRequest1(ref vcPostgresqlWorkerObj,
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) 
			{
				e.Cancel = true;
				return;
			}
	
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
            
			string sCondition = "where departure_date = ";
			sCondition += "'";
            sCondition += cPassengerFilterSearchObj.sDepartureDate;
			sCondition += "'";
			sCondition += " and (";

			for(int i = 0; i < cTripDatabaseDataObj.lsTripNumber.Count; i++)
			{
				if(i != 0)sCondition += " or ";
				sCondition += "vehicle_trip_number = ";
				sCondition += "'";
				sCondition += cTripDatabaseDataObj.lsTripNumber[i];
				sCondition += "'";
			}

			sCondition += ")";
			sCondition += ";";
			
			List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCTripPassengerDataObj = null;
			if ((Traveler.WorkSources.CDatabaseExchanger.PassengerRequest(ref vcPostgresqlWorkerObj, 
				ref dataBaseTableObj, sCondition, ref sErrorMessage) == false) || 
				(AddPassengers(ref dataBaseTableObj.lDataBaseRows_, ref lCTripPassengerDataObj, ref sErrorMessage) == false))
			{
				e.Cancel = true;
			}
			else
			{
				if(BackgroundWorkerObj_.CancellationPending == true)
					e.Cancel = true;		
				else
				{
					e.Cancel = false;
					e.Result = lCTripPassengerDataObj;
				}		
			}
		}

		private void PassengerFilterWorkingThreadEnd(object sender, RunWorkerCompletedEventArgs e)
		{
			string sErrorMessage = null;
			if (e.Error != null)
            {
                ClearPassengerList(ref sErrorMessage);
            }
            else if (e.Cancelled)
            {
                ClearPassengerList(ref sErrorMessage);
            }
            else
            {
				List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCTripPassengerDataObj = 
					(List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData>)e.Result;
				if (lCTripPassengerDataObj == null) return;
	
				SetPassengerList(ref lCTripPassengerDataObj, ref sErrorMessage);

				DoEvents(ref sErrorMessage); 
				
				SetListViewItemColor(ref sErrorMessage);
            }		
		}

		private void PassengerSearchKeyUpBeginThread(object sender, DoWorkEventArgs e)
		{
			string sInputedText = (string)e.Argument;

			string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseDriversTableObj = null;
            string sCondition = "where ((position('";
            sCondition += sInputedText;
            sCondition += "' in ticket_number) != 0) or ";
            
            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in passenger_first_name) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in passenger_last_name) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in passenger_driver_license_number) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in passenger_seat_number) != 0) or ";

			sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in vehicle_trip_number) != 0) or ";

			sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in ticket_sale_date) != 0) or ";

            sCondition += "(position('";
            sCondition += sInputedText;
            sCondition += "' in departure_date) != 0))";

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;
			
			List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCTripPassengerDataObj = null;
			if ((Traveler.WorkSources.CDatabaseExchanger.PassengerRequest(ref vcPostgresqlWorkerObj, 
				ref dataBaseDriversTableObj, sCondition, ref sErrorMessage) == false) || 
				(AddPassengers(ref dataBaseDriversTableObj.lDataBaseRows_, ref lCTripPassengerDataObj, ref sErrorMessage) == false))
			{
				e.Cancel = true;
			}
			else
			{
				if(BackgroundWorkerObj_.CancellationPending == true)
					e.Cancel = true;		
				else
				{
					e.Cancel = false;
					e.Result = lCTripPassengerDataObj;
				}		
			}
		}

		private void PassengerSearchKeyUpEndThread(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
                
            }
            else
            {
				List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData> lCTripPassengerDataObj = 
					(List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTicketDatabaseData>)e.Result;
				if (lCTripPassengerDataObj == null) return;
	

				string sErrorMessage = null;
				SetPassengerList(ref lCTripPassengerDataObj, ref sErrorMessage);

				DoEvents(ref sErrorMessage); 
				
				SetListViewItemColor(ref sErrorMessage);
            }		
		}

		bool SetControlNameList(ref string sErrorMessage)
		{
			try
			{
				lccTextBoxesObj_.Clear();
				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbPassengerListViewItemTicketNumber", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerTicketNumber, 
					szMaxTextSize = new Size(){ Width = 170 } });

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbPassengerListViewItemFirstname", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerFirstname,
					szMaxTextSize = new Size(){ Width = 135 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbPassengerListViewItemsLastname", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerLastname,
					szMaxTextSize = new Size(){ Width = 135 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbPassengerListViewItemDriverLicenseNumber", ctrlObject = null,
					gvcGridViewColumn = clmGridViewListViewPassengerDriverLicenseNumber, 
					szMaxTextSize = new Size(){ Width = 190 }});
			
				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbListViewItemPassengerSeatNumber", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerSeatNumber,
					szMaxTextSize = new Size(){ Width = 160 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbListViewItemPassengerDepartureTime", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerDepartureTime, 
					szMaxTextSize = new Size(){ Width = 180 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbListViewItemPassengerRoute", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerRoute, 
					szMaxTextSize = new Size(){ Width = 105 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbListViewItemPassengerTicketSaleDate", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerTicketSaleDate, 
					szMaxTextSize = new Size(){ Width = 194 }});

				lccTextBoxesObj_.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox(){ 
					sControlName = "tbListViewItemPassengerDepartureDate", ctrlObject = null, 
					gvcGridViewColumn = clmGridViewListViewPassengerDepartureDate, 
					szMaxTextSize = new Size(){ Width = 180 }});
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool GetGridViewItemTextBoxCommonWidth(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj, 
			ref string sCommonWidth, ref string sErrorMessage)
		{
			try
			{
				int iCommonWidth = 0;
				foreach(NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.
					CGridViewItemTextBox cControlObj in lccTextBoxesObj)		
					iCommonWidth += (int)cControlObj.szMaxTextSize.Width;
		
				sCommonWidth = iCommonWidth.ToString();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetListViewItemSize(ref string sErrorMessage)
		{
			try
			{
				ListView lvListViewObj = lvPassengers as ListView;
				if (lvListViewObj == null) return false;

				if(GetGridViewItemTextBoxSize(ref lvListViewObj, ref lccTextBoxesObj_, 
					ref sErrorMessage) == false) return false;

				string sCommonWidth = null;
				if(GetGridViewItemTextBoxCommonWidth(ref lccTextBoxesObj_, ref sCommonWidth,
					ref sErrorMessage) == false) return false;

				double dblCommonWidth = int.Parse(sCommonWidth);
				double dblReminder = lvPassengers.ActualWidth - dblCommonWidth - 196;
				double dblAdditionalColumnWidth = 0;
				if (dblReminder > 0)
				dblAdditionalColumnWidth = dblReminder / 10;

				foreach (NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.
					CGridViewItemTextBox cControlObj in lccTextBoxesObj_)
				{
					cControlObj.gvcGridViewColumn.Width = cControlObj.szMaxTextSize.Width + dblAdditionalColumnWidth;
				}			
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetListViewItemColor(ref string sErrorMessage)
		{
			try
			{
				ListView lvListViewObj = lvPassengers as ListView;
				if (lvListViewObj == null) return false;

				if (SetListViewItemColor(ref lvListViewObj, ref lccTextBoxesObj_, 
					"imgListViewItemPassengerDriverLicensePicture", 
					"brdListViewItemPassengerDriverLicensePicture", ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool WorkingThreadInit(ref string sErrorMessage)
		{
			try
			{
				BackgroundWorkerObj_.WorkerSupportsCancellation = true;
				BackgroundWorkerObj_.DoWork += PassengerSearchKeyUpBeginThread;
				BackgroundWorkerObj_.RunWorkerCompleted += PassengerSearchKeyUpEndThread;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool PassengerFilterWorkingThreadInit(ref string sErrorMessage)
		{
			try
			{
				bgwPassengerFilterSearchObj_.WorkerSupportsCancellation = true;
				bgwPassengerFilterSearchObj_.DoWork += PassengerFilterWorkingThreadBegin;
				bgwPassengerFilterSearchObj_.RunWorkerCompleted += PassengerFilterWorkingThreadEnd;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		
	

		bool EnablePassengerFilterControls(ref string sErrorMessage)
		{
			try
			{
				if (cbRoutes == null) return false;
				if (cbDepartureTime == null) return false;
				if (dpDepartureDate == null) return false;
				if (cbPassengerFilter == null) return false;

				bool bIsEnabled = false;
				if(cbPassengerFilter.IsChecked == true)
					bIsEnabled = true;
				else if(cbPassengerFilter.IsChecked == false)
					bIsEnabled = false;	

				cbRoutes.IsEnabled = bIsEnabled;
				cbDepartureTime.IsEnabled = bIsEnabled;
				dpDepartureDate.IsEnabled = bIsEnabled;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool FullRouteComboBox(ref string sErrorMessage)
		{
			try
			{
				lcRouteDatabaseDataObj_ = null;
				RouteRequest(ref lcRouteDatabaseDataObj_, ref sErrorMessage);
				ClearRouteComboBox(ref sErrorMessage);
				FullRouteComboBox(ref lcRouteDatabaseDataObj_, ref sErrorMessage);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool FullRouteComboBox(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData> lcRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				AddAllRoutesItem(ref sErrorMessage);

				if(lcRouteDatabaseDataObj == null) return false;
				if(lcRouteDatabaseDataObj.Count == 0) return false;
				foreach(var cRouteDatabaseDataObj in lcRouteDatabaseDataObj)
				{
					if (cRouteDatabaseDataObj == null) return false;
					if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sRouteName) == true) return false;
					cbRoutes.Items.Add(cRouteDatabaseDataObj.sRouteName);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool AddAllRoutesItem(ref string sErrorMessage)
		{
			try
			{
				cbRoutes.Items.Add("All Routes");
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool RouteRequest(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData> lcRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (lcRouteDatabaseDataObj != null) return false;
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteRequest(ref vcPostgresqlWorkerObj,
					ref lcRouteDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool FullTripComboBox(
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				TripRequest(ref cTripDatabaseDataObj, ref sErrorMessage);
				NmspTraveler.WorkSources.CPostgresqlWorker.SortByTime(ref cTripDatabaseDataObj.lsDepartureTime, ref sErrorMessage);
				FullTripComboBox1(ref cTripDatabaseDataObj, ref sErrorMessage);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private bool ClearRouteComboBox(ref string sErrorMessage)
		{
			try
			{
				cbRoutes.Items.Clear();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private bool ClearTripComboBox(ref string sErrorMessage)
		{
			try
			{
				cbDepartureTime.Items.Clear();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool FullTripComboBox1(
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if(cTripDatabaseDataObj == null) return false;
				if(cTripDatabaseDataObj.lsDepartureTime == null) return false;
				if(cTripDatabaseDataObj.lsDepartureTime.Count == 0) return false;
				foreach(string sDepartureTime in cTripDatabaseDataObj.lsDepartureTime)
				{
					if (NmspTraveler.CString.IsStringEmpty(sDepartureTime) == true) return false;
					cbDepartureTime.Items.Add(sDepartureTime);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool AddAllTripsItem(ref string sErrorMessage)
		{
			try
			{
				cbDepartureTime.Items.Add("All Times");
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetAllTripsItem(ref string sErrorMessage)
		{
			try
			{
				foreach(var vcbDepartureTimeItem in cbDepartureTime.Items)
				{
					if (vcbDepartureTimeItem == null) return false;

					if(vcbDepartureTimeItem.ToString() == "All Times")
					{
						cbDepartureTime.SelectedItem = vcbDepartureTimeItem;
						break;
					}
				}	
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetTripComboBoxDefault(ref string sErrorMessage)
		{
			try
			{
				cbDepartureTime.Text = "Departure Time";
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool TripRequest(
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cTripDatabaseDataObj == null) return false;
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				if (Traveler.WorkSources.CDatabaseExchanger.TripRequest(ref vcPostgresqlWorkerObj,
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetRouteComboBox(string sRouteName, ref string sErrorMessage)
		{
			try
			{
				if (cbRoutes == null) return false;

				foreach(var vcbRouteItem in cbRoutes.Items)
				{
					if (vcbRouteItem == null) return false;

					if(vcbRouteItem.ToString() == sRouteName)
					{
						cbRoutes.SelectedItem = vcbRouteItem;
						break;
					}	
				}	
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool SetTripComboBox(string sDepartureTime, ref string sErrorMessage)
		{
			try
			{
				if (cbDepartureTime == null) return false;

				foreach(var vcbDepartureTimeItem in cbDepartureTime.Items)
				{
					if (vcbDepartureTimeItem == null) return false;

					if(vcbDepartureTimeItem.ToString() == sDepartureTime)
					{
						cbDepartureTime.SelectedItem = vcbDepartureTimeItem;
						break;
					}
				}	
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool PassengerFilterSearch(ref NmspTraveler.WorkSources.NmspWorkingStructures.
			NmspPassengerWindow.CPassengerFilterSearch cPassengerFilterSearchObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (bgwPassengerFilterSearchObj_ == null) return false;
                
				bgwPassengerFilterSearchObj_.CancelAsync();

				while(bgwPassengerFilterSearchObj_.IsBusy == true)
				{
					Thread.Sleep(50);
					DoEvents(ref sErrorMessage); 
				}

				bgwPassengerFilterSearchObj_.RunWorkerAsync(cPassengerFilterSearchObj);
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
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);
			SetControlNameList(ref sErrorMessage);
			SetListViewItemColor(ref sErrorMessage);
			SetListViewItemSize(ref sErrorMessage);
			WorkingThreadInit(ref sErrorMessage);
			PassengerFilterWorkingThreadInit(ref sErrorMessage);
			EnablePassengerFilterControls(ref sErrorMessage);
			FullRouteComboBox(ref sErrorMessage);
			SetRouteComboBox(cTripInfoObj_.sRouteName, ref sErrorMessage);

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseData1Obj =
				new WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1();
			if (cTripDatabaseData1Obj == null) return;

			string sRouteNumber = null;
			if (cTripInfoObj_.sRouteName == "All Routes")
				sRouteNumber = "All Routes";
			else
				GetRouteNumberByRouteName(cTripInfoObj_.sRouteName, ref sRouteNumber, ref sErrorMessage);
			
			if (NmspTraveler.CString.IsStringEmpty(sRouteNumber) == true) return;

			cTripDatabaseData1Obj.sVehicleRouteNumber = sRouteNumber;

			ClearTripComboBox(ref sErrorMessage);
			AddAllTripsItem(ref sErrorMessage);
			FullTripComboBox(ref cTripDatabaseData1Obj, ref sErrorMessage);
			
			SetTripComboBox(cTripInfoObj_.sDepartureTime, ref sErrorMessage);

			string sDepartureDate = null;
			if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, ref sDepartureDate, ref sErrorMessage) == false) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch
				cPassengerFilterSearchObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch() { sRouteNumber = sRouteNumber, 
				sDepartureDate = sDepartureDate, sDepartureTime = cTripInfoObj_.sDepartureTime };

			PassengerFilterSearch(ref cPassengerFilterSearchObj, ref sErrorMessage);
		}

		private bool GetRouteNumberByRouteName(string sRouteName, ref string sRouteNumber, ref string sErrorMessage)
		{
			try
			{
				foreach(var cRouteDatabaseDataObj in lcRouteDatabaseDataObj_)
				{
					if (cRouteDatabaseDataObj.sRouteName == sRouteName)
					{
						sRouteNumber = cRouteDatabaseDataObj.sRouteNumber;
						break;
					}		
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
			
			
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

		private void PassengerSearchKeyUp(object sender, KeyEventArgs e)
        {
			if (BackgroundWorkerObj_ == null) return;
                
            string sInputedText = tbPassengerSearch.Text;
		
			BackgroundWorkerObj_.CancelAsync();

			string sErrorMessage = null;
			while(BackgroundWorkerObj_.IsBusy == true)
			{
				Thread.Sleep(50);
				DoEvents(ref sErrorMessage); 
			}

            BackgroundWorkerObj_.RunWorkerAsync(sInputedText);
        }

		private void OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			DatePicker dpDepartureDateObj = sender as DatePicker;
            if (dpDepartureDateObj == null) return;
            if (dpDepartureDateObj.IsDropDownOpen == false)return;

			string sErrorMessage = null;
			string sDepartureDate = null;
			if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDateObj, ref sDepartureDate, ref sErrorMessage) == false) return;

			string sRouteNumber = null;
			GetRouteNumberByRouteName(cbRoutes.Text, ref sRouteNumber, ref sErrorMessage);
			
			if (NmspTraveler.CString.IsStringEmpty(sRouteNumber) == true) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch
				cPassengerFilterSearchObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch() { sRouteNumber = sRouteNumber, 
				sDepartureDate = sDepartureDate, sDepartureTime = cbDepartureTime.Text };

			PassengerFilterSearch(ref cPassengerFilterSearchObj, ref sErrorMessage);
		}

		private void OnPassengerFilterCheckBox(object sender, RoutedEventArgs e)
		{
			string sErrorMessage = null;
			EnablePassengerFilterControls(ref sErrorMessage);

			if(cbPassengerFilter.IsChecked == true)
			{
				string sDepartureDate = null;
				if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, 
				ref sDepartureDate, ref sErrorMessage) == false) return;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch
				cPassengerFilterSearchObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch() { sRouteNumber = cbRoutes.Text, 
				sDepartureDate = sDepartureDate, sDepartureTime = cbDepartureTime.Text };

				PassengerFilterSearch(ref cPassengerFilterSearchObj, ref sErrorMessage);
			}
			else if(cbPassengerFilter.IsChecked == false)
			{
				PassengersDataRequest(ref sErrorMessage);
			}	
		}

		private void OnDepartureTimeComboBoxItemSelectionChanged(object sender, RoutedEventArgs e)
		{
			ComboBox cbDepartureTimeObj = sender as ComboBox;
			if (cbDepartureTimeObj == null) return;
			if (cbDepartureTimeObj.SelectedValue == null) return;
			if (cbDepartureTimeObj.IsDropDownOpen == false)return;

			string sDepartureTime = cbDepartureTimeObj.SelectedValue.ToString();

			if (NmspTraveler.CString.IsStringEmpty(sDepartureTime) == true) return;

			string sErrorMessage = null;
			string sDepartureDate = null;
			if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, 
				ref sDepartureDate, ref sErrorMessage) == false) return;

			string sRouteNumber = null;
			GetRouteNumberByRouteName(cbRoutes.Text, ref sRouteNumber, ref sErrorMessage);	
			if (NmspTraveler.CString.IsStringEmpty(sRouteNumber) == true) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch
				cPassengerFilterSearchObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch() { sRouteNumber = sRouteNumber, 
				sDepartureDate = sDepartureDate, sDepartureTime = sDepartureTime };

			PassengerFilterSearch(ref cPassengerFilterSearchObj, ref sErrorMessage);
		}

		private void OnRouteComboBoxItemSelectionChanged(object sender, RoutedEventArgs e)
		{
			ComboBox cbRoutesObj = sender as ComboBox;
			if (cbRoutesObj == null) return;
			if (cbRoutesObj.SelectedValue == null) return;
			if (cbRoutesObj.IsDropDownOpen == false)return;

			string sSelectedRouteName = cbRoutesObj.SelectedValue.ToString();

			if (NmspTraveler.CString.IsStringEmpty(sSelectedRouteName) == true) return;

			string sErrorMessage = null;
			string sRouteNumber = null;

			ClearTripComboBox(ref sErrorMessage);
			AddAllTripsItem(ref sErrorMessage);
			SetAllTripsItem(ref sErrorMessage);

			if (sSelectedRouteName == "All Routes")
			{
				sRouteNumber = "All Routes";
			}
			else if (sSelectedRouteName != "All Routes")
			{
				GetRouteNumberByRouteName(sSelectedRouteName, ref sRouteNumber, ref sErrorMessage);

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseData1Obj =
				new WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1();
				if (cTripDatabaseData1Obj == null) return;

				cTripDatabaseData1Obj.sVehicleRouteNumber = sRouteNumber;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseData1Obj.sVehicleRouteNumber) == true) return;
				FullTripComboBox(ref cTripDatabaseData1Obj, ref sErrorMessage);
			}
						
			if (NmspTraveler.CString.IsStringEmpty(sRouteNumber) == true) return;
	
			

			string sDepartureDate = null;
			if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, ref sDepartureDate, ref sErrorMessage) == false) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CPassengerFilterSearch
				cPassengerFilterSearchObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
				NmspPassengerWindow.CPassengerFilterSearch() { sRouteNumber = sRouteNumber, 
				sDepartureDate = sDepartureDate, sDepartureTime = cbDepartureTime.Text };

			PassengerFilterSearch(ref cPassengerFilterSearchObj, ref sErrorMessage);
		}

		


		private NmspTraveler.CDataHub cDataHubObj_ = null;

		private BackgroundWorker BackgroundWorkerObj_ = new BackgroundWorker();
		private BackgroundWorker bgwPassengerFilterSearchObj_ = new BackgroundWorker();

		List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj_ = 
			new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox>();

		List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.
			CRouteDatabaseData> lcRouteDatabaseDataObj_ = null;

		NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo cTripInfoObj_ = null;
	}
}
