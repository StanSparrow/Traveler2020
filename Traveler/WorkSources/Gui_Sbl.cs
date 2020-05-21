using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NmspTraveler
{
    class CGui
    {
        public static bool ShowMessageBox(Window windowObj, string sMessageText, string sCaptionText)
        {
            var vMbwObj = new CMessageBoxWindow(sMessageText, sCaptionText);
            if (vMbwObj == null) return false;
            vMbwObj.Owner = windowObj;
            vMbwObj.ShowDialog();

            return true;
        }

        public static bool ShowQuestionMessageBox(Window windowObj, string sMessageText, string sCaptionText)
        {
            var vMbwObj = new CQuestionMessageBoxWindow(sMessageText, sCaptionText);
            if (vMbwObj == null) return false;
            vMbwObj.Owner = windowObj;
            vMbwObj.ShowDialog();
            if (vMbwObj.GetDialogResult() == false) return false;

            return true;
        }

		public static bool DataPickerDateToFormatStringDate(ref DatePicker dpObject, ref string sdpDate, ref string sErrorMessage)
        {
			try
			{
				if (dpObject == null) return false;
				if (dpObject.SelectedDate.HasValue == false) return false;
				sdpDate = dpObject.SelectedDate.Value.ToString("MM.dd.yyyy", 
					System.Globalization.CultureInfo.InvariantCulture);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

            return true;
        }

        public static Control GetElementObj(ref ListView lvListViewObj, String sControlName, int iItemIndex)
        {
			if (iItemIndex < 0) return null;

            var ContainerObj = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return null;
            var ChildrenObj = AllChildren(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return null;

            for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
            {
                var vFirstElementObj = ChildrenObj.ElementAt(iElementIndex);
				if (vFirstElementObj == null) continue;
				if (vFirstElementObj.Name == "PART_ContentHost") continue;
				if (NmspTraveler.CString.IsStringEmpty(vFirstElementObj.Name) == true) continue;

                if (vFirstElementObj.Name == sControlName)
                    return vFirstElementObj;
            }
          
            return null;
        }

		public static bool GetElementListObj(ref ListView lvListViewObj, 
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lControls, 
			int iItemIndex)
        {
			if (lControls == null) return false;
			if (lControls.Count() == 0) return false;
			if (iItemIndex < 0) return false;

			var ContainerObj = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return false;
            var ChildrenObj = AllChildren(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return false;

			bool bRetVal = true;
			bool bT = false;
			foreach (NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lControls)
			{
				if (cControlObj == null) continue;
				cControlObj.ctrlObject = null;

				bT = false;
				for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
				{
					var vFirstElementObj = ChildrenObj.ElementAt(iElementIndex);
					if (vFirstElementObj == null) continue;
					if (vFirstElementObj.Name == "PART_ContentHost") continue;
					if (NmspTraveler.CString.IsStringEmpty(vFirstElementObj.Name) == true) continue;

					if (vFirstElementObj.Name == cControlObj.sControlName)
					{
						cControlObj.ctrlObject = vFirstElementObj;
						bT = true;
						break;
					}
				}

				if (bT == false)
					bRetVal = false;
			}

            return bRetVal;
        }


        public static List<Control> AllChildren(DependencyObject parent)
        {
            var ListObj = new List<Control> { };
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var vChildObj = VisualTreeHelper.GetChild(parent, i);
                if (vChildObj == null) continue;

				if (vChildObj is Control)
                    ListObj.Add(vChildObj as Control);

                ListObj.AddRange(AllChildren(vChildObj));
            }

            return ListObj;
        }

        public static List<DependencyObject> AllChildren1(DependencyObject parent)
        {
            var ListObj = new List<DependencyObject> { };
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var vChildObj = VisualTreeHelper.GetChild(parent, i);
				if (vChildObj == null) continue;

                ListObj.Add(vChildObj);
                ListObj.AddRange(AllChildren1(vChildObj));
            }

            return ListObj;
        }

        public Grid GetChildrenGrid(ref ListView lvListViewObj, String sControlName, int iItemIndex)
        {
			if (iItemIndex < 0) return null;

            var ContainerObj = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return null;
            var ChildrenObj = GetChildrenElement(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return null;

            for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
            {
                Grid grObj = ChildrenObj.ElementAt(iElementIndex) as Grid;
                if (grObj == null) continue;

                if (grObj.Name == sControlName)
                    return grObj;
            }
           
            return null;
        }

        public List<DependencyObject> GetChildrenElement(DependencyObject parent)
        {
            var ListObj = new List<DependencyObject> { };
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var ChildObj = VisualTreeHelper.GetChild(parent, i);
                if (ChildObj == null) continue;
                ListObj.Add(ChildObj);
            }

            return ListObj;
        }


        public static Image GetImageElement(ref ListView lvListViewObj, String sControlName, int iItemIndex)
        {   
			if (iItemIndex < 0) return null;

            var ContainerObj = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return null;
            var ChildrenObj = AllChildren1(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return null;

            for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
            {
                Image imageObj = ChildrenObj.ElementAt(iElementIndex) as Image;
                if (imageObj == null) continue;

                if (imageObj.Name == sControlName)
                    return imageObj;
            }
            
            return null;
        }

		public static Border GetBorderElement(ref ListView lvListViewObj, String sControlName, int iItemIndex)
        {
			if (iItemIndex < 0) return null;

            var ContainerObj = lvListViewObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return null;
            var ChildrenObj = AllChildren1(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return null;

            for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
            {
                Border borderObj = ChildrenObj.ElementAt(iElementIndex) as Border;
                if (borderObj == null) continue;

                if (borderObj.Name == sControlName)
                    return borderObj;
            }
            
            return null;
        }

        public static Control GetElementObj(ref ListBox lvListBoxObj, String sControlName, int iItemIndex)
        {
			if (iItemIndex < 0) return null;

            var ContainerObj = lvListBoxObj.ItemContainerGenerator.ContainerFromIndex(iItemIndex);
            if (ContainerObj == null) return null;
            var ChildrenObj = AllChildren(ContainerObj);
            if ((ChildrenObj == null) || (ChildrenObj.Count == 0)) return null;

            for (int iElementIndex = 0; iElementIndex < ChildrenObj.Count; iElementIndex++)
            {
                var vFirstElementObj = ChildrenObj.ElementAt(iElementIndex);
				if (vFirstElementObj == null) continue;
				if (vFirstElementObj.Name == "PART_ContentHost") continue;
				if (NmspTraveler.CString.IsStringEmpty(vFirstElementObj.Name) == true) continue;

                if (vFirstElementObj.Name == sControlName)
                    return vFirstElementObj;
            }

            return null;
        }

		public static bool SetStretchImage(ref ListView lvListViewObj, 
			string sPictureControlName, ref string sErrorMessage)
		{
			try
			{
				if (lvListViewObj == null) return false;

				for(int i = 0; i < lvListViewObj.Items.Count; i++)
				{
					var vPictureImageElementObj = NmspTraveler.CGui.GetImageElement(
						ref lvListViewObj, sPictureControlName, i);
					if(vPictureImageElementObj == null) return false;

					var vSource = vPictureImageElementObj.Source;
					if (vSource == null) return false;

					var vDependencyObjectType = vSource.DependencyObjectType;
					if (vDependencyObjectType == null) return false;

					if(vDependencyObjectType.Name == "BitmapImage")
						vPictureImageElementObj.Stretch = Stretch.Uniform;			
				}
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
