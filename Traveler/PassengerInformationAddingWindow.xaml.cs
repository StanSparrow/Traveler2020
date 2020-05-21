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
using Microsoft.Win32;
using System.ComponentModel;

namespace NmspTraveler
{
    public partial class CPassengerInformationAddingWindow : NmspTraveler.CBaseWindow
    {
        public CPassengerInformationAddingWindow(ref NmspTraveler.CDataHub cDataHubObj, 
            ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;
            cTicketDataObj_ = cTicketDataObj;
        }

        private bool ClearFreePassengerSeatNumbers()
        {
            cbAvailableSeatNumbers.Items.Clear();

            return true;
        }

        private bool SetFreePassengerSeatNumbers(ref List<string> lsVehicleNumberOfFreeSeats)
        {
            if (lsVehicleNumberOfFreeSeats == null) return false;
            if (lsVehicleNumberOfFreeSeats.Count() == 0) return false;

            foreach (string sVehicleNumberOfFreeSeat in lsVehicleNumberOfFreeSeats)
                cbAvailableSeatNumbers.Items.Add(sVehicleNumberOfFreeSeat);

            return true;
        }

        bool SetFreePassengerSeatNumbers(ref DatePicker dpDepartureDate, ref string sErrorMessage)
        {
            if (dpDepartureDate == null) return false;

            string sDepartureDate = null;
            if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, ref sDepartureDate, ref sErrorMessage) == false) return false;
     
			if (cDataHubObj_ == null) return false;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return false;

            List<string> lsNumberOfPassengerTakenSeats = null;
            if (Traveler.WorkSources.CDatabaseExchanger.TakenSeatsRequest(
				ref vcPostgresqlWorkerObj, ref cTicketDataObj_, ref lsNumberOfPassengerTakenSeats, 
				sDepartureDate, ref sErrorMessage) == false) return false;

            List<string> lsNumberOfPassengerFreeSeats = null;
            if (FreeSeatsGetting(cTicketDataObj_.sVehicleNumberOfSeats, ref lsNumberOfPassengerTakenSeats,
                ref lsNumberOfPassengerFreeSeats, ref sErrorMessage) == false) return false;

            if(SetFreePassengerSeatNumbers(ref lsNumberOfPassengerFreeSeats) == false) return false;

            return true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            DatePicker dpDepartureDateObj = dpDepartureDate as DatePicker;
            if (dpDepartureDateObj == null) return;

            string sErrorMessage = null;
            SetFreePassengerSeatNumbers(ref dpDepartureDateObj, ref sErrorMessage);     
   
			
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);
        }

        public string GetDepartureDate()
        {
            return sDepartureDate_;
        }

        public string GetPassengerFirstName()
        {
            return sPassengerFirstName_;
        }

        public string GetPassengerLastName()
        {
            return sPassengerLastName_;
        }

        public string GetPassengerSeatNumber()
        {
            return sPassengerSeatNumber_;
        }

        public string GetPassengerDriverLicenseNumber()
        {
            return sPassengerDriverLicenseNumber_;
        }

        public string GetPassengerDriverLicensePictureAsString()
        {
            return sPassengerDriverLicensePicture_;
        }

        private bool FreeSeatsGetting(string sVehicleNumberOfSeats, ref List<string> lsNumberOfPassengerTakenSeats, 
            ref List<string> lsNumberOfPassengerFreeSeats, ref string sErrorMessage)
        {
            try
            {
                if (sVehicleNumberOfSeats == null) return false;
                int iVehicleNumberOfSeats = int.Parse(sVehicleNumberOfSeats);
                if (iVehicleNumberOfSeats == 0) return false;

                if (lsNumberOfPassengerTakenSeats == null) return false;

                if (lsNumberOfPassengerFreeSeats != null) return false;
                lsNumberOfPassengerFreeSeats = new List<string>();
                if (lsNumberOfPassengerFreeSeats == null) return false;

                bool bContinue = false;
                for (int iVehicleNumberOfSeatsIndex = 1;
                    iVehicleNumberOfSeatsIndex <= iVehicleNumberOfSeats;
                    iVehicleNumberOfSeatsIndex++)
                {
                    bContinue = false;
                    foreach (string sNumberOfPassengerTakenSeat in lsNumberOfPassengerTakenSeats)
                    {
                        if (iVehicleNumberOfSeatsIndex.ToString() == sNumberOfPassengerTakenSeat)
                        {
                            bContinue = true;
                            break;
                        }
                    }

                    if (bContinue == true) continue;

                    lsNumberOfPassengerFreeSeats.Add(iVehicleNumberOfSeatsIndex.ToString());
                }
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        bool ValidatingPassengerInformation(ref string sMessage)
        {
            if (NmspTraveler.CString.IsStringEmpty(sDepartureDate_) == true)
            {
                sMessage = "Set the Departure Date first!";
                return false; 
            }

            if (NmspTraveler.CString.IsStringEmpty(sPassengerFirstName_) == true)
            {
                sMessage = "Set the Passenger First Name first!";
                return false; 
            }

            if (NmspTraveler.CString.IsStringEmpty(sPassengerLastName_) == true)
            {
                sMessage = "Set the Passenger Last Name first!";
                return false;
            }

            if ((NmspTraveler.CString.IsStringEmpty(sPassengerSeatNumber_) == true) || (sPassengerSeatNumber_ == "No seat"))
            {
                sMessage = "Set the Passenger Seat Number first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(sPassengerDriverLicenseNumber_) == true)
            {
                sMessage = "Set the Passenger Driver License Number first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(sPassengerDriverLicensePicture_) == true)
            {
                sMessage = "Set the Passenger Driver License Picture first!";
                return false;
            }
                
            sMessage = "The ticket has printed!";
            return true;
        }
            
        private void OnOkButton(object sender, RoutedEventArgs e)
        {
            if ((BackgroundWorkerObj_ != null) && (BackgroundWorkerObj_.IsBusy == true))
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"Wait for the upload to finish!", 
					"Traveler - Passenger Information");
                return;
            }

			string sErrorMessage = null;
            if (NmspTraveler.CGui.DataPickerDateToFormatStringDate(ref dpDepartureDate, ref sDepartureDate_, ref sErrorMessage) == false) return;

            sPassengerFirstName_ = tbPassengerFirstName.Text;
            sPassengerLastName_ = tbPassengerLastName.Text;

            ComboBox cbSeatNumbers = cbAvailableSeatNumbers as ComboBox;
            if ((cbSeatNumbers != null) && (cbSeatNumbers.SelectedValue != null))               
                sPassengerSeatNumber_ = cbSeatNumbers.SelectedValue.ToString();
       
            sPassengerDriverLicenseNumber_ = tbPassengerDriverLicenseNumber.Text;

            string sMessage = null;
            bool bRetVal = ValidatingPassengerInformation(ref sMessage);
            
            if (bRetVal == false)
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					sMessage, "Traveler - Passenger Information");
                return;
            }
                   
            SetDialogResult(true);
            this.Close();
        }

        private void OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((BackgroundWorkerObj_ != null) && (BackgroundWorkerObj_.IsBusy == true))
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"Wait for the upload to finish!", 
					"Traveler - Passenger Information");
                return;
            }

            DatePicker dpDepartureDateObj = sender as DatePicker;
            if (dpDepartureDateObj == null) return;
            if (dpDepartureDateObj.IsDropDownOpen == true)
            {
                ClearFreePassengerSeatNumbers();
                string sErrorMessage = null;
                SetFreePassengerSeatNumbers(ref dpDepartureDateObj, ref sErrorMessage);
            }         
        }

        private void OnCancelButton(object sender, RoutedEventArgs e)
        {
            if ((BackgroundWorkerObj_ != null) && (BackgroundWorkerObj_.IsBusy == true))
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"Wait for the upload to finish!", 
					"Traveler - Passenger Information");
                return;
            }

            SetDialogResult(false);
            this.Close();
        }

        private void OnAddPassengerDriverLicensePictureButton(object sender, RoutedEventArgs e)
        {
            if ((BackgroundWorkerObj_ != null) && (BackgroundWorkerObj_.IsBusy == true))
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"Wait for the upload to finish!", 
					"Traveler - Passenger Information");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false) return;

            BackgroundWorkerObj_ = null;
            BackgroundWorkerObj_ = new BackgroundWorker();
            BackgroundWorkerObj_.DoWork += AddPassengerDriverLicensePictureBeginThread;
            BackgroundWorkerObj_.RunWorkerCompleted += AddPassengerDriverLicensePictureEndThread;
            BackgroundWorkerObj_.RunWorkerAsync(openFileDialog.FileName);

            mePassengerDriverLicensePicture.Opacity = 1.0;
            mePassengerDriverLicensePicture.Stretch = Stretch.Fill;
            mePassengerDriverLicensePicture.Source = new Uri(@"pack://siteoforigin:,,,/Resource/Gifs/BusMovement.gif");
        }

        private void AddPassengerDriverLicensePictureBeginThread(object sender, DoWorkEventArgs e)
        {
            string sFileName_lv = (string)e.Argument;

            string sErrorMessage = null;
			string sPassengerDriverLicensePicture = null;
            if (NmspTraveler.WorkSources.CPicture.PictureFileNameToPictureByteString(sFileName_lv,
                ref sPassengerDriverLicensePicture, ref sErrorMessage) == false)
                e.Cancel = true;
            else
            {
                e.Cancel = false;
				e.Result = new NmspTraveler.WorkSources.NmspWorkingStructures.
					NmspPassengerInformationAddingWindow.CPassengerDriverLicensePicture() {  
					sFileName = sFileName_lv, sPictureByteString = sPassengerDriverLicensePicture };
            }
        }

        private void AddPassengerDriverLicensePictureEndThread(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
                
            }
            else
            {
                mePassengerDriverLicensePicture.Opacity = 1.0;
                mePassengerDriverLicensePicture.Stretch = Stretch.None;

				NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerInformationAddingWindow.
					CPassengerDriverLicensePicture cPassengerDriverLicensePictureObj = 
					(NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerInformationAddingWindow.
					CPassengerDriverLicensePicture)e.Result;

				if (cPassengerDriverLicensePictureObj == null) return;

				sPassengerDriverLicensePicture_ = cPassengerDriverLicensePictureObj.sPictureByteString;
				mePassengerDriverLicensePicture.Source = new Uri(cPassengerDriverLicensePictureObj.sFileName);
            }
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            mePassengerDriverLicensePicture.Position = new TimeSpan(0, 0, 1);
            mePassengerDriverLicensePicture.Play();
        }

        override protected void OnCloseButton(object sender, RoutedEventArgs e)
        {
            if ((BackgroundWorkerObj_ != null) && (BackgroundWorkerObj_.IsBusy == true))
            {
                NmspTraveler.CGui.ShowMessageBox(this, 
					"Wait for the upload to finish!", 
					"Traveler - Passenger Information");
                return;
            }

            SetDialogResult(false);
            this.Close();
        }


        private string sDepartureDate_ = null;
        private string sPassengerFirstName_ = null;
        private string sPassengerLastName_ = null;
        private string sPassengerSeatNumber_ = null;
        private string sPassengerDriverLicenseNumber_ = null;
        private string sPassengerDriverLicensePicture_ = null;

        private BackgroundWorker BackgroundWorkerObj_ = null;

        private NmspTraveler.CDataHub cDataHubObj_ = null;
        private NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj_ = null;
    }
}
