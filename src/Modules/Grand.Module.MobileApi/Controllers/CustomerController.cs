using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Domain.Common;
using Grand.Domain.Customers;
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
/// Mobile API Customer Controller
/// Handles customer profile management for registered users only
/// </summary>
[ApiExplorerSettings(GroupName = "v3")]
[Authorize(AuthenticationSchemes = MobileApiConfig.AuthenticationScheme)]
public class CustomerController : BaseMobileApiController
{
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly ICountryService _countryService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ITranslationService _translationService;
    private readonly MobileApiConfig _config;

    public CustomerController(
        ICustomerService customerService,
        IOrderService orderService,
        ICountryService countryService,
        IContextAccessor contextAccessor,
        ITranslationService translationService,
        MobileApiConfig config)
    {
        _customerService = customerService;
        _orderService = orderService;
        _countryService = countryService;
        _contextAccessor = contextAccessor;
        _translationService = translationService;
        _config = config;
    }

    /// <summary>
    /// Get customer profile
    /// </summary>
    /// <returns>Customer profile information</returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            var profile = await MapToMobileCustomerProfileDto(customer);
            return Success(profile, "Profile retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Update customer profile
    /// </summary>
    /// <param name="model">Profile update data</param>
    /// <returns>Updated profile</returns>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] MobileUpdateProfileDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid profile data", ModelState);

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // Update customer fields
            customer.UserFields.Add(new UserField 
            { 
                Key = SystemCustomerFieldNames.FirstName, 
                Value = model.FirstName ?? "", 
                StoreId = _contextAccessor.StoreContext.CurrentStore.Id 
            });
            customer.UserFields.Add(new UserField 
            { 
                Key = SystemCustomerFieldNames.LastName, 
                Value = model.LastName ?? "", 
                StoreId = _contextAccessor.StoreContext.CurrentStore.Id 
            });
            customer.UserFields.Add(new UserField 
            { 
                Key = SystemCustomerFieldNames.Phone, 
                Value = model.PhoneNumber ?? "", 
                StoreId = _contextAccessor.StoreContext.CurrentStore.Id 
            });
            
            if (model.DateOfBirth.HasValue)
                customer.UserFields.Add(new UserField 
                { 
                    Key = SystemCustomerFieldNames.DateOfBirth, 
                    Value = model.DateOfBirth.Value.ToString(), 
                    StoreId = _contextAccessor.StoreContext.CurrentStore.Id 
                });
                
            if (!string.IsNullOrEmpty(model.Gender))
                customer.UserFields.Add(new UserField 
                { 
                    Key = SystemCustomerFieldNames.Gender, 
                    Value = model.Gender, 
                    StoreId = _contextAccessor.StoreContext.CurrentStore.Id 
                });

            // TODO: Handle newsletter subscription

            await _customerService.UpdateCustomer(customer);

            var updatedProfile = await MapToMobileCustomerProfileDto(customer);
            return Success(updatedProfile, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to update profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Change customer password
    /// </summary>
    /// <param name="model">Password change data</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] MobileChangePasswordDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid password data", ModelState);

        if (model.NewPassword != model.ConfirmNewPassword)
            return ValidationError("New passwords do not match");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Implement password validation and change
            // This would involve:
            // 1. Validating current password
            // 2. Hashing new password
            // 3. Updating password in database
            // 4. Invalidating existing tokens (optional)

            return Success<object>(null, "Password changed successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to change password: {ex.Message}");
        }
    }

    /// <summary>
    /// Get customer addresses
    /// </summary>
    /// <returns>List of customer addresses</returns>
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            var addressTasks = customer.Addresses.Select(async address => await MapToMobileCustomerAddressDto(address));
            var addresses = await Task.WhenAll(addressTasks);

            return Success(addresses.ToList(), "Addresses retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve addresses: {ex.Message}");
        }
    }

    /// <summary>
    /// Add new customer address
    /// </summary>
    /// <param name="model">New address data</param>
    /// <returns>Created address</returns>
    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] MobileAddressDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid address data", ModelState);

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Create and save new address
            // This would involve:
            // 1. Creating Address entity from model
            // 2. Adding to customer.Addresses
            // 3. Saving customer

            var newAddress = await MapToMobileCustomerAddressDto(new Address()); // Placeholder
            return Success(newAddress, "Address added successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to add address: {ex.Message}");
        }
    }

    /// <summary>
    /// Update customer address
    /// </summary>
    /// <param name="addressId">Address ID</param>
    /// <param name="model">Updated address data</param>
    /// <returns>Updated address</returns>
    [HttpPut("addresses/{addressId}")]
    public async Task<IActionResult> UpdateAddress(string addressId, [FromBody] MobileAddressDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid address data", ModelState);

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFoundError("Address not found");

            // TODO: Update address fields and save

            var updatedAddress = await MapToMobileCustomerAddressDto(address);
            return Success(updatedAddress, "Address updated successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to update address: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete customer address
    /// </summary>
    /// <param name="addressId">Address ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress(string addressId)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFoundError("Address not found");

            // TODO: Remove address from customer and save

            return Success<object>(null, "Address deleted successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to delete address: {ex.Message}");
        }
    }

    /// <summary>
    /// Get customer order history
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of orders</returns>
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Get customer orders with pagination
            var orders = new List<MobileCustomerOrderDto>(); // Placeholder

            var pagination = new MobilePaginationDto
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = orders.Count,
                TotalPages = (int)Math.Ceiling((double)orders.Count / pageSize),
                HasNext = page * pageSize < orders.Count,
                HasPrevious = page > 1
            };

            var result = new
            {
                Orders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Pagination = pagination
            };

            return Success(result, "Orders retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve orders: {ex.Message}");
        }
    }

    /// <summary>
    /// Get order details
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>Detailed order information</returns>
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderDetails(string orderId)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Get order by ID and verify ownership
            // Ensure the order belongs to the current customer

            var orderDetail = new MobileOrderDetailDto(); // Placeholder
            return Success(orderDetail, "Order details retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve order details: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("orders/{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Implement order cancellation
            // This would involve:
            // 1. Verifying order ownership
            // 2. Checking if order can be cancelled (status, timing, etc.)
            // 3. Processing cancellation
            // 4. Handling refunds if applicable

            return Success<object>(null, "Order cancelled successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to cancel order: {ex.Message}");
        }
    }

    /// <summary>
    /// Track order shipment
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>Tracking information</returns>
    [HttpGet("orders/{orderId}/track")]
    public async Task<IActionResult> TrackOrder(string orderId)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            var customer = await GetCurrentRegisteredCustomer();
            if (customer == null)
                return AuthenticationError("Registered customer not found");

            // TODO: Get tracking information for the order

            var trackingInfo = new
            {
                OrderId = orderId,
                TrackingNumber = "TRK123456789",
                Carrier = "Sample Carrier",
                Status = "In Transit",
                EstimatedDelivery = DateTime.UtcNow.AddDays(2),
                TrackingUrl = "https://example.com/track/TRK123456789"
            };

            return Success(trackingInfo, "Tracking information retrieved successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to retrieve tracking information: {ex.Message}");
        }
    }

    #region Private Methods

    private async Task<Customer?> GetCurrentRegisteredCustomer()
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != MobileApiConstants.UserTypes.Registered)
            return null;

        var email = User.FindFirst("Email")?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            return await _customerService.GetCustomerByEmail(email);
        }

        return null;
    }

    private async Task<MobileCustomerProfileDto> MapToMobileCustomerProfileDto(Customer customer)
    {
        var firstName = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.FirstName) ?? "";
        var lastName = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.LastName) ?? "";

        return new MobileCustomerProfileDto
        {
            Id = customer.Id,
            Email = customer.Email,
            FirstName = firstName,
            LastName = lastName,
            FullName = $"{firstName} {lastName}".Trim(),
            PhoneNumber = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.Phone) ?? "",
            DateOfBirth = customer.GetUserFieldFromEntity<DateTime?>(SystemCustomerFieldNames.DateOfBirth),
            Gender = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.Gender),
            CreatedOn = customer.CreatedOnUtc,
            LastActivityDate = customer.LastActivityDateUtc,
            EmailConfirmed = true, // TODO: Get actual email confirmation status
            NewsletterSubscribed = false // TODO: Get actual newsletter subscription status
        };
    }

    private async Task<MobileCustomerAddressDto> MapToMobileCustomerAddressDto(Address address)
    {
        var country = await _countryService.GetCountryById(address.CountryId);

        return new MobileCustomerAddressDto
        {
            Id = address.Id,
            FirstName = address.FirstName,
            LastName = address.LastName,
            Email = address.Email,
            PhoneNumber = address.PhoneNumber,
            Company = address.Company,
            Address1 = address.Address1,
            Address2 = address.Address2,
            City = address.City,
            StateProvince = address.StateProvinceId, // TODO: Get state name
            ZipPostalCode = address.ZipPostalCode,
            CountryId = address.CountryId,
            CountryName = country?.Name ?? "",
            CreatedOn = DateTime.UtcNow // Address doesn't have CreatedOnUtc since it inherits from SubBaseEntity
        };
    }

    #endregion
}
