using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart, LibraryContext>, ICartRepository
    {
        public void DeleteFromCart(int cartId, int bookId)
        {
            using (var context = new LibraryContext())
            {
                var cartItem = context.CartItems
                                      .Where(c => c.CartId == cartId && c.BookId == bookId)
                                      .FirstOrDefault();
                context.Remove(cartItem);
                context.SaveChanges();
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            using (var context = new LibraryContext())
            {
                return context.Carts
                              .Include(c => c.CartItems)
                              .ThenInclude(c => c.Book)
                              .FirstOrDefault(i => i.UserId == userId);
            }
        }

        public override void Update(Cart entity)
        {
            using (var context = new LibraryContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }
    }
}
