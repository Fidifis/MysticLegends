using MysticLegendsShared.Models;

namespace MysticLegendsServer;

public static class InventoryHandling
{
    public static int? FindPositionInInventory(CharacterInventory inventory, int desiredPosition = 0) =>
        FindPositionInInventory(inventory.InventoryItems, inventory.Capacity, desiredPosition);

    public static int? FindPositionInInventory(CityInventory inventory, int desiredPosition = 0) =>
        FindPositionInInventory(inventory.InventoryItems, inventory.Capacity, desiredPosition);

    public static int? FindPositionInInventory(IEnumerable<InventoryItem> inventory, int capacity, int desiredPosition = 0)
    {
        bool positionFound = false;

        for (int i = 0; i < capacity; i++)
        {
            if (inventory.FirstOrDefault(item => item.Position == desiredPosition) is not null)
            {
                if (++desiredPosition >= capacity)
                    desiredPosition -= capacity;
            }
            else
            {
                positionFound = true;
                break;
            }
        }

        return positionFound ? desiredPosition : null;
    }
}
