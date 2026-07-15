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
    public partial class SmallTabletDeviceStyle : ResourceDictionary
    {
        public static SmallTabletDeviceStyle SharedInstance { get; } = new SmallTabletDeviceStyle();
        public SmallTabletDeviceStyle()
        {
            InitializeComponent();
        }
    }
}