namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Mobile checkout session data
/// </summary>
public class MobileCheckoutDto
{
    /// <summary>
    /// Cart items to checkout
    /// </summary>
    public List<MobileCartItemDto> CartItems { get; set; } = new();

    /// <summary>
    /// Billing address
    /// </summary>
    public MobileAddressDto? BillingAddress { get; set; }

    /// <summary>
    /// Shipping address (null if same as billing)
    /// </summary>
    public MobileAddressDto? ShippingAddress { get; set; }

    /// <summary>
    /// Whether shipping address is same as billing
    /// </summary>
    public bool ShippingAddressSameAsBilling { get; set; } = true;

    /// <summary>
    /// Available shipping methods
    /// </summary>
    public List<MobileShippingMethodDto> AvailableShippingMethods { get; set; } = new();

    /// <summary>
    /// Selected shipping method
    /// </summary>
    public MobileShippingMethodDto? SelectedShippingMethod { get; set; }

    /// <summary>
    /// Available payment methods
    /// </summary>
    public List<MobilePaymentMethodDto> AvailablePaymentMethods { get; set; } = new();

    /// <summary>
    /// Selected payment method
    /// </summary>
    public MobilePaymentMethodDto? SelectedPaymentMethod { get; set; }

    /// <summary>
    /// Order totals
    /// </summary>
    public MobileCartTotalsDto Totals { get; set; } = new();

    /// <summary>
    /// Customer information (for guest checkout)
    /// </summary>
    public MobileCheckoutCustomerDto? CustomerInfo { get; set; }

    /// <summary>
    /// Whether this is a guest checkout
    /// </summary>
    public bool IsGuestCheckout { get; set; }

    /// <summary>
    /// Order notes/comments
    /// </summary>
    public string? OrderNotes { get; set; }
}

/// <summary>
/// Mobile address data
/// </summary>
public class MobileAddressDto
{
    /// <summary>
    /// Address ID (null for new address)
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Company name (optional)
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Address line 1
    /// </summary>
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// Address line 2 (optional)
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State/Province
    /// </summary>
    public string StateProvince { get; set; } = string.Empty;

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    public string ZipPostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country ID
    /// </summary>
    public string CountryId { get; set; } = string.Empty;

    /// <summary>
    /// Country name
    /// </summary>
    public string CountryName { get; set; } = string.Empty;
}

/// <summary>
/// Mobile shipping method
/// </summary>
public class MobileShippingMethodDto
{
    /// <summary>
    /// Shipping method ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Shipping method name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Shipping cost
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Formatted cost
    /// </summary>
    public string CostFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Delivery time estimate
    /// </summary>
    public string? DeliveryTime { get; set; }

    /// <summary>
    /// Whether this method is selected
    /// </summary>
    public bool IsSelected { get; set; }
}

/// <summary>
/// Mobile payment method
/// </summary>
public class MobilePaymentMethodDto
{
    /// <summary>
    /// Payment method ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Payment method name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Payment fee
    /// </summary>
    public decimal Fee { get; set; }

    /// <summary>
    /// Formatted fee
    /// </summary>
    public string FeeFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Logo URL
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Whether this method is selected
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Whether this method requires additional info
    /// </summary>
    public bool RequiresAdditionalInfo { get; set; }
}

/// <summary>
/// Customer info for guest checkout
/// </summary>
public class MobileCheckoutCustomerDto
{
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
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Set billing address request
/// </summary>
public class MobileSetBillingAddressDto
{
    /// <summary>
    /// Address data
    /// </summary>
    public MobileAddressDto Address { get; set; } = new();

    /// <summary>
    /// Whether to save this address to customer's address book
    /// </summary>
    public bool SaveToAddressBook { get; set; } = false;
}

/// <summary>
/// Set shipping address request
/// </summary>
public class MobileSetShippingAddressDto
{
    /// <summary>
    /// Address data (null if same as billing)
    /// </summary>
    public MobileAddressDto? Address { get; set; }

    /// <summary>
    /// Whether shipping address is same as billing
    /// </summary>
    public bool SameAsBilling { get; set; } = true;

    /// <summary>
    /// Whether to save this address to customer's address book
    /// </summary>
    public bool SaveToAddressBook { get; set; } = false;
}

/// <summary>
/// Select shipping method request
/// </summary>
public class MobileSelectShippingMethodDto
{
    /// <summary>
    /// Shipping method ID
    /// </summary>
    public string ShippingMethodId { get; set; } = string.Empty;
}

/// <summary>
/// Select payment method request
/// </summary>
public class MobileSelectPaymentMethodDto
{
    /// <summary>
    /// Payment method ID
    /// </summary>
    public string PaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// Additional payment info (card details, etc.)
    /// </summary>
    public Dictionary<string, string> AdditionalInfo { get; set; } = new();
}

/// <summary>
/// Place order request
/// </summary>
public class MobilePlaceOrderDto
{
    /// <summary>
    /// Order notes/comments
    /// </summary>
    public string? OrderNotes { get; set; }

    /// <summary>
    /// Terms and conditions acceptance
    /// </summary>
    public bool AcceptTermsAndConditions { get; set; }

    /// <summary>
    /// Newsletter subscription (for guest checkout)
    /// </summary>
    public bool SubscribeToNewsletter { get; set; } = false;
}

/// <summary>
/// Order confirmation response
/// </summary>
public class MobileOrderConfirmationDto
{
    /// <summary>
    /// Order ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

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
    /// Order status
    /// </summary>
    public string OrderStatus { get; set; } = string.Empty;

    /// <summary>
    /// Payment status
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Order date
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Estimated delivery date
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; set; }

    /// <summary>
    /// Payment URL (for external payment methods)
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Whether payment is required immediately
    /// </summary>
    public bool RequiresPayment { get; set; }

    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}