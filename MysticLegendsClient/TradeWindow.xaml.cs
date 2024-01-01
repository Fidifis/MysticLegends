using MysticLegendsClient.Resources;
using MysticLegendsShared.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro TradeWindow.xaml
    /// </summary>
    public partial class TradeWindow : Window, ISingleInstanceWindow, IItemView
    {
        private readonly string cityName;
        private ItemSlot itemSlot;

        private ItemViewRelation? ViewRelation;

        public event IItemView.ItemDropEventHandler? ItemDropEvent;

        public bool CanMoveItems => true;
        public bool CanTransitItems => true;

        public IEnumerable<InventoryItem> Items
        { 
            get 
            {
                if (itemSlot.Item is not null)
                    yield return itemSlot.Item;
            }
            set
            {
                if (value.Any())
                {
                    Debug.Assert(!value.Skip(1).Any(), "Array must contain only one item");
                    AddItem(value.First());
                }
                else
                    RemoveItem();
            }
        }

        public TradeWindow(string cityName)
        {
            InitializeComponent();
            this.cityName = cityName;
            itemSlot = new(this, 0);

            ItemDropEvent += ItemDrop;
        }

        public void ShowWindow()
        {
            SingleInstanceWindow.CommonShowWindowTasks(this);
        }

        private void ItemSlot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var data = new DataObject(typeof(ItemSlot), itemSlot);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }

        private void ItemSlot_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ItemSlot)))
            {
                var sourceSlot = (ItemSlot)e.Data.GetData(typeof(ItemSlot));
                var targetSlot = itemSlot;

                InvokeItemDropEvent(sourceSlot.Owner, new ItemDropEventArgs(sourceSlot, targetSlot));
            }
        }

        public void AddItem(InventoryItem item)
        {
            itemSlot.Item = item;
            itemImage.Source = BitmapTools.ImageFromResource(ItemIcons.ResourceManager.GetString(item.Item.Icon)!);
        }

        public void RemoveItem()
        {
            itemSlot.Item = null;
            itemImage.Source = null;
        }

        public void UpdateItem(InventoryItem updatedItem) => AddItem(updatedItem);

        public void InvokeItemDropEvent(IItemView sender, ItemDropEventArgs args) => ItemDropEvent?.Invoke(sender, args);

        public void AddRelation(ItemViewRelation relation)
        {
            ViewRelation?.CloseRelation();
            ViewRelation = relation;
        }

        private void RemoveRelationAny()
        {
            ViewRelation = null;
            RemoveItem();
        }

        public void RemoveRelationFromManaged(ItemSlot managed)
        {
            RemoveRelationAny();
        }

        public void RemoveRelationFromTransit(ItemSlot transit)
        {
            RemoveRelationAny();
        }

        public void CloseRelations()
        {
            ViewRelation?.CloseRelation();
        }

        public ItemViewRelation? GetRelationBySlot(ItemSlot slot) => ViewRelation;

        private void ItemDrop(IItemView sender, ItemDropEventArgs args)
        {
            if (args.IsHandover)
            {
                // TODO ...
            }
            else if (sender == tradeView)
            {

            }
            else
            {
                // Item comes from inventory

                var itemCopy = args.FromSlot.Item!.PartialCopy();

                if (ItemViewRelation.EstablishRelation(args.FromSlot, args.ToSlot))
                    AddItem(itemCopy);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CloseRelations();
        }
    }
}
