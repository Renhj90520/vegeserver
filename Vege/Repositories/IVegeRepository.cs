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
        ItemsResult<Product> GetAllProduct(int? id, int? catetoryId, int? index, int? perPage, string name, int? state);
        Task<Product> AddProduct(Product product);
        Task<Category> AddCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories(string state);
        Task<Unit> AddUnit(Unit unit);
        Task<IEnumerable<Unit>> GetAllUnits();
        //Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid);
        //Task<bool> AddCartItem(string openId, CartItem cartItem);
        ItemsResult<OrderDTO> GetAllOrders(string openId, int? index, int? perPage, string keyword, int? state, DateTime? begin, DateTime? end, bool? noshowRemove);
        Task<Order> AddOrder(Order order);
        Task<bool> UpdateOrder(int id, JsonPatchDocument<Order> patchDoc);
        Task<bool> RemoveOrder(int id);
        Task<bool> DeleteUnit(int id);
        Task<string> GetFilePath(int id);
        Task<Picture> AddPicture(string path);
        Task<IEnumerable<Address>> GetAllAddress(string openId);
        Task<Address> AddAddress(Address address);
        Task<bool> RemoveProductPic(string picPath);
        Task<bool> RemoveCategoryPic(int categoryid);
        Task<string> RemoveCategory(int id);
        Task<bool> UpdateProduct(Product newProduct);
        Task<bool> DeleteAddr(int id);
        Task<bool> UpdateCate(Category cate);
        Task<bool> UpdateUnit(Unit unit);
        Task<bool> UpdateProduct(int id, JsonPatchDocument<Product> patchDoc);
        Task<User> AddUser(User user);
        Task<bool> CheckUserExists(string openid);
        Task<bool> CheckUserExists(string userName, string password);
        Task<ItemsResult<User>> GetAllUsers(int? index, int? perPage, string keyword);
        Task<Favorite> AddFavorite(Favorite fav);
        Task<bool> DeleteFavorite(int id);
        Task<IEnumerable<FavoriteDTO>> getFavorites(string openid, int? productId);

        Task refundOrder(int id, RefundWrapper wrapper, Result<bool> result);
        Task<bool> reorder(int id1, int id2);

        Task<bool> changePwd(string username, string oldpwd, string newpwd, Result<bool> result);

        Task<bool> patchCate(int id, JsonPatchDocument<Category> patchDoc);

        Task<bool> reorderCate(int id1, int id2);

        Task<Coupon> addCoupon(Coupon coupon);

        Task patchCoupon(int id, JsonPatchDocument<Coupon> patchDoc, Result<bool> result);

        Task updateCoupon(int id, Coupon coupon, Result<bool> result);

        Task<IEnumerable<Coupon>> getAllCoupons();

        Task<Coupon> getValidityCoupon();

        Task<bool> removeCouponPic(int id);
    }
}
