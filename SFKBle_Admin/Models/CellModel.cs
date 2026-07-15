using Android.Telephony;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SFKBle_Admin
{
    public class CellInfo : INotifyPropertyChanged
    {
        private string batteryname { get; set; }
        private string cellname { get; set; }
        private decimal cellvolts { get; set; }
        private decimal cell1 { get; set; }
        private decimal cell2 { get; set; }
        private decimal cell3 { get; set; }
        private decimal cell4 { get; set; }
        private decimal cell5 { get; set; }
        private decimal cell6 { get; set; }
        private decimal cell7 { get; set; }
        private decimal cell8 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string CellName
        {
            get { return cellname; }
            set
            {
                cellname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CellName"));
            }
        }
        public string BatteryName
        {
            get { return batteryname; }
            set
            {
                batteryname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BatteryName"));
            }
        }
        public decimal CellVolts
        {
            get { return cellvolts; }
            set
            {
                cellvolts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CellVolts"));
            }
        }
        public decimal Cell1
        {
            get { return cell1; }
            set
            {
                cell1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell1"));
            }
        }
        public decimal Cell2
        {
            get { return cell2; }
            set
            {
                cell2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell2"));
            }
        }
        public decimal Cell3
        {
            get { return cell3; }
            set
            {
                cell3 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell3"));
            }
        }
        public decimal Cell4
        {
            get { return cell4; }
            set
            {
                cell4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell4"));
            }
        }
        public decimal Cell5
        {
            get { return cell5; }
            set
            {
                cell5 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell5"));
            }
        }
        public decimal Cell6
        {
            get { return cell6; }
            set
            {
                cell6 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell6"));
            }
        }
        public decimal Cell7
        {
            get { return cell7; }
            set
            {
                cell7 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell7"));
            }
        }
        public decimal Cell8
        {
            get { return cell8; }
            set
            {
                cell8 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell8"));
            }
        }

    }
    public class CellInfoForCombine : INotifyPropertyChanged
    {
        private string cellname { get; set; }
        private decimal cell { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CellName
        {
            get { return cellname; }
            set
            {
                cellname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CellName"));
            }
        }
        public decimal Cell
        {
            get { return cell; }
            set
            {
                cell = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cell"));
            }
        }
    }
    public class TempCellInfo
    {
        public string batteryname { get; set; }
        public string cellname { get; set; }
        public decimal cellvolts { get; set; }
        public decimal cell1 { get; set; }
        public decimal cell2 { get; set; }
        public decimal cell3 { get; set; }
        public decimal cell4 { get; set; }
        public decimal cell5 { get; set; }
        public decimal cell6 { get; set; }
        public decimal cell7 { get; set; }
        public decimal cell8 { get; set; }
    }
    public class CellModel
    {
        public ObservableCollection<CellInfo> Data { get; set; }
        public ObservableCollection<Brush> Colors { get; set; }
        public ObservableCollection<Brush> TextColors { get; set; }

        public CellModel()
        {
            Data = new ObservableCollection<CellInfo>();

            Data.Add(new CellInfo { CellName = "1", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "2", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "3", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "4", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "5", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "6", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "7", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "8", CellVolts = 0.0M });

            Colors = new ObservableCollection<Brush>();
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));

            TextColors = new ObservableCollection<Brush>();
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
        }
    }
    public class BattetryData
    {
        public ObservableCollection<CellInfo> BData1 { get; set; }
        public ObservableCollection<CellInfo> BData2 { get; set; }
        public ObservableCollection<CellInfo> BData3 { get; set; }
        public ObservableCollection<CellInfo> BData4 { get; set; }
        public ObservableCollection<CellInfo> BData5 { get; set; }
        public ObservableCollection<CellInfo> BData6 { get; set; }
        public ObservableCollection<CellInfo> BData7 { get; set; }
        public ObservableCollection<CellInfo> BData8 { get; set; }
        public ObservableCollection<Brush> Battery1Colors { get; set; }
        public ObservableCollection<Brush> Battery2Colors { get; set; }
        public ObservableCollection<Brush> Battery3Colors { get; set; }
        public ObservableCollection<Brush> Battery4Colors { get; set; }
        public ObservableCollection<Brush> Battery5Colors { get; set; }
        public ObservableCollection<Brush> Battery6Colors { get; set; }
        public ObservableCollection<Brush> Battery7Colors { get; set; }
        public ObservableCollection<Brush> Battery8Colors { get; set; }
        public ObservableCollection<Brush> Battery1TextColors { get; set; }
        public ObservableCollection<Brush> Battery2TextColors { get; set; }
        public ObservableCollection<Brush> Battery3TextColors { get; set; }
        public ObservableCollection<Brush> Battery4TextColors { get; set; }
        public ObservableCollection<Brush> Battery5TextColors { get; set; }
        public ObservableCollection<Brush> Battery6TextColors { get; set; }
        public ObservableCollection<Brush> Battery7TextColors { get; set; }
        public ObservableCollection<Brush> Battery8TextColors { get; set; }
        public ObservableCollection<CellInfoForCombine> SingleGraphBattery { get; set; }
        public ObservableCollection<Brush> SingleGraphCellColor { get; set; }
        public ObservableCollection<Brush> SingleGraphTextColor { get; set; }

        public BattetryData()
        {
            BData1 = new ObservableCollection<CellInfo>();
            BData2 = new ObservableCollection<CellInfo>();
            BData3 = new ObservableCollection<CellInfo>();
            BData4 = new ObservableCollection<CellInfo>();

            BData5 = new ObservableCollection<CellInfo>();
            BData6 = new ObservableCollection<CellInfo>();
            BData7 = new ObservableCollection<CellInfo>();
            BData8 = new ObservableCollection<CellInfo>();

            Battery1Colors = new ObservableCollection<Brush>();
            Battery2Colors = new ObservableCollection<Brush>();
            Battery3Colors = new ObservableCollection<Brush>();
            Battery4Colors = new ObservableCollection<Brush>();

            Battery5Colors = new ObservableCollection<Brush>();
            Battery6Colors = new ObservableCollection<Brush>();
            Battery7Colors = new ObservableCollection<Brush>();
            Battery8Colors = new ObservableCollection<Brush>();

            Battery1TextColors = new ObservableCollection<Brush>();
            Battery2TextColors = new ObservableCollection<Brush>();
            Battery3TextColors = new ObservableCollection<Brush>();
            Battery4TextColors = new ObservableCollection<Brush>();

            Battery5TextColors = new ObservableCollection<Brush>();
            Battery6TextColors = new ObservableCollection<Brush>();
            Battery7TextColors = new ObservableCollection<Brush>();
            Battery8TextColors = new ObservableCollection<Brush>();


            SingleGraphBattery = new ObservableCollection<CellInfoForCombine>();
            SingleGraphCellColor = new ObservableCollection<Brush>();
            SingleGraphTextColor = new ObservableCollection<Brush>();
        }
    }
    public class TempCellViewModel
    {
        public List<CellInfo> Data { get; set; }
        public List<Brush> Colors { get; set; }
        public List<Brush> TextColors { get; set; }

        public TempCellViewModel()
        {
            Data = new List<CellInfo>();

            Data.Add(new CellInfo { CellName = "1", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "2", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "3", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "4", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "5", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "6", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "7", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "8", CellVolts = 0.0M });

            Colors = new List<Brush>();
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));

            TextColors = new List<Brush>();
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
        }
    }
    public class CellViewModel
    {
        public ObservableCollection<CellInfo> Data { get; set; }
        public ObservableCollection<Brush> Colors { get; set; }
        public ObservableCollection<Brush> TextColors { get; set; }

        public CellViewModel()
        {
            Data = new ObservableCollection<CellInfo>();

            Data.Add(new CellInfo { CellName = "1", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "2", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "3", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "4", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "5", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "6", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "7", CellVolts = 0.0M });
            Data.Add(new CellInfo { CellName = "8", CellVolts = 0.0M });

            Colors = new ObservableCollection<Brush>();
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));
            Colors.Add(Color.FromArgb("#000000"));

            TextColors = new ObservableCollection<Brush>();
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
            TextColors.Add(Color.FromArgb("#ffffff"));
        }
    }
}