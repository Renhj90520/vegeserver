using Microsoft.AspNetCore.JsonPatch;
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
        Task<Category> AddCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<bool> AddUnit(Unit unit);
        Task<IEnumerable<Unit>> GetAllUnits();
        Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid);
        Task<bool> AddCartItem(string openId, CartItem cartItem);
        ItemsResult<OrderDTO> GetAllOrders(string openId, int? index, int? perPage, string keyword, DateTime? begin, DateTime? end, bool? noshowRemove);
        Task<bool> AddOrder(Order order);
        Task<bool> UpdateOrder(int id, JsonPatchDocument<Order> patchDoc);
        Task<bool> RemoveOrder(int id);
        Task<bool> DeleteUnit(int id);
        Task<string> GetFilePath(int id);
        Task<Picture> AddPicture(string path);
        Task<IEnumerable<Address>> GetAllAddress(string openId);
        Task<Address> AddAddress(Address address);
        Task<bool> RemoveProductPic(string picPath);
        Task<bool> RemoveCategoryPic(int categoryid);
    }
}
