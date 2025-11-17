namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Mobile-optimized category data
/// </summary>
public class MobileCategoryDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Category image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Number of products in category
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Parent category ID
    /// </summary>
    public string? ParentCategoryId { get; set; }

    /// <summary>
    /// Subcategories
    /// </summary>
    public List<MobileCategoryDto> SubCategories { get; set; } = new();
}

/// <summary>
/// Mobile-optimized product summary for listings
/// </summary>
public class MobileProductSummaryDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Primary product image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Original price (for sales/discounts)
    /// </summary>
    public decimal? OldPrice { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Formatted price string
    /// </summary>
    public string PriceFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Average rating
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Whether product is in stock
    /// </summary>
    public bool InStock { get; set; }

    /// <summary>
    /// Whether product is on sale
    /// </summary>
    public bool OnSale { get; set; }

    /// <summary>
    /// Product brand name
    /// </summary>
    public string Brand { get; set; } = string.Empty;
}

/// <summary>
/// Mobile-optimized detailed product information
/// </summary>
public class MobileProductDetailDto : MobileProductSummaryDto
{
    /// <summary>
    /// Full product description
    /// </summary>
    public string FullDescription { get; set; } = string.Empty;

    /// <summary>
    /// Product images
    /// </summary>
    public List<MobileProductImageDto> Images { get; set; } = new();

    /// <summary>
    /// Product attributes/options
    /// </summary>
    public List<MobileProductAttributeDto> Attributes { get; set; } = new();

    /// <summary>
    /// Product specifications
    /// </summary>
    public List<MobileProductSpecificationDto> Specifications { get; set; } = new();

    /// <summary>
    /// Related products
    /// </summary>
    public List<MobileProductSummaryDto> RelatedProducts { get; set; } = new();

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Minimum quantity to order
    /// </summary>
    public int MinimumQuantity { get; set; } = 1;

    /// <summary>
    /// Maximum quantity to order
    /// </summary>
    public int MaximumQuantity { get; set; } = 999;

    /// <summary>
    /// Product SKU
    /// </summary>
    public string Sku { get; set; } = string.Empty;
}

/// <summary>
/// Product image for mobile
/// </summary>
public class MobileProductImageDto
{
    /// <summary>
    /// Image URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail URL
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// Alt text for accessibility
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is the primary image
    /// </summary>
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Product attribute for mobile
/// </summary>
public class MobileProductAttributeDto
{
    /// <summary>
    /// Attribute ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Attribute name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Attribute type (color, size, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Available values
    /// </summary>
    public List<MobileAttributeValueDto> Values { get; set; } = new();

    /// <summary>
    /// Whether this attribute is required
    /// </summary>
    public bool Required { get; set; }
}

/// <summary>
/// Attribute value for mobile
/// </summary>
public class MobileAttributeValueDto
{
    /// <summary>
    /// Value ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Value name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Color hex code (for color attributes)
    /// </summary>
    public string? ColorHex { get; set; }

    /// <summary>
    /// Price adjustment for this option
    /// </summary>
    public decimal PriceAdjustment { get; set; }

    /// <summary>
    /// Whether this option is in stock
    /// </summary>
    public bool InStock { get; set; } = true;
}

/// <summary>
/// Product specification for mobile
/// </summary>
public class MobileProductSpecificationDto
{
    /// <summary>
    /// Specification name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Specification value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Specification group/category
    /// </summary>
    public string Group { get; set; } = string.Empty;
}

/// <summary>
/// Mobile product search/filter request
/// </summary>
public class MobileProductSearchDto
{
    /// <summary>
    /// Search query
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Category ID to filter by
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// Brand ID to filter by
    /// </summary>
    public string? BrandId { get; set; }

    /// <summary>
    /// Minimum price
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Sort order (price-asc, price-desc, name, rating, etc.)
    /// </summary>
    public string SortBy { get; set; } = "relevance";

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Only show products in stock
    /// </summary>
    public bool InStockOnly { get; set; } = false;

    /// <summary>
    /// Only show products on sale
    /// </summary>
    public bool OnSaleOnly { get; set; } = false;
}

/// <summary>
/// Mobile paginated product list response
/// </summary>
public class MobileProductListDto
{
    /// <summary>
    /// Products for current page
    /// </summary>
    public List<MobileProductSummaryDto> Products { get; set; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public MobilePaginationDto Pagination { get; set; } = new();

    /// <summary>
    /// Applied filters
    /// </summary>
    public MobileProductSearchDto Filters { get; set; } = new();
}

/// <summary>
/// Mobile pagination information
/// </summary>
public class MobilePaginationDto
{
    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious { get; set; }
}