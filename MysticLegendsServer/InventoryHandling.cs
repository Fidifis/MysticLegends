using MysticLegendsShared.Models;

namespace MysticLegendsServer;

public static class InventoryHandling
{
    public static int? FindPositionInInventory(CharacterInventory inventory, int desiredPosition = 0)
    {
        bool positionFound = false;

        var capacity = inventory.Capacity;
        for (int i = 0; i < capacity; i++)
        {
            if (inventory.InventoryItems.FirstOrDefault(item => item.Position == desiredPosition) is not null)
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
