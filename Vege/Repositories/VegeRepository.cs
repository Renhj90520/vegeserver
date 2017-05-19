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

        public ItemsResult<Order> GetAllOrders(string openId,int? index,int? perPage)
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

            ItemsResult<Order> result=new ItemsResult<Order>();
            result.Count=orders.Count();
            if(index!=null){
                result.Items=orders.Skip((index.Value-1)*perPage.Value).Take(perPage.Value);
            }
            else{
                result.Items=orders;
            }
            return result;
        }

        public async Task<bool> AddOrder(Order order)
        {
            this.context.Orders.Add(order);
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

        public async Task<IEnumerable<Category>> GetAllCategories(){
            return (await this.context.Categories.ToListAsync());
        }

        public async Task<Picture> AddPicture(string name,string path){
            var picture=await this.context.Pictures.AddAsync(new Picture{Name=name,Path=path});
            await this.context.SaveChangesAsync();
            return picture;
        }

        public async Task<string> GetFilePath(int id){
            var picture=await this.context.Pictures.Where(p=>p.id==id).FirstOrDefaultAsync();
            if(picture!=null){
                return picture.Path;
            }else{
                return "";
            }
        }
    }
}
