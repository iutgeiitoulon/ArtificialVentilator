﻿#pragma checksum "..\..\WpfRespirateurMonitor.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B201150780D4D4754E8FE6D03C652BF62D2DA9F0"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using SciChart.Charting;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Common;
using SciChart.Charting.Common.AttachedProperties;
using SciChart.Charting.Common.Databinding;
using SciChart.Charting.Common.Extensions;
using SciChart.Charting.Common.Helpers;
using SciChart.Charting.Common.MarkupExtensions;
using SciChart.Charting.HistoryManagers;
using SciChart.Charting.Model;
using SciChart.Charting.Model.ChartData;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Numerics;
using SciChart.Charting.Numerics.CoordinateCalculators;
using SciChart.Charting.Numerics.CoordinateProviders;
using SciChart.Charting.Numerics.DeltaCalculators;
using SciChart.Charting.Numerics.TickProviders;
using SciChart.Charting.Themes;
using SciChart.Charting.ViewportManagers;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.Axes.DiscontinuousAxis;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using SciChart.Charting.Visuals.Axes.LogarithmicAxis;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.RenderableSeries.Animations;
using SciChart.Charting.Visuals.Shapes;
using SciChart.Charting.Visuals.TradeChart;
using SciChart.Charting.Visuals.TradeChart.MultiPane;
using SciChart.Charting3D;
using SciChart.Charting3D.Annotations;
using SciChart.Charting3D.Axis;
using SciChart.Charting3D.Common.Extensions;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.Modifiers;
using SciChart.Charting3D.Modifiers.Tooltip3D;
using SciChart.Charting3D.PointMarkers;
using SciChart.Charting3D.Primitives;
using SciChart.Charting3D.RenderableSeries;
using SciChart.Charting3D.Visuals.AxisLabels;
using SciChart.Core.AttachedProperties;
using SciChart.Core.MarkupExtensions;
using SciChart.Core.Utility.Mouse;
using SciChart.Data.Model;
using SciChart.Data.Numerics;
using SciChart.Drawing;
using SciChart.Drawing.Common;
using SciChart.Drawing.Extensions;
using SciChart.Drawing.HighQualityRasterizer;
using SciChart.Drawing.HighSpeedRasterizer;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using SciChart.Drawing.XamlRasterizer;
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
using WpfOscilloscopeControl;
using WpfRespirateur_Interface_Monitor;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace WpfRespirateur_Interface_Monitor {
    
    
    /// <summary>
    /// WpfRespirateurMonitor
    /// </summary>
    public partial class WpfRespirateurMonitor : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemUsePitot4;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemUsePitot6;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemAdvanced;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemInit;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemReanimation;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemAssistance;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridApplication;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WpfOscilloscopeControl.WpfOscilloscope oscilloVolume;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxVolume;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelVolumeCurrentVal;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WpfOscilloscopeControl.WpfOscilloscope oscilloPression;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxPression;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelPressionCurrentVal;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSeuilDetection;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSeuilDetectionVal;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderSeuil;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox groupBoxCycles;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelCycles;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonCycleP;
        
        #line default
        #line hidden
        
        
        #line 155 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonCycleM;
        
        #line default
        #line hidden
        
        
        #line 161 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonStartStop;
        
        #line default
        #line hidden
        
        
        #line 165 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSessionStart;
        
        #line default
        #line hidden
        
        
        #line 168 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSessionDuree;
        
        #line default
        #line hidden
        
        
        #line 171 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSessionStop;
        
        #line default
        #line hidden
        
        
        #line 174 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelState;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelLearningPhaseInitDispersion;
        
        #line default
        #line hidden
        
        
        #line 198 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelLearningPhaseTrainingClusters;
        
        #line default
        #line hidden
        
        
        #line 201 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelLearningPhaseMonitoringFailures;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelLearningStatus;
        
        #line default
        #line hidden
        
        
        #line 207 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelNumberOfClusters;
        
        #line default
        #line hidden
        
        
        #line 209 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox ClusterFeaturesGroupBox;
        
        #line default
        #line hidden
        
        
        #line 221 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DataGridCluster;
        
        #line default
        #line hidden
        
        
        #line 235 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.SciChart3DSurface sciChart;
        
        #line default
        #line hidden
        
        
        #line 237 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.Camera3D CameraSciChart;
        
        #line default
        #line hidden
        
        
        #line 242 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D ScatterSeries3DSample;
        
        #line default
        #line hidden
        
        
        #line 244 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.PointMarkers.EllipsePointMarker3D EllipseSample;
        
        #line default
        #line hidden
        
        
        #line 247 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D ScatterSeries3DCluster;
        
        #line default
        #line hidden
        
        
        #line 249 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.PointMarkers.EllipsePointMarker3D EllipseCluster;
        
        #line default
        #line hidden
        
        
        #line 252 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D ScatterSeries3DClusterDispersion;
        
        #line default
        #line hidden
        
        
        #line 254 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.PointMarkers.EllipsePointMarker3D EllipseDispersion;
        
        #line default
        #line hidden
        
        
        #line 257 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D ScatterSeries3DClusterDistance;
        
        #line default
        #line hidden
        
        
        #line 259 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.PointMarkers.EllipsePointMarker3D EllipseDistance;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D ScatterSeries3DHighlight;
        
        #line default
        #line hidden
        
        
        #line 266 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.PointMarkers.EllipsePointMarker3D EllipseHighlight;
        
        #line default
        #line hidden
        
        
        #line 272 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.Axis.NumericAxis3D axisX;
        
        #line default
        #line hidden
        
        
        #line 276 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.Axis.NumericAxis3D axisY;
        
        #line default
        #line hidden
        
        
        #line 280 "..\..\WpfRespirateurMonitor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SciChart.Charting3D.Axis.NumericAxis3D axisZ;
        
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
            System.Uri resourceLocater = new System.Uri("/WpfRespirateur_Interface_Monitor;component/wpfrespirateurmonitor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\WpfRespirateurMonitor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            this.MenuItemUsePitot4 = ((System.Windows.Controls.MenuItem)(target));
            
            #line 17 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemUsePitot4.Checked += new System.Windows.RoutedEventHandler(this.MenuItemUsePitot4_Checked);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MenuItemUsePitot6 = ((System.Windows.Controls.MenuItem)(target));
            
            #line 18 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemUsePitot6.Checked += new System.Windows.RoutedEventHandler(this.MenuItemUsePitot6_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MenuItemAdvanced = ((System.Windows.Controls.MenuItem)(target));
            
            #line 19 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemAdvanced.Click += new System.Windows.RoutedEventHandler(this.MenuItemAdvanced_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.MenuItemInit = ((System.Windows.Controls.MenuItem)(target));
            
            #line 20 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemInit.Click += new System.Windows.RoutedEventHandler(this.MenuItemInit_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MenuItemReanimation = ((System.Windows.Controls.MenuItem)(target));
            
            #line 23 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemReanimation.Click += new System.Windows.RoutedEventHandler(this.MenuItemReanimation_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MenuItemAssistance = ((System.Windows.Controls.MenuItem)(target));
            
            #line 24 "..\..\WpfRespirateurMonitor.xaml"
            this.MenuItemAssistance.Click += new System.Windows.RoutedEventHandler(this.MenuItemAssistance_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.GridApplication = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.oscilloVolume = ((WpfOscilloscopeControl.WpfOscilloscope)(target));
            
            #line 56 "..\..\WpfRespirateurMonitor.xaml"
            this.oscilloVolume.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ZoomOnGraph_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.textBoxVolume = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            
            #line 82 "..\..\WpfRespirateurMonitor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonVolumeSet);
            
            #line default
            #line hidden
            return;
            case 11:
            this.labelVolumeCurrentVal = ((System.Windows.Controls.Label)(target));
            return;
            case 12:
            this.oscilloPression = ((WpfOscilloscopeControl.WpfOscilloscope)(target));
            
            #line 92 "..\..\WpfRespirateurMonitor.xaml"
            this.oscilloPression.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ZoomOnGraph_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 13:
            this.textBoxPression = ((System.Windows.Controls.TextBox)(target));
            return;
            case 14:
            
            #line 118 "..\..\WpfRespirateurMonitor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonPressionSet);
            
            #line default
            #line hidden
            return;
            case 15:
            this.labelPressionCurrentVal = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.labelSeuilDetection = ((System.Windows.Controls.Label)(target));
            return;
            case 17:
            this.labelSeuilDetectionVal = ((System.Windows.Controls.Label)(target));
            return;
            case 18:
            this.sliderSeuil = ((System.Windows.Controls.Slider)(target));
            
            #line 129 "..\..\WpfRespirateurMonitor.xaml"
            this.sliderSeuil.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 19:
            this.groupBoxCycles = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 20:
            this.labelCycles = ((System.Windows.Controls.Label)(target));
            return;
            case 21:
            this.ButtonCycleP = ((System.Windows.Controls.Button)(target));
            
            #line 154 "..\..\WpfRespirateurMonitor.xaml"
            this.ButtonCycleP.Click += new System.Windows.RoutedEventHandler(this.ButtonCycleP_Click);
            
            #line default
            #line hidden
            return;
            case 22:
            this.ButtonCycleM = ((System.Windows.Controls.Button)(target));
            
            #line 155 "..\..\WpfRespirateurMonitor.xaml"
            this.ButtonCycleM.Click += new System.Windows.RoutedEventHandler(this.ButtonCycleM_Click);
            
            #line default
            #line hidden
            return;
            case 23:
            this.ButtonStartStop = ((System.Windows.Controls.Button)(target));
            
            #line 161 "..\..\WpfRespirateurMonitor.xaml"
            this.ButtonStartStop.Click += new System.Windows.RoutedEventHandler(this.ButtonStartStop_Click);
            
            #line default
            #line hidden
            return;
            case 24:
            this.labelSessionStart = ((System.Windows.Controls.Label)(target));
            return;
            case 25:
            this.labelSessionDuree = ((System.Windows.Controls.Label)(target));
            return;
            case 26:
            this.labelSessionStop = ((System.Windows.Controls.Label)(target));
            return;
            case 27:
            this.labelState = ((System.Windows.Controls.Label)(target));
            return;
            case 28:
            
            #line 176 "..\..\WpfRespirateurMonitor.xaml"
            ((System.Windows.Controls.GroupBox)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ZoomOnGraph_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 29:
            this.labelLearningPhaseInitDispersion = ((System.Windows.Controls.Label)(target));
            return;
            case 30:
            this.labelLearningPhaseTrainingClusters = ((System.Windows.Controls.Label)(target));
            return;
            case 31:
            this.labelLearningPhaseMonitoringFailures = ((System.Windows.Controls.Label)(target));
            return;
            case 32:
            this.labelLearningStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 33:
            this.labelNumberOfClusters = ((System.Windows.Controls.Label)(target));
            return;
            case 34:
            this.ClusterFeaturesGroupBox = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 35:
            this.DataGridCluster = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 36:
            this.sciChart = ((SciChart.Charting3D.SciChart3DSurface)(target));
            return;
            case 37:
            this.CameraSciChart = ((SciChart.Charting3D.Camera3D)(target));
            return;
            case 38:
            this.ScatterSeries3DSample = ((SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D)(target));
            return;
            case 39:
            this.EllipseSample = ((SciChart.Charting3D.PointMarkers.EllipsePointMarker3D)(target));
            return;
            case 40:
            this.ScatterSeries3DCluster = ((SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D)(target));
            return;
            case 41:
            this.EllipseCluster = ((SciChart.Charting3D.PointMarkers.EllipsePointMarker3D)(target));
            return;
            case 42:
            this.ScatterSeries3DClusterDispersion = ((SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D)(target));
            return;
            case 43:
            this.EllipseDispersion = ((SciChart.Charting3D.PointMarkers.EllipsePointMarker3D)(target));
            return;
            case 44:
            this.ScatterSeries3DClusterDistance = ((SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D)(target));
            return;
            case 45:
            this.EllipseDistance = ((SciChart.Charting3D.PointMarkers.EllipsePointMarker3D)(target));
            return;
            case 46:
            this.ScatterSeries3DHighlight = ((SciChart.Charting3D.RenderableSeries.ScatterRenderableSeries3D)(target));
            return;
            case 47:
            this.EllipseHighlight = ((SciChart.Charting3D.PointMarkers.EllipsePointMarker3D)(target));
            return;
            case 48:
            this.axisX = ((SciChart.Charting3D.Axis.NumericAxis3D)(target));
            return;
            case 49:
            this.axisY = ((SciChart.Charting3D.Axis.NumericAxis3D)(target));
            return;
            case 50:
            this.axisZ = ((SciChart.Charting3D.Axis.NumericAxis3D)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

