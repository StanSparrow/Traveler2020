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
    public partial class CTicketWindow : NmspTraveler.CBaseWindow
    {
        public CTicketWindow(
			ref NmspTraveler.CDataHub cDataHubObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj)
        {
            InitializeComponent();

            cDataHubObj_ = cDataHubObj;
            cTicketDataObj_ = cTicketDataObj;

            lsVehicleNumberOfFreeSeats_ = new List<string>();       
        }

        bool MaxTicketNumberIncrementing(ref string sTicketNumber, ref string sErrorMessage)
        {
            try
            {
                if(sTicketNumber == null)return false;
                if (sTicketNumber.Count() > 6) return false;
                if (sTicketNumber == "999999") return false;

                if (sTicketNumber.Count() == 0)
                    sTicketNumber = "163100";

                sTicketNumber = (int.Parse(sTicketNumber) + 1).ToString();
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        bool MaxTicketNumberStretching(ref string sTicketNumber, ref string sErrorMessage)
        {
            try 
            {
                StringBuilder sbTicketNumber = new StringBuilder();
                if (sbTicketNumber == null) return false;
                foreach (char chTicketNumber in sTicketNumber)
                {
                    sbTicketNumber.Append(chTicketNumber);
                    sbTicketNumber.Append(' ');
                }

                sTicketNumber = sbTicketNumber.ToString();
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (cTicketDataObj_ == null) return;

            string sErrorMessage = null;

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

            if (Traveler.WorkSources.CDatabaseExchanger.MaxTicketNumberRequest(ref vcPostgresqlWorkerObj, 
				ref cTicketDataObj_, ref cTicketDataObj_.sTicketNumber, ref sErrorMessage) == false) return;
            
			if (MaxTicketNumberIncrementing(ref cTicketDataObj_.sTicketNumber, ref sErrorMessage) == false) return;

			string sTicketNumber = cTicketDataObj_.sTicketNumber;
			if (MaxTicketNumberStretching(ref sTicketNumber, ref sErrorMessage) == false) return;

            tbFrom.Text = cTicketDataObj_.sFrom;
            tbTo.Text = cTicketDataObj_.sTo;
            tbDepartureTime.Text = cTicketDataObj_.sDepartureTime;
            tbArrivalTime.Text = cTicketDataObj_.sArrivalTime;
            tbTravelTime.Text = cTicketDataObj_.sTravelTime;
            tbVehiclePlatform.Text = cTicketDataObj_.sVehiclePlatform;
            tbVehicle.Text = cTicketDataObj_.sVehicle;
            tbDriver.Text = cTicketDataObj_.sDriver;
            tbTicketNumber1.Text = sTicketNumber;
            tbTicketNumber2.Text = sTicketNumber;
            tbPrice.Text = cTicketDataObj_.sPrice + " $";


			
			SetWindowCaption(ref cDataHubObj_, ref tbCaption, ref sErrorMessage);
        }

        private void OnAddPassengerInformationButton(object sender, RoutedEventArgs e)
        {
            if (cDataHubObj_ == null) return;
            if (cTicketDataObj_ == null) return;

            NmspTraveler.CPassengerInformationAddingWindow cPassengerInformationWindowObj =
                new NmspTraveler.CPassengerInformationAddingWindow(ref cDataHubObj_, ref cTicketDataObj_);
            if (cPassengerInformationWindowObj == null) return;

            cPassengerInformationWindowObj.ShowDialog();
            if (cPassengerInformationWindowObj.GetDialogResult() == false) return;

            tbDepartureDate.Text = cTicketDataObj_.sDepartureDate = cPassengerInformationWindowObj.GetDepartureDate();
            tbPassengerFirstName.Text = cTicketDataObj_.sPassengerFirstName = cPassengerInformationWindowObj.GetPassengerFirstName();
            tbPassengerLastName.Text = cTicketDataObj_.sPassengerLastName = cPassengerInformationWindowObj.GetPassengerLastName();
            tbPassengerSeatNumber.Text = cTicketDataObj_.sPassengerSeatNumber = cPassengerInformationWindowObj.GetPassengerSeatNumber();
            tbPassengerDriverLicenseNumber.Text = cTicketDataObj_.sPassengerDriverLicenseNumber = cPassengerInformationWindowObj.GetPassengerDriverLicenseNumber();
            cTicketDataObj_.sPassengerDriverLicensePicture = cPassengerInformationWindowObj.GetPassengerDriverLicensePictureAsString();
        }

        public bool IsThereFreeTickets()
        {
            return lsVehicleNumberOfFreeSeats_.Count() == 0 ? false : true;
        }

        bool ValidatingPassengerInformation(ref string sMessage)
        {
            if (NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sPassengerFirstName) == true)
            {
                sMessage = "Set the Passenger First Name first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sPassengerLastName) == true)
            {
                sMessage = "Set the Passenger Last Name first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sPassengerDriverLicenseNumber) == true)
            {
                sMessage = "Set the Passenger Driver License Number first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sDepartureDate) == true)
            {
                sMessage = "Set the Departure Date first!";
                return false;
            }

            if ((NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sPassengerSeatNumber) == true) || 
                (cTicketDataObj_.sPassengerSeatNumber == "No seat"))
            {
                sMessage = "Set the Passenger Seat Number first!";
                return false;
            }

            if (NmspTraveler.CString.IsStringEmpty(cTicketDataObj_.sPassengerDriverLicensePicture) == true)
            {
                sMessage = "Set the Passenger Driver License Picture first!";
                return false;
            }

            sMessage = "The ticket has printed!";
            return true;
        }

        private void OnPrintButton(object sender, RoutedEventArgs e)
        {
            string sMessage = null;
            bool bRetVal = ValidatingPassengerInformation(ref sMessage);
   
            if (bRetVal == false)
            {
                NmspTraveler.CGui.ShowMessageBox(this, sMessage, "Traveler - Ticket");
                return;
            }

            if (NmspTraveler.CGui.ShowQuestionMessageBox(this, 
				"Do you really want to print the ticket?", 
				"Traveler - Ticket") == false)
                return;

			if (cDataHubObj_ == null) return;
            var vcPostgresqlWorkerObj = cDataHubObj_.GetPostgresqlWorkerObject();
            if (vcPostgresqlWorkerObj == null) return;

			if (Traveler.WorkSources.CDatabaseExchanger.TicketSendingRequest(ref vcPostgresqlWorkerObj,
				ref cTicketDataObj_, ref sMessage) == false)return;

            NmspTraveler.CGui.ShowMessageBox(this, sMessage, "Traveler - Ticket");

            this.Close();
        }


        private NmspTraveler.CDataHub cDataHubObj_ = null;
        private NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj_ = null;

        List<string> lsVehicleNumberOfFreeSeats_ = null;
    }
}
