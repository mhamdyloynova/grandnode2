namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Mobile customer profile data
/// </summary>
public class MobileCustomerProfileDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Customer since date
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Last activity date
    /// </summary>
    public DateTime LastActivityDate { get; set; }

    /// <summary>
    /// Whether email is confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Newsletter subscription status
    /// </summary>
    public bool NewsletterSubscribed { get; set; }
}

/// <summary>
/// Update customer profile request
/// </summary>
public class MobileUpdateProfileDto
{
    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Newsletter subscription
    /// </summary>
    public bool NewsletterSubscribed { get; set; }
}

/// <summary>
/// Change password request
/// </summary>
public class MobileChangePasswordDto
{
    /// <summary>
    /// Current password
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Customer address for mobile
/// </summary>
public class MobileCustomerAddressDto : MobileAddressDto
{
    /// <summary>
    /// Whether this is the default billing address
    /// </summary>
    public bool IsDefaultBilling { get; set; }

    /// <summary>
    /// Whether this is the default shipping address
    /// </summary>
    public bool IsDefaultShipping { get; set; }

    /// <summary>
    /// Address creation date
    /// </summary>
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Mobile order summary for customer
/// </summary>
public class MobileCustomerOrderDto
{
    /// <summary>
    /// Order ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Order total
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Formatted order total
    /// </summary>
    public string OrderTotalFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Order date
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public string OrderStatus { get; set; } = string.Empty;

    /// <summary>
    /// Payment status
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Shipping status
    /// </summary>
    public string ShippingStatus { get; set; } = string.Empty;

    /// <summary>
    /// Number of items in order
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Whether order can be cancelled
    /// </summary>
    public bool CanCancel { get; set; }

    /// <summary>
    /// Whether order can be returned
    /// </summary>
    public bool CanReturn { get; set; }

    /// <summary>
    /// Tracking number
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Estimated delivery date
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; set; }
}

/// <summary>
/// Detailed order information
/// </summary>
public class MobileOrderDetailDto : MobileCustomerOrderDto
{
    /// <summary>
    /// Order items
    /// </summary>
    public List<MobileOrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Billing address
    /// </summary>
    public MobileAddressDto BillingAddress { get; set; } = new();

    /// <summary>
    /// Shipping address
    /// </summary>
    public MobileAddressDto? ShippingAddress { get; set; }

    /// <summary>
    /// Shipping method
    /// </summary>
    public string ShippingMethod { get; set; } = string.Empty;

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Order totals breakdown
    /// </summary>
    public MobileCartTotalsDto Totals { get; set; } = new();

    /// <summary>
    /// Order notes
    /// </summary>
    public string? OrderNotes { get; set; }

    /// <summary>
    /// Order history/status changes
    /// </summary>
    public List<MobileOrderHistoryDto> History { get; set; } = new();
}

/// <summary>
/// Order item details
/// </summary>
public class MobileOrderItemDto
{
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
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Total price for this item
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Formatted unit price
    /// </summary>
    public string UnitPriceFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Formatted total price
    /// </summary>
    public string TotalPriceFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Selected attributes
    /// </summary>
    public List<MobileCartItemAttributeDto> Attributes { get; set; } = new();
}

/// <summary>
/// Order history entry
/// </summary>
public class MobileOrderHistoryDto
{
    /// <summary>
    /// Status/event description
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Event date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
}