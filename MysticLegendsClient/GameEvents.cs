namespace MysticLegendsClient;

internal class GameEvents
{
    public event EventHandler<CurrencyUpdateEventArgs>? CurrencyUpdateEvent;
    public void CurrencyUpdate(object? sender, CurrencyUpdateEventArgs e) => CurrencyUpdateEvent?.Invoke(sender, e);

    public event EventHandler<CharacterInventoryUpdateEventArgs>? CharacterInventoryUpdateEvent;
    public void CharacterInventoryUpdate(object? sender, CharacterInventoryUpdateEventArgs e) => CharacterInventoryUpdateEvent?.Invoke(sender, e);
}
