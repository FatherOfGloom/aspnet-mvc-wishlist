namespace Wish.Models;

public interface IWishListRepository
{
    IEnumerable<WishListItem> GetWishListItems();
    WishListItem FindById(int id);
    Task<WishListItem> FindByIdAsync(int id);
    void DeleteById(int id);
    Task DeleteByIdAsync(int id);
    void AppendItem(WishListItem item);
    void UpdateItem(WishListItem item);
    bool ItemExists(int id);
    Task SaveAsync();
    void Save();
    WishListItem FindItem(Func<WishListItem, bool> predicate);
}