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
    public partial class CQuestionMessageBoxWindow : NmspTraveler.CBaseWindow
    {
        public CQuestionMessageBoxWindow(string sMessageText, string sCaptionText)
        {
            InitializeComponent();

            tbMessage.Text = sMessageText;
            tbCaption.Text = sCaptionText;
        }

        private void OnYesButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(true);
            this.Close();
        }
        private void OnNoButton(object sender, RoutedEventArgs e)
        {
            SetDialogResult(false);
            this.Close();
        }
    }
}
