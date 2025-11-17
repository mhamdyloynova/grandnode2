namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Mobile shopping cart data
/// </summary>
public class MobileCartDto
{
    /// <summary>
    /// Cart items
    /// </summary>
    public List<MobileCartItemDto> Items { get; set; } = new();

    /// <summary>
    /// Cart totals
    /// </summary>
    public MobileCartTotalsDto Totals { get; set; } = new();

    /// <summary>
    /// Total number of items in cart
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Whether cart is empty
    /// </summary>
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Applied discount codes
    /// </summary>
    public List<string> AppliedCoupons { get; set; } = new();
}

/// <summary>
/// Mobile cart item
/// </summary>
public class MobileCartItemDto
{
    /// <summary>
    /// Cart item ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Product ID
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Product image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Total price for this item (UnitPrice * Quantity)
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Formatted unit price
    /// </summary>
    public string UnitPriceFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted total price
    /// </summary>
    public string TotalPriceFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Selected product attributes
    /// </summary>
    public List<MobileCartItemAttributeDto> Attributes { get; set; } = new();

    /// <summary>
    /// Maximum quantity available
    /// </summary>
    public int MaxQuantity { get; set; } = 999;

    /// <summary>
    /// Whether item is in stock
    /// </summary>
    public bool InStock { get; set; } = true;

    /// <summary>
    /// Any warnings for this item
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Cart item attribute
/// </summary>
public class MobileCartItemAttributeDto
{
    /// <summary>
    /// Attribute name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Selected value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Price adjustment for this attribute
    /// </summary>
    public decimal PriceAdjustment { get; set; }
}

/// <summary>
/// Mobile cart totals
/// </summary>
public class MobileCartTotalsDto
{
    /// <summary>
    /// Subtotal (before discounts and shipping)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Total discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Shipping cost
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Grand total
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Formatted subtotal
    /// </summary>
    public string SubtotalFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted discount amount
    /// </summary>
    public string DiscountFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted shipping cost
    /// </summary>
    public string ShippingFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted tax amount
    /// </summary>
    public string TaxFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted grand total
    /// </summary>
    public string TotalFormatted { get; set; } = string.Empty;
}

/// <summary>
/// Add item to cart request
/// </summary>
public class MobileAddToCartDto
{
    /// <summary>
    /// Product ID to add
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Quantity to add
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Selected product attributes
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();

    /// <summary>
    /// Custom price (for products that allow customer-entered price)
    /// </summary>
    public decimal? CustomPrice { get; set; }
}

/// <summary>
/// Update cart item request
/// </summary>
public class MobileUpdateCartItemDto
{
    /// <summary>
    /// New quantity (0 to remove item)
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Apply coupon request
/// </summary>
public class MobileApplyCouponDto
{
    /// <summary>
    /// Coupon code to apply
    /// </summary>
    public string CouponCode { get; set; } = string.Empty;
}