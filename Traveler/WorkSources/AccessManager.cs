using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler.WorkSources
{
    public class CAccessManager
    {
        public CAccessManager()
        {
			lAccessLevels_.Add(new CAccessLevel() { sAccessLevel = "1", 
				lAccessAreas = new List<CAccessArea>() { 
					new CAccessArea(){ sAccessAreaCode = "1", sAccessAreaDescription = "view of the information about trips" }, 
			} });

			lAccessLevels_.Add(new CAccessLevel() { sAccessLevel = "2", 
				lAccessAreas = new List<CAccessArea>() { 
					new CAccessArea(){ sAccessAreaCode = "1", sAccessAreaDescription = "view of the information about trips" }, 
					new CAccessArea(){ sAccessAreaCode = "2", sAccessAreaDescription = "selling and printing of the ticket" },
					new CAccessArea(){ sAccessAreaCode = "3", sAccessAreaDescription = "opening and closing of an operation day" },
			} });

			lAccessLevels_.Add(new CAccessLevel() { sAccessLevel = "3", 
				lAccessAreas = new List<CAccessArea>() { 
					new CAccessArea(){ sAccessAreaCode = "1", sAccessAreaDescription = "view of the information about trips" }, 
					new CAccessArea(){ sAccessAreaCode = "2", sAccessAreaDescription = "selling and printing of the ticket" },
					new CAccessArea(){ sAccessAreaCode = "3", sAccessAreaDescription = "opening and closing of an operation day" },
					new CAccessArea(){ sAccessAreaCode = "4", sAccessAreaDescription = "appointment of one of the drivers for the trip" }, 
					new CAccessArea(){ sAccessAreaCode = "5", sAccessAreaDescription = "appointment of one of the vehicle for the trip" },
					new CAccessArea(){ sAccessAreaCode = "6", sAccessAreaDescription = "view of the driver list" },
					new CAccessArea(){ sAccessAreaCode = "7", sAccessAreaDescription = "view of the vehicle list" },
					new CAccessArea(){ sAccessAreaCode = "8", sAccessAreaDescription = "adding, editing, deletion of routes and trips" },
			} });

			lAccessLevels_.Add(new CAccessLevel() { sAccessLevel = "4", 
				lAccessAreas = new List<CAccessArea>() { 
					new CAccessArea(){ sAccessAreaCode = "1", sAccessAreaDescription = "view of the information about trips" }, 
					new CAccessArea(){ sAccessAreaCode = "2", sAccessAreaDescription = "selling and printing of the ticket" },
					new CAccessArea(){ sAccessAreaCode = "3", sAccessAreaDescription = "opening and closing of an operation day" },
					new CAccessArea(){ sAccessAreaCode = "4", sAccessAreaDescription = "appointment of one of the drivers for the trip" }, 
					new CAccessArea(){ sAccessAreaCode = "5", sAccessAreaDescription = "appointment of one of the vehicle for the trip" },
					new CAccessArea(){ sAccessAreaCode = "6", sAccessAreaDescription = "view of the driver list" },
					new CAccessArea(){ sAccessAreaCode = "7", sAccessAreaDescription = "view of the vehicle list" },
					new CAccessArea(){ sAccessAreaCode = "8", sAccessAreaDescription = "adding, editing, deletion of routes and trips" },
					new CAccessArea(){ sAccessAreaCode = "9", sAccessAreaDescription = "adding, editing, deletion of accounts" },
					new CAccessArea(){ sAccessAreaCode = "10", sAccessAreaDescription = "settings" },
					new CAccessArea(){ sAccessAreaCode = "11", sAccessAreaDescription = "view of the passengers list" },
			} });
        }

		public bool GetOperationPermissionByArea(string sAccessAreaCode, string sAccessAreaDescription)
		{
			if (sAccessAreaCode == null) return false;
			if (sAccessAreaCode.Count() <= 0) return false;
			if (sAccessAreaDescription == null) return false;
			if (sAccessAreaDescription.Count() <= 0) return false;

			if (lFreeAccessAreas_ == null) return false;
			if (lFreeAccessAreas_.Count() <= 0) return false;

			foreach(CAccessArea cAccessAreaObj in lFreeAccessAreas_)
			{
				if (cAccessAreaObj == null) continue;
				if (cAccessAreaObj.sAccessAreaCode == null) continue;
				if (cAccessAreaObj.sAccessAreaDescription == null) continue;
				if (cAccessAreaObj.sAccessAreaCode.Count() <= 0) continue;
				if (cAccessAreaObj.sAccessAreaDescription.Count() <= 0) continue;

				if ((cAccessAreaObj.sAccessAreaCode == sAccessAreaCode) &&
					(cAccessAreaObj.sAccessAreaDescription == sAccessAreaDescription))
					return true;
			}

			return false;
		}

		public class CAccessLevel
		{
			public string sAccessLevel = null;
			public List<CAccessArea> lAccessAreas = new List<CAccessArea>();
		}

        public class CAccessArea
        {	
            public string sAccessAreaCode = null;
            public string sAccessAreaDescription = null;
        }  

		public bool LoadFreeAccessAreas(string sCurrentAccessLevel, string sCurrentAccessDescription)
		{
			if (sCurrentAccessLevel == null) return false;
			if (sCurrentAccessLevel.Count() <= 0) return false;
			if (sCurrentAccessDescription == null) return false;
			if (sCurrentAccessDescription.Count() <= 0) return false;
			if (lAccessLevels_ == null) return false;
			if (lAccessLevels_.Count != 4) return false;

			foreach(CAccessLevel cAccessLevelObj in lAccessLevels_)
			{
				if (cAccessLevelObj == null) return false;
				if (cAccessLevelObj.lAccessAreas == null) return false;
				if (cAccessLevelObj.lAccessAreas.Count() == 0) return false;

				if (cAccessLevelObj.sAccessLevel == sCurrentAccessLevel)
				{
					lFreeAccessAreas_ = cAccessLevelObj.lAccessAreas;
					break;
				}	
			}

			return true;
		}

		private List<CAccessArea> lFreeAccessAreas_ = new List<CAccessArea>();
		private List<CAccessLevel> lAccessLevels_ = new List<CAccessLevel>();
    }
}
