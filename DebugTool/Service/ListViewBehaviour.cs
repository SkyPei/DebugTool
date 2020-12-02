using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DebugTool.Service
{
    public static class ListViewBehaviour
    {
        public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy",
       typeof(bool), typeof(ListViewBehaviour), new UIPropertyMetadata(AutoCopyChanged));

        public static bool GetAutoCopy(DependencyObject obj_)
        {
            return (bool)obj_.GetValue(AutoCopyProperty);
        }

        public static void SetAutoCopy(DependencyObject obj_, bool value_)
        {
            obj_.SetValue(AutoCopyProperty, value_);
        }

        private static void AutoCopyChanged(DependencyObject obj_, DependencyPropertyChangedEventArgs e_)
        {
            var listView = obj_ as ListView;
            if (listView != null)
            {
                if ((bool)e_.NewValue)
                {
                    ExecutedRoutedEventHandler handler =
                        (sender_, arg_) =>
                        {
                            if (listView.SelectedItems != null && listView.SelectedItems.Count > 0)
                            {
                                //Copy what ever your want here
                                StringBuilder sb = new StringBuilder();
                                foreach (var item in listView.SelectedItems)
                                {
                                    sb.AppendLine(item.ToString());
                                }
                                Clipboard.SetDataObject(sb.ToString());
                            }
                        };

                    var command = new RoutedCommand("Copy", typeof(ListView));
                    command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
                    listView.CommandBindings.Add(new CommandBinding(command, handler));
                }
            }
        }
    }
}
