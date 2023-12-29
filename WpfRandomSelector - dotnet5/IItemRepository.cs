namespace WpfRandomSelector
{
    interface IItemRepository
    {
        void GetItemList(string path);

        string ReturnItem();

    }
}
