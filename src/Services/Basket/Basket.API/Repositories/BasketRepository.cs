using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisChache;

        public BasketRepository(IDistributedCache redisChache)
        {
            _redisChache = redisChache ?? throw new ArgumentNullException(nameof(redisChache));
        }

        public async Task DeleteBasket(string username)
        {
            await _redisChache.RemoveAsync(username);
        }

        public async Task<ShoppingCart> GetBasket(string username)
        {
            var basket =await _redisChache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisChache.SetStringAsync(basket.Username, JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.Username);
        }
    }
}
