using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Timers;
using Windows.UI.Text;
using Microsoft.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clock
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PresenterWindow : Window
    {
        
        public PresenterWindow()
        {
            this.InitializeComponent();
            this.RemoveBorders();
            UTCTimeProvider.OnFullSecond += UTCTimeProvider_OnFullSecond;
        }

        private void UTCTimeProvider_OnFullSecond(DateTime obj)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                serverTime.Text = obj.ToString("HH:mm:ss");
            });
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Visibility = Visibility.Collapsed;
            this.PreventResizing()
                .SetTransparency(0.9)
                .SetTopMost();
            
        }

    }

}
