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
        Task<IEnumerable<Category>> GetAllCategories();
        Task<bool> AddUnit(Unit unit);
        Task<IEnumerable<Unit>> GetAllUnits();
        Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid);
        Task<bool> AddCartItem(string openId, CartItem cartItem);
        ItemsResult<Order> GetAllOrders(string openId,int? index,int? perPage);
        Task<bool> AddOrder(Order order);
        Task<bool> DeleteUnit(int id);
        Task<string> GetFilePath(int id);
        Task<Picture> AddPicture(string path);
    }
}
