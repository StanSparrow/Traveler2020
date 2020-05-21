﻿#pragma checksum "..\..\RouteWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2B1E08A81812CDC8309A80A3B1BE3A8A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using NmspTraveler;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace NmspTraveler {
    
    
    /// <summary>
    /// CRouteWindow
    /// </summary>
    public partial class CRouteWindow : NmspTraveler.CBaseWindow, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 197 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbCaption;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRouteWindowHideButton;
        
        #line default
        #line hidden
        
        
        #line 208 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRouteWindowCloseButton;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRouteWindowAboutButton;
        
        #line default
        #line hidden
        
        
        #line 220 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRouteWindowHelpButton;
        
        #line default
        #line hidden
        
        
        #line 226 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvRoutes;
        
        #line default
        #line hidden
        
        
        #line 232 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridViewColumn VehicleRouteNumberGridViewColumn;
        
        #line default
        #line hidden
        
        
        #line 240 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridViewColumn VehicleRouteNameGridViewColumn;
        
        #line default
        #line hidden
        
        
        #line 257 "..\..\RouteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridViewColumn VehicleDepartureTimeGridViewColumn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Traveler;component/routewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\RouteWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbCaption = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.btnRouteWindowHideButton = ((System.Windows.Controls.Button)(target));
            
            #line 203 "..\..\RouteWindow.xaml"
            this.btnRouteWindowHideButton.Click += new System.Windows.RoutedEventHandler(this.OnHideButton);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnRouteWindowCloseButton = ((System.Windows.Controls.Button)(target));
            
            #line 209 "..\..\RouteWindow.xaml"
            this.btnRouteWindowCloseButton.Click += new System.Windows.RoutedEventHandler(this.OnCloseButton);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnRouteWindowAboutButton = ((System.Windows.Controls.Button)(target));
            
            #line 215 "..\..\RouteWindow.xaml"
            this.btnRouteWindowAboutButton.Click += new System.Windows.RoutedEventHandler(this.OnAboutButton);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnRouteWindowHelpButton = ((System.Windows.Controls.Button)(target));
            
            #line 221 "..\..\RouteWindow.xaml"
            this.btnRouteWindowHelpButton.Click += new System.Windows.RoutedEventHandler(this.OnHelpButton);
            
            #line default
            #line hidden
            return;
            case 6:
            this.lvRoutes = ((System.Windows.Controls.ListView)(target));
            
            #line 227 "..\..\RouteWindow.xaml"
            this.lvRoutes.Loaded += new System.Windows.RoutedEventHandler(this.OnListViewLoaded);
            
            #line default
            #line hidden
            
            #line 227 "..\..\RouteWindow.xaml"
            this.lvRoutes.MouseMove += new System.Windows.Input.MouseEventHandler(this.OnListViewMouseMove);
            
            #line default
            #line hidden
            
            #line 228 "..\..\RouteWindow.xaml"
            this.lvRoutes.MouseLeave += new System.Windows.Input.MouseEventHandler(this.OnListViewMouseMove);
            
            #line default
            #line hidden
            
            #line 228 "..\..\RouteWindow.xaml"
            this.lvRoutes.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnListViewItemSelected);
            
            #line default
            #line hidden
            return;
            case 7:
            this.VehicleRouteNumberGridViewColumn = ((System.Windows.Controls.GridViewColumn)(target));
            return;
            case 8:
            this.VehicleRouteNameGridViewColumn = ((System.Windows.Controls.GridViewColumn)(target));
            return;
            case 10:
            this.VehicleDepartureTimeGridViewColumn = ((System.Windows.Controls.GridViewColumn)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 9:
            
            #line 251 "..\..\RouteWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.OnListViewTextBoxFocusChanged);
            
            #line default
            #line hidden
            
            #line 251 "..\..\RouteWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.OnListViewTextBoxFocusChanged);
            
            #line default
            #line hidden
            break;
            case 11:
            
            #line 282 "..\..\RouteWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.OnListBoxTextBoxFocusChanged);
            
            #line default
            #line hidden
            
            #line 282 "..\..\RouteWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.OnListBoxTextBoxFocusChanged);
            
            #line default
            #line hidden
            break;
            case 12:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.KeyDownEvent;
            
            #line 328 "..\..\RouteWindow.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.OnListViewKeyDown);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

