using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.DTO;
using Vege.Models;
using Vege.Utils;
using WxPayAPI;

namespace Vege.Repositories
{
    public class VegeRepository : IVegeRepository
    {
        private VegeContext context;
        private ILogger<VegeRepository> log;

        public VegeRepository(VegeContext context, ILogger<VegeRepository> log)
        {
            this.context = context;
            this.log = log;
        }

        public async Task<Category> AddCategory(Category category)
        {
            var newCate = this.context.Categories.Add(category).Entity;
            if ((await this.context.SaveChangesAsync()) > 0)
            {
                return newCate;
            }
            else
            {
                return null;
            }
        }

        public async Task<Product> AddProduct(Product product)
        {
            var maxseq = await context.Products.Where(p => p.CategoryId == product.CategoryId).Select(p => p.Sequence).MaxAsync(p => p);
            product.Sequence = maxseq + 1;

            var newProduct = this.context.Products.Add(product).Entity;
            if (await this.context.SaveChangesAsync() > 0)
            {
                return newProduct;
            }
            return null;
        }

        public async Task<Unit> AddUnit(Unit unit)
        {
            var un = this.context.Units.Add(unit);
            if ((await this.context.SaveChangesAsync()) > 0)
            {
                return un.Entity;
            }
            else
            {
                return null;
            }
        }

        public ItemsResult<Product> GetAllProduct(int? id, int? catetoryId, int? index, int? perPage, string name, int? state)
        {
            var products = this.context.Products.Include(p => p.Pictures).Select(p => p);
            if (state != null)
            {
                products = products.Where(p => p.State == state);
            }

            if (id == null && catetoryId != null)
            {
                products = products.Where(p => p.CategoryId == catetoryId);
            }
            if (id == null && !string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            if (id != null)
            {
                products = products.Where(p => p.Id == id);
            }

            //var items = (from p in products
            //             join u in this.context.Units on p.UnitId equals u.Id into pus
            //             from pu in pus.DefaultIfEmpty()
            //             select new Product()
            //             {
            //                 Id = p.Id,
            //                 Name = p.Name,
            //                 CategoryId = p.CategoryId,
            //                 Description = p.Description,
            //                 Pictures = p.Pictures,
            //                 Price = p.Price,
            //                 TotalCount = p.TotalCount,
            //                 UnitId = p.UnitId,
            //                 UnitName = pu.Name,
            //                 Step = p.Step,
            //                 State = p.State
            //             });
            products = products.OrderBy(p => p.Sequence);
            var result = new ItemsResult<Product>();
            result.count = products.Count();
            if (index != null)
            {
                result.items = products.Skip((index.Value - 1) * perPage.Value).Take(perPage.Value);
            }
            else
            {
                result.items = products;
            }

            return result;
        }

        //public async Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid)
        //{
        //    var cart = await this.context.ShoppingCarts.Include(c => c.Products).FirstOrDefaultAsync();
        //    if (cart != null)
        //    {
        //        var cartItems = from cp in cart.Products
        //                        join p in this.context.Products.Include(p => p.Pictures) on cp.ProductId equals p.Id
        //                        join u in this.context.Units on p.UnitId equals u.Id
        //                        select new CartItemDTO
        //                        {
        //                            Id = p.Id,
        //                            Count = cp.Count,
        //                            Description = p.Description,
        //                            Name = p.Name,
        //                            Pictures = p.Pictures,
        //                            Price = p.Price,
        //                            UnitName = u.Name
        //                        };
        //        return cartItems;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public async Task<bool> AddCartItem(string openId, CartItem cartItem)
        //{
        //    var cart = await this.context.ShoppingCarts.Include(c => c.Products).FirstOrDefaultAsync();
        //    if (cart == null)
        //    {
        //        ShoppingCart newCart = new ShoppingCart()
        //        {
        //            OpenId = openId,
        //            Products = new List<CartItem>()
        //        };
        //        newCart.Products.Add(cartItem);
        //        await this.context.ShoppingCarts.AddAsync(newCart);
        //        return (await this.context.SaveChangesAsync()) > 0;
        //    }
        //    else
        //    {
        //        if (cart.Products.Any(p => p.ProductId == cartItem.ProductId))
        //        {
        //            cart.Products.Where(p => p.ProductId == cartItem.ProductId).FirstOrDefault().Count += cartItem.Count;
        //        }
        //        else
        //        {
        //            cart.Products.Add(cartItem);
        //        }
        //        return (await this.context.SaveChangesAsync()) > 0;
        //    }
        //}

        public ItemsResult<OrderDTO> GetAllOrders(string openId, int? index, int? perPage, string keyword, int? state, DateTime? begin, DateTime? end, bool? noshowRemove)
        {
            var orders = this.context.Orders.Include(order => order.Products).Join(this.context.Addresses, i => i.AddressId, a => a.Id, (ii, aa) => new OrderDTO
            {
                Id = ii.Id,
                Area = aa.Area,
                CancelTime = ii.CancelTime,
                City = aa.City,
                CreateTime = ii.CreateTime,
                FinishTime = ii.FinishTime,
                Name = aa.Name,
                OpenId = ii.OpenId,
                Phone = aa.Phone,
                Province = aa.Province,
                State = ii.State,
                Street = aa.Street,
                DeliveryCharge = ii.DeliveryCharge,
                WXOrderId = ii.WXOrderId,
                Latitude = ii.Latitude,
                Longitude = ii.Longitude,
                RefundNote = ii.RefundNote,
                CancelReason = ii.CancelReason,
                IsPaid = ii.IsPaid,
                Products = ii.Products.Join(this.context.Products, op => op.ProductId, p => p.Id, (oop, pp) => new OrderItemDTO
                {
                    Id = oop.Id,
                    CategoryId = pp.CategoryId,
                    Count = oop.Count,
                    Description = pp.Description,
                    Name = pp.Name,
                    Pictures = pp.Pictures,
                    Price = oop.Price,
                    State = pp.State,
                    Step = pp.Step,
                    TotalCount = pp.TotalCount,
                    UnitId = pp.UnitId,
                    UnitName = pp.UnitName
                })
            });
            if (!string.IsNullOrEmpty(openId))
            {
                orders = orders.Where(o => o.OpenId == openId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                orders = orders.Where(o => (o.Name.ToLower().Contains(keyword.ToLower()) || o.Phone.Contains(keyword)));
            }
            if (begin != null)
            {
                orders = orders.Where(o => o.CreateTime.CompareTo(begin.Value) >= 0);
            }
            if (end != null)
            {
                orders = orders.Where(o => o.CreateTime.CompareTo(end.Value.AddDays(1).AddSeconds(-1)) <= 0);
            }
            if (noshowRemove != null)
            {
                if (noshowRemove.Value)
                {
                    orders = orders.Where(o => o.State != 7);
                }
            }
            if (state != null)
            {
                orders = orders.Where(o => o.State == state.Value);
            }
            orders = orders.OrderBy(o => o.State).ThenByDescending(o => o.Id);
            ItemsResult<OrderDTO> result = new ItemsResult<OrderDTO>();
            result.count = orders.Count();
            if (index != null)
            {
                result.items = orders.Skip((index.Value - 1) * perPage.Value).Take(perPage.Value);
            }
            else
            {
                result.items = orders;
            }

            return result;
        }

        public async Task<Order> AddOrder(Order order)
        {
            var newOrder = this.context.Orders.Add(order).Entity;
            //this.context.CartItems.RemoveRange(this.context.CartItems);
            return (await this.context.SaveChangesAsync()) > 0 ? newOrder : null;
        }

        public async Task<IEnumerable<Unit>> GetAllUnits()
        {
            return await context.Units.ToListAsync();
        }

        public async Task<bool> DeleteUnit(int id)
        {
            var unit = await this.context.Units.FirstOrDefaultAsync(u => u.Id == id);
            this.context.Remove(unit);
            return (await this.context.SaveChangesAsync() > 0);
        }

        public async Task<IEnumerable<Category>> GetAllCategories(string state)
        {
            if (!string.IsNullOrEmpty(state))
            {
                return (await this.context.Categories.Where(c => c.State == state).ToListAsync());
            }
            return (await this.context.Categories.ToListAsync());
        }

        public async Task<Picture> AddPicture(string path)
        {
            var picture = await this.context.Pictures.AddAsync(new Picture { Path = path });
            await this.context.SaveChangesAsync();
            return picture.Entity;
        }

        public async Task<string> GetFilePath(int id)
        {
            var picture = await this.context.Pictures.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (picture != null)
            {
                return picture.Path;
            }
            else
            {
                return "";
            }
        }

        public async Task<IEnumerable<Address>> GetAllAddress(string openId)
        {
            var addresses = this.context.Addresses;
            if (string.IsNullOrEmpty(openId))
            {
                return await addresses.ToListAsync();
            }
            else
            {
                return await addresses.Where(a => a.OpenId == openId).ToListAsync();
            }
        }

        public async Task<Address> AddAddress(Address address)
        {
            var addr = (await this.context.Addresses.AddAsync(address)).Entity;
            var succ = await this.context.SaveChangesAsync();
            if (succ > 0)
            {
                return addr;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateOrder(int id, JsonPatchDocument<Order> patchDoc)
        {
            var order = await this.context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            //var orderToPatch = new Order
            //{
            //    AddressId = order.AddressId,
            //    CancelReason = order.CancelReason,
            //    CancelTime = order.CancelTime,
            //    CreateTime = order.CreateTime,
            //    FinishTime = order.FinishTime,
            //    OpenId = order.OpenId,
            //    Products = order.Products,
            //    State = order.State
            //};

            patchDoc.ApplyTo(order);
            return (await this.context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> RemoveOrder(int id)
        {
            Order order = await this.context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }
            JsonPatchDocument<Order> orderPatch = new JsonPatchDocument<Order>();
            var oper = new Operation<Order>() { op = "replace", path = "/State", value = "4" };
            orderPatch.Operations.Add(oper);
            orderPatch.ApplyTo(order);
            return (await this.context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> RemoveProductPic(string picpath)
        {
            var pic = await this.context.Pictures.FirstOrDefaultAsync(p => p.Path.Contains(picpath));
            if (pic != null)
            {
                this.context.Pictures.Remove(pic);
                return (await this.context.SaveChangesAsync() > 0);
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> RemoveCategoryPic(int categoryid)
        {
            var category = await this.context.Categories.FindAsync(categoryid);
            if (category != null)
            {
                JsonPatchDocument<Category> jsonPatchDoc = new JsonPatchDocument<Category>();
                var oper = new Operation<Category> { op = "replace", path = "IconPath", value = "" };
                jsonPatchDoc.Operations.Add(oper);
                jsonPatchDoc.ApplyTo(category);
                return (await this.context.SaveChangesAsync() > 0);
            }
            else
            {
                return true;
            }
        }
        public async Task<string> RemoveCategory(int id)
        {
            var cate = await this.context.FindAsync<Category>(id);
            if (cate != null)
            {
                this.context.Categories.Remove(cate);
                if (await this.context.SaveChangesAsync() > 0)
                {
                    return cate.IconPath;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateProduct(Product newProduct)
        {
            this.context.Products.Update(newProduct);

            return (await this.context.SaveChangesAsync() > 0);
        }

        public async Task<bool> DeleteAddr(int id)
        {
            var addr = await this.context.Addresses.FindAsync(id);
            if (addr != null)
            {
                this.context.Addresses.Remove(addr);

                return (await this.context.SaveChangesAsync() > 0);
            }
            return true;
        }

        public async Task<bool> UpdateCate(Category cate)
        {
            this.context.Categories.Update(cate);
            return (await this.context.SaveChangesAsync() > 0);
        }

        public async Task<bool> UpdateUnit(Unit unit)
        {
            this.context.Units.Update(unit);
            await this.context.Products.Where(p => p.UnitId == unit.Id).ForEachAsync(pp =>
            {
                pp.UnitId = unit.Id;
                pp.UnitName = unit.Name;
                pp.Step = unit.Step;
            });

            return (await this.context.SaveChangesAsync() > 0);
        }

        public async Task<bool> UpdateProduct(int id, JsonPatchDocument<Product> patchDoc)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            //var orderToPatch = new Order
            //{
            //    AddressId = order.AddressId,
            //    CancelReason = order.CancelReason,
            //    CancelTime = order.CancelTime,
            //    CreateTime = order.CreateTime,
            //    FinishTime = order.FinishTime,
            //    OpenId = order.OpenId,
            //    Products = order.Products,
            //    State = order.State
            //};

            patchDoc.ApplyTo(product);
            return (await this.context.SaveChangesAsync()) > 0;
        }

        public async Task<User> AddUser(User user)
        {
            var u = await this.context.Users.AddAsync(user);
            return (await this.context.SaveChangesAsync() > 0) ? u.Entity : null;
        }

        public async Task<bool> CheckUserExists(string openid)
        {
            var user = await this.context.Users.Where(u => u.OpenId == openid).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<ItemsResult<User>> GetAllUsers(int? index, int? perPage, string keyword)
        {
            var result = new ItemsResult<User>();
            var users = this.context.Users.Where(u => !string.IsNullOrEmpty(u.OpenId));
            if (!string.IsNullOrEmpty(keyword))
            {
                users = users.Where(u => u.Name.ToLower().Contains(keyword.ToLower()) || u.OpenId.ToLower().Contains(keyword.ToLower()));
            }

            result.count = await users.CountAsync();
            if (index != null && perPage != null)
            {
                users = users.Skip((index.Value - 1) * perPage.Value).Take(perPage.Value);
            }

            result.items = await users.ToListAsync();
            return result;
        }

        public async Task<bool> CheckUserExists(string userName, string password)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
            return user != null;
        }

        public async Task<Favorite> AddFavorite(Favorite fav)
        {
            var favorite = await this.context.Favorites.AddAsync(fav);
            if (await this.context.SaveChangesAsync() > 0)
            {
                return favorite.Entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteFavorite(int id)
        {
            var fav = await this.context.Favorites.FindAsync(id);
            if (fav == null)
            {
                return true;
            }
            else
            {
                this.context.Favorites.Remove(fav);
                if ((await this.context.SaveChangesAsync()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<IEnumerable<FavoriteDTO>> getFavorites(string openid, int? productId)
        {
            var favorites = this.context.Favorites.Join(this.context.Products.Where(p => p.State == 1).Include(p => p.Pictures), i => i.ProductId, p => p.Id, (ii, pp) => new FavoriteDTO()
            {
                Id = pp.Id,
                Name = pp.Name,
                FavId = ii.Id,
                OpenId = ii.OpenId,
                Pictures = pp.Pictures,
                Step = pp.Step,
                Price = pp.Price,
                UnitId = pp.UnitId,
                UnitName = pp.UnitName
            }).Where(f => f.OpenId == openid);
            if (productId != null)
            {
                return await favorites.Where(f => f.Id == productId).ToListAsync();
            }
            else
            {
                return await favorites.ToListAsync();
            }
        }

        public async Task refundOrder(int id, RefundWrapper wrapper, Result<bool> result)
        {
            Order order = await context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.IsPaid == "1")
                {
                    if (!string.IsNullOrEmpty(order.WXOrderId))
                    {
                        WxPayData data = new WxPayData(log);
                        data.SetValue("out_trade_no", order.WXOrderId);
                        data.SetValue("total_fee", wrapper.TotalCost);
                        data.SetValue("refund_fee", wrapper.RefundCost);
                        data.SetValue("out_refund_no", WxPayApi.GenerateOutTradeNo(id));

                        WxPayData res = await WxPayApi.Refund(data, log, 15);
                        if (res != null)
                        {
                            if ("SUCCESS".Equals(res.GetValue("return_code")))
                            {
                                order.RefundNote = wrapper.RefundNote;
                                order.IsPaid = "2";
                                await context.SaveChangesAsync();
                                result.state = 1;
                                result.body = true;
                            }
                            else
                            {
                                result.state = 0;
                                result.message = res.GetValue("return_msg").ToString();
                            }
                        }
                        else
                        {
                            result.state = 0;
                            result.message = "微信支付接口请求失败";
                        }
                    }
                    else
                    {
                        result.state = 0;
                        result.message = "微信支付订单号不存在";
                    }
                }
                else
                {
                    result.state = 0;
                    result.message = "订单未使用微信支付";
                }

            }
            else
            {
                result.state = 0;
                result.message = "订单不存在";
            }

        }

        public async Task<bool> reorder(int id1, int id2)
        {
            var pro1 = await context.Products.FindAsync(id1);
            if (pro1 != null)
            {
                var pro2 = await context.Products.FindAsync(id2);
                if (pro2 != null)
                {
                    var seq1 = pro1.Sequence;
                    var seq2 = pro2.Sequence;
                    pro1.Sequence = seq2;
                    pro2.Sequence = seq1;
                    return (await context.SaveChangesAsync()) > 0;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> changePwd(string username, string oldpwd, string newpwd, Result<bool> result)
        {
            var encryptedOld = MD5Encrypter.GetMD5Hash(oldpwd);
            var user = await context.Users.Where(u => u.UserName == username && u.Password == encryptedOld).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Password = MD5Encrypter.GetMD5Hash(newpwd);
                return (await context.SaveChangesAsync()) > 0;
            }
            else
            {
                result.message = "旧密码错误";
                return false;
            }
        }

        public async Task<bool> patchCate(int id, JsonPatchDocument<Category> patchDoc)
        {
            var cate = await context.Categories.FindAsync(id);
            if (cate != null)
            {
                patchDoc.ApplyTo(cate);
                return (await context.SaveChangesAsync() > 0);
            }
            return false;
        }
    }
}
