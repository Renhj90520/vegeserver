using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.DTO;
using Vege.Models;

namespace Vege.Repositories
{
    public interface IVegeRepository
    {
        ItemsResult<Product> GetAllProduct(int? id, int? catetoryId, int? index, int? perPage);

        Task<Product> AddProduct(Product product);

        Task<bool> AddCategory(Category category);

        Task<bool> AddUnit(Unit unit);
        Task<IEnumerable<Unit>> GetAllUnits();
        Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid);
        Task<bool> AddCartItem(string openId, CartItem cartItem);
        IEnumerable<Order> GetAllOrders(string openId);
        Task<bool> AddOrder(Order order);
        Task<bool> DeleteUnit(int id);
    }
}
