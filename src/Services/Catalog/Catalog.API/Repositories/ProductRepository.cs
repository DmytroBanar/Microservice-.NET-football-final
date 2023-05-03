﻿using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context
                        .Products
                        .Find(_ => true)
                        .ToListAsync();
    }

    public async Task<Product> GetProductAsync(string id)
    {
        return await _context
                        .Products
                        .Find(p => p.Id == id)
                        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);
        return await _context
                        .Products
                        .Find(filter)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
        return await _context
                        .Products
                        .Find(filter)
                        .ToListAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var updateResult = await _context
                                    .Products
                                    .ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
        return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        DeleteResult deleteResult = await _context
                                                .Products
                                                .DeleteOneAsync(filter);
        return deleteResult.IsAcknowledged
            && deleteResult.DeletedCount > 0;
    }
}
