﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using TP.SF.Common.Model;

namespace TP.SF.Common.Contracts.Services
{
    public interface ICatalogService : IService
    {
        Task CreateOrUpdateCategoryAsync(Category category);

        Task<List<Category>> GetCategoriesAsync();

        Task<Category> GetCategoryAsync(Guid categoryId);

        Task DeleteCategoryAsync(Guid categoryId);

        Task<long> GetCategoriesCountAsync();

        Task CreateOrUpdateProductAsync(Product product);

        Task<Product> GetProductAsync(Guid productId);

        Task DeleteProductAsync(Guid productId);

        Task<long> GetProductsCountAsync();

        Task<List<Guid>> GetProductsInCategoryAsync(Guid categoryId);

        Task AddProductToCategoryAsync(Guid categoryId, Guid productId);

        Task RemoveProductFromCategoryAsync(Guid categoryId, Guid productId);
    }
}
