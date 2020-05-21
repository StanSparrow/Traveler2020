using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler.WorkSources
{
    public class CPostgresqlWorker
    {
        public class DataBaseColumn
        {
            public string sName { get; set; }
            public string sValue { get; set; }
        }

        public class DataBaseRow
        {
            public List<string> lDataBaseColumnValue_;
        }

        public class DataBaseTable
        {
            public List<string> lDataBaseColumnName_;
            public List<DataBaseRow> lDataBaseRows_;
        }

        public static bool GetTimeParts(string sTime, ref List< string > lsTimeParts, ref string sErrorMessage)
        {
			try
			{
				if (lsTimeParts != null) return false;
				lsTimeParts = new List<string>();
				if (lsTimeParts == null) return false;

				var vlTime = sTime.Split(' ');
				if (vlTime == null) return false;
				if (vlTime.Count() < 2) return false;
				lsTimeParts.Add(vlTime[1]);

				vlTime = vlTime[0].Split(':');
				if (vlTime == null) return false;
				if (vlTime.Count() < 2) return false;

				lsTimeParts.Add(vlTime[0]);
				lsTimeParts.Add(vlTime[1]);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
        }

        public static bool TimeComparing(string sFirstTime, string sLastTime, ref string sErrorMessage)
        {
			try
			{
				List<string> lsFirstTimeParts = null;
				if (GetTimeParts(sFirstTime, ref lsFirstTimeParts, ref sErrorMessage) == false) return false;
				if (lsFirstTimeParts == null) return false;
				if (lsFirstTimeParts.Count < 3) return false;

				List<string> lsLastTimeParts = null;
				if (GetTimeParts(sLastTime, ref lsLastTimeParts, ref sErrorMessage) == false) return false;
				if (lsLastTimeParts == null) return false;
				if (lsLastTimeParts.Count < 3) return false;

				if ((lsFirstTimeParts[0] == "pm") && (lsLastTimeParts[0] == "am")) return true;
				else if ((lsFirstTimeParts[0] == "am") && (lsLastTimeParts[0] == "pm")) return false;

				if ((lsFirstTimeParts[1] != "12") && (lsLastTimeParts[1] == "12")) return true;
				else if ((lsFirstTimeParts[1] == "12") && (lsLastTimeParts[1] != "12")) return false;

				if (int.Parse(lsFirstTimeParts[1]) > int.Parse(lsLastTimeParts[1])) return true;
				else if (int.Parse(lsFirstTimeParts[1]) < int.Parse(lsLastTimeParts[1])) return false;

				if (int.Parse(lsFirstTimeParts[2]) > int.Parse(lsLastTimeParts[2])) return true;
				else if (int.Parse(lsFirstTimeParts[2]) < int.Parse(lsLastTimeParts[2])) return false;
			}
            catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
        }

        public static bool SortByTime(ref List< string > lsTime, ref string sErrorMessage)
        {
			try
			{
				if (lsTime.Count == 0) return false;

				string sTemp = null;
				for (int i = 0; i < lsTime.Count; i++)
					for (int j = 0; j < lsTime.Count - (i + 1); j++)
					{
						if (TimeComparing(lsTime[j], lsTime[j + 1], ref sErrorMessage) == true)
						{
							sTemp = lsTime[j];
							lsTime[j] = lsTime[j + 1];
							lsTime[j + 1] = sTemp;
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
        

        private bool Open(ref string sErrorMessage)
        {
            try
            {
                npgsqlConnectionObj_.Open();
            }
            catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private bool Close(ref string sErrorMessage)
        { 
            try
            {
                npgsqlConnectionObj_.Close();
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public bool IsConnected(ref string sErrorMessage)
        {
			try
			{
				if (Open(ref sErrorMessage) == false) return false;
				if (Close(ref sErrorMessage) == false) return false;
			}
			catch(Exception ex)
            {
                sErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public CPostgresqlWorker(string sIp, string sPort, string sDataBaseName, string sLogin, string sPassword)
        {
            sConnectionString_ = "Server=";
            sConnectionString_ += sIp;
            sConnectionString_ += ";Port=";
            sConnectionString_ += sPort;
            sConnectionString_ += ";Database=";
            sConnectionString_ += sDataBaseName;
            sConnectionString_ += ";User Id=";
            sConnectionString_ += sLogin;
            sConnectionString_ += ";Password=";
            sConnectionString_ += sPassword;
            sConnectionString_ += ";";

            npgsqlConnectionObj_ = new Npgsql.NpgsqlConnection(sConnectionString_);
        }

        public static bool Connect(string sIp, string sPort, string sDataBaseName, string sLogin, 
			string sPassword, ref CPostgresqlWorker cPostgresqlWorkerObj, ref string sErrorMessage)
        {
			try
			{
				cPostgresqlWorkerObj = null;
				cPostgresqlWorkerObj = new NmspTraveler.WorkSources.CPostgresqlWorker(
					sIp, sPort, sDataBaseName, sLogin, sPassword);

				if (cPostgresqlWorkerObj.IsConnected(ref sErrorMessage) == false)
				{
					cPostgresqlWorkerObj = null;
					return false;
				}
			}
			catch (Exception ex)
            {
                sErrorMessage = ex.Message;
				return false;
            }   

            return true;
        }

        public bool CreateDataBase()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }

        public bool CreateTable()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }

        public bool InsertRow(string sTableName, ref List<DataBaseColumn> dbcColumns,
            string sCondition, ref string sErrorMessage)
        {
			try
            {
				if (npgsqlConnectionObj_ == null) return false;

				string sQueryString = "INSERT INTO ";
				sQueryString += sTableName;
				sQueryString += "(";

				for (int i = 0; i < dbcColumns.Count; i++)
				{
					sQueryString += dbcColumns[i].sName;
					if (i < dbcColumns.Count - 1) sQueryString += ", ";
				}

				sQueryString += ") VALUES(";

				for (int i = 0; i < dbcColumns.Count; i++)
				{
					sQueryString += "'";
					sQueryString += dbcColumns[i].sValue;
					sQueryString += "'";
					if (i < dbcColumns.Count - 1) sQueryString += ", ";
				}

				sQueryString += ");";

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.OpenAsync();
                vQuery.ExecuteNonQueryAsync();
                npgsqlConnectionObj_.Close();
            }
            catch (Exception ex)
            {
                sErrorMessage = ex.Message;
				return false;
            }      

            return true;
        }

        public bool InsertColumn()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }

        public bool EditRow(string sTableName, ref List<DataBaseColumn> dbcColumns, 
            string sCondition, ref string sErrorMessage)
        {
			try
            {
				if (npgsqlConnectionObj_ == null) return false;

				string sQueryString = "UPDATE ";
				sQueryString += sTableName;
				sQueryString += " SET ";

				for (int i = 0; i < dbcColumns.Count; i++)
				{
					sQueryString += dbcColumns[i].sName;
					sQueryString += " = '";
					sQueryString += dbcColumns[i].sValue;
					sQueryString += "'";
					if (i < dbcColumns.Count - 1) sQueryString += ",";
				}

				if ((sCondition != null) && (sCondition.Count() != 0))
				{
					sQueryString += " ";
					sQueryString += sCondition;
				}

				sQueryString += ";";
   
                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                npgsqlDataReaderObj.Read();

                vQuery.ExecuteNonQuery();
                
                npgsqlConnectionObj_.Close();
            }
            catch (Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
				return false;
            }

            return true;
        }

        public bool DropRow(string sTableName, string sCondition, ref string sErrorMessage)
        {
			try
            {
				if (npgsqlConnectionObj_ == null) return false;

				string sQueryString = "DELETE FROM ";
				sQueryString += sTableName;

				if ((sCondition != null) && (sCondition.Count() != 0))
				{
					sQueryString += " ";
					sQueryString += sCondition;
				}

				sQueryString += ";";

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                npgsqlDataReaderObj.Read();

                vQuery.ExecuteNonQuery();
                
                npgsqlConnectionObj_.Close();
            }
            catch (Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
				return false;
            }

            return true;
        }

        public bool DropTable()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }

        public bool DropDataBase()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }


        public bool SelectRow()
        {
            if (npgsqlConnectionObj_ == null) return false;

            return true;
        }

        private bool SelectTableColumnNames(string sTableName, 
			ref List<string> lDataBaseColumnName, ref string sErrorMessage)
        {
			try
            {
				if (npgsqlConnectionObj_ == null) return false;

				string sQueryString = "SELECT * from ";
				sQueryString += sTableName;
				sQueryString += " where false;";

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                npgsqlDataReaderObj.Read();

                if (lDataBaseColumnName != null) return false;
                lDataBaseColumnName = new List<string>();
                if (lDataBaseColumnName == null) return false;

                for (int i = 0; i < npgsqlDataReaderObj.FieldCount; i++) 
                    lDataBaseColumnName.Add(npgsqlDataReaderObj.GetName(i));
                
                npgsqlConnectionObj_.Close();
            }
            catch (Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
				return false;
            }

            return true;
        }

        public bool SelectTableColumns(string sTableName, List < string > lsColumns, string sCondition, 
            ref DataBaseTable dataBaseTableObj, ref string sErrorMessage)
        {
			try
            {
				string sQueryString = "SELECT ";

				for(int i = 0; i < lsColumns.Count; i++)
				{
					sQueryString += lsColumns[i];
					if(i < lsColumns.Count - 1)sQueryString += ",";
				}

				sQueryString += " FROM ";
				sQueryString += sTableName;

				if ((sCondition != null) && (sCondition.Count() != 0))
				{
					sQueryString += " ";
					sQueryString += sCondition;
				}
                
				sQueryString += ";";

				if (dataBaseTableObj != null) return false;
				dataBaseTableObj = new DataBaseTable();
				if (dataBaseTableObj == null) return false;

				if (dataBaseTableObj.lDataBaseRows_ != null) return false;
				dataBaseTableObj.lDataBaseRows_ = new List<DataBaseRow>();
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                while (npgsqlDataReaderObj.Read())
                {
					List<string> lDataBaseColumnValue = new List<string>();
					if (lDataBaseColumnValue == null) continue;

                    for (int i = 0; i < npgsqlDataReaderObj.FieldCount; i++)
                    {
						if (npgsqlDataReaderObj[i] == null) continue;
                        lDataBaseColumnValue.Add(npgsqlDataReaderObj[i].ToString());
                    }

					dataBaseTableObj.lDataBaseRows_.Add(new DataBaseRow() { lDataBaseColumnValue_ = lDataBaseColumnValue });
                }

                npgsqlConnectionObj_.Close();

				if (dataBaseTableObj.lDataBaseColumnName_ != null) return false;
				dataBaseTableObj.lDataBaseColumnName_ = new List<string>();
				if (dataBaseTableObj.lDataBaseColumnName_ == null) return false;

				foreach (string sColumn in lsColumns)
					dataBaseTableObj.lDataBaseColumnName_.Add(sColumn);
            }
            catch (Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
                return false;
            }

            return true;
        }

        public bool SelectTable(string sTableName, ref DataBaseTable dataBaseTableObj, ref string sErrorMessage)
        {
			try
            {
				string sQueryString = "SELECT * FROM ";
				sQueryString += sTableName;
				sQueryString += ";";

				if (dataBaseTableObj != null) return false;
				dataBaseTableObj = new DataBaseTable();
				if (dataBaseTableObj == null) return false;

				if (dataBaseTableObj.lDataBaseRows_ != null) return false;
				dataBaseTableObj.lDataBaseRows_ = new List<DataBaseRow>();
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                while (npgsqlDataReaderObj.Read())
                {
                    dataBaseTableObj.lDataBaseRows_.Add(new DataBaseRow(){});

                    dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_ = new List<string>();

                    if (dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_ == null) continue;

                    for(int i = 0; i < npgsqlDataReaderObj.FieldCount; i++)
                    {        
                        dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_.Add(npgsqlDataReaderObj[i].ToString());         
                    }          
                }        

                npgsqlConnectionObj_.Close();  
 
				if (SelectTableColumnNames(sTableName, ref dataBaseTableObj.lDataBaseColumnName_, ref sErrorMessage) == false) return false;
            }
            catch(Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
                return false;
            }

            return true;
        }

        public bool SelectTable(string sTableName, string sCondition, 
			ref DataBaseTable dataBaseTableObj, ref string sErrorMessage)
        {
			try
            {
				string sQueryString = "SELECT * FROM ";
				sQueryString += sTableName;

				if ((sCondition != null) && (sCondition.Count() != 0))
				{
					sQueryString += " ";
					sQueryString += sCondition;
				}

				sQueryString += ";";

				if (dataBaseTableObj != null) return false;
				dataBaseTableObj = new DataBaseTable();
				if (dataBaseTableObj == null) return false;

				if (dataBaseTableObj.lDataBaseRows_ != null) return false;
				dataBaseTableObj.lDataBaseRows_ = new List<DataBaseRow>();
				if (dataBaseTableObj.lDataBaseRows_ == null) return false;

                var vQuery = new Npgsql.NpgsqlCommand(sQueryString, npgsqlConnectionObj_);
                if (vQuery == null) return false;
                npgsqlConnectionObj_.Close();
                npgsqlConnectionObj_.Open();

                Npgsql.NpgsqlDataReader npgsqlDataReaderObj = vQuery.ExecuteReader();
                if (npgsqlDataReaderObj == null) return false;

                while (npgsqlDataReaderObj.Read())
                {
                    dataBaseTableObj.lDataBaseRows_.Add(new DataBaseRow() { });

                    dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_ = new List<string>();

                    if (dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_ == null) continue;

                    for (int i = 0; i < npgsqlDataReaderObj.FieldCount; i++)
                    {
                        dataBaseTableObj.lDataBaseRows_[dataBaseTableObj.lDataBaseRows_.Count - 1].
                            lDataBaseColumnValue_.Add(npgsqlDataReaderObj[i].ToString());
                    }
                }

                npgsqlConnectionObj_.Close();

				if (SelectTableColumnNames(sTableName, ref dataBaseTableObj.lDataBaseColumnName_, ref sErrorMessage) == false) return false;
            }
            catch (Npgsql.NpgsqlException npgsqlException)
            {
                sErrorMessage = npgsqlException.Message;
                return false;
            }

            return true;
        }

        private string sConnectionString_ = null;
        private Npgsql.NpgsqlConnection npgsqlConnectionObj_ = null;
    }
}
