using MysticLegendsShared.Models;

namespace MysticLegendsClient;

internal class GameEvents
{
    public event EventHandler<UpdateEventArgs<int>>? CurrencyUpdateEvent;
    public void CurrencyUpdate(object? sender, UpdateEventArgs<int> e) => CurrencyUpdateEvent?.Invoke(sender, e);

    public event EventHandler<UpdateEventArgs<IReadOnlyCollection<InventoryItem>>>? CharacterInventoryUpdateEvent;
    public void CharacterInventoryUpdate(object? sender, UpdateEventArgs<IReadOnlyCollection<InventoryItem>> e) => CharacterInventoryUpdateEvent?.Invoke(sender, e);

    public event EventHandler<UpdateEventArgs<Character>>? CharacterUpdateEvent;
    public void CharacterUpdate(object? sender, UpdateEventArgs<Character> e) => CharacterUpdateEvent?.Invoke(sender, e);

    public event EventHandler<UpdateEventArgs<Character>>? CharacterWithItemsUpdateEvent;
    public void CharacterWithItemsUpdate(object? sender, UpdateEventArgs<Character> e) => CharacterWithItemsUpdateEvent?.Invoke(sender, e);
}
