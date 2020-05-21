using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traveler.WorkSources
{
	public class CDatabaseExchanger
	{
		public static bool UploadDriverPicture(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			string sPictureFileName, string sVehicleDriverNumber, ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				string sByteString = null;
				if (NmspTraveler.WorkSources.CPicture.PictureFileNameToPictureByteString(
					sPictureFileName, ref sByteString, ref sErrorMessage) == false)
					return false;

				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> dbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				if (dbcColumns == null) return false;

				dbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_driver_picture", sValue = sByteString });

				string sCondition = " WHERE vehicle_driver_number = '";
				sCondition += sVehicleDriverNumber;
				sCondition += "'";

				if (cPostgresqlWorkerObj.EditRow("drivers", ref dbcColumns, sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool DriverRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
            ref NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable, 
            string sCondition, ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				if (cPostgresqlWorkerObj.SelectTable("drivers", sCondition, 
					ref dataBaseTable, ref sErrorMessage) == false)
					return false;

				if (dataBaseTable == null) return false;
				if (dataBaseTable.lDataBaseRows_ == null) return false;
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool UploadVehiclePicture(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			string sPictureFileName, string sVehicleNumber, ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				string sByteString = null;
				if (NmspTraveler.WorkSources.CPicture.PictureFileNameToPictureByteString(
					sPictureFileName, ref sByteString, ref sErrorMessage) == false)
					return false;

				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> dbcColumns =
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				if (dbcColumns == null) return false;

				dbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn(){
					sName = "vehicle_picture", sValue = sByteString });

				string sCondition = " WHERE vehicle_number = '";
				sCondition += sVehicleNumber;
				sCondition += "'";

				if (cPostgresqlWorkerObj.EditRow("vehicles", ref dbcColumns, sCondition, ref sErrorMessage) == false)
					return false;

			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool VehicleRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
            ref NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable, 
            string sCondition, ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				if(cPostgresqlWorkerObj.SelectTable("vehicles", sCondition, ref dataBaseTable, 
					ref sErrorMessage) == false)return false;

				if (dataBaseTable == null) return false;
				if (dataBaseTable.lDataBaseRows_ == null) return false;
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool PassengerRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
            ref NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable, 
            string sCondition, ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				if(cPostgresqlWorkerObj.SelectTable("tickets", sCondition, ref dataBaseTable, 
					ref sErrorMessage) == false)return false;

				if (dataBaseTable == null) return false;
				if (dataBaseTable.lDataBaseRows_ == null) return false;
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool TripRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cTripDatabaseDataObj == null) return false;
				if (cPostgresqlWorkerObj == null) return false;
				if (cTripDatabaseDataObj.lsTripNumber == null) return false;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.lsTripNumber[0]) == true) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("departure_time");
				lsColumns.Add("vehicle_route_number");

				string sCondition = "where vehicle_trip_number = ";
				sCondition += "'";
                sCondition += cTripDatabaseDataObj.lsTripNumber[0];
                sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
 				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if(dataBaseTableObj.lDataBaseRows_[0] == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 2) return false;
				if (NmspTraveler.CString.IsStringEmpty(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1]) == true) return false;
				
				cTripDatabaseDataObj.sDepartureTime = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				cTripDatabaseDataObj.sRouteNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TripRequest1(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cTripDatabaseDataObj == null) return false;
				if (cPostgresqlWorkerObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("vehicle_trip_number");

				string sCondition = null;
				if(cTripDatabaseDataObj.sRouteNumber != "All Routes")
				{
					sCondition = "where vehicle_route_number = '";
					sCondition += cTripDatabaseDataObj.sRouteNumber;
					sCondition += "'";
				}
				
				if (cTripDatabaseDataObj.sDepartureTime != "All Times")
				{
					if(cTripDatabaseDataObj.sRouteNumber != "All Routes")
						sCondition += " and departure_time = '";
					else if(cTripDatabaseDataObj.sRouteNumber == "All Routes")
						sCondition = "where departure_time = '";

					sCondition += cTripDatabaseDataObj.sDepartureTime;
					sCondition += "'";
				}
					
				if((cTripDatabaseDataObj.sRouteNumber != "All Routes") || 
					(cTripDatabaseDataObj.sDepartureTime != "All Times"))
					sCondition += ";";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;

				foreach (var vDataBaseRow in dataBaseTableObj.lDataBaseRows_)
				{
					if (vDataBaseRow.lDataBaseColumnValue_ == null) return false;
					if (vDataBaseRow.lDataBaseColumnValue_.Count != 1) return false;
					if (NmspTraveler.CString.IsStringEmpty(vDataBaseRow.lDataBaseColumnValue_[0]) == true) return false;

					cTripDatabaseDataObj.lsTripNumber.Add(vDataBaseRow.lDataBaseColumnValue_[0]);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData> lcRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (lcRouteDatabaseDataObj != null) return false;

				lcRouteDatabaseDataObj = new List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData>();
				if (lcRouteDatabaseDataObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("vehicle_route_number");
				lsColumns.Add("vehicle_route_name");

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("routes", lsColumns, null, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
 				
				foreach (var vDataBaseRow in dataBaseTableObj.lDataBaseRows_)
                {
                    if (vDataBaseRow.lDataBaseColumnValue_ == null) return false;
                    if (vDataBaseRow.lDataBaseColumnValue_.Count != 2) return false;

					lcRouteDatabaseDataObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.
						NmspPassengerWindow.CRouteDatabaseData() { sRouteNumber = vDataBaseRow.lDataBaseColumnValue_[0],
						sRouteName = vDataBaseRow.lDataBaseColumnValue_[1] });
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TripRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CTripDatabaseData1 cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cTripDatabaseDataObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("departure_time");

				string sCondition = null;

				if(cTripDatabaseDataObj.sVehicleRouteNumber != "All Routes")
				{
					sCondition += "where vehicle_route_number = ";
					sCondition += "'";
					sCondition += cTripDatabaseDataObj.sVehicleRouteNumber;
					sCondition += "';";
				}
				
				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
 				
				foreach (var vDataBaseRow in dataBaseTableObj.lDataBaseRows_)
                {
                    if (vDataBaseRow.lDataBaseColumnValue_ == null) return false;
                    if (vDataBaseRow.lDataBaseColumnValue_.Count != 1) return false;

					cTripDatabaseDataObj.lsDepartureTime.Add(vDataBaseRow.lDataBaseColumnValue_[0]);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}


		public static bool TripNumberByDepartureTimeAndRouteNumberRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.
			CTripNumberByDepartureTimeAndRouteNumberRequest cTripNumberByDepartureTimeAndRouteNumberRequestObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cTripNumberByDepartureTimeAndRouteNumberRequestObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("vehicle_trip_number");

				string sCondition = "where departure_time = ";
				sCondition += "'";
                sCondition += cTripNumberByDepartureTimeAndRouteNumberRequestObj.sDepartureTime;
                sCondition += "' and vehicle_route_number = '";
				sCondition += cTripNumberByDepartureTimeAndRouteNumberRequestObj.sRouteNumber;
				sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
      
				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
 				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if(dataBaseTableObj.lDataBaseRows_[0] == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;
				if (NmspTraveler.CString.IsStringEmpty(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;

				cTripNumberByDepartureTimeAndRouteNumberRequestObj.sTripNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteNumberByRouteNameRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.
			CVehicleRouteNumberByRouteNameRequest cVehicleRouteNumberByRouteNameRequestObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cVehicleRouteNumberByRouteNameRequestObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("vehicle_route_number");

				string sCondition = "where vehicle_route_name = ";
				sCondition += "'";
                sCondition += cVehicleRouteNumberByRouteNameRequestObj.sRouteName;
                sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("routes", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
      
				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
 				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if(dataBaseTableObj.lDataBaseRows_[0] == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;
				if (NmspTraveler.CString.IsStringEmpty(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;

				cVehicleRouteNumberByRouteNameRequestObj.sRouteNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspPassengerWindow.CRouteDatabaseData cRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cRouteDatabaseDataObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("vehicle_route_name");

				string sCondition = "where vehicle_route_number = ";
				sCondition += "'";
                sCondition += cRouteDatabaseDataObj.sRouteNumber;
                sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("routes", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
      
				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
 				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if(dataBaseTableObj.lDataBaseRows_[0] == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;
				if (NmspTraveler.CString.IsStringEmpty(dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;

				cRouteDatabaseDataObj.sRouteName = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TicketSendingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				cTicketDataObj.sTicketSaleDate = NmspTraveler.WorkSources.CDataTime.GetDateNow();

				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns =
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				if (ldbcColumns == null) return false;

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "ticket_number",
					sValue = cTicketDataObj.sTicketNumber
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "passenger_first_name",
					sValue = cTicketDataObj.sPassengerFirstName
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "passenger_last_name",
					sValue = cTicketDataObj.sPassengerLastName
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "passenger_driver_license_number",
					sValue = cTicketDataObj.sPassengerDriverLicenseNumber
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "passenger_driver_license_picture",
					sValue = cTicketDataObj.sPassengerDriverLicensePicture
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "passenger_seat_number",
					sValue = cTicketDataObj.sPassengerSeatNumber
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "vehicle_trip_number",
					sValue = cTicketDataObj.sVehicleTripNumber
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "ticket_sale_date",
					sValue = cTicketDataObj.sTicketSaleDate
				});

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn()
				{
					sName = "departure_date",
					sValue = cTicketDataObj.sDepartureDate
				});

				if (cPostgresqlWorkerObj.InsertRow("tickets", ref ldbcColumns, "", ref sErrorMessage) == false)
					return false;

			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }


		public static bool MaxTicketNumberRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			ref string sTicketNumber, 
			ref string sErrorMessage)
        {
            try
            {
                if (cTicketDataObj == null) return false;

                if (cPostgresqlWorkerObj == null) return false;

                List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;

                lsColumns.Add("max(cast(ticket_number as integer))");

                string sCondition = "where vehicle_trip_number = '";
                sCondition += cTicketDataObj.sVehicleTripNumber;
                sCondition += "'";

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("tickets", lsColumns, sCondition,
				   ref dataBaseTable, ref sErrorMessage) == false) 
					return false;

                if (dataBaseTable == null) return false;
                if (dataBaseTable.lDataBaseRows_ == null) return false;
                if (dataBaseTable.lDataBaseRows_.Count != 1) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;

                sTicketNumber = dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_[0];
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

		public static bool AccountDeletionRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			string sOldUserName, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				string sCondition = "WHERE traveler_account_user_name = '";
				sCondition += sOldUserName;
				sCondition += "'";

				if (cPostgresqlWorkerObj.DropRow("accounts", sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}


		public static bool MaxTripNumberRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDatabaseData cTripDatabaseDataObj, 
			ref string sErrorMessage)
        {
            try
            {
                if (cTripDatabaseDataObj == null) return false;
                if (cPostgresqlWorkerObj == null) return false;

                List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;

                lsColumns.Add("max(cast(vehicle_trip_number as integer))");

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, "",
				   ref dataBaseTable, ref sErrorMessage) == false) 
					return false;

                if (dataBaseTable == null) return false;
                if (dataBaseTable.lDataBaseRows_ == null) return false;
                if (dataBaseTable.lDataBaseRows_.Count != 1) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;

				if (NmspTraveler.CString.IsStringEmpty(dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;
                cTripDatabaseDataObj.sTripNumber = dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_[0];
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }
		
		public static bool MaxRouteNumberRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDatabaseData cRouteDatabaseDataObj, 
			ref string sErrorMessage)
        {
            try
            {
                if (cRouteDatabaseDataObj == null) return false;
                if (cPostgresqlWorkerObj == null) return false;

                List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;

                lsColumns.Add("max(cast(vehicle_route_number as integer))");

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("routes", lsColumns, "",
				   ref dataBaseTable, ref sErrorMessage) == false) 
					return false;

                if (dataBaseTable == null) return false;
                if (dataBaseTable.lDataBaseRows_ == null) return false;
                if (dataBaseTable.lDataBaseRows_.Count != 1) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
                if (dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 1) return false;

				if (NmspTraveler.CString.IsStringEmpty(dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_[0]) == true) return false;
                cRouteDatabaseDataObj.sRouteNumber = dataBaseTable.lDataBaseRows_[0].lDataBaseColumnValue_[0];
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }


		public static bool TripAddingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDatabaseData cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cTripDatabaseDataObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sTripNumber) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sDepartureTime) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sRouteNumber) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;
			
				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_trip_number", sValue = cTripDatabaseDataObj.sTripNumber });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "departure_time", sValue = cTripDatabaseDataObj.sDepartureTime });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_route_number", sValue = cTripDatabaseDataObj.sRouteNumber });

				if (cPostgresqlWorkerObj.InsertRow("trips", ref ldbcColumns, "", ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TripEditingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDatabaseData cTripDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cTripDatabaseDataObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sTripNumber) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sDepartureTime) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cTripDatabaseDataObj.sRouteNumber) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;
			
				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "departure_time", sValue = cTripDatabaseDataObj.sDepartureTime });

				string sCondition = "WHERE vehicle_trip_number = '";
				sCondition += cTripDatabaseDataObj.sTripNumber;
				sCondition += "' and vehicle_route_number = '";
				sCondition += cTripDatabaseDataObj.sRouteNumber;
				sCondition += "'";

				if (cPostgresqlWorkerObj.EditRow("trips", ref ldbcColumns, sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}


		public static bool TripDeletionRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspTripWindow.CTripDeletionRequest cTripDeletionRequestObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cTripDeletionRequestObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cTripDeletionRequestObj.sRouteNumber) == true) return false;
	
				string sCondition = "WHERE vehicle_route_number = '";
				sCondition += cTripDeletionRequestObj.sRouteNumber;
				sCondition += "'";

				if ((NmspTraveler.CString.IsStringEmpty(cTripDeletionRequestObj.sTripNumber) == false) &&
					(NmspTraveler.CString.IsStringEmpty(cTripDeletionRequestObj.sDepartureTime) == false))
				{
					sCondition += " and vehicle_trip_number = '";
					sCondition += cTripDeletionRequestObj.sTripNumber;
					sCondition += "'";

					sCondition += " and departure_time = '";
					sCondition += cTripDeletionRequestObj.sDepartureTime;
					sCondition += "'";
				}
					
				if (cPostgresqlWorkerObj.DropRow("trips", sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteDeletionRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDeletionRequest cRouteDeletionRequestObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cRouteDeletionRequestObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cRouteDeletionRequestObj.sRouteNumber) == true) return false;
	
				string sCondition = "WHERE vehicle_route_number = '";
				sCondition += cRouteDeletionRequestObj.sRouteNumber;
				sCondition += "'";

				if (cPostgresqlWorkerObj.DropRow("routes", sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteEditingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDatabaseData cRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cRouteDatabaseDataObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sRouteNumber) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sRouteName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sDeparturePlatform) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;
			
				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_route_name", sValue = cRouteDatabaseDataObj.sRouteName });

				string sCondition = "WHERE vehicle_route_number = '";
				sCondition += cRouteDatabaseDataObj.sRouteNumber;
				sCondition += "'";

				if (cPostgresqlWorkerObj.EditRow("routes", ref ldbcColumns, sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteAddingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspRouteWindow.CRouteDatabaseData cRouteDatabaseDataObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cRouteDatabaseDataObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sRouteNumber) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sRouteName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cRouteDatabaseDataObj.sDeparturePlatform) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;
			
				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_route_number", sValue = cRouteDatabaseDataObj.sRouteNumber });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_route_name", sValue = cRouteDatabaseDataObj.sRouteName });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "vehicle_departure_platform", sValue = cRouteDatabaseDataObj.sDeparturePlatform });

				if (cPostgresqlWorkerObj.InsertRow("routes", ref ldbcColumns, "", ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool AccountAddingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cAccountObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserRole) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserAccessLevel) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sPassword) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;
			
				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = 
					new List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_name", sValue = cAccountObj.sUserName });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_access_level", sValue = cAccountObj.sUserAccessLevel });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_role", sValue = cAccountObj.sUserRole });

				string sPassword = null;
				if(NmspTraveler.WorkSources.CEncryption.GetMd5Hash(cAccountObj.sPassword, ref sPassword) == false)return false;

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_password", sValue = sPassword });

				if (cPostgresqlWorkerObj.InsertRow("accounts", ref ldbcColumns, "", ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool AccountEditingRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj, 
			string sOldUserName, ref string sErrorMessage)
		{
			try
			{
				if (cAccountObj == null) return false;
				
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserRole) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sUserAccessLevel) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(cAccountObj.sPassword) == true) return false;

				if (cPostgresqlWorkerObj == null) return false;

				string sCondition = " WHERE traveler_account_user_name = '";
				sCondition += sOldUserName;
				sCondition += "'";

				List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn> ldbcColumns = new 
					List<NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn>();

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_name", sValue = cAccountObj.sUserName });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_access_level", sValue = cAccountObj.sUserAccessLevel });
				
				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_user_role", sValue = cAccountObj.sUserRole });

				string sPassword = null;
				if(NmspTraveler.WorkSources.CEncryption.GetMd5Hash(
					cAccountObj.sPassword, ref sPassword) == false)return false;

				ldbcColumns.Add(new NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseColumn() { 
					sName = "traveler_account_password", sValue = sPassword });

				if (cPostgresqlWorkerObj.EditRow("accounts", ref ldbcColumns, sCondition, ref sErrorMessage) == false)
					return false;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TakenSeatsRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			ref List<string> lsNumberOfPassengerTakenSeats, 
			string sDepartureDate, ref string sErrorMessage)
        {
            try
            {
                if (cPostgresqlWorkerObj == null) return false;
				if (cTicketDataObj == null) return false;

                if (lsNumberOfPassengerTakenSeats != null) return false;
                lsNumberOfPassengerTakenSeats = new List<string>();
                if (lsNumberOfPassengerTakenSeats == null) return false;

                List<string> lsColumns = new List<string>();
                lsColumns.Add("passenger_seat_number");

                string sCondition = "where vehicle_trip_number = '";
                sCondition += cTicketDataObj.sVehicleTripNumber;
                sCondition += "' and departure_date = '";
                sCondition += sDepartureDate;
                sCondition += "'";

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
                if (cPostgresqlWorkerObj.SelectTableColumns("tickets", lsColumns, sCondition,
                    ref dataBaseTable, ref sErrorMessage) == false) return false;

                if (dataBaseTable == null) return false;
                if (dataBaseTable.lDataBaseRows_ == null) return false;

                foreach (var vDataBaseRow in dataBaseTable.lDataBaseRows_)
                {
                    if (vDataBaseRow.lDataBaseColumnValue_ == null) continue;
                    if (vDataBaseRow.lDataBaseColumnValue_.Count != 1) continue;

                    lsNumberOfPassengerTakenSeats.Add(vDataBaseRow.lDataBaseColumnValue_[0]);
                }
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }
     
            return true;
        }

		public static bool AccountRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj, 
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> laAccounts, 
			ref string sErrorMessage)
		{
			try
			{
				if (laAccounts == null) return false;

				if (cPostgresqlWorkerObj == null) return false;

                List<string> lsColumns = new List<string>();
                lsColumns.Add("traveler_account_user_name");
				lsColumns.Add("traveler_account_user_access_level");
				lsColumns.Add("traveler_account_user_role");

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
                if (cPostgresqlWorkerObj.SelectTableColumns("accounts", lsColumns, null,
                    ref dataBaseTable, ref sErrorMessage) == false) return false;

                if (dataBaseTable == null) return false;
                if (dataBaseTable.lDataBaseRows_ == null) return false;

                foreach (var vDataBaseRow in dataBaseTable.lDataBaseRows_)
                {
                    if (vDataBaseRow.lDataBaseColumnValue_ == null) continue;
                    if (vDataBaseRow.lDataBaseColumnValue_.Count != lsColumns.Count()) continue;

					laAccounts.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData()
					{
						sUserName = vDataBaseRow.lDataBaseColumnValue_[0],
						sUserAccessLevel = vDataBaseRow.lDataBaseColumnValue_[1],
						sUserRole = vDataBaseRow.lDataBaseColumnValue_[2],
					});
                }
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool FullAccountRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
            ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData> lcAccountsObj, 
            ref string sErrorMessage)
        {
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTable = null;
				if (cPostgresqlWorkerObj.SelectTable("accounts", ref dataBaseTable, 
					ref sErrorMessage) == false) return false;

				if (dataBaseTable == null) return false;
				if (dataBaseTable.lDataBaseRows_ == null) return false;
				if (dataBaseTable.lDataBaseRows_.Count <= 0) return false;

				lcAccountsObj.Clear();
				foreach(var vDataBaseRow in dataBaseTable.lDataBaseRows_)
				{
					if (vDataBaseRow.lDataBaseColumnValue_ == null) return false;
					if (vDataBaseRow.lDataBaseColumnValue_.Count != 4) return false;

					lcAccountsObj.Add(new NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData(){ 
						sUserName = vDataBaseRow.lDataBaseColumnValue_[0],
 						sPassword = vDataBaseRow.lDataBaseColumnValue_[1],
						sUserAccessLevel = vDataBaseRow.lDataBaseColumnValue_[2],	
						sUserRole = vDataBaseRow.lDataBaseColumnValue_[3], 
					});
				}	
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

		public static bool AccountRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CAccountDatabaseData cAccountObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (cAccountObj == null) return false;
				if (cPostgresqlWorkerObj == null) return false;

                List<string> lsColumns = new List<string>();
				lsColumns.Add("traveler_account_password");
				lsColumns.Add("traveler_account_user_access_level");
				lsColumns.Add("traveler_account_user_role");

				string sCondition = "where traveler_account_user_name = '";
				sCondition += cAccountObj.sUserName;
				sCondition += "'";

                NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
                if (cPostgresqlWorkerObj.SelectTableColumns("accounts", lsColumns, sCondition,
                    ref dataBaseTableObj, ref sErrorMessage) == false) return false;

                if (dataBaseTableObj == null) return false;
                if (dataBaseTableObj.lDataBaseRows_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count() != 1) return false;

				cAccountObj.sPassword = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				cAccountObj.sUserAccessLevel = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
				cAccountObj.sUserRole = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[2];
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool RouteRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			string sVehicleRouteName, ref string sVehicleRouteNumber, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;
				if (cTicketDataObj == null) return false;

				List<string> lsColumns = new List<string>();
				lsColumns.Add("vehicle_route_number");
				lsColumns.Add("vehicle_departure_platform");

				string sCondition = "where ";
				sCondition += "vehicle_route_name";
				sCondition += " = ";
				sCondition += "'";
				sCondition += sVehicleRouteName;
				sCondition += "'";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;

				if (cPostgresqlWorkerObj.SelectTableColumns("routes", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false)
					return false;
				
				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 2) return false;

				sVehicleRouteNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				if (sVehicleRouteNumber == null) return false;
				if (sVehicleRouteNumber.Count() == 0) return false;

				cTicketDataObj.sVehiclePlatform = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TripRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			string sVehicleRouteNumber, string sVehicleDepartureTime, ref string sVehicleDriverNumber, 
			ref string sVehicleNumber, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
				
				lsColumns.Add("vehicle_trip_number");
				lsColumns.Add("travel_time");
				lsColumns.Add("vehicle_driver_number");
				lsColumns.Add("vehicle_number");
				lsColumns.Add("trip_price");

				string sCondition = "where ";
				sCondition += "vehicle_route_number";
				sCondition += " = ";
				sCondition += "'";
				sCondition += sVehicleRouteNumber;
				sCondition += "'";

				sCondition += " and ";
				sCondition += "departure_time";
				sCondition += " = ";
				sCondition += "'";
				sCondition += sVehicleDepartureTime;
				sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 5) return false;

				cTicketDataObj.sVehicleTripNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				cTicketDataObj.sTravelTime = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
				sVehicleDriverNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[2];
				sVehicleNumber = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[3];
				cTicketDataObj.sPrice = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[4];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool TripRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			string sVehicleRouteNumber, ref List<string> lsDepartureTimes, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;
					
				lsColumns.Add("departure_time");

				string sCondition = "where vehicle_route_number = ";
				sCondition += "'";
                sCondition += sVehicleRouteNumber;
                sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("trips", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false; 
      
				foreach(var vTripInfo in dataBaseTableObj.lDataBaseRows_)
				{
					if (vTripInfo == null) return false;
					if (vTripInfo.lDataBaseColumnValue_ == null) return false;
					if (vTripInfo.lDataBaseColumnValue_.Count() == 0) return false;

					lsDepartureTimes.Add(vTripInfo.lDataBaseColumnValue_[0]);
				}
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		public static bool DriverRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			string sVehicleDriverNumber, ref string sPictureByteString, 
			ref string sAdditionalDriverInformation, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;

				lsColumns.Add("vehicle_driver_first_name");
				lsColumns.Add("vehicle_driver_last_name");
				lsColumns.Add("vehicle_driver_gender");
				lsColumns.Add("vehicle_driver_picture");
				lsColumns.Add("vehicle_driver_birth_date");
				lsColumns.Add("vehicle_driver_employment_date");
				lsColumns.Add("vehicle_driver_rating");

				string sCondition = "where ";
				sCondition += "vehicle_driver_number";
				sCondition += " = ";
				sCondition += "'";
				sCondition += sVehicleDriverNumber;
				sCondition += "';";

				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("drivers", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count < 0) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count > 1) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 7) return false;

				string sDriverName = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				sDriverName += " ";
				sDriverName += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
				sDriverName += ", ";
				sDriverName += NmspTraveler.WorkSources.CDataTime.GetYearsUntillNow(
					dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[4]);

				sDriverName += " years old, ";
				sDriverName += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[2];

				cTicketDataObj.sDriver = sDriverName;

				sAdditionalDriverInformation = ", experience ";
				sAdditionalDriverInformation += NmspTraveler.WorkSources.CDataTime.GetYearsUntillNow(
					dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[5]);
				sAdditionalDriverInformation += " years, rating ";
				sAdditionalDriverInformation += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[6];

				sPictureByteString = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[3];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}


		public static bool VehicleRequest(
			ref NmspTraveler.WorkSources.CPostgresqlWorker cPostgresqlWorkerObj,
			ref NmspTraveler.WorkSources.NmspWorkingStructures.CTicketData cTicketDataObj,
			string sVehicleNumber, ref string sPictureByteString, 
			ref string sAdditionalVehicleInformation, ref string sErrorMessage)
		{
			try
			{
				if (cPostgresqlWorkerObj == null) return false;

				List<string> lsColumns = new List<string>();
				if (lsColumns == null) return false;

				lsColumns.Add("vehicle_type");
				lsColumns.Add("vehicle_model");
				lsColumns.Add("vehicle_picture");
				lsColumns.Add("vehicle_licence_plate");
				lsColumns.Add("vehicle_manufacture_year");
				lsColumns.Add("vehicle_manufacture_country");
				lsColumns.Add("vehicle_technical_state");
				lsColumns.Add("vehicle_rating");
				lsColumns.Add("vehicle_number_of_seats");

				string sCondition = "where ";
				sCondition += "vehicle_number";
				sCondition += " = ";
				sCondition += "'";
				sCondition += (int.Parse(sVehicleNumber) % 10).ToString();
				sCondition += "';";
         
				NmspTraveler.WorkSources.CPostgresqlWorker.DataBaseTable dataBaseTableObj = null;
				if (cPostgresqlWorkerObj.SelectTableColumns("vehicles", lsColumns, sCondition, 
					ref dataBaseTableObj, ref sErrorMessage) == false) return false;

				if (dataBaseTableObj == null) return false;
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_.Count != 1) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_ == null) return false;
				if (dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_.Count != 9) return false;

				string sVehicleName = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[0];
				sVehicleName += ", ";
				sVehicleName += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[1];
				sVehicleName += ", ";
				sVehicleName += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[8];
				sVehicleName += " seats, ";
				sVehicleName += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[3];
				cTicketDataObj.sVehicle = sVehicleName;

				sAdditionalVehicleInformation = ", ";	
				sAdditionalVehicleInformation += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[4];
				sAdditionalVehicleInformation += ", ";
				sAdditionalVehicleInformation += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[5];
				sAdditionalVehicleInformation += ", technical state ";
				sAdditionalVehicleInformation += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[6];
				sAdditionalVehicleInformation += ", rating ";
				sAdditionalVehicleInformation += dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[7];

				cTicketDataObj.sVehicleNumberOfSeats = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[8];

				sPictureByteString = dataBaseTableObj.lDataBaseRows_[0].lDataBaseColumnValue_[2];
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}
	}
}
