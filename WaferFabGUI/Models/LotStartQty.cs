using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WaferFabGUI.Models
{
    public class LotStartQty : INotifyPropertyChanged
    {
        public string LotType { get; set; }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            { 
                _quantity = value;
                NotifyOfPropertyChange();
            }
        }

        public LotStartQty(string lotType, int quantity)
        {
            LotType = lotType;
            Quantity = quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
