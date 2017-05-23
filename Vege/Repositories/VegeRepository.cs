﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.DTO;
using Vege.Models;

namespace Vege.Repositories
{
    public class VegeRepository : IVegeRepository
    {
        private VegeContext context;
        public VegeRepository(VegeContext context)
        {
            this.context = context;
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
            var newProduct = this.context.Products.Add(product).Entity;
            if (await this.context.SaveChangesAsync() > 0)
            {
                return newProduct;
            }
            return null;
        }

        public async Task<bool> AddUnit(Unit unit)
        {
            this.context.Units.Add(unit);

            return (await this.context.SaveChangesAsync()) > 0;
        }

        public ItemsResult<Product> GetAllProduct(int? id, int? catetoryId, int? index, int? perPage)
        {
            var products = this.context.Products.Include(p => p.Pictures).Select(p => p)
            .Where(p => p.State == 1);

            if (id == null && catetoryId != null)
            {
                products = products.Where(p => p.CategoryId == catetoryId);
            }
            if (id != null)
            {
                products = products.Where(p => p.Id == id);
            }

            var items = (from p in products
                         join u in this.context.Units on p.UnitId equals u.Id into pus
                         from pu in pus.DefaultIfEmpty()
                         select new Product()
                         {
                             Id = p.Id,
                             Name = p.Name,
                             CategoryId = p.CategoryId,
                             Description = p.Description,
                             Pictures = p.Pictures,
                             Price = p.Price,
                             TotalCount = p.TotalCount,
                             UnitName = pu.Name
                         });

            var result = new ItemsResult<Product>();
            result.Count = items.Count();
            if (index != null)
            {
                result.Items = items.Skip((index.Value - 1) * perPage.Value).Take(perPage.Value);
            }
            else
            {
                result.Items = items;
            }

            return result;
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid)
        {
            var cart = await this.context.ShoppingCarts.Include(c => c.Products).FirstOrDefaultAsync();
            if (cart != null)
            {
                var cartItems = from cp in cart.Products
                                join p in this.context.Products.Include(p => p.Pictures) on cp.ProductId equals p.Id
                                join u in this.context.Units on p.UnitId equals u.Id
                                select new CartItemDTO
                                {
                                    Id = p.Id,
                                    Count = cp.Count,
                                    Description = p.Description,
                                    Name = p.Name,
                                    Pictures = p.Pictures,
                                    Price = p.Price,
                                    UnitName = u.Name
                                };
                return cartItems;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddCartItem(string openId, CartItem cartItem)
        {
            var cart = await this.context.ShoppingCarts.Include(c => c.Products).FirstOrDefaultAsync();
            if (cart == null)
            {
                ShoppingCart newCart = new ShoppingCart()
                {
                    OpenId = openId,
                    Products = new List<CartItem>()
                };
                newCart.Products.Add(cartItem);
                await this.context.ShoppingCarts.AddAsync(newCart);
                return (await this.context.SaveChangesAsync()) > 0;
            }
            else
            {
                if (cart.Products.Any(p => p.ProductId == cartItem.ProductId))
                {
                    cart.Products.Where(p => p.ProductId == cartItem.ProductId).FirstOrDefault().Count += cartItem.Count;
                }
                else
                {
                    cart.Products.Add(cartItem);
                }
                return (await this.context.SaveChangesAsync()) > 0;
            }
        }

        public ItemsResult<OrderDTO> GetAllOrders(string openId, int? index, int? perPage, string keyword, DateTime? begin, DateTime? end, bool? noshowRemove)
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
            if (!string.IsNullOrEmpty(keyword))
            {
                orders = orders.Where(o => (o.Name == keyword || o.Phone == keyword));
            }
            if (begin != null)
            {
                orders = orders.Where(o => o.CreateTime.CompareTo(begin.Value) >= 0);
            }
            if (end != null)
            {
                orders = orders.Where(o => o.CreateTime.CompareTo(end.Value) <= 0);
            }
            if (noshowRemove != null)
            {
                if (noshowRemove.Value)
                {
                    orders = orders.Where(o => o.State != 4);
                }
            }
            orders.OrderBy(o => o.State).ThenByDescending(o => o.Id);
            ItemsResult<OrderDTO> result = new ItemsResult<OrderDTO>();
            result.Count = orders.Count();
            if (index != null)
            {
                result.Items = orders.Skip((index.Value - 1) * perPage.Value).Take(perPage.Value);
            }
            else
            {
                result.Items = orders;
            }

            return result;
        }

        public async Task<bool> AddOrder(Order order)
        {
            this.context.Orders.Add(order);
            this.context.CartItems.RemoveRange(this.context.CartItems);
            return (await this.context.SaveChangesAsync()) > 0;
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

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
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
                return await addresses.Where(a => a.UserId == 1).ToListAsync();
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

        public async Task<bool> UpdateOrder(Order order)
        {
            this.context.Orders.Update(this.context.Orders.Attach(order).Entity);
            return (await this.context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> RemoveOrder(int id)
        {
            Order order = await this.context.Orders.FindAsync(id);
            if (order != null)
            {
                order.State = 4;
            }
            this.context.Orders.Update(this.context.Orders.Attach(order).Entity);
            return (await this.context.SaveChangesAsync()) > 0;
        }
    }
}
