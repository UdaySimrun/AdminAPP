using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFKBle_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MobileDevicesStyle : ResourceDictionary
    {
        public static MobileDevicesStyle SharedInstance { get; } = new MobileDevicesStyle();
        public MobileDevicesStyle()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
            }
        }
    }
}