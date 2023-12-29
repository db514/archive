using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfRandomSelector
{
    /// <summary>
    /// Interaction logic for AlreadyViewListDisplay.xaml
    /// </summary>
    public partial class AlreadyViewListDisplay : Window
    {      
        public AlreadyViewListDisplay()
        {
            InitializeComponent();
        }

        public AlreadyViewListDisplay(List<string> dataSet)
        {
            InitializeComponent();
            ListboxViewedItems.ItemsSource = dataSet.OrderBy(q => q).ToList();
        }

       
    }
}
