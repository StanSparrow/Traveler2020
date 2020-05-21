using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace NmspTraveler.WorkSources
{
	namespace NmspWorkingStructures
	{
		namespace NmspCommon
		{
			public class CPicture
			{
				public bool bDefaultPicture = false;
				public ImageSource isPicture { get; set; }
			}

			public class CGridViewItemTextBox
			{
				public string sControlName = null;
				public Control ctrlObject = null;
				public Size szMaxTextSize = new Size() { Width = 0, Height = 0 };
				public GridViewColumn gvcGridViewColumn = null; 
			}

			public class CAccountDatabaseData
			{
				public string sUserRole = null;
				public string sUserName = null;
				public string sUserAccessLevel = null;
				public string sPassword = null;
			}

			public class CTripInfo
			{
				public string sRouteName { get; set; }
				public string sDepartureTime { get; set; }
				public string sTripNumber { get; set; }
			}
		}

		namespace NmspPassengerWindow
		{
			public class CTripDatabaseData
			{
				public List<string> lsTripNumber = new List<string>();
				public string sRouteNumber { get; set; }
				public string sDepartureTime { get; set; }
			}

			public class CTripDatabaseData1
			{
				public string sVehicleRouteNumber { get; set; }

				public List<string> lsDepartureTime = new List<string>();
			}

			public class CRouteDatabaseData
			{
				public string sRouteNumber { get; set; }
				public string sRouteName { get; set; }
			}

			public class CTicketDatabaseData
			{
				public string sTicketNumber { get; set; }
				public string sPassengerFirstname { get; set; }
				public string sPassengerLastname { get; set; }
				public string sPassengerDriverLicenseNumber { get; set; }
				public NmspCommon.CPicture cPictureObj { get; set; }
				public string sPassengerSeatNumber { get; set; }
				public string sPassengerRoute { get; set; }
				public string sPassengerDepartureTime { get; set; }
				public string sTicketSaleDate { get; set; }
				public string sDepartureDate { get; set; }
			}

			public class CPassengerFilterSearch
			{
				public string sRouteNumber { get; set; }
				public string sDepartureDate { get; set; }
				public string sDepartureTime { get; set; }
			}
		}

		namespace NmspDriverWindow
		{

		}

		namespace NmspVehicleWindow
		{

		}

		namespace NmspTripWindow
		{
			public class CTripDatabaseData
			{
				public string sTripNumber { get; set; }
				public string sDepartureTime { get; set; }
				public string sRouteNumber { get; set; }
			}

			public class CTripNumberByDepartureTimeAndRouteNumberRequest
			{
				public string sTripNumber { get; set; }
				public string sDepartureTime { get; set; }
				public string sRouteNumber { get; set; }
			}		

			public class CTripDeletionRequest
			{
				public string sRouteNumber { get; set; }
				public string sTripNumber { get; set; }
				public string sDepartureTime { get; set; }
			}
		}

		namespace NmspRouteWindow
		{
			public class CRouteDatabaseData
			{
				public string sRouteNumber { get; set; }
				public string sRouteName { get; set; }
				public string sDeparturePlatform { get; set; }
			}

			public class CVehicleRouteNumberByRouteNameRequest
			{
				public string sRouteNumber { get; set; }
				public string sRouteName { get; set; }
			}

			public class CRouteDeletionRequest
			{
				public string sRouteNumber { get; set; }
			}
		}

		namespace NmspSettingWindow
		{
			public class CSettingsJsonData
			{
				public string sIp { get; set; }
				public string sPort { get; set; }
				public string sDatabaseName { get; set; }
				public string sUser { get; set; }
				public string sPassword { get; set; }
			}
		}

		namespace NmspPassengerInformationAddingWindow
		{
			public class CPassengerDriverLicensePicture
			{
				public string sFileName;
				public string sPictureByteString;
			}
		}

		

		

		

		public class CTripVehicleData
		{
			public string sVehicleType { get; set; }

			public string sVehicleModel { get; set; }

			public NmspCommon.CPicture cPictureObj { get; set; }

			public string sVehicleLicencePlate { get; set; }

			public string sVehicleManufactureYear { get; set; }

			public string sVehicleCountryOfOrigin { get; set; }

			public string sVehicleTechnicalState { get; set; }

			public string sVehicleRating { get; set; }

			public string sPassengerCapacity { get; set; }
		}

		public class CVehicleDriverData
		{
			public string sFirstName { get; set; }

			public string sLastName { get; set; }

			public NmspCommon.CPicture cPictureObj { get; set; }

			public string sGender { get; set; }

			public string sBirthDate { get; set; }

			public string sDateOfEmployment { get; set; }

			public string sLenthOfService { get; set; }
            
			public string sRating { get; set; }
		}

		

		

		public class CVehicleTripData
		{
			public string sTripKey { get; set; }

			public string sTripValue { get; set; }

			public ImageSource isTripPicture { get; set; }
		}

		public class CTicketData
		{
			public string sPassengerFirstName = null;
			public string sPassengerLastName = null;

			public string sPassengerDriverLicenseNumber = null;
			public string sPassengerSeatNumber = null;
			public string sPassengerDriverLicensePicture = null;

			public string sFrom = null;
			public string sTo = null;
			public string sDepartureDate = null;
			public string sDepartureTime = null;
			public string sArrivalTime = null;
			public string sTravelTime = null;
			public string sVehiclePlatform = null;
			public string sVehicle = null;
			public string sVehicleNumberOfSeats = null;
			public string sDriver = null;
			public string sPrice = null;
			public string sVehicleTripNumber = null;
			public string sTicketNumber = null;
			public string sTicketSaleDate = null;
		}

		public class CVehicleDepartureTime
		{
			public CVehicleDepartureTime()
			{
				iVehicleTripNumber = -1;
			}

			public int iVehicleTripNumber { get; set; }
			public string sVehicleDepartureTime { get; set; }
		}

		public class CVehicleRoutesData
		{
			public CVehicleRoutesData()
			{
				iVehicleRouteNumber = -1;
			}

			public int iVehicleRouteNumber { get; set; }
			public string sRouteName { get; set; }
			public IEnumerable<CVehicleDepartureTime> ieVehicleDepartureTime { get; set; }
		}
	}
}
