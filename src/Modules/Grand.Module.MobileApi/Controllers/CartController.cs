using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Interfaces.Storage;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Orders;
using Grand.Infrastructure;
using Grand.Module.MobileApi.Constants;
using Grand.Module.MobileApi.DTOs;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Security.Claims;
using Grand.Business.Core.Extensions;
using Grand.SharedKernel.Extensions;
using System.Linq;

namespace Grand.Module.MobileApi.Controllers;

/// <summary>
/// Mobile API Cart Controller
/// Handles shopping cart operations for both guest and registered users
/// </summary>
[ApiExplorerSettings(GroupName = "v3")]
[Authorize(AuthenticationSchemes = MobileApiConfig.AuthenticationScheme)]
public class CartController : BaseMobileApiController
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly IPictureService _pictureService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ITranslationService _translationService;
    private readonly IShoppingCartValidator _cartValidator;
    private readonly MobileApiConfig _config;

    public CartController(
        IShoppingCartService shoppingCartService,
        IProductService productService,
        ICustomerService customerService,
        IPictureService pictureService,
        IContextAccessor contextAccessor,
        ITranslationService translationService,
        IShoppingCartValidator cartValidator,
        MobileApiConfig config)
    {
        _shoppingCartService = shoppingCartService;
        _productService = productService;
        _customerService = customerService;
        _pictureService = pictureService;
        _contextAccessor = contextAccessor;
        _translationService = translationService;
        _cartValidator = cartValidator;
        _config = config;
    }

    /// <summary>
    /// Get current shopping cart
    /// </summary>
    /// <returns>Current cart contents and totals</returns>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Cart retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve cart: {ex.Message}");
        }
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    /// <param name="model">Add to cart request</param>
    /// <returns>Updated cart</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] MobileAddToCartDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid add to cart data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var product = await _productService.GetProductById(model.ProductId);
            if (product == null)
                return NotFoundError("Product not found");

            if (!product.Published)
                return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Product not available");

            // Validate quantity
            if (model.Quantity <= 0)
                return ValidationError("Quantity must be greater than 0");

            if (model.Quantity > product.StockQuantity)
                return ValidationError("Requested quantity exceeds available stock");

            // Add to cart
            var result = await _shoppingCartService.AddToCart(customer, model.ProductId, 
                ShoppingCartType.ShoppingCart, _contextAccessor.StoreContext.CurrentStore.Id, 
                quantity: model.Quantity);

            if (result.warnings != null && result.warnings.Any())
            {
                return ValidationError("Unable to add item to cart", result.warnings);
            }

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Item added to cart successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to add item to cart: {ex.Message}");
        }
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    /// <param name="itemId">Cart item ID</param>
    /// <param name="model">Update request</param>
    /// <returns>Updated cart</returns>
    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateCartItem(string itemId, [FromBody] MobileUpdateCartItemDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid update data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cartItem = customer.ShoppingCartItems
                .FirstOrDefault(sci => sci.Id == itemId && sci.ShoppingCartTypeId == ShoppingCartType.ShoppingCart);

            if (cartItem == null)
                return NotFoundError("Cart item not found");

            if (model.Quantity <= 0)
            {
                // Remove item if quantity is 0 or less
                await _shoppingCartService.DeleteShoppingCartItem(customer, cartItem);
            }
            else
            {
                // Update quantity - simplified approach
                cartItem.Quantity = model.Quantity;
                await _customerService.UpdateCustomer(customer);
            }

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Cart updated successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to update cart item: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    /// <param name="itemId">Cart item ID</param>
    /// <returns>Updated cart</returns>
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveCartItem(string itemId)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cartItem = customer.ShoppingCartItems
                .FirstOrDefault(sci => sci.Id == itemId && sci.ShoppingCartTypeId == ShoppingCartType.ShoppingCart);

            if (cartItem == null)
                return NotFoundError("Cart item not found");

            await _shoppingCartService.DeleteShoppingCartItem(customer, cartItem);

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Item removed from cart successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to remove cart item: {ex.Message}");
        }
    }

    /// <summary>
    /// Clear entire cart
    /// </summary>
    /// <returns>Empty cart</returns>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // Get all cart items and delete them one by one
            var cartItems = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id, ShoppingCartType.ShoppingCart);
            foreach (var item in cartItems)
            {
                await _shoppingCartService.DeleteShoppingCartItem(customer, item);
            }

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Cart cleared successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to clear cart: {ex.Message}");
        }
    }

    /// <summary>
    /// Get cart totals only
    /// </summary>
    /// <returns>Cart totals without items</returns>
    [HttpGet("totals")]
    public async Task<IActionResult> GetCartTotals()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            var cartItems = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id, 
                ShoppingCartType.ShoppingCart);

            var totals = await CalculateCartTotals(cartItems.ToList());
            return Success(totals, "Cart totals calculated successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to calculate cart totals: {ex.Message}");
        }
    }

    /// <summary>
    /// Apply discount coupon
    /// </summary>
    /// <param name="model">Coupon request</param>
    /// <returns>Updated cart with applied discount</returns>
    [HttpPost("apply-coupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] MobileApplyCouponDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid coupon data", ModelState);

        try
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return AuthenticationError("Customer not found");

            // TODO: Implement coupon application logic
            // This would typically involve:
            // 1. Validating the coupon code
            // 2. Checking eligibility (customer, products, minimum order, etc.)
            // 3. Applying the discount
            // 4. Saving the applied coupon to customer

            var cart = await MapToMobileCartDto(customer);
            return Success(cart, "Coupon applied successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to apply coupon: {ex.Message}");
        }
    }

    #region Private Methods

    private async Task<MobileCartDto> MapToMobileCartDto(Domain.Customers.Customer customer)
    {
        var cartItems = await _shoppingCartService.GetShoppingCart(_contextAccessor.StoreContext.CurrentStore.Id, 
            ShoppingCartType.ShoppingCart);

        var itemTasks = cartItems.Select(async item => await MapToMobileCartItemDto(item));
        var items = await Task.WhenAll(itemTasks);

        var totals = await CalculateCartTotals(cartItems.ToList());

        return new MobileCartDto
        {
            Items = items.ToList(),
            Totals = totals,
            TotalItems = items.Sum(i => i.Quantity),
            IsEmpty = !items.Any(),
            AppliedCoupons = new List<string>() // TODO: Get applied coupons
        };
    }

    private async Task<MobileCartItemDto> MapToMobileCartItemDto(ShoppingCartItem cartItem)
    {
        var product = await _productService.GetProductById(cartItem.ProductId);
        var currency = _contextAccessor.WorkContext.WorkingCurrency;
        var language = _contextAccessor.WorkContext.WorkingLanguage;

        // Get product image
        var imageUrl = "";
        var primaryImage = product.ProductPictures.FirstOrDefault();
        if (primaryImage != null)
        {
            var picture = await _pictureService.GetPictureById(primaryImage.PictureId);
            if (picture != null)
            {
                imageUrl = await _pictureService.GetPictureUrl(picture, 150); // Thumbnail size for cart
            }
        }

        var unitPrice = (decimal)(cartItem.EnteredPrice > 0 ? cartItem.EnteredPrice : product.Price);
        var totalPrice = unitPrice * cartItem.Quantity;

        return new MobileCartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = product.GetTranslation(x => x.Name, language.Id, false),
            Sku = product.Sku,
            ImageUrl = imageUrl,
            UnitPrice = unitPrice,
            Quantity = cartItem.Quantity,
            TotalPrice = totalPrice,
            Currency = currency.CurrencyCode,
            UnitPriceFormatted = unitPrice.ToString("C"),
            TotalPriceFormatted = totalPrice.ToString("C"),
            MaxQuantity = product.StockQuantity,
            InStock = product.StockQuantity > 0,
            Attributes = new List<MobileCartItemAttributeDto>(), // TODO: Map attributes
            Warnings = new List<string>()
        };
    }

    private async Task<MobileCartTotalsDto> CalculateCartTotals(List<ShoppingCartItem> cartItems)
    {
        var currency = _contextAccessor.WorkContext.WorkingCurrency;

        // Simple calculation - in a real implementation, you'd use the order calculation services
        var subtotal = 0m;
        
        foreach (var item in cartItems)
        {
            var product = await _productService.GetProductById(item.ProductId);
            var unitPrice = (decimal)(item.EnteredPrice > 0 ? item.EnteredPrice : product.Price);
            subtotal += unitPrice * item.Quantity;
        }

        var discountAmount = 0m; // TODO: Calculate discounts
        var shippingCost = 0m;   // TODO: Calculate shipping
        var taxAmount = 0m;      // TODO: Calculate tax
        var total = subtotal - discountAmount + shippingCost + taxAmount;

        return new MobileCartTotalsDto
        {
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingCost = shippingCost,
            TaxAmount = taxAmount,
            Total = total,
            Currency = currency.CurrencyCode,
            SubtotalFormatted = subtotal.ToString("C"),
            DiscountFormatted = discountAmount.ToString("C"),
            ShippingFormatted = shippingCost.ToString("C"),
            TaxFormatted = taxAmount.ToString("C"),
            TotalFormatted = total.ToString("C")
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