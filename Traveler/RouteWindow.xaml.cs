using System;
using System.Collections;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NmspTraveler
{
    public partial class CRouteWindow : NmspTraveler.CBaseWindow
    {
        public CRouteWindow(ref NmspTraveler.CDataHub cDataHubObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;

            List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData> lCVehicleRoutesDataObj = null;
            
            VehicleRoutesDataRequest(ref lCVehicleRoutesDataObj);  

            lCVehicleRoutesDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.
				CVehicleRoutesData() { iVehicleRouteNumber = -1, sRouteName = "Add route" });

            lvRoutes.ItemsSource = lCVehicleRoutesDataObj;
        }
        
		private void SetListViewItemColor(ref ListView lvListViewObj)   
		{
            TextBox tbListViewItemTextBoxObj = null;
            ListBox lbListViewItemListBoxObj = null;
            TextBox tbListViewItemListBoxItemTextBoxObj = null;

            for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
            {
                if (iListViewItemIndex == lvListViewObj.SelectedIndex) continue;

                tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbVehicleRouteNameTextBox", iListViewItemIndex);
                if (tbListViewItemTextBoxObj == null) continue;
                tbListViewItemTextBoxObj.Foreground = Brushes.White;

                lbListViewItemListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", iListViewItemIndex);
                if (lbListViewItemListBoxObj == null) continue;
                lbListViewItemListBoxObj.Foreground = Brushes.White;
                lbListViewItemListBoxObj.Focusable = false;
                lbListViewItemListBoxObj.SelectedIndex = -1;

                for (int iListBoxItemIndex = 0; iListBoxItemIndex < lbListViewItemListBoxObj.Items.Count; iListBoxItemIndex++)
                {
					tbListViewItemListBoxItemTextBoxObj = null;
                    tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lbListViewItemListBoxObj, "tbVehicleDepartureTime", iListBoxItemIndex);
                    if (tbListViewItemListBoxItemTextBoxObj == null) continue;
                    tbListViewItemListBoxItemTextBoxObj.Foreground = Brushes.White;
                    tbListViewItemListBoxItemTextBoxObj.Focusable = false;
                }  
            }

            tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbVehicleRouteNameTextBox", lvListViewObj.SelectedIndex);
            if (tbListViewItemTextBoxObj != null)
            {
                tbListViewItemTextBoxObj.Foreground = Brushes.Gray;
            }

            lbListViewItemListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
            if (lbListViewItemListBoxObj != null)
            {
                lbListViewItemListBoxObj.Foreground = Brushes.Gray;
            }

            if (lbListViewItemListBoxObj != null)
            {
                for (int iListBoxItemIndex = 0; iListBoxItemIndex < lbListViewItemListBoxObj.Items.Count; iListBoxItemIndex++)
                {
					tbListViewItemListBoxItemTextBoxObj = null;
                    tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lbListViewItemListBoxObj, "tbVehicleDepartureTime", iListBoxItemIndex);
                    if (tbListViewItemListBoxItemTextBoxObj == null) continue;
                    tbListViewItemListBoxItemTextBoxObj.Foreground = Brushes.Gray;
                }      
            }
        }

        private bool VehicleRoutesDataRequest(
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData> lCVehicleRoutesDataObj)
        {
            if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;

            string sErrorMessage = null;
            NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseRoutesTableObj = null;
            vcPostgresqlWorkerObj.SelectTable("routes", ref dataBaseRoutesTableObj, ref sErrorMessage);

            if (dataBaseRoutesTableObj == null) return false;
            if (dataBaseRoutesTableObj.lDataBaseColumnName_ == null) return false;
            if (dataBaseRoutesTableObj.lDataBaseColumnName_.Count < 2) return false;
            if (dataBaseRoutesTableObj.lDataBaseRows_ == null) return false;
            if (dataBaseRoutesTableObj.lDataBaseRows_.Count == 0) return false;

            if (lCVehicleRoutesDataObj != null) return false;
            lCVehicleRoutesDataObj = new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData>();
            if (lCVehicleRoutesDataObj == null) return false;

            List<string> lsTime = new List<string>();
            if (lsTime == null) return false;
            
			foreach (NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseRow dataBaseRowObj in 
				dataBaseRoutesTableObj.lDataBaseRows_)
            {
				lsTime.Clear();
				if (Traveler.WorkSources.CDatabaseExchanger.TripRequest(ref vcPostgresqlWorkerObj, 
					dataBaseRowObj.lDataBaseColumnValue_[0] , ref lsTime, ref sErrorMessage) == false)
					return false;

                NmspTraveler.WorkSources.CPostgresqlWorker.SortByTime(ref lsTime, ref sErrorMessage);

                var vObservableCollectionObj = new ObservableCollection<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime>();
                if (vObservableCollectionObj == null) continue;

                for (int iTripIndex = 0; iTripIndex < lsTime.Count; iTripIndex++)
                {
                    vObservableCollectionObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime() {
                        sVehicleDepartureTime = lsTime[iTripIndex],
                        iVehicleTripNumber = iTripIndex
                    });
                }

				vObservableCollectionObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime() {
                        sVehicleDepartureTime = "Add trip",
                        iVehicleTripNumber = -1
                    });

                lCVehicleRoutesDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData()
                {
                    iVehicleRouteNumber = int.Parse(dataBaseRowObj.lDataBaseColumnValue_[0]),
                    sRouteName = dataBaseRowObj.lDataBaseColumnValue_[1],
                    ieVehicleDepartureTime = vObservableCollectionObj
                });
                 
            }

            return true;
        }
       
		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			string sErrorMessage = null;
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);

		}

        private void OnListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView lvListViewObj = sender as ListView;

            if (lvListViewObj != null)
                SetListViewItemColor(ref lvListViewObj);
        }

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            VehicleDepartureTimeGridViewColumn.Width -= VehicleRouteNameGridViewColumn.ActualWidth;
        }

        private void OnListViewMouseMove(object sender, MouseEventArgs e)
        {
            ListView lvListViewObj = lvRoutes as ListView;

            if (lvListViewObj != null)
                SetListViewItemColor(ref lvListViewObj);
        }

        private void OnListBoxTextBoxFocusChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

			if(tb.IsFocused == true)
			{
				sOldTripName_ = tb.Text;
			}
            else if (tb.IsFocused == false)
            {
                ListView lvListViewObj = lvRoutes as ListView;
                if (lvListViewObj == null) return;

                if ((lvListViewObj.SelectedIndex < 0) || (lvListViewObj.SelectedIndex >= lvListViewObj.Items.Count)) return;

                ListBox lbListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
                if (lbListBoxObj == null) return;

                if (lbListBoxObj.SelectedIndex < 0) return;

                TextBox tbListViewItemListBoxItemTextBoxObj = null;
                for (int iListBoxItemIndex = 0; iListBoxItemIndex < lbListBoxObj.Items.Count; iListBoxItemIndex++)
                {
                    tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
						ref lbListBoxObj, "tbVehicleDepartureTime", iListBoxItemIndex);
                    if (tbListViewItemListBoxItemTextBoxObj == null) continue;

                    tbListViewItemListBoxItemTextBoxObj.Focusable = false;
                }

				if(tb.Text == sOldTripName_) return;

                tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
					ref lbListBoxObj, "tbVehicleDepartureTime", lbListBoxObj.SelectedIndex);
                if (tbListViewItemListBoxItemTextBoxObj == null) return;

                NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeDataObj = 
					(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime)lbListBoxObj.SelectedItem;
                if (cVehicleDepartureTimeDataObj == null) return;

                if (cVehicleDepartureTimeDataObj.sVehicleDepartureTime == "Add trip") return;
                
				string sErrorMessage = null;
				if (cVehicleDepartureTimeDataObj.iVehicleTripNumber != -1)
				{
					EditTrip(ref lvListViewObj, cVehicleDepartureTimeDataObj.sVehicleDepartureTime, sOldTripName_, ref sErrorMessage);
				}
				else if (cVehicleDepartureTimeDataObj.iVehicleTripNumber == -1)
				{
					AddNewTrip(ref lvListViewObj, ref lbListBoxObj, ref cVehicleDepartureTimeDataObj, ref sErrorMessage);
				}
            }
        }

        private void OnListViewTextBoxFocusChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

			if(tb.IsFocused == true)
			{
				sOldRouteName_ = tb.Text;
			}
            else if (tb.IsFocused == false)
            {
                ListView lvListViewObj = lvRoutes as ListView;
                if (lvListViewObj == null) return;

                for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
                {
                    TextBox tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
						ref lvListViewObj, "tbVehicleRouteNameTextBox", iListViewItemIndex);
                    if (tbListViewItemTextBoxObj == null) continue;

                    tbListViewItemTextBoxObj.Focusable = false;
                }

                if (lvListViewObj.SelectedIndex < 0) return;
				if(tb.Text == sOldRouteName_) return;

                NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
					(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
                if (cVehicleRoutesDataObj == null) return;

                string sVehicleRouteNameObj = cVehicleRoutesDataObj.sRouteName;
                if (sVehicleRouteNameObj == "Add route") return;

				string sErrorMessage = null;
                if (cVehicleRoutesDataObj.iVehicleRouteNumber != -1) // editing
				{
					EditRoute(ref lvListViewObj, ref cVehicleRoutesDataObj, sOldRouteName_, ref sErrorMessage);
				}
				else if (cVehicleRoutesDataObj.iVehicleRouteNumber == -1) // adding
				{
					AddNewRoute(ref lvListViewObj, ref cVehicleRoutesDataObj, ref sErrorMessage);
				}	
            }
        }

		bool AddNewGuiRoute(
			ref ListView lvListViewObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj,
			ref string sErrorMessage)
		{
			try
			{
				cVehicleRoutesDataObj.iVehicleRouteNumber = lvListViewObj.Items.Count - 1;

                cVehicleRoutesDataObj.ieVehicleDepartureTime = 
					new ObservableCollection<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime>{
                    new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime() { 
						sVehicleDepartureTime = "Add trip"} };

                List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData> lcVehicleRoutesDataObj = 
					new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData>();

                for (int i = 0; i < lvRoutes.Items.Count; i++)
                    lcVehicleRoutesDataObj.Add((NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvRoutes.Items.GetItemAt(i));

                lcVehicleRoutesDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.
					CVehicleRoutesData() { iVehicleRouteNumber = -1, sRouteName = "Add route" });

                lvRoutes.ItemsSource = lcVehicleRoutesDataObj;

                lvRoutes.UpdateLayout();

                lvRoutes.ScrollIntoView(lvRoutes.Items[lvListViewObj.SelectedIndex]);

                SetListViewItemColor(ref lvListViewObj);

                lvRoutes.SelectedIndex = lvRoutes.Items.Count - 1;

                ListViewItem lvListViewSelectedItem = lvRoutes.ItemContainerGenerator.ContainerFromIndex(lvListViewObj.SelectedIndex) as ListViewItem;
                if (lvListViewSelectedItem != null) lvListViewSelectedItem.Focus();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool EditDbRoute(string sOldRouteName, string sNewRouteName, ref string sErrorMessage)
		{
			try
			{
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest 
					cVehicleRouteNumberByRouteNameRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest(){ sRouteName = sOldRouteName };
				if (cVehicleRouteNumberByRouteNameRequestObj == null) return false;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteNumberByRouteNameRequest(
					ref vcPostgresqlWorkerObj, ref cVehicleRouteNumberByRouteNameRequestObj, 
					ref sErrorMessage) == false) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDatabaseData 
					cRouteDatabaseDataObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspRouteWindow.CRouteDatabaseData();
				if (cRouteDatabaseDataObj == null) return false;

				cRouteDatabaseDataObj.sRouteName = sNewRouteName;
				cRouteDatabaseDataObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;
				cRouteDatabaseDataObj.sDeparturePlatform = cRouteDatabaseDataObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteEditingRequest(ref vcPostgresqlWorkerObj, 
					ref cRouteDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool AddNewDbRoute(string sRouteName, ref string sErrorMessage)
		{
			try
			{
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDatabaseData 
					cRouteDatabaseDataObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspRouteWindow.CRouteDatabaseData();

				if (Traveler.WorkSources.CDatabaseExchanger.MaxRouteNumberRequest(ref vcPostgresqlWorkerObj, 
					ref cRouteDatabaseDataObj, ref sErrorMessage) == false) return false;

				cRouteDatabaseDataObj.sRouteName = sRouteName;
				cRouteDatabaseDataObj.sRouteNumber = (int.Parse(cRouteDatabaseDataObj.sRouteNumber) + 1).ToString();
				cRouteDatabaseDataObj.sDeparturePlatform = cRouteDatabaseDataObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteAddingRequest(ref vcPostgresqlWorkerObj, 
					ref cRouteDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool AddNewGuiTrip(
			ref ListView lvListViewObj,
			ref ListBox lbListBoxObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeDataObj,
			ref string sErrorMessage)
		{
			try
			{
				cVehicleDepartureTimeDataObj.iVehicleTripNumber = lbListBoxObj.Items.Count - 1;

                List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime> lcVehicleDepartureTimeDataObj = 
					new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime>();

                for (int iListBoxItemIndex = 0; iListBoxItemIndex < lbListBoxObj.Items.Count; iListBoxItemIndex++)
                    lcVehicleDepartureTimeDataObj.Add((NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime)
						lbListBoxObj.Items.GetItemAt(iListBoxItemIndex));

                lcVehicleDepartureTimeDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime()
				{ iVehicleTripNumber = -1, sVehicleDepartureTime = "Add trip" });

                lbListBoxObj.ItemsSource = lcVehicleDepartureTimeDataObj;

                lbListBoxObj.UpdateLayout();

                lbListBoxObj.ScrollIntoView(lbListBoxObj.Items[lbListBoxObj.SelectedIndex]);

                SetListViewItemColor(ref lvListViewObj);

                lbListBoxObj.SelectedIndex = lbListBoxObj.Items.Count - 1;

                ListBoxItem lbListBoxSelectedItem = lbListBoxObj.ItemContainerGenerator.ContainerFromIndex(lbListBoxObj.SelectedIndex) as ListBoxItem;
                if (lbListBoxSelectedItem != null) lbListBoxSelectedItem.Focus();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool AddNewDbTrip(
			ref ListView lvListViewObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
					(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
                if (cVehicleRoutesDataObj == null) return false;

				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest 
					cVehicleRouteNumberByRouteNameRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest(){ sRouteName = cVehicleRoutesDataObj.sRouteName };

				if (Traveler.WorkSources.CDatabaseExchanger.RouteNumberByRouteNameRequest(
					ref vcPostgresqlWorkerObj, ref cVehicleRouteNumberByRouteNameRequestObj, 
					ref sErrorMessage) == false) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDatabaseData 
					cTripDatabaseDataObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspTripWindow.CTripDatabaseData();

				if (Traveler.WorkSources.CDatabaseExchanger.MaxTripNumberRequest(ref vcPostgresqlWorkerObj, 
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) return false;

				cTripDatabaseDataObj.sTripNumber = (int.Parse(cTripDatabaseDataObj.sTripNumber) + 1).ToString();
				cTripDatabaseDataObj.sDepartureTime = cVehicleDepartureTimeDataObj.sVehicleDepartureTime;
				cTripDatabaseDataObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;
				
				if (Traveler.WorkSources.CDatabaseExchanger.TripAddingRequest(ref vcPostgresqlWorkerObj, 
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool EditDbTrip(
			ref ListView lvListViewObj,
			string sNewDepartureTime, 
			string sOldDepartureTime,
			ref string sErrorMessage)
		{
			try
			{
				NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
					(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
                if (cVehicleRoutesDataObj == null) return false;

				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest 
					cVehicleRouteNumberByRouteNameRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest(){ sRouteName = cVehicleRoutesDataObj.sRouteName };

				if (Traveler.WorkSources.CDatabaseExchanger.RouteNumberByRouteNameRequest(
					ref vcPostgresqlWorkerObj, ref cVehicleRouteNumberByRouteNameRequestObj, 
					ref sErrorMessage) == false) return false;



				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripNumberByDepartureTimeAndRouteNumberRequest 
					cTripNumberByDepartureTimeAndRouteNumberRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspTripWindow.CTripNumberByDepartureTimeAndRouteNumberRequest();

				cTripNumberByDepartureTimeAndRouteNumberRequestObj.sDepartureTime = sOldDepartureTime;
				cTripNumberByDepartureTimeAndRouteNumberRequestObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.TripNumberByDepartureTimeAndRouteNumberRequest(
					ref vcPostgresqlWorkerObj, ref cTripNumberByDepartureTimeAndRouteNumberRequestObj, 
					ref sErrorMessage) == false) return false;




				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDatabaseData 
					cTripDatabaseDataObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspTripWindow.CTripDatabaseData();

				cTripDatabaseDataObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;
				cTripDatabaseDataObj.sDepartureTime = sNewDepartureTime;
				cTripDatabaseDataObj.sTripNumber = cTripNumberByDepartureTimeAndRouteNumberRequestObj.sTripNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.TripEditingRequest(ref vcPostgresqlWorkerObj, 
					ref cTripDatabaseDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool EditTrip(
			ref ListView lvListViewObj,
			string sNewTripName, 
			string sOldTripName,
			ref string sErrorMessage)
		{
			try
			{
				//if (AddNewGuiTrip(ref lvListViewObj, ref lbListBoxObj, ref cVehicleDepartureTimeDataObj, ref sErrorMessage) == false) return false;
				if (EditDbTrip(ref lvListViewObj, sNewTripName, sOldTripName, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool AddNewTrip(
			ref ListView lvListViewObj,
			ref ListBox lbListBoxObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (AddNewGuiTrip(ref lvListViewObj, ref lbListBoxObj, ref cVehicleDepartureTimeDataObj, ref sErrorMessage) == false) return false;
				if (AddNewDbTrip(ref lvListViewObj, ref cVehicleDepartureTimeDataObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool EditRoute(
			ref ListView lvListViewObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj,
 			string sOldRouteName, ref string sErrorMessage)
		{
			try
			{
				//if (EditGuiRoute(ref lvListViewObj, ref cVehicleRoutesDataObj, ref sErrorMessage) == false) return false;
				if (EditDbRoute(sOldRouteName, cVehicleRoutesDataObj.sRouteName, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

		bool AddNewRoute(
			ref ListView lvListViewObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (AddNewGuiRoute(ref lvListViewObj, ref cVehicleRoutesDataObj, ref sErrorMessage) == false) return false;
				if (AddNewDbRoute(cVehicleRoutesDataObj.sRouteName, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}
			
			return true;
		}

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            ListView lvListViewObj = lvRoutes as ListView;
            if (lvListViewObj == null) return;
            if (lvListViewObj.SelectedIndex < 0) return;

            TextBox tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
				ref lvListViewObj, "tbVehicleRouteNameTextBox", lvListViewObj.SelectedIndex);
            if (tbListViewItemTextBoxObj == null) return;

            ListBox lbListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(
				ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
            if (lbListBoxObj == null) return;

            if (Keyboard.IsKeyDown(Key.Enter) == true)
            {
                if (tbListViewItemTextBoxObj.Focusable == true)
                    tbListViewItemTextBoxObj.Focusable = false;

                if ((lbListBoxObj.SelectedIndex < 0) || (lbListBoxObj.SelectedIndex >= lbListBoxObj.Items.Count)) return;

                TextBox tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lbListBoxObj, "tbVehicleDepartureTime", lbListBoxObj.SelectedIndex);
                if (tbListViewItemListBoxItemTextBoxObj == null) return;

                if (tbListViewItemListBoxItemTextBoxObj.Focusable == true)
                    tbListViewItemListBoxItemTextBoxObj.Focusable = false;   
            } 
        }

		private bool OpenSelectedTrip(
			ref ListBox lbListBoxObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj)
		{
            NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeDataObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime)lbListBoxObj.SelectedItem;

            if (cVehicleDepartureTimeDataObj == null) return false;

			NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo cTripInfoObj =
				new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo() 
				{ sRouteName = cVehicleRoutesDataObj.sRouteName, 
					sDepartureTime = cVehicleDepartureTimeDataObj.sVehicleDepartureTime };

			if (cTripInfoObj == null) return false;

            CTripWindow tripWindowObj = new CTripWindow(ref cDataHubObj_, ref cTripInfoObj);
            tripWindowObj.Owner = this;
            tripWindowObj.ShowDialog();
			
			return true;
		}

        override protected void OnCloseButton(object sender, RoutedEventArgs e)
        {
            if (NmspTraveler.CGui.ShowQuestionMessageBox(this,
				"Do you want to log out?", "Traveler - Route") == false)
                return;

            this.Hide();
        }

        private void OnAboutButton(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindowObj = new AboutWindow();
            aboutWindowObj.Owner = this;
            aboutWindowObj.ShowDialog();
        }

		private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if ((e.Command == ApplicationCommands.Copy) ||
				(e.Command == ApplicationCommands.Cut)  || 
				(e.Command == ApplicationCommands.Paste)) 
				  e.Handled = true;	 
		}	

		private void OnRouteTextBoxContextMenuKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}	

		private void OnTripTextBoxContextMenuKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}	

        private void OnHelpButton(object sender, RoutedEventArgs e)
        {

        }



		private void EditRouteName(object sender, RoutedEventArgs e)
		{
			if (cDataHubObj_ == null) return;
			var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
			if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("8", "adding, editing, deletion of routes and trips") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Route");
				return;
			}

			ListView lvListViewObj = lvRoutes as ListView;
            if (lvListViewObj == null) return;
			if (lvListViewObj.SelectedIndex < 0) return;

			TextBox tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
				ref lvListViewObj, "tbVehicleRouteNameTextBox", lvListViewObj.SelectedIndex);
            if (tbListViewItemTextBoxObj == null) return;

            if (tbListViewItemTextBoxObj.Focusable == false)
            {
                tbListViewItemTextBoxObj.Focusable = true;
                tbListViewItemTextBoxObj.SelectAll();
                tbListViewItemTextBoxObj.Focus();
                e.Handled = true;
            }
		}

		bool DeleteGuiRoute(ref ListView lvListViewObj, ref string sErrorMessage)
		{
			try
			{
				int iNewSelectedIndex = 0;
				if (lvListViewObj.SelectedIndex > 0) iNewSelectedIndex = lvListViewObj.SelectedIndex - 1;
				else iNewSelectedIndex = 1;

				List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData> lcVehicleRoutesDataObj = 
					new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData>();
				if (lcVehicleRoutesDataObj == null) return false;
                
				for (int iListViewItemIndex = 0; iListViewItemIndex < lvRoutes.Items.Count; iListViewItemIndex++)
					if (iListViewItemIndex != lvListViewObj.SelectedIndex)
						lcVehicleRoutesDataObj.Add((NmspTraveler.WorkSources.NmspWorkingStructures.
							CVehicleRoutesData)lvRoutes.Items.GetItemAt(iListViewItemIndex));

				lvRoutes.SelectedIndex = iNewSelectedIndex;
				lvRoutes.ItemsSource = lcVehicleRoutesDataObj;

				lvRoutes.UpdateLayout();
				lvRoutes.ScrollIntoView(lvRoutes.Items[0]);

				SetListViewItemColor(ref lvListViewObj);

				ListViewItem lvListViewSelectedItem = lvRoutes.ItemContainerGenerator.ContainerFromIndex(iNewSelectedIndex) as ListViewItem;
				if (lvListViewSelectedItem != null) lvListViewSelectedItem.Focus();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool DeleteDbRoute(string sRouteName, ref string sErrorMessage)
		{
			try
			{
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest 
					cVehicleRouteNumberByRouteNameRequestObj = new WorkSources.NmspWorkingStructures.NmspRouteWindow.
						CVehicleRouteNumberByRouteNameRequest();
				if(cVehicleRouteNumberByRouteNameRequestObj == null)return false;

				cVehicleRouteNumberByRouteNameRequestObj.sRouteName = sRouteName;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteNumberByRouteNameRequest(
					ref vcPostgresqlWorkerObj, ref cVehicleRouteNumberByRouteNameRequestObj, ref sErrorMessage) == false) return false;


				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDeletionRequest cTripDeletionRequestObj =
					new NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDeletionRequest();
				if (cTripDeletionRequestObj == null) return false;

				cTripDeletionRequestObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.TripDeletionRequest(ref vcPostgresqlWorkerObj, 
					ref cTripDeletionRequestObj, ref sErrorMessage) == false) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDeletionRequest cRouteDeletionRequestObj =
					new NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDeletionRequest();
				if (cRouteDeletionRequestObj == null) return false;

				cRouteDeletionRequestObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteDeletionRequest(ref vcPostgresqlWorkerObj, 
					ref cRouteDeletionRequestObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void DeleteRoute(object sender, RoutedEventArgs e)
		{
			if (cDataHubObj_ == null) return;
			var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
			if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("8", 
				"adding, editing, deletion of routes and trips") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Route");
				return;
			}

			ListView lvListViewObj = lvRoutes as ListView;
            if (lvListViewObj == null) return;
			if (lvListViewObj.SelectedIndex < 0) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
            if (cVehicleRoutesDataObj == null) return;

            if ((cVehicleRoutesDataObj.sRouteName == "Add route") && 
				(cVehicleRoutesDataObj.iVehicleRouteNumber == -1)) return;

			string sErrorMessage = null;
			if(DeleteGuiRoute(ref lvListViewObj, ref sErrorMessage) == false) return;
			if(DeleteDbRoute(cVehicleRoutesDataObj.sRouteName, ref sErrorMessage) == false) return;
		}	

		private void OpenTrip(object sender, MouseButtonEventArgs e)
		{
			if (cDataHubObj_ == null) return;
            var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
            if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("1", 
				"view of the information about trips") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Route");
				return;
			}

			ListView lvListViewObj = lvRoutes as ListView;
            if (lvListViewObj == null) return;
			if (lvListViewObj.SelectedIndex < 0) return;

			ListBox lbListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
			if (lbListBoxObj == null) return;
			if (lbListBoxObj.SelectedIndex < 0) return;

			NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
            if (cVehicleRoutesDataObj == null) return;

			OpenSelectedTrip(ref lbListBoxObj, ref cVehicleRoutesDataObj); 
		}

		private void EditTripDepartureTime(object sender, MouseButtonEventArgs e)
		{
			if (cDataHubObj_ == null) return;
			var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
			if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("8", 
				"adding, editing, deletion of routes and trips") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Route");
				return;
			}

            ListView lvListViewObj = lvRoutes as ListView;
			if (lvListViewObj == null) return;
			if (lvListViewObj.SelectedIndex < 0) return;

			ListBox lbListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
			if (lbListBoxObj == null) return;
			if (lbListBoxObj.SelectedIndex < 0) return;

            TextBox tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
				ref lbListBoxObj, "tbVehicleDepartureTime", lbListBoxObj.SelectedIndex);

            if (tbListViewItemListBoxItemTextBoxObj == null) return;
                
            if (tbListViewItemListBoxItemTextBoxObj.Focusable == false)
            {
                tbListViewItemListBoxItemTextBoxObj.Focusable = true;
                tbListViewItemListBoxItemTextBoxObj.SelectAll();
                tbListViewItemListBoxItemTextBoxObj.Focus();
            }          
		}


		bool DeleteGuiTrip(ref ListView lvListViewObj, ref ListBox lbListBoxObj, ref string sErrorMessage)
		{
			try
			{
				int iNewSelectedIndex = 0;
				if (lbListBoxObj.SelectedIndex > 0) iNewSelectedIndex = lbListBoxObj.SelectedIndex - 1;
				else iNewSelectedIndex = 1;

				List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime> lcVehicleDepartureTimeDataObj = 
					new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime>();
				if (lcVehicleDepartureTimeDataObj == null) return false;

				for (int iListBoxItemIndex = 0; iListBoxItemIndex < lbListBoxObj.Items.Count; iListBoxItemIndex++)
					if (iListBoxItemIndex != lbListBoxObj.SelectedIndex)
						lcVehicleDepartureTimeDataObj.Add((NmspTraveler.WorkSources.NmspWorkingStructures.
							CVehicleDepartureTime)lbListBoxObj.Items.GetItemAt(iListBoxItemIndex));

				lbListBoxObj.ItemsSource = lcVehicleDepartureTimeDataObj;

				lbListBoxObj.SelectedIndex = iNewSelectedIndex;

				lbListBoxObj.UpdateLayout();
				lbListBoxObj.ScrollIntoView(lvRoutes.Items[0]);

				SetListViewItemColor(ref lvListViewObj);

				ListBoxItem lbListBoxSelectedItem = lbListBoxObj.ItemContainerGenerator.
					ContainerFromIndex(iNewSelectedIndex) as ListBoxItem;
				if (lbListBoxSelectedItem != null) lbListBoxSelectedItem.Focus();
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		bool DeleteDbTrip(ref ListView lvListViewObj, ref ListBox lbListBoxObj, ref string sErrorMessage)
		{
			try
			{
				if (cDataHubObj_ == null) return false;
				var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
				if (vcPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData cVehicleRoutesDataObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleRoutesData)lvListViewObj.SelectedItem;
				if (cVehicleRoutesDataObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime)lbListBoxObj.SelectedItem;
				if (cVehicleDepartureTimeObj == null) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest 
					cVehicleRouteNumberByRouteNameRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspRouteWindow.CVehicleRouteNumberByRouteNameRequest(){ sRouteName = cVehicleRoutesDataObj.sRouteName };
				if (cVehicleRouteNumberByRouteNameRequestObj == null) return false;

				if (Traveler.WorkSources.CDatabaseExchanger.RouteNumberByRouteNameRequest(
					ref vcPostgresqlWorkerObj, ref cVehicleRouteNumberByRouteNameRequestObj, 
					ref sErrorMessage) == false) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripNumberByDepartureTimeAndRouteNumberRequest 
					cTripNumberByDepartureTimeAndRouteNumberRequestObj = new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspTripWindow.CTripNumberByDepartureTimeAndRouteNumberRequest();

				cTripNumberByDepartureTimeAndRouteNumberRequestObj.sDepartureTime = cVehicleDepartureTimeObj.sVehicleDepartureTime;
				cTripNumberByDepartureTimeAndRouteNumberRequestObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.TripNumberByDepartureTimeAndRouteNumberRequest(
					ref vcPostgresqlWorkerObj, ref cTripNumberByDepartureTimeAndRouteNumberRequestObj, 
					ref sErrorMessage) == false) return false;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDeletionRequest cTripDeletionRequestObj =
					new NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDeletionRequest();
				if (cTripDeletionRequestObj == null) return false;

				cTripDeletionRequestObj.sRouteNumber = cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber;
				cTripDeletionRequestObj.sDepartureTime = cVehicleDepartureTimeObj.sVehicleDepartureTime;
				cTripDeletionRequestObj.sTripNumber = cTripNumberByDepartureTimeAndRouteNumberRequestObj.sTripNumber;

				if (Traveler.WorkSources.CDatabaseExchanger.TripDeletionRequest(ref vcPostgresqlWorkerObj, 
					ref cTripDeletionRequestObj, ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		private void DeleteTrip(object sender, MouseButtonEventArgs e)
		{
			if (cDataHubObj_ == null) return;
			var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
			if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("8", 
				"adding, editing, deletion of routes and trips") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Route");
				return;
			}

            ListView lvListViewObj = lvRoutes as ListView;
            if (lvListViewObj == null) return;
			if (lvListViewObj.SelectedIndex < 0) return;

			ListBox lbListBoxObj = (ListBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "lbRoutes", lvListViewObj.SelectedIndex);
			if (lbListBoxObj == null) return;
			if (lbListBoxObj.SelectedIndex < 0) return;

            TextBox tbListViewItemListBoxItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
				ref lbListBoxObj, "tbVehicleDepartureTime", lbListBoxObj.SelectedIndex);
            if (tbListViewItemListBoxItemTextBoxObj == null) return;

            NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime cVehicleDepartureTimeObj = 
				(NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleDepartureTime)lbListBoxObj.SelectedItem;
            if (cVehicleDepartureTimeObj == null) return;

            if ((cVehicleDepartureTimeObj.sVehicleDepartureTime == "Add trip") && 
				(cVehicleDepartureTimeObj.iVehicleTripNumber == -1)) return;

			string sErrorMessage = null;
			if(DeleteGuiTrip(ref lvListViewObj, ref lbListBoxObj, ref sErrorMessage) == false) return;
			if(DeleteDbTrip(ref lvListViewObj, ref lbListBoxObj, ref sErrorMessage) == false) return;
		}


		string sOldRouteName_ = null;
		string sOldTripName_ = null;

        private NmspTraveler.CDataHub cDataHubObj_ = null;
    }
}
