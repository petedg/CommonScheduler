using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CommonScheduler
{
    /// <summary>
    /// Static Class with one method to set proper string resource (depends on system localization)
    /// Method SetLanguageDictionary should be executed before InitializeComponent() in each window class
    /// </summary>
    static class Multilingual
    {
        public static void SetLanguageDictionary(ResourceDictionary windowResourceDictionary)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "pl-PL":
                    dict.Source = new Uri("\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
            }
            windowResourceDictionary.MergedDictionaries.Add(dict);
        }
    }
}
