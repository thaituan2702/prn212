﻿#pragma checksum "..\..\..\..\Viewss\OwnerDashboard.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7B6BAD63D322D87A99CE9CCDD3E88927BF4BA7E0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace VehicleEmissionManagement.UI.Viewss {
    
    
    /// <summary>
    /// OwnerDashboard
    /// </summary>
    public partial class OwnerDashboard : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 28 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainContent;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid WelcomePanel;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid VehiclesPanel;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid VehiclesGrid;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SchedulePanel;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox VehicleComboBox;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox StationComboBox;
        
        #line default
        #line hidden
        
        
        #line 155 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DatePicker;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\Viewss\OwnerDashboard.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox TimeComboBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.12.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VehicleEmissionManagement.UI;component/viewss/ownerdashboard.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.12.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MyVehicles_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 10 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Schedule_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 14 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.InspectionHistory_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 18 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Notifications_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 22 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ProfileManagement_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 24 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LogoutButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.MainContent = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.WelcomePanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.VehiclesPanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            
            #line 57 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddVehicle_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.VehiclesGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 14:
            this.SchedulePanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 15:
            this.VehicleComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 16:
            this.StationComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 17:
            this.DatePicker = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 18:
            this.TimeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 19:
            
            #line 180 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ScheduleButton_Click);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 187 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CancelScheduleButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.12.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 12:
            
            #line 93 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditVehicle_Click);
            
            #line default
            #line hidden
            break;
            case 13:
            
            #line 99 "..\..\..\..\Viewss\OwnerDashboard.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteVehicle_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

