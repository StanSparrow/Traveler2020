using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;

namespace NmspTraveler
{
	public class CBaseWindow : Window
	{
		public bool GetDialogResult()
        {
            return bDialogResult_;
        }

		public void SetDialogResult(bool bDialogResult)
		{
			bDialogResult_ = bDialogResult;
		}

		public BitmapImage GetNoPicture()
		{
			return biNoPictureObj_;
		}

		virtual protected void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed) && (e.GetPosition(this).Y <= 30))
                this.DragMove();
        }

		virtual protected void OnHideButton(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        virtual protected void OnCloseButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(false);
            this.Close();
        }

		protected bool SetWindowCaption(ref NmspTraveler.CDataHub cDataHubObj, ref TextBlock tbCaption, ref string sErrorMessage)
		{
			try
			{
				var vCommonCurrentAccountInformationObj = cDataHubObj.GetCommonCurrentAccountInformationObject();
				if (vCommonCurrentAccountInformationObj == null) return false;
				string sCurrentUserName = vCommonCurrentAccountInformationObj.GetUserName();
				if (NmspTraveler.CString.IsStringEmpty(sCurrentUserName) == true) return false;

				string sCaption = tbCaption.Text;
				if (NmspTraveler.CString.IsStringEmpty(sCaption) == true) return false;
				sCaption += " (";
				sCaption += sCurrentUserName;
				sCaption += ")";
				tbCaption.Text = sCaption;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		protected bool GetGridViewItemTextBoxSize(ref ListView lvListViewObj, 
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj, 
			ref string sErrorMessage)
		{
			try
			{
				if (lvListViewObj == null) return false;
				if (lvListViewObj.Items.Count == 0) return false;
				if (lccTextBoxesObj == null) return false;
				if (lccTextBoxesObj.Count == 0) return false;

				int iTextBoxIndex = 0;
				Size szTextSize = Size.Empty;
				for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
				{
					if (NmspTraveler.CGui.GetElementListObj(ref lvListViewObj, ref lccTextBoxesObj, iListViewItemIndex) == false) continue;

					iTextBoxIndex = 0;
					foreach(NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lccTextBoxesObj)
					{
						if (cControlObj == null) continue;
						if (cControlObj.ctrlObject == null) continue;

						szTextSize = Size.Empty;
						GetTextSize(ref cControlObj.ctrlObject, ((TextBox)(cControlObj.ctrlObject)).Text, ref szTextSize, ref sErrorMessage);

						if(szTextSize.Width > cControlObj.szMaxTextSize.Width)
							cControlObj.szMaxTextSize.Width = szTextSize.Width + 50;

						if(szTextSize.Height > cControlObj.szMaxTextSize.Height)
							cControlObj.szMaxTextSize.Height = szTextSize.Height;

						cControlObj.ctrlObject = null;

						iTextBoxIndex++;
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

		protected bool SetListViewItemColor(ref ListView lvListViewObj, 
			ref List<NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox> lccTextBoxesObj, 
			string sPictureControlName, string sPictureBorderControlName, ref string sErrorMessage)
        {
			try
			{
				if (lvListViewObj == null) return false;
				if (lvListViewObj.Items.Count == 0) return false;
				if (lccTextBoxesObj == null) return false;
				if (lccTextBoxesObj.Count == 0) return false;
				if (NmspTraveler.CString.IsStringEmpty(sPictureControlName) == true) return false;
				if (NmspTraveler.CString.IsStringEmpty(sPictureBorderControlName) == true) return false;

				Border brdImageBorderElementObj = null;

				for (int iListViewItemIndex = 0; iListViewItemIndex < lvListViewObj.Items.Count; iListViewItemIndex++)
				{
					dynamic vcTripPassengerDataObj = lvListViewObj.Items[iListViewItemIndex];
					if (vcTripPassengerDataObj == null) continue;
					if (vcTripPassengerDataObj.cPictureObj == null) continue;

					if (vcTripPassengerDataObj.cPictureObj.bDefaultPicture == true)
					{
						var vImageElementObj = NmspTraveler.CGui.GetImageElement(ref lvListViewObj, 
							sPictureControlName, iListViewItemIndex);
						if(vImageElementObj == null) continue;

						vImageElementObj.Stretch = Stretch.Uniform;
					}
			
					if (iListViewItemIndex == lvListViewObj.SelectedIndex) continue;

					NmspTraveler.CGui.GetElementListObj(ref lvListViewObj, ref lccTextBoxesObj, iListViewItemIndex);

					foreach(NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lccTextBoxesObj)
					{
						if (cControlObj == null) continue;
						if (cControlObj.ctrlObject == null) continue;

						cControlObj.ctrlObject.Foreground = Brushes.White;
						cControlObj.ctrlObject.Background = Brushes.Transparent;
						cControlObj.ctrlObject = null;
					}

					brdImageBorderElementObj = NmspTraveler.CGui.GetBorderElement(
						ref lvListViewObj, sPictureBorderControlName, iListViewItemIndex);
					if(brdImageBorderElementObj == null) continue;

					brdImageBorderElementObj.BorderBrush = Brushes.White;
				}

				NmspTraveler.CGui.GetElementListObj(ref lvListViewObj, ref lccTextBoxesObj, lvListViewObj.SelectedIndex);

				foreach(NmspTraveler.WorkSources.NmspWorkingStructures.NmspCommon.CGridViewItemTextBox cControlObj in lccTextBoxesObj)
				{
					if (cControlObj == null) continue;
					if (cControlObj.ctrlObject == null) continue;
					cControlObj.ctrlObject.Foreground = Brushes.Gray;
					cControlObj.ctrlObject = null;
				}

				brdImageBorderElementObj = NmspTraveler.CGui.GetBorderElement(ref lvListViewObj, 
					sPictureBorderControlName, lvListViewObj.SelectedIndex);

				if(brdImageBorderElementObj != null)
					brdImageBorderElementObj.BorderBrush = Brushes.Gray;
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
        }

		protected static bool DoEvents(ref string sErrorMessage)
		{
			try
			{
				Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}

		protected bool GetTextSize(ref Control ctrl, string sText, ref Size szTextSize, ref string sErrorMessage)
		{
			try
			{
				var formattedText = new FormattedText(sText, CultureInfo.CurrentCulture, 
				FlowDirection.LeftToRight, new Typeface(ctrl.FontFamily, ctrl.FontStyle, 
				ctrl.FontWeight, ctrl.FontStretch), ctrl.FontSize, Brushes.Black, 
				new NumberSubstitution(), TextFormattingMode.Display);

				szTextSize = new Size(formattedText.Width, formattedText.Height);
			}
			catch(Exception ex)
			{
				sErrorMessage = ex.Message;
				return false;
			}

			return true;
		}


		private bool bDialogResult_ = false;

		private BitmapImage biNoPictureObj_ = new BitmapImage(
			new Uri(@"pack://siteoforigin:,,,/Resource/Pictures/NoPicture.bmp"));
	}
}
