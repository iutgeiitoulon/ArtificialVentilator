using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFInterfaceUnsupervisedLearning
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Ensure SetLicenseKey is called once, before any SciChartSurface instance is created 
            // Check this code into your version-control and it will enable SciChart 
            // for end-users of your application. 
            // 
            // You can test the Runtime Key is installed correctly by Running your application 
            // OUTSIDE Of Visual Studio (no debugger attached). Trial watermarks should be removed. 
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("d5LxyOnankWo/T3eKtzC21Tya6NHt33F9l4FYjTyb2UekJiUGgfEz7/b5WpRlc1ZiOd6phLwjGdRp3F2H5yLm1IHW+axuBaFjhhXXAeZVkFujziMXokyLRyAV8SMFuHNo+1f/2WRzIb0aDYkAMBvJ4+PzhdvXlxYyYAo0U/fdu3S0AzC8l1afiw0GDy79vthx87DaKd7gZXysRIV/3z1lVM4saOrB0uX3FXgzCzidsceIsA63cehAI+EUQyii63o2NsyQWgSUuvaGnCUtU2oOR1B7QZkzjr14hXDLgNItEs46CjSEf5ngd3CiGuVfBGKWuv77y5ZyCML+zT+2D1/qoRJE91GalxUhL6YWLvxc0lLmQYNeg/nh6ksTnFj8qRkFfeDMM1otVAGWJYWA4OKuckXjJHFDqDRulIun8BmBOekbo5aLK/yCH5QqZZt1K2sKVvbt0AGdlCTG6UDUyoeMFzwJo3J21ch3znwt16DhceBDYyb");

        }
    }
}
