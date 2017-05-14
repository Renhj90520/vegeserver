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
        List<Product> GetAllProduct(int? id, int? catetoryId);

        Task<Product> AddProduct(Product product);

        Task<bool> AddCategory(Category category);

        Task<bool> AddUnit(Unit unit);
        Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid);
        Task<bool> AddCartItem(string openId, CartItem cartItem);
        IEnumerable<Order> GetAllOrders(string openId);
        Task<bool> AddOrder(Order order);

    }
}
