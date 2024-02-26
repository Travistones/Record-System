using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Data
{
    public class Grade : BindableBase
    {
        private double _price = double.NaN;
        private Qualities _quality;

        public double Price
        {
            get => _price;

            set => SetProperty(ref _price, value);
        }

        public Qualities Quality
        {
            get => _quality;

            set => SetProperty(ref _quality, value);
        }

        public bool HasPrice
        {
            get => Price != double.NaN;
        }
    }
}
