using Grand.Business.Core.Interfaces.Catalog.Brands;
using Grand.Business.Core.Interfaces.Catalog.Categories;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Storage;
using Grand.Domain.Catalog;
using Grand.Infrastructure;
using Grand.Module.MobileApi.DTOs;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Grand.Business.Core.Extensions;
using Grand.SharedKernel.Extensions;

namespace Grand.Module.MobileApi.Controllers;

/// <summary>
/// Mobile API Catalog Controller
/// Handles product browsing, search, and catalog navigation
/// </summary>
[ApiExplorerSettings(GroupName = "v3")]
public class CatalogController : BaseMobileApiController
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly IPictureService _pictureService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ITranslationService _translationService;
    private readonly MobileApiConfig _config;

    public CatalogController(
        ICategoryService categoryService,
        IProductService productService,
        IBrandService brandService,
        IPictureService pictureService,
        IContextAccessor contextAccessor,
        ITranslationService translationService,
        MobileApiConfig config)
    {
        _categoryService = categoryService;
        _productService = productService;
        _brandService = brandService;
        _pictureService = pictureService;
        _contextAccessor = contextAccessor;
        _translationService = translationService;
        _config = config;
    }

    /// <summary>
    /// Get all product categories
    /// </summary>
    /// <returns>List of categories with subcategories</returns>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var storeId = _contextAccessor.StoreContext.CurrentStore.Id;
            var categories = await _categoryService.GetAllCategories(storeId: storeId, showHidden: false);
            
            var mobileCategoriesTask = categories
                .Where(c => string.IsNullOrEmpty(c.ParentCategoryId))
                .Select(async category => await MapToMobileCategoryDto(category, categories.ToList()));

            var mobileCategories = await Task.WhenAll(mobileCategoriesTask);

            return Success(mobileCategories.ToList(), "Categories retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve categories: {ex.Message}");
        }
    }

    /// <summary>
    /// Get products in a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="search">Search and filter parameters</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet("categories/{categoryId}/products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryProducts(string categoryId, [FromQuery] MobileProductSearchDto search)
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var category = await _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return NotFoundError("Category not found");

            search.CategoryId = categoryId;
            var productList = await SearchProductsInternal(search);

            return Success(productList, $"Products in category {category.Name} retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve category products: {ex.Message}");
        }
    }

    /// <summary>
    /// Search products
    /// </summary>
    /// <param name="search">Search parameters</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchProducts([FromQuery] MobileProductSearchDto search)
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var productList = await SearchProductsInternal(search);
            return Success(productList, "Product search completed successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Product search failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Get featured products
    /// </summary>
    /// <param name="count">Number of featured products to return</param>
    /// <returns>List of featured products</returns>
    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 10)
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var storeId = _contextAccessor.StoreContext.CurrentStore.Id;
            var productIds = await _productService.GetAllProductsDisplayedOnHomePage();
            
            var featuredProductTasks = productIds
                .Take(count)
                .Select(async productId => 
                {
                    var product = await _productService.GetProductById(productId);
                    return product != null ? await MapToMobileProductSummaryDto(product) : null;
                });

            var featuredProducts = (await Task.WhenAll(featuredProductTasks))
                .Where(p => p != null)
                .ToList();

            return Success(featuredProducts.ToList(), "Featured products retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve featured products: {ex.Message}");
        }
    }

    /// <summary>
    /// Get product details
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>Detailed product information</returns>
    [HttpGet("products/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductDetails(string productId)
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return NotFoundError("Product not found");

            if (!product.Published || !product.IsAvailable())
                return NotFoundError("Product not available");

            var productDetail = await MapToMobileProductDetailDto(product);

            return Success(productDetail, "Product details retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve product details: {ex.Message}");
        }
    }

    /// <summary>
    /// Get products by brand
    /// </summary>
    /// <param name="brandId">Brand ID</param>
    /// <param name="search">Search and filter parameters</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet("brands/{brandId}/products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrandProducts(string brandId, [FromQuery] MobileProductSearchDto search)
    {
        if (!_config.Enabled)
            return Error(Constants.MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var brand = await _brandService.GetBrandById(brandId);
            if (brand == null)
                return NotFoundError("Brand not found");

            search.BrandId = brandId;
            var productList = await SearchProductsInternal(search);

            return Success(productList, $"Products by {brand.Name} retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve brand products: {ex.Message}");
        }
    }

    #region Private Methods

    private async Task<MobileProductListDto> SearchProductsInternal(MobileProductSearchDto search)
    {
        var storeId = _contextAccessor.StoreContext.CurrentStore.Id;
        var language = _contextAccessor.WorkContext.WorkingLanguage;
        
        // Build search parameters
        var searchTerms = search.Query;
        var categoryIds = !string.IsNullOrEmpty(search.CategoryId) ? new List<string> { search.CategoryId } : new List<string>();
        var brandId = search.BrandId ?? "";

        // Get products based on search criteria
        var searchResult = await _productService.SearchProducts(
            storeId: storeId,
            categoryIds: categoryIds,
            brandId: brandId,
            keywords: searchTerms,
            searchDescriptions: !string.IsNullOrEmpty(searchTerms),
            languageId: language.Id,
            priceMin: search.MinPrice.HasValue ? (double)search.MinPrice : null,
            priceMax: search.MaxPrice.HasValue ? (double)search.MaxPrice : null,
            pageIndex: search.Page - 1,
            pageSize: search.PageSize,
            showHidden: false
        );

        var products = searchResult.products;

        // Apply additional filters and map to mobile DTOs
        IEnumerable<Product> filteredProducts = products;
        if (search.InStockOnly)
        {
            filteredProducts = products.Where(p => p.StockQuantity > 0);
        }

        // Map to mobile DTOs
        var productSummaryTasks = filteredProducts
            .Select(async product => await MapToMobileProductSummaryDto(product));

        var productSummaries = await Task.WhenAll(productSummaryTasks);

        // Calculate pagination
        var totalCount = products.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / search.PageSize);

        return new MobileProductListDto
        {
            Products = productSummaries.ToList(),
            Pagination = new MobilePaginationDto
            {
                CurrentPage = search.Page,
                TotalPages = totalPages,
                TotalItems = totalCount,
                PageSize = search.PageSize,
                HasNext = search.Page < totalPages,
                HasPrevious = search.Page > 1
            },
            Filters = search
        };
    }

    private async Task<MobileCategoryDto> MapToMobileCategoryDto(Category category, List<Category> allCategories)
    {
        var imageUrl = "";
        if (!string.IsNullOrEmpty(category.PictureId))
        {
            var picture = await _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
            {
                imageUrl = await _pictureService.GetPictureUrl(picture);
            }
        }

        var subcategories = allCategories
            .Where(c => c.ParentCategoryId == category.Id)
            .Select(async subcat => await MapToMobileCategoryDto(subcat, allCategories));

        return new MobileCategoryDto
        {
            Id = category.Id,
            Name = category.GetTranslation(x => x.Name, _contextAccessor.WorkContext.WorkingLanguage.Id, false),
            Description = category.GetTranslation(x => x.Description, _contextAccessor.WorkContext.WorkingLanguage.Id, false),
            ImageUrl = imageUrl,
            ParentCategoryId = category.ParentCategoryId,
            SubCategories = (await Task.WhenAll(subcategories)).ToList()
        };
    }

    private async Task<MobileProductSummaryDto> MapToMobileProductSummaryDto(Product product)
    {
        var currency = _contextAccessor.WorkContext.WorkingCurrency;
        var language = _contextAccessor.WorkContext.WorkingLanguage;

        // Get primary image
        var imageUrl = "";
        var primaryImage = product.ProductPictures.FirstOrDefault();
        if (primaryImage != null)
        {
            var picture = await _pictureService.GetPictureById(primaryImage.PictureId);
            if (picture != null)
            {
                imageUrl = await _pictureService.GetPictureUrl(picture);
            }
        }

        // Get brand name
        var brandName = "";
        if (!string.IsNullOrEmpty(product.BrandId))
        {
            var brand = await _brandService.GetBrandById(product.BrandId);
            brandName = brand?.GetTranslation(x => x.Name, language.Id, false) ?? "";
        }

        return new MobileProductSummaryDto
        {
            Id = product.Id,
            Name = product.GetTranslation(x => x.Name, language.Id, false),
            ShortDescription = product.GetTranslation(x => x.ShortDescription, language.Id, false),
            ImageUrl = imageUrl,
            Price = (decimal)product.Price,
            OldPrice = product.OldPrice > product.Price ? (decimal?)product.OldPrice : null,
            Currency = currency.CurrencyCode,
            PriceFormatted = product.Price.ToString("C"),
            InStock = product.StockQuantity > 0,
            OnSale = product.OldPrice > product.Price,
            Brand = brandName
        };
    }

    private async Task<MobileProductDetailDto> MapToMobileProductDetailDto(Product product)
    {
        var summary = await MapToMobileProductSummaryDto(product);
        var language = _contextAccessor.WorkContext.WorkingLanguage;

        // Get all product images
        var imageTasks = product.ProductPictures
            .Select(async pp =>
            {
                var picture = await _pictureService.GetPictureById(pp.PictureId);
                if (picture == null) return null;
                
                var imageUrl = await _pictureService.GetPictureUrl(picture);
                var thumbnailUrl = await _pictureService.GetPictureUrl(picture, 150);

                return new MobileProductImageDto
                {
                    Url = imageUrl,
                    ThumbnailUrl = thumbnailUrl,
                    AltText = picture.AltAttribute ?? summary.Name,
                    IsPrimary = pp.DisplayOrder == 0
                };
            });

        var images = (await Task.WhenAll(imageTasks))
            .Where(img => img != null)
            .Cast<MobileProductImageDto>()
            .ToList();

        return new MobileProductDetailDto
        {
            Id = summary.Id,
            Name = summary.Name,
            ShortDescription = summary.ShortDescription,
            FullDescription = product.GetTranslation(x => x.FullDescription, language.Id, false),
            ImageUrl = summary.ImageUrl,
            Price = summary.Price,
            OldPrice = summary.OldPrice,
            Currency = summary.Currency,
            PriceFormatted = summary.PriceFormatted,
            Rating = summary.Rating,
            ReviewCount = summary.ReviewCount,
            InStock = summary.InStock,
            OnSale = summary.OnSale,
            Brand = summary.Brand,
            Images = images,
            StockQuantity = product.StockQuantity,
            MinimumQuantity = product.OrderMinimumQuantity,
            MaximumQuantity = product.OrderMaximumQuantity,
            Sku = product.Sku,
            Attributes = new List<MobileProductAttributeDto>(), // TODO: Implement attributes mapping
            Specifications = new List<MobileProductSpecificationDto>(), // TODO: Implement specifications mapping
            RelatedProducts = new List<MobileProductSummaryDto>() // TODO: Implement related products
        };
    }

    #endregion
}
