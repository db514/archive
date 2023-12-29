using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace WpfRandomSelector
{
    /// <summary>
    /// Interaction logic for AlreadyViewListDisplay.xaml
    /// </summary>
    public partial class AlreadyViewListDisplay : Window
    {
        readonly MainWindow mainWindow;
        

        List<string> listboxItems = new List<string>();

        public AlreadyViewListDisplay()
        {
            InitializeComponent();
        }

        //The constructor takes the AlreadyViewedList and a reference to the calling window 
        public AlreadyViewListDisplay(List<string> dataSet, Window main)
        {
            //set mainwWindow to the passed in reference window
            mainWindow = main as MainWindow;

            InitializeComponent();

            listboxItems = dataSet.OrderBy(q => q).ToList();

            ListboxViewedItems.ItemsSource = listboxItems;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            List<string> ItemsToDelete;

            ItemsToDelete = ListboxViewedItems.SelectedItems.Cast<string>().ToList();
            DeleteItems(ItemsToDelete);
        }

        private void DeleteItems(List<string> itemsToBeDeleted)
        {           
            //call the delete from the calling window
            mainWindow.ItemsToBeDeletedChanged(itemsToBeDeleted);

            foreach (string item in itemsToBeDeleted)
            {
                listboxItems.Remove(item);
            }

            ListboxViewedItems.Items.Refresh();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // DeleteItems(listboxItems);
            listboxItems.Clear();
            ListboxViewedItems.Items.Refresh();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ListboxViewedItems.Items.Refresh();
        }
    }
}
