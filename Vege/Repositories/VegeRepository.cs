using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> AddCategory(Category category)
        {
            this.context.Categories.Add(category);

            return (await this.context.SaveChangesAsync()) > 0;
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

        public List<Product> GetAllProduct(int? id, int? catetoryId)
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

            var result = (from p in products
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

            return result.ToList();
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllProductInCart(string openid)
        {
            var cart = await this.context.ShoppingCarts.FirstOrDefaultAsync();
            if (cart != null)
            {
                var cartItems = from cp in cart.Products
                                join p in this.context.Products on cp.ProductId equals p.Id
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
            var cart = await this.context.ShoppingCarts.FirstOrDefaultAsync();
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
                cart.Products.Add(cartItem);
                return (await this.context.SaveChangesAsync()) > 0;
            }
        }

        public IEnumerable<Order> GetAllOrders(string openId)
        {
            var orders = this.context.Orders.Include(order => order.Products)
            .ThenInclude(item => item.Join(this.context.Products, i => i.ProductId, p => p.Id, (ii, pp) => new OrderItemDTO
            {
                CategoryId = pp.CategoryId,
                Count = ii.Count,
                Description = pp.Description,
                Id = pp.Id,
                Name = pp.Name,
                Pictures = pp.Pictures,
                Price = ii.Price,
                UnitName = pp.UnitName
            }));
            return orders;
        }

        public async Task<bool> AddOrder(Order order)
        {
            this.context.Orders.Add(order);
            return (await this.context.SaveChangesAsync()) > 0;
        }
    }
}
