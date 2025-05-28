using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class CarritoCompraRepository : ICarritoCompraRepository
    {
        private readonly IDatabase _database;

        //private readonly IConnectionMultiplexer _redis;
        public CarritoCompraRepository(IConnectionMultiplexer redis)
        {
            
            _database = redis.GetDatabase();
        }
        public Task<bool> AddItemToCarritoAsync(string carritoId, CarritoItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ClearCarritoCompraAsync(string carritoId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteCarritoCompraAsync(string carritoId)
        {
           return await  _database.KeyDeleteAsync(carritoId);
        }

        public async Task<CarritoCompra> GetCarritoCompraAsync(string carritoId)
        {
            var data = await _database.StringGetAsync(carritoId);

            return data.IsNullOrEmpty ? null: JsonSerializer.Deserialize<CarritoCompra>(data);

        }

        public Task<bool> RemoveItemFromCarritoAsync(string carritoId, int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task<CarritoCompra> UpdateCarritoCompraAsync(CarritoCompra carritoCompra)
        {
           var status = await _database.StringSetAsync(carritoCompra.Id, JsonSerializer.Serialize(carritoCompra),TimeSpan.FromDays(30));
            
            if (!status) return null;
            return await GetCarritoCompraAsync(carritoCompra.Id);
        }
    }
}
