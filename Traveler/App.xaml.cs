using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NmspTraveler
{
    public partial class App : Application
    {
        App()
        {
            string sErrorMessage = null;
            var cTravelerManager = new WorkSources.CTravelerManager();
            cTravelerManager.Init(ref sErrorMessage);
            cTravelerManager.Run();
        }


        // Entry point method
        [STAThread]
        public static void Main()
        {
            App cTravelerObj = new App();
            if (cTravelerObj == null) return;
            cTravelerObj.Run();
        } 
    }
}

