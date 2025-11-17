using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Domain.Orders;
using Grand.Infrastructure;
using Grand.Module.MobileApi.Constants;
using Grand.Module.MobileApi.DTOs;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Grand.Business.Core.Extensions;
using Grand.SharedKernel.Extensions;

namespace Grand.Module.MobileApi.Controllers;

/// <summary>
/// Mobile API Checkout Controller
/// Handles complete checkout process including guest checkout
/// </summary>
[ApiExplorerSettings(GroupName = "v3")]
[Authorize(AuthenticationSchemes = MobileApiConfig.AuthenticationScheme)]
public class CheckoutController : BaseMobileApiController
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ICustomerService _customerService;
    private readonly IShippingService _shippingService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ICountryService _countryService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ITranslationService _translationService;
    private readonly MobileApiConfig _config;

    public CheckoutController(
        IShoppingCartService shoppingCartService,
        ICustomerService customerService,
        IShippingService shippingService,
        IPaymentService paymentService,
        IOrderService orderService,
        ICountryService countryService,
        IContextAccessor contextAccessor,
        ITranslationService translationService,
        MobileApiConfig config)
    {
        _shoppingCartService = shoppingCartService;
        _customerService = customerService;
        _shippingService = shippingService;
        _paymentService = paymentService;
        _orderService = orderService;
        _countryService = countryService;
        _contextAccessor = contextAccessor;
        _translationService = translationService;
        _config = config;
    }

    /// <summary>
    /// Start checkout process
    /// </summary>
    /// <returns>Initial checkout data</returns>
    [HttpGet("start")]
    public async Task<IActionResult> StartCheckout()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // Validate cart has items
            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            if (!cart.Any())
                return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Cart is empty");

            var checkout = await BuildCheckoutDto(customer, cart.ToList());
            return Success(checkout, "Checkout started successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to start checkout: {ex.Message}");
        }
    }

    /// <summary>
    /// Set billing address
    /// </summary>
    /// <param name="model">Billing address data</param>
    /// <returns>Updated checkout data</returns>
    [HttpPost("billing-address")]
    public async Task<IActionResult> SetBillingAddress([FromBody] MobileSetBillingAddressDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid billing address data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // TODO: Validate and save billing address
            // This would typically involve:
            // 1. Validating the address data
            // 2. Saving to customer's address book (if requested)
            // 3. Setting as checkout billing address

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            var checkout = await BuildCheckoutDto(customer, cart.ToList());
            checkout.BillingAddress = model.Address;

            return Success(checkout, "Billing address set successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to set billing address: {ex.Message}");
        }
    }

    /// <summary>
    /// Set shipping address
    /// </summary>
    /// <param name="model">Shipping address data</param>
    /// <returns>Updated checkout data with shipping methods</returns>
    [HttpPost("shipping-address")]
    public async Task<IActionResult> SetShippingAddress([FromBody] MobileSetShippingAddressDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid shipping address data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            var checkout = await BuildCheckoutDto(customer, cart.ToList());
            
            if (model.SameAsBilling)
            {
                checkout.ShippingAddressSameAsBilling = true;
                checkout.ShippingAddress = null;
            }
            else
            {
                checkout.ShippingAddressSameAsBilling = false;
                checkout.ShippingAddress = model.Address;
            }

            // Load available shipping methods
            checkout.AvailableShippingMethods = await GetAvailableShippingMethods(customer, cart.ToList());

            return Success(checkout, "Shipping address set successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to set shipping address: {ex.Message}");
        }
    }

    /// <summary>
    /// Get available shipping methods
    /// </summary>
    /// <returns>List of available shipping methods</returns>
    [HttpGet("shipping-methods")]
    public async Task<IActionResult> GetShippingMethods()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            var shippingMethods = await GetAvailableShippingMethods(customer, cart.ToList());
            return Success(shippingMethods, "Shipping methods retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to get shipping methods: {ex.Message}");
        }
    }

    /// <summary>
    /// Select shipping method
    /// </summary>
    /// <param name="model">Shipping method selection</param>
    /// <returns>Updated checkout data with updated totals</returns>
    [HttpPost("shipping-method")]
    public async Task<IActionResult> SelectShippingMethod([FromBody] MobileSelectShippingMethodDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid shipping method data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // TODO: Save selected shipping method to customer/session

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            var checkout = await BuildCheckoutDto(customer, cart.ToList());

            // Mark selected shipping method
            var selectedMethod = checkout.AvailableShippingMethods.FirstOrDefault(sm => sm.Id == model.ShippingMethodId);
            if (selectedMethod != null)
            {
                // Reset all selections
                foreach (var method in checkout.AvailableShippingMethods)
                    method.IsSelected = false;
                
                // Set selected method
                selectedMethod.IsSelected = true;
                checkout.SelectedShippingMethod = selectedMethod;

                // Update totals with shipping cost
                checkout.Totals.ShippingCost = selectedMethod.Cost;
                checkout.Totals.ShippingFormatted = selectedMethod.CostFormatted;
                checkout.Totals.Total = checkout.Totals.Subtotal + checkout.Totals.ShippingCost + checkout.Totals.TaxAmount - checkout.Totals.DiscountAmount;
                checkout.Totals.TotalFormatted = checkout.Totals.Total.ToString("C");
            }

            // Load payment methods after shipping is selected
            checkout.AvailablePaymentMethods = await GetAvailablePaymentMethods(customer);

            return Success(checkout, "Shipping method selected successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to select shipping method: {ex.Message}");
        }
    }

    /// <summary>
    /// Get available payment methods
    /// </summary>
    /// <returns>List of available payment methods</returns>
    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var paymentMethods = await GetAvailablePaymentMethods(customer);
            return Success(paymentMethods, "Payment methods retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to get payment methods: {ex.Message}");
        }
    }

    /// <summary>
    /// Select payment method
    /// </summary>
    /// <param name="model">Payment method selection</param>
    /// <returns>Updated checkout data</returns>
    [HttpPost("payment-method")]
    public async Task<IActionResult> SelectPaymentMethod([FromBody] MobileSelectPaymentMethodDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid payment method data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // TODO: Save selected payment method and additional info

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            var checkout = await BuildCheckoutDto(customer, cart.ToList());

            // Mark selected payment method
            var selectedMethod = checkout.AvailablePaymentMethods.FirstOrDefault(pm => pm.Id == model.PaymentMethodId);
            if (selectedMethod != null)
            {
                // Reset all selections
                foreach (var method in checkout.AvailablePaymentMethods)
                    method.IsSelected = false;
                
                // Set selected method
                selectedMethod.IsSelected = true;
                checkout.SelectedPaymentMethod = selectedMethod;

                // Update totals with payment fee
                checkout.Totals.Total += selectedMethod.Fee;
                checkout.Totals.TotalFormatted = checkout.Totals.Total.ToString("C");
            }

            return Success(checkout, "Payment method selected successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to select payment method: {ex.Message}");
        }
    }

    /// <summary>
    /// Place order
    /// </summary>
    /// <param name="model">Order placement data</param>
    /// <returns>Order confirmation</returns>
    [HttpPost("place-order")]
    public async Task<IActionResult> PlaceOrder([FromBody] MobilePlaceOrderDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid order data", ModelState);

        if (!model.AcceptTermsAndConditions)
            return ValidationError("You must accept the terms and conditions");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cart = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id,
                ShoppingCartType.ShoppingCart);

            if (!cart.Any())
                return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Cart is empty");

            // TODO: Complete order placement logic
            // This would involve:
            // 1. Final validation of cart, addresses, shipping, payment
            // 2. Creating the order
            // 3. Processing payment (if immediate payment required)
            // 4. Clearing the cart
            // 5. Sending order confirmation

            // For now, create a mock order confirmation
            var orderConfirmation = new MobileOrderConfirmationDto
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                OrderTotal = (decimal)cart.Sum(item => item.EnteredPrice > 0 ? item.EnteredPrice * item.Quantity : 0), // Simplified calculation
                OrderTotalFormatted = "TBD", // Would be calculated properly
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                OrderDate = DateTime.UtcNow,
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5),
                RequiresPayment = true,
                Message = "Your order has been placed successfully! You will receive an email confirmation shortly."
            };

            return Success(orderConfirmation, "Order placed successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to place order: {ex.Message}");
        }
    }

    #region Private Methods

    private async Task<MobileCheckoutDto> BuildCheckoutDto(Domain.Customers.Customer customer, List<ShoppingCartItem> cartItems)
    {
        var userType = User.FindFirst("UserType")?.Value ?? MobileApiConstants.UserTypes.Guest;
        var isGuestCheckout = userType == MobileApiConstants.UserTypes.Guest;

        // Convert cart items to checkout items
        var cartItemTasks = cartItems.Select(async item => await MapToMobileCartItemDto(item));
        var checkoutItems = await Task.WhenAll(cartItemTasks);

        // Calculate totals
        var totals = await CalculateCartTotals(cartItems);

        var checkout = new MobileCheckoutDto
        {
            CartItems = checkoutItems.ToList(),
            Totals = totals,
            IsGuestCheckout = isGuestCheckout,
            ShippingAddressSameAsBilling = true
        };

        // Set customer info for guest checkout
        if (isGuestCheckout)
        {
            checkout.CustomerInfo = new MobileCheckoutCustomerDto
            {
                Email = customer.Email ?? "",
                // Other fields would be filled from previous steps or user input
            };
        }

        return checkout;
    }

    private async Task<List<MobileShippingMethodDto>> GetAvailableShippingMethods(Domain.Customers.Customer customer, List<ShoppingCartItem> cart)
    {
        // TODO: Implement actual shipping method retrieval
        // This would involve calling the shipping service with cart and address details

        return new List<MobileShippingMethodDto>
        {
            new MobileShippingMethodDto
            {
                Id = "standard",
                Name = "Standard Shipping",
                Description = "5-7 business days",
                Cost = 9.99m,
                CostFormatted = "$9.99",
                DeliveryTime = "5-7 business days"
            },
            new MobileShippingMethodDto
            {
                Id = "express",
                Name = "Express Shipping",
                Description = "2-3 business days",
                Cost = 19.99m,
                CostFormatted = "$19.99",
                DeliveryTime = "2-3 business days"
            }
        };
    }

    private async Task<List<MobilePaymentMethodDto>> GetAvailablePaymentMethods(Domain.Customers.Customer customer)
    {
        // TODO: Implement actual payment method retrieval
        // This would involve calling the payment service

        return new List<MobilePaymentMethodDto>
        {
            new MobilePaymentMethodDto
            {
                Id = "creditcard",
                Name = "Credit Card",
                Description = "Pay with credit or debit card",
                Fee = 0m,
                FeeFormatted = "$0.00"
            },
            new MobilePaymentMethodDto
            {
                Id = "paypal",
                Name = "PayPal",
                Description = "Pay with your PayPal account",
                Fee = 0m,
                FeeFormatted = "$0.00"
            },
            new MobilePaymentMethodDto
            {
                Id = "cashondelivery",
                Name = "Cash on Delivery",
                Description = "Pay when you receive your order",
                Fee = 5.00m,
                FeeFormatted = "$5.00"
            }
        };
    }

    // Reuse cart mapping methods from CartController
    private async Task<MobileCartItemDto> MapToMobileCartItemDto(ShoppingCartItem cartItem)
    {
        // TODO: Implement cart item mapping (same as in CartController)
        // For now, return a simplified version
        return new MobileCartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            // Other properties would be mapped from product data
        };
    }

    private async Task<MobileCartTotalsDto> CalculateCartTotals(List<ShoppingCartItem> cartItems)
    {
        // TODO: Implement proper total calculation (same as in CartController)
        // For now, return a simplified version
        var currency = _contextAccessor.WorkContext.WorkingCurrency;
        
        return new MobileCartTotalsDto
        {
            Subtotal = 100m, // Simplified
            DiscountAmount = 0m,
            ShippingCost = 0m,
            TaxAmount = 0m,
            Total = 100m,
            Currency = currency.CurrencyCode,
            SubtotalFormatted = "$100.00",
            DiscountFormatted = "$0.00",
            ShippingFormatted = "$0.00",
            TaxFormatted = "$0.00",
            TotalFormatted = "$100.00"
        };
    }

    private async Task<Domain.Customers.Customer?> GetCurrentCustomer()
    {
        var email = User.FindFirst("Email")?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            return await _customerService.GetCustomerByEmail(email);
        }

        var guid = User.FindFirst("Guid")?.Value;
        if (!string.IsNullOrEmpty(guid) && Guid.TryParse(guid, out var customerGuid))
        {
            return await _customerService.GetCustomerByGuid(customerGuid);
        }

        return null;
    }

    #endregion
}
