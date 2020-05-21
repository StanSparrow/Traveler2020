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
using System.Text.RegularExpressions;
using System.IO;


namespace NmspTraveler
{
    public partial class CTripWindow : NmspTraveler.CBaseWindow
    {
        public CTripWindow(
			ref NmspTraveler.CDataHub cDataHubObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo cTripInfoObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;

			cTripInfoObj_ = cTripInfoObj;

            cTicketDataObj_ = new NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData();

            List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData> lCVehicleTripDataObj = 
				new List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData>();
            if(lCVehicleTripDataObj == null)return;

            VehicleTripDataRequest(cTripInfoObj.sRouteName, cTripInfoObj.sDepartureTime, ref lCVehicleTripDataObj);  
            
            lvTrips.ItemsSource = lCVehicleTripDataObj;
        }

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			string sErrorMessage = null;
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);

		}

        private void AddListViewItem(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData> lCVehicleTripDataObj, 
			string sKey, string sValue)
        {
            lCVehicleTripDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData() { sTripKey = sKey, sTripValue = sValue });
        }

        private void AddListViewItem(
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData> lCVehicleTripDataObj, 
			string sKey, string sValue, ref ImageSource isPicture)
        {
            lCVehicleTripDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData() { sTripKey = sKey, sTripValue = sValue, 
                isTripPicture = isPicture });
        }


		


        private bool VehicleTripDataRequest(
            string sVehicleRouteName, string sVehicleDepartureTime,
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData> lCVehicleTripDataObj)
        {
            if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;

            String[] sarrDelimiter = { " - " };
            var vVehicleRouteName = sVehicleRouteName.Split(sarrDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (vVehicleRouteName == null) return false;
            if (vVehicleRouteName.Count() != 2) return false;
            cTicketDataObj_.sFrom = vVehicleRouteName[0];
            cTicketDataObj_.sTo = vVehicleRouteName[1];

            cTicketDataObj_.sDepartureTime = sVehicleDepartureTime;

			string sVehicleRouteNumber = null;
			string sErrorMessage = null;
			if (Traveler.WorkSources.CDatabaseExchanger.RouteRequest(ref vcPostgresqlWorkerObj, 
				ref cTicketDataObj_, sVehicleRouteName, ref sVehicleRouteNumber, ref sErrorMessage) == false) return false;

			string sVehicleNumber = null;
			string sVehicleDriverNumber = null;
			if (Traveler.WorkSources.CDatabaseExchanger.TripRequest(ref vcPostgresqlWorkerObj, 
				ref cTicketDataObj_, sVehicleRouteNumber, sVehicleDepartureTime, ref sVehicleDriverNumber, 
				ref sVehicleNumber, ref sErrorMessage) == false) return false;

			string sPictureByteString = null;
			string sAdditionalDriverInformation = null;
			if (Traveler.WorkSources.CDatabaseExchanger.DriverRequest(ref vcPostgresqlWorkerObj,
				ref cTicketDataObj_, sVehicleDriverNumber, ref sPictureByteString, 
				ref sAdditionalDriverInformation, ref sErrorMessage) == false) return false;


			ImageSource isDriverPicture = null;
			if (NmspTraveler.WorkSources.CPicture.DataBasePictureBytesToImageSource(sPictureByteString,
				ref isDriverPicture, ref sErrorMessage) == false) return false;

			sPictureByteString = null;
			string sAdditionalVehicleInformation = null;
			if (Traveler.WorkSources.CDatabaseExchanger.VehicleRequest(ref vcPostgresqlWorkerObj,
				ref cTicketDataObj_, sVehicleNumber, ref sPictureByteString, 
				ref sAdditionalVehicleInformation, ref sErrorMessage) == false) return false;

            ImageSource isVehiclePicture = null;
            NmspTraveler.WorkSources.CPicture.DataBasePictureBytesToImageSource(sPictureByteString,
                    ref isVehiclePicture, ref sErrorMessage);


            AddListViewItem(ref lCVehicleTripDataObj, "Route Name", sVehicleRouteName);
            AddListViewItem(ref lCVehicleTripDataObj, "Departure Time, (HH:MM)", sVehicleDepartureTime);
            AddListViewItem(ref lCVehicleTripDataObj, "Arrival Time, (HH:MM)", "HH:MM Xm");
            AddListViewItem(ref lCVehicleTripDataObj, "Travel Time, (HH:MM)", cTicketDataObj_.sTravelTime);
            AddListViewItem(ref lCVehicleTripDataObj, "Driver", cTicketDataObj_.sDriver + sAdditionalDriverInformation, ref isDriverPicture);
            AddListViewItem(ref lCVehicleTripDataObj, "Vehicle", cTicketDataObj_.sVehicle + sAdditionalVehicleInformation, ref isVehiclePicture);
            AddListViewItem(ref lCVehicleTripDataObj, "Price, ($)", cTicketDataObj_.sPrice);

            return true;
        }


        private void SetListViewItemColor(ref ListView lvListViewObj)
        {
            TextBox tbListViewItemTextBoxObj = null;
			Border brdTripListViewItemPictureBorderObj = null;

            for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
            {
                if (iListViewItemIndex == lvListViewObj.SelectedIndex) continue;

                tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemKey", iListViewItemIndex);
                if (tbListViewItemTextBoxObj == null) continue;
                tbListViewItemTextBoxObj.Foreground = Brushes.White;
                tbListViewItemTextBoxObj.Background = Brushes.Transparent;

                tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", iListViewItemIndex);
                if (tbListViewItemTextBoxObj == null) continue;
                tbListViewItemTextBoxObj.Foreground = Brushes.White;
                tbListViewItemTextBoxObj.Background = Brushes.Transparent;
                tbListViewItemTextBoxObj.Focusable =  false;
      
				brdTripListViewItemPictureBorderObj = NmspTraveler.CGui.GetBorderElement(ref lvListViewObj, "brdTripListViewItemPictureBorder", iListViewItemIndex);
				if (brdTripListViewItemPictureBorderObj != null)
					brdTripListViewItemPictureBorderObj.BorderBrush = Brushes.White;
            }

            tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemKey", lvListViewObj.SelectedIndex);
            if (tbListViewItemTextBoxObj != null)  
                tbListViewItemTextBoxObj.Foreground = Brushes.Gray;

            tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", lvListViewObj.SelectedIndex);
            if (tbListViewItemTextBoxObj != null)
                tbListViewItemTextBoxObj.Foreground = Brushes.Gray;

			brdTripListViewItemPictureBorderObj = NmspTraveler.CGui.GetBorderElement(ref lvListViewObj, "brdTripListViewItemPictureBorder", lvListViewObj.SelectedIndex);
			if (brdTripListViewItemPictureBorderObj != null)
				brdTripListViewItemPictureBorderObj.BorderBrush = Brushes.Gray;
        }

        private void EnableListViewItem(ref ListView lvListViewObj, int iSeletectItemIndex, bool bEnable)
        {
            ListViewItem lvListViewItem = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iSeletectItemIndex) as ListViewItem;
            if (lvListViewItem == null) return;
            lvListViewItem.IsEnabled = bEnable;
        }

        private void EnableListViewItems(ref ListView lvListViewObj, ref List< int > liFilteredIndex, bool bEnable)
        {
            bool bFilteredIndex = false;
            ListViewItem lvListViewItem = null;
            for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
            {
                bFilteredIndex = false;
                for (int iFilteredIndex = 0; iFilteredIndex < liFilteredIndex.Count; iFilteredIndex++)
                {
                    if (iListViewItemIndex == liFilteredIndex[iFilteredIndex])
                    {
                        bFilteredIndex = true;
                        break;
                    }
                }

                if(bFilteredIndex == true)continue;  

                lvListViewItem = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iListViewItemIndex) as ListViewItem;
                lvListViewItem.IsEnabled = bEnable;
            }
        }
   
        private void PreviewExecutedTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData cVehicleTripDataObj = 
				tb.DataContext as NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData;
            if (cVehicleTripDataObj == null) return;

            String sClipboardText = Clipboard.GetText(TextDataFormat.Text);
            if (sClipboardText == null) return;
           
            if (e.Command == ApplicationCommands.Paste)
            {      
                if ((cVehicleTripDataObj.sTripKey == "Travel Time, (HH:MM)"))
                {    
                    e.Handled = true;
                }
                else if ((cVehicleTripDataObj.sTripKey == "Price, ($)"))
                { 
                    e.Handled = true;
                }
            }
            else if(e.Command == ApplicationCommands.Cut)
            {
                if ((cVehicleTripDataObj.sTripKey == "Travel Time, (HH:MM)"))
                {
                    e.Handled = true;
                }
                else if ((cVehicleTripDataObj.sTripKey == "Price, ($)"))
                {
                    e.Handled = true;
                }
            }
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            ListView lvListViewObj = lvTrips as ListView;
            if (lvListViewObj == null) return;
            if ((lvListViewObj.SelectedIndex < 0) || (lvListViewObj.SelectedIndex >= lvListViewObj.Items.Count)) return;

            TextBox tb = sender as TextBox;
            if (tb == null) return;
                
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);  
        }

        private void OnTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            ListView lvListViewObj = lvTrips as ListView;
            if (lvListViewObj == null) return;
            if ((lvListViewObj.SelectedIndex < 0) || (lvListViewObj.SelectedIndex >= lvListViewObj.Items.Count)) return;

            if (lvListViewObj.SelectedIndex == 3)
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                int iCaretIndex = tb.CaretIndex;
                string sTextBoxText = tb.Text;
                StringBuilder sbTextBoxTextNew = new System.Text.StringBuilder(sTextBoxText);

                var v = e.Changes.ElementAt(0);

                int i = 0;
                while (i < (v.RemovedLength - v.AddedLength))
                {
                    sbTextBoxTextNew.Insert(v.Offset + i, v.Offset + i == 2 ? ':' : '0');
                    i++;
                }

                if ((sbTextBoxTextNew.Length >= 2) && (sbTextBoxTextNew[2] != ':')) sbTextBoxTextNew[2] = ':';

                tb.Text = sbTextBoxTextNew.ToString();

                if (iCaretIndex > 0)
                    tb.CaretIndex = iCaretIndex;

                tb.ScrollToHorizontalOffset(iCaretIndex);
                
                e.Handled = true;   
            }
            if (lvListViewObj.SelectedIndex == 6)
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                int iCaretIndex = tb.CaretIndex;
                string sTextBoxText = tb.Text;
                StringBuilder sbTextBoxTextNew = new System.Text.StringBuilder(sTextBoxText);

                var v = e.Changes.ElementAt(0);

                int i = 0;
                while (i < (v.RemovedLength - v.AddedLength))
                {
                    sbTextBoxTextNew.Insert(v.Offset + i, v.Offset + i == 3 ? '.' : '0');
                    i++;
                }

                if ((sbTextBoxTextNew.Length >= 3) && (sbTextBoxTextNew[3] != '.')) sbTextBoxTextNew[3] = '.';

                tb.Text = sbTextBoxTextNew.ToString();

                if (iCaretIndex > 0)
                    tb.CaretIndex = iCaretIndex;

                tb.ScrollToHorizontalOffset(iCaretIndex);

                e.Handled = true;
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(tb == null)return;
            NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData cVehicleTripDataObj = 
				tb.DataContext as NmspTraveler.WorkSources.NmspWorkingStructures.CVehicleTripData;
            if (cVehicleTripDataObj == null) return;
            StringBuilder sbTextBoxText = new System.Text.StringBuilder(tb.Text);
            if (sbTextBoxText == null) return;
            int iCaretIndex = tb.CaretIndex;

            if ((cVehicleTripDataObj.sTripKey == "Travel Time, (HH:MM)"))
            {
                bool bInputAllowed = (!(new Regex("[^0-9]+").IsMatch(e.Text)) && !((tb.CaretIndex == 3) && (int.Parse(e.Text) > 5)));

                if (bInputAllowed == true)
                {
                    if ((tb.CaretIndex < sbTextBoxText.Length) && (e.Text.Count() > 0)) sbTextBoxText[tb.CaretIndex] = e.Text[0];
                    tb.Text = sbTextBoxText.ToString();
                    tb.CaretIndex = iCaretIndex + 1;
                    tb.ScrollToHorizontalOffset(iCaretIndex);
                }

                e.Handled = !bInputAllowed || ((tb.Text.Count() == 5) && (tb.SelectedText.Count() == 0));
            }
            else if((cVehicleTripDataObj.sTripKey == "Price, ($)"))
            {
                bool bInputAllowed = !(new Regex("[^0-9]+").IsMatch(e.Text));

                if (bInputAllowed == true)
                {
                    if ((tb.CaretIndex < sbTextBoxText.Length) && (e.Text.Count() > 0)) sbTextBoxText[tb.CaretIndex] = e.Text[0];
                    tb.Text = sbTextBoxText.ToString();
                    tb.CaretIndex = iCaretIndex + 1;
                    tb.ScrollToHorizontalOffset(iCaretIndex);
                }

                e.Handled = !bInputAllowed || ((tb.Text.Count() == 6) && (tb.SelectedText.Count() == 0));
            }    
        }

        private void OnListViewMouseMove(object sender, MouseEventArgs e)
        {
            ListView vListViewObj = lvTrips as ListView;

            if (vListViewObj != null)
                SetListViewItemColor(ref vListViewObj);
        }

        private string GetArrivalTime(string sDepartureTime_fp, string sTravelTime_fp)
        {
            if ((sDepartureTime_fp == null) || (sDepartureTime_fp.Count() < 8)) return null;
            if ((sTravelTime_fp == null) || (sTravelTime_fp.Count() < 5)) return null;
            string[] sarrDepartureTime = sDepartureTime_fp.Split(' ');
            if ((sarrDepartureTime == null) || (sarrDepartureTime.Count() < 2)) return null;

            string[] sarrDepartureTimeHoursAndMinutes = sarrDepartureTime[0].Split(':');
            if ((sarrDepartureTimeHoursAndMinutes == null) || (sarrDepartureTimeHoursAndMinutes.Count() < 2)) return null;

            string[] sarrTravelTimeHoursAndMinutes = sTravelTime_fp.Split(':');
            if ((sarrTravelTimeHoursAndMinutes == null) || (sarrTravelTimeHoursAndMinutes.Count() < 2)) return null;

            int iSumHours = int.Parse(sarrDepartureTimeHoursAndMinutes[0]) + int.Parse(sarrTravelTimeHoursAndMinutes[0]);
            int iSumMinutes = int.Parse(sarrDepartureTimeHoursAndMinutes[1]) + int.Parse(sarrTravelTimeHoursAndMinutes[1]);

            int iAdditioanalHours = 0;
            if (iSumMinutes > 60)
            {
                iAdditioanalHours = iSumMinutes / 60;
                iSumMinutes %= 60;
                iSumHours += iAdditioanalHours;
            }

            int i12HourCycle = 0;
            if(iSumHours > 12)
            {
                i12HourCycle = iSumHours / 12;
                iSumHours %= 12;
            }

            if (iSumHours == 0) iSumHours = 12;

            string sSumHours = "0";
            if(iSumHours < 10)
                sSumHours += iSumHours.ToString();  
            else 
                sSumHours = iSumHours.ToString();

            string sSumMinutes = "0";
            if (iSumMinutes < 10)
                sSumMinutes += iSumMinutes.ToString();
            else 
                sSumMinutes = iSumMinutes.ToString();

            int iBeginPartIndex = 0;
            string[] sXmParts = { "am", "pm" };
            string sXmPart = sarrDepartureTime[1];
            for (int i = 0; i < sXmParts.Count(); i++)
            {
                if(sXmParts[i] == sXmPart)
                {
                    iBeginPartIndex = i;
                    break;
                }
            }

            sXmPart = sXmParts[(iBeginPartIndex + i12HourCycle) % sXmParts.Count()];

            return sSumHours + ":" + sSumMinutes + " " + sXmPart;
        }

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            clmTripGridViewListViewValueColumn.Width = lvTrips.ActualWidth - clmTripGridViewListViewKeyColumn.ActualWidth;

            ListView lvListViewObj = lvTrips as ListView;
            if (lvListViewObj == null) return;

            string sDepartureTime = null;
            string sTravelTime = null;
            TextBox tbListViewItemKeyTextBoxObj = null;
            TextBox tbListViewItemValueTextBoxObj = null;
			Border brdTripListViewItemPictureBorderObj = null;
            Image iTripListViewItemAdditionalPictureValueImageObj = null;
            TextBox tbListViewItemArrivalTimeTextBoxObj = null;
            for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
            {
                tbListViewItemKeyTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemKey", iListViewItemIndex);
                if (tbListViewItemKeyTextBoxObj == null) continue;

                tbListViewItemValueTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", iListViewItemIndex);
                if (tbListViewItemValueTextBoxObj == null) continue;

                if (tbListViewItemKeyTextBoxObj.Text == "Departure Time, (HH:MM)")
                    sDepartureTime = tbListViewItemValueTextBoxObj.Text;

                if (tbListViewItemKeyTextBoxObj.Text == "Arrival Time, (HH:MM)")
                    tbListViewItemArrivalTimeTextBoxObj = tbListViewItemValueTextBoxObj;

                if (tbListViewItemKeyTextBoxObj.Text == "Travel Time, (HH:MM)")
                    sTravelTime = tbListViewItemValueTextBoxObj.Text;

                if ((tbListViewItemKeyTextBoxObj.Text == "Driver") || (tbListViewItemKeyTextBoxObj.Text == "Vehicle"))
                {
					iTripListViewItemAdditionalPictureValueImageObj = NmspTraveler.CGui.GetImageElement(ref lvListViewObj, "iTripListViewItemAdditionalPictureValue", iListViewItemIndex);
					if (iTripListViewItemAdditionalPictureValueImageObj == null) continue;

					brdTripListViewItemPictureBorderObj = NmspTraveler.CGui.GetBorderElement(ref lvListViewObj, "brdTripListViewItemPictureBorder", iListViewItemIndex);
					if (brdTripListViewItemPictureBorderObj == null) continue;

					brdTripListViewItemPictureBorderObj.Visibility = System.Windows.Visibility.Visible;

                    iTripListViewItemAdditionalPictureValueImageObj.Visibility = System.Windows.Visibility.Visible;
                }
            }
 
            cTicketDataObj_.sArrivalTime = GetArrivalTime(sDepartureTime, sTravelTime);

            if (tbListViewItemArrivalTimeTextBoxObj != null)
                tbListViewItemArrivalTimeTextBoxObj.Text = cTicketDataObj_.sArrivalTime;

            EnableListViewItem(ref lvListViewObj, 0, false);
            EnableListViewItem(ref lvListViewObj, 1, false);
            EnableListViewItem(ref lvListViewObj, 2, false);

            //SetHeightListViewItem(ref lvListViewObj, 4, 200);
            //SetHeightListViewItem(ref lvListViewObj, 5, 200);
        }

        bool SetHeightListViewItem(ref ListView lvListViewObj, int iItemIndex, int iHeight)
        {
            ListViewItem lvListViewItem = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex) as ListViewItem;
            if (lvListViewItem == null) return false;
            lvListViewItem.Height = iHeight;
            return true;
        }

        private void OnListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView lvListViewObj = lvTrips as ListView;
           
            if (lvListViewObj != null)
                SetListViewItemColor(ref lvListViewObj);
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            ListView lvListViewObj = lvTrips as ListView;
            if (lvListViewObj == null) return;

            int iSeletectItemIndex = lvListViewObj.SelectedIndex;
            if ((iSeletectItemIndex < 0) || (iSeletectItemIndex >= lvListViewObj.Items.Count)) return;

            TextBox tbListViewItemTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(
				ref lvListViewObj, "tbTripListViewItemValue", iSeletectItemIndex);
            if (tbListViewItemTextBoxObj == null) return;

            Image iTripListViewItemAdditionalPictureValueImageObj = NmspTraveler.CGui.GetImageElement(
				ref lvListViewObj, "iTripListViewItemAdditionalPictureValue", iSeletectItemIndex);
            if (iTripListViewItemAdditionalPictureValueImageObj == null) return;


            if ((Keyboard.IsKeyDown(Key.F2) == true) && (Keyboard.IsKeyDown(Key.T) == true))
            {
                if(iSeletectItemIndex == 3)
                {
					if (cDataHubObj_ == null) return;
					var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
					if (vcAccessManagerObj == null) return;
					if (vcAccessManagerObj.GetOperationPermissionByArea("8", 
						"adding, editing, deletion of routes and trips") == false)
					{
						NmspTraveler.CGui.ShowMessageBox(this, 
							"You don't have permission to perform this operation!", 
							"Traveler - Trip");
						return;
					}

                    if (tbListViewItemTextBoxObj.Focusable == false)
                    {
                        List<int> li = new List<int>() { 0, 1, 2, iSeletectItemIndex };
                        EnableListViewItems(ref lvListViewObj, ref li, false);

                        tbListViewItemTextBoxObj.Focusable = true;
                        tbListViewItemTextBoxObj.SelectAll();
                        tbListViewItemTextBoxObj.Focus();
                    }
                }
                else if(iSeletectItemIndex == 4)
                {
					if (cDataHubObj_ == null) return;
					var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
					if (vcAccessManagerObj == null) return;
					if (vcAccessManagerObj.GetOperationPermissionByArea("6", "view of the driver list") == false)
					{
						NmspTraveler.CGui.ShowMessageBox(this, 
							"You don't have permission to perform this operation!", 
							"Traveler - Trip");
						return;
					}

                    NmspTraveler.CDriverWindow cDriverWindowObj = new NmspTraveler.CDriverWindow(ref cDataHubObj_);
                    
                    cDriverWindowObj.Owner = this;
                    cDriverWindowObj.ShowDialog();

                    if (!cDriverWindowObj.DialogResult.HasValue || !cDriverWindowObj.DialogResult.Value)
                        return;

                    string sDriverName = cDriverWindowObj.GetFirstName();
                    sDriverName += " ";
                    sDriverName += cDriverWindowObj.GetLastName();
                    sDriverName += ", ";
                    sDriverName += NmspTraveler.WorkSources.CDataTime.GetYearsUntillNow(cDriverWindowObj.GetBirthDate());
                    sDriverName += " years old, ";
                    sDriverName += cDriverWindowObj.GetGender();

                    string sAdditionalDriverInformation = ", experience ";
                    sAdditionalDriverInformation += cDriverWindowObj.GetExperience();
                    sAdditionalDriverInformation += " years, rating ";
                    sAdditionalDriverInformation += cDriverWindowObj.GetRating();

					sDriverName += sAdditionalDriverInformation;

                    tbListViewItemTextBoxObj.Text = sDriverName;
                    iTripListViewItemAdditionalPictureValueImageObj.Source = cDriverWindowObj.GetPicture();
                }
                else if(iSeletectItemIndex == 5)
                {
					if (cDataHubObj_ == null) return;
					var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
					if (vcAccessManagerObj == null) return;
					if (vcAccessManagerObj.GetOperationPermissionByArea("7", 
						"view of the vehicle list") == false)
					{
						NmspTraveler.CGui.ShowMessageBox(this, 
							"You don't have permission to perform this operation!", 
							"Traveler - Trip");
						return;
					}

                    NmspTraveler.CVehicleWindow cVehicleWindowObj = new NmspTraveler.CVehicleWindow(ref cDataHubObj_);
                    cVehicleWindowObj.Width = this.Width;
                    cVehicleWindowObj.Height = this.Height;
                    cVehicleWindowObj.Owner = this;
                    cVehicleWindowObj.ShowDialog();

                    if (!cVehicleWindowObj.DialogResult.HasValue || !cVehicleWindowObj.DialogResult.Value)
                        return;

                    string sVehicleName = cVehicleWindowObj.GetVehicleType();
                    sVehicleName += ", ";
                    sVehicleName += cVehicleWindowObj.GetVehicleModel();
                    sVehicleName += ", ";
					sVehicleName += cVehicleWindowObj.GetVehiclePassengerCapacity();
					sVehicleName += " seats, ";
                    sVehicleName += cVehicleWindowObj.GetVehicleLicensePlate();

					string sAdditionalVehicleInformation = ", ";
                    sAdditionalVehicleInformation += cVehicleWindowObj.GetVehicleYearOfManufacture();
                    sAdditionalVehicleInformation += ", ";
                    sAdditionalVehicleInformation += cVehicleWindowObj.GetVehicleCountryOfOrigin();
                    sAdditionalVehicleInformation += ", technical state ";
                    sAdditionalVehicleInformation += cVehicleWindowObj.GetVehicleTechnicalState();
                    sAdditionalVehicleInformation += ", rating ";
                    sAdditionalVehicleInformation += cVehicleWindowObj.GetVehicleRating();

					sVehicleName += sAdditionalVehicleInformation;

                    tbListViewItemTextBoxObj.Text = sVehicleName;
                    iTripListViewItemAdditionalPictureValueImageObj.Source = cVehicleWindowObj.GetVehiclePicture();
                }
                else if (iSeletectItemIndex == 6)
                {
					if (cDataHubObj_ == null) return;
					var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
					if (vcAccessManagerObj == null) return;
					if (vcAccessManagerObj.GetOperationPermissionByArea("8", 
						"adding, editing, deletion of routes and trips") == false)
					{
						NmspTraveler.CGui.ShowMessageBox(this, 
							"You don't have permission to perform this operation!", 
							"Traveler - Trip");
						return;
					}

                    if (tbListViewItemTextBoxObj.Focusable == false)
                    {
                        List<int> li = new List<int>() { 0, 1, 2, iSeletectItemIndex };
                        EnableListViewItems(ref lvListViewObj, ref li, false);

                        tbListViewItemTextBoxObj.Focusable = true;
                        tbListViewItemTextBoxObj.SelectAll();
                        tbListViewItemTextBoxObj.Focus();
                    }
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (iSeletectItemIndex == 3)
                {
                    if (tbListViewItemTextBoxObj.Focusable == true)
                        tbListViewItemTextBoxObj.Focusable = false;

                    List<int> li = new List<int>() { 0, 1, 2, iSeletectItemIndex };
                    EnableListViewItems(ref lvListViewObj, ref li, true);

                    TextBox tbListViewItemDepartureTimeTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", 1);
                    if (tbListViewItemDepartureTimeTextBoxObj == null) return;

                    TextBox tbListViewItemArrivalTimeTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", 2);
                    if (tbListViewItemArrivalTimeTextBoxObj == null) return;

                    TextBox tbListViewItemTravelTimeTextBoxObj = (TextBox)NmspTraveler.CGui.GetElementObj(ref lvListViewObj, "tbTripListViewItemValue", 3);
                    if (tbListViewItemArrivalTimeTextBoxObj == null) return;

                    tbListViewItemArrivalTimeTextBoxObj.Text = GetArrivalTime(tbListViewItemDepartureTimeTextBoxObj.Text, 
                        tbListViewItemTravelTimeTextBoxObj.Text);
                }
                else if (iSeletectItemIndex == 6)
                {
                    if (tbListViewItemTextBoxObj.Focusable == true)
                        tbListViewItemTextBoxObj.Focusable = false;

                    List<int> li = new List<int>() { 0, 1, 2, iSeletectItemIndex };
                    EnableListViewItems(ref lvListViewObj, ref li, true);
                }     
            }
        }

        private void OnDeleteTripButton(object sender, RoutedEventArgs e)
        {

        }

        private void OnPrintTicketButton(object sender, RoutedEventArgs e)
        {
			if (cDataHubObj_ == null) return;
            var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
            if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("2", 
				"selling and printing of the ticket") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Trip");
				return;
			}

            if (cTicketDataObj_ == null) return;
            if (cDataHubObj_ == null) return;

            NmspTraveler.CTicketWindow cTicketWindowObj = new NmspTraveler.CTicketWindow(ref cDataHubObj_, ref cTicketDataObj_);
            if (cTicketWindowObj == null) return;

            cTicketWindowObj.ShowDialog();
        }

		private void OnPassengersButton(object sender, RoutedEventArgs e)
		{
			if (cDataHubObj_ == null) return;
            var vcAccessManagerObj = cDataHubObj_.GetAccessManagerObject();
            if (vcAccessManagerObj == null) return;
			if (vcAccessManagerObj.GetOperationPermissionByArea("11", 
				"view of the passengers list") == false)
			{
				NmspTraveler.CGui.ShowMessageBox(this, 
					"You don't have permission to perform this operation!", 
					"Traveler - Trip");
				return;
			}

            if (cTicketDataObj_ == null) return;
            if (cDataHubObj_ == null) return;
			if (cTripInfoObj_ == null) return;

			cTripInfoObj_.sTripNumber = cTicketDataObj_.sVehicleTripNumber;

			NmspTraveler.CPassengerWindow cPassengerWindowObj = 
				new NmspTraveler.CPassengerWindow(ref cDataHubObj_, cTripInfoObj_);
            if (cPassengerWindowObj == null) return;

            cPassengerWindowObj.ShowDialog();
		}

		
        private NmspTraveler.CDataHub cDataHubObj_ = null;
        private NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj_ = null;
		private NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CTripInfo cTripInfoObj_ = null;
    }
}

