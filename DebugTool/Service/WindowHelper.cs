using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DebugTool.Service
{
    public static class WindowHelper
    {
        public static readonly DependencyProperty ShowMinimizeButtonProperty = DependencyProperty.RegisterAttached("ShowMinimizeButton", typeof(bool), typeof(WindowHelper), new PropertyMetadata(true));


        public static void SetShowMinimizeButton(DependencyObject element, bool value)
        {
            element.SetValue(ShowMinimizeButtonProperty, value);
        }

        public static bool GetShowMinimizeButton(DependencyObject element)
        {
            return (bool)element.GetValue(ShowMinimizeButtonProperty);
        }

        public static readonly DependencyProperty ShowMaximizeButtonProperty = DependencyProperty.RegisterAttached("ShowMaximizeButton", typeof(bool), typeof(WindowHelper), new PropertyMetadata(true));


        public static void SetShowMaximizeButton(DependencyObject element, bool value)
        {
            element.SetValue(ShowMaximizeButtonProperty, value);
        }

        public static bool GetShowMaximizeButton(DependencyObject element)
        {
            return (bool)element.GetValue(ShowMaximizeButtonProperty);
        }

    }
}
