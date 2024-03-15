using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Wish.Models;

public class WishListRepository : IWishListRepository, IDisposable
{
    private readonly WishListContext _context;

    private bool _disposed = false;

    public WishListRepository(WishListContext context)
    {
        _context = context;
    }

    public IEnumerable<WishListItem> GetWishListItems() => _context.WishListItems.ToArray();

    public WishListItem FindItem(Func<WishListItem, bool> predicate)
    {
        return _context.WishListItems.FirstOrDefault(predicate) ?? WishListItem.NotFound;
    }
    
    public WishListItem FindById(int id)
    {
        return _context
            .WishListItems
            .Find(id)
            ?? WishListItem.NotFound;
    }

    public async Task<WishListItem> FindByIdAsync(int id)
    {
        return await _context
            .WishListItems
            .FindAsync(id) 
            ?? WishListItem.NotFound;
    }

    public void DeleteById(int id)
    {
        var item = FindById(id);
        if (item != WishListItem.NotFound)
        {
            _context.WishListItems.Remove(item);
        }
    }

    public async Task DeleteByIdAsync(int id)
    {
        var item = await FindByIdAsync(id);
        if (item != WishListItem.NotFound)
        {
            _context.WishListItems.Remove(item);
        }
    }

    public bool ItemExists(int id) 
        => (_context.WishListItems?.Any(e => e.Id == id)).GetValueOrDefault();

    public void UpdateItem(WishListItem item) => _context.Update(item);

    public void AppendItem(WishListItem item) => _context.Add(item);

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public void Save() => _context.SaveChanges();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}