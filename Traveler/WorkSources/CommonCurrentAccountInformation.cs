using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NmspTraveler.WorkSources
{
	public class CCommonCurrentAccountInformation
	{
		public void SetUserName(string sUserName)
		{
			sUserName_ = sUserName;
		}

		public string GetUserName()
		{
			return sUserName_;
		}

		private string sUserName_;
	}
}
