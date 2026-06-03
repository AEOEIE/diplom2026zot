//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NewDiplom.Data;
//using NewDiplom.DTOs.Shipments;
//using NewDiplom.Entities;
//using System.Data;
//using System.Security.Claims;

//namespace NewDiplom.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//[Authorize]
//public class ShipmentsController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public ShipmentsController(AppDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet("my")]
//    [Authorize(Roles = "Client")]
//    public async Task<IActionResult> My()
//    {
//        //var userId = int.Parse(
//        //    User.Claims.First(x => x.Type == "nameid").Value);
//        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

//        if (userIdClaim == null)
//        {
//            return Unauthorized();
//        }

//        var userId = int.Parse(userIdClaim.Value);

//        //

//        var shipments = await _context.Shipments
//            .Include(x => x.ServiceType)
//            .Include(x => x.DeliveryMethod)
//            .Include(x => x.CurrentStatus)
//            .Where(x => x.ClientId == userId).Include(x => x.CurrentOffice)  // Добавьте
//            .Include(x => x.DestinationOffice)
//            .ToListAsync();

//        var result = shipments.Select(x => new ShipmentDto
//        {
//            Id = x.Id,
//            TrackingNumber = x.TrackingNumber,
//            RecipientName = x.RecipientName,
//            DestinationAddress = x.DestinationAddress,
//            ServiceType = x.ServiceType.Name,
//            DeliveryMethod = x.DeliveryMethod.Name,
//            Status = x.CurrentStatus.Name,
//            WeightKg = x.WeightKg,
//            TotalPrice = x.TotalPrice,
//            AcceptedAt = x.AcceptedAt,
//            RecipientPhone = x.RecipientPhone,
//            CurrentOfficeName = x.CurrentOffice?.Name,
//            DestinationOfficeName = x.DestinationOffice?.Name,
//        });

//        return Ok(result);
//    }

//    [HttpGet]
//    [Authorize(Roles = "Admin,Operator,DepartmentHead")]
//    public async Task<IActionResult> GetAll()
//    {
//        var shipments = await _context.Shipments
//            .Include(x => x.ServiceType)
//            .Include(x => x.DeliveryMethod)
//            .Include(x => x.CurrentStatus).Include(x => x.CurrentOffice)  
//            .Include(x => x.DestinationOffice)
//            .ToListAsync();

//        var result = shipments.Select(x => new ShipmentDto
//        {
//            Id = x.Id,
//            TrackingNumber = x.TrackingNumber,
//            RecipientName = x.RecipientName,
//            DestinationAddress = x.DestinationAddress,
//            ServiceType = x.ServiceType.Name,
//            DeliveryMethod = x.DeliveryMethod.Name,
//            Status = x.CurrentStatus.Name,
//            WeightKg = x.WeightKg,
//            TotalPrice = x.TotalPrice,
//            AcceptedAt = x.AcceptedAt,
//            RecipientPhone = x.RecipientPhone,
//            CurrentOfficeName = x.CurrentOffice?.Name,
//            DestinationOfficeName = x.DestinationOffice?.Name,
//        });

//        return Ok(result);
//    }

//    [HttpGet("{id}")]
//    [Authorize(Roles = "Admin,Operator,DepartmentHead,Client")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var userIdClaim = User.FindFirst(
//            ClaimTypes.NameIdentifier);

//        if (userIdClaim == null)
//            return Unauthorized();

//        var userId = int.Parse(userIdClaim.Value);

//        var role = User.FindFirst(
//            ClaimTypes.Role)?.Value;

//        var shipment = await _context.Shipments
//            .Include(x => x.ServiceType)
//            .Include(x => x.DeliveryMethod)
//            .Include(x => x.CurrentStatus).Include(x => x.CurrentOffice)  
//            .Include(x => x.DestinationOffice)
//            .FirstOrDefaultAsync(x => x.Id == id);

//        if (shipment == null)
//            return NotFound();

//        if (role == "Client"
//            && shipment.ClientId != userId)
//        {
//            return Forbid();
//        }

//        var result = new ShipmentDto
//        {
//            Id = shipment.Id,

//            TrackingNumber =
//                shipment.TrackingNumber,

//            RecipientName =
//                shipment.RecipientName,

//            DestinationAddress =
//                shipment.DestinationAddress,

//            ServiceType =
//                shipment.ServiceType.Name,

//            DeliveryMethod =
//                shipment.DeliveryMethod.Name,

//            Status =
//                shipment.CurrentStatus.Name,

//            WeightKg =
//                shipment.WeightKg,

//            TotalPrice =
//                shipment.TotalPrice,

//            AcceptedAt =
//                shipment.AcceptedAt,
//            RecipientPhone = shipment.RecipientPhone,
//            CurrentOfficeName = shipment.CurrentOffice?.Name,
//            DestinationOfficeName = shipment.DestinationOffice?.Name,
//        };

//        return Ok(result);
//    }

//    //[HttpPost]
//    //[Authorize(Roles = "Client,Operator,Admin")]
//    //public async Task<IActionResult> Create(CreateShipmentRequest request)
//    //{
//    //    var userId = int.Parse(
//    //        User.Claims.First(x => x.Type == "nameid").Value);

//    //    var serviceType = await _context.ServiceTypes
//    //        .FirstOrDefaultAsync(x => x.Id == request.ServiceTypeId);

//    //    var deliveryMethod = await _context.DeliveryMethods
//    //        .FirstOrDefaultAsync(x => x.Id == request.DeliveryMethodId);

//    //    var acceptedStatus = await _context.ShipmentStatuses
//    //        .FirstAsync(x => x.Name == "Принято");

//    //    if (serviceType == null || deliveryMethod == null)
//    //        return BadRequest("Неверные данные");

//    //    var totalPrice =
//    //        serviceType.BasePrice +
//    //        deliveryMethod.AdditionalPrice+(request.WeightKg * 100);

//    //    var code = Random.Shared
//    //        .Next(100000, 999999)
//    //        .ToString();

//    //    var role = User.FindFirst(ClaimTypes.Role)?.Value;
//    //    int senderEmployeeId = userId;

//    //        var shipment = new Shipment
//    //        {
//    //            TrackingNumber = Guid.NewGuid()
//    //                .ToString()
//    //                .Replace("-", "")
//    //                .Substring(0, 12)
//    //                .ToUpper(),

//    //            ClientId = userId,

//    //            SenderEmployeeId = senderEmployeeId,

//    //            CurrentOfficeId = 1,

//    //            DestinationOfficeId = request.DestinationOfficeId,

//    //            RecipientName = request.RecipientName,

//    //            DestinationAddress = request.DestinationAddress,

//    //            ServiceTypeId = request.ServiceTypeId,

//    //            DeliveryMethodId = request.DeliveryMethodId,

//    //            CurrentStatusId = acceptedStatus.Id,

//    //            WeightKg = request.WeightKg,

//    //            DeclaredValue = request.DeclaredValue,

//    //            TotalPrice = totalPrice,

//    //            AcceptedAt = DateTime.UtcNow,

//    //            Notes = request.Notes,

//    //            ConfirmationCode = code,

//    //            IsConfirmed = false,

//    //            IsDeleted = false,
//    //            RecipientPhone = request.RecipientPhone

//    //        };

//    //    _context.Shipments.Add(shipment);

//    //    await _context.SaveChangesAsync();

//    //    return Ok();
//    //}
//    [HttpPost]
//    [Authorize(Roles = "Client,Operator,Admin")]
//    public async Task<IActionResult> Create(CreateShipmentRequest request)
//    {
//        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//        if (userIdClaim == null)
//            return Unauthorized();

//        var userId = int.Parse(userIdClaim.Value);
//        var role = User.FindFirst(ClaimTypes.Role)?.Value;

//        // Валидация
//        if (string.IsNullOrWhiteSpace(request.RecipientName))
//            return BadRequest("Укажите получателя");

//        if (string.IsNullOrWhiteSpace(request.RecipientPhone))
//            return BadRequest("Укажите телефон получателя");

//        if (request.CurrentOfficeId == 0)
//            return BadRequest("Выберите отделение отправления");

//        if (request.DestinationOfficeId == 0)
//            return BadRequest("Выберите отделение назначения");

//        if (request.ServiceTypeId == 0)
//            return BadRequest("Выберите тип услуги");

//        if (request.DeliveryMethodId == 0)
//            return BadRequest("Выберите способ доставки");

//        if (request.WeightKg <= 0)
//            return BadRequest("Укажите вес");

//        var serviceType = await _context.ServiceTypes
//            .FirstOrDefaultAsync(x => x.Id == request.ServiceTypeId && x.IsActive !=false);

//        var deliveryMethod = await _context.DeliveryMethods
//            .FirstOrDefaultAsync(x => x.Id == request.DeliveryMethodId && x.IsActive != false);

//        var acceptedStatus = await _context.ShipmentStatuses
//            .FirstAsync(x => x.Name == "Принято");

//        if (serviceType == null || deliveryMethod == null)
//            return BadRequest("Неверные данные");

//        var totalPrice = serviceType.BasePrice +
//                         deliveryMethod.AdditionalPrice +
//                         (request.WeightKg * 100);

//        var code = Random.Shared.Next(100000, 999999).ToString();

//        // Определяем ClientId и EmployeeId
//        int clientId;
//        int? senderEmployeeId = null;
//        int currentOfficeId;

//        if (role == "Client")
//        {
//            // Клиент создает для себя
//            clientId = userId;
//            currentOfficeId = request.CurrentOfficeId;
//        }
//        else
//        {
//            // Сотрудник создает от имени клиента
//            if (!request.ClientId.HasValue || request.ClientId == 0)
//                return BadRequest("Выберите клиента");

//            clientId = request.ClientId.Value;
//            senderEmployeeId = userId;

//            // Берем отделение сотрудника
//            var employee = await _context.Users
//                .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive != false);
//            currentOfficeId = employee?.PostOfficeId ?? request.CurrentOfficeId;
//        }

//        var shipment = new Shipment
//        {
//            TrackingNumber = Guid.NewGuid()
//                .ToString()
//                .Replace("-", "")
//                .Substring(0, 12)
//                .ToUpper(),

//            ClientId = clientId,
//            SenderEmployeeId = senderEmployeeId,
//            CurrentOfficeId = currentOfficeId,
//            DestinationOfficeId = request.DestinationOfficeId,

//            RecipientName = request.RecipientName,
//            DestinationAddress = request.DestinationAddress,
//            RecipientPhone = request.RecipientPhone,

//            ServiceTypeId = request.ServiceTypeId,
//            DeliveryMethodId = request.DeliveryMethodId,
//            CurrentStatusId = acceptedStatus.Id,

//            WeightKg = request.WeightKg,
//            DeclaredValue = request.DeclaredValue,
//            TotalPrice = totalPrice,

//            AcceptedAt = DateTime.UtcNow,
//            Notes = request.Notes,
//            ConfirmationCode = code,
//            IsConfirmed = false,
//            IsDeleted = false
//        };

//        _context.Shipments.Add(shipment);
//        await _context.SaveChangesAsync();

//        return Ok();
//    }

//    [HttpPut("{id}/status")]
//    [Authorize(Roles = "Admin,Operator")]
//    public async Task<IActionResult> UpdateStatus(
//        int id,
//        UpdateShipmentStatusRequest request)
//    {
//        var shipment = await _context.Shipments
//            .FirstOrDefaultAsync(x => x.Id == id);

//        if (shipment == null)
//            return NotFound();

//        shipment.CurrentStatusId = request.StatusId;

//        await _context.SaveChangesAsync();

//        return Ok();
//    }

//    [HttpGet("track/{trackingNumber}")]
//    [AllowAnonymous]
//    public async Task<IActionResult> Track(string trackingNumber)
//    {
//        var shipment = await _context.Shipments
//            .Include(x => x.CurrentStatus)
//            .FirstOrDefaultAsync(x =>
//                x.TrackingNumber == trackingNumber);

//        if (shipment == null)
//            return NotFound();

//        return Ok(new
//        {
//            shipment.TrackingNumber,
//            Status = shipment.CurrentStatus.Name,
//            shipment.AcceptedAt
//        });
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.DTOs.Shipments;
using NewDiplom.Entities;
using NewDiplom.Services; // ← ДОБАВИТЬ
using System.Data;
using System.Security.Claims;

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; // ← ДОБАВИТЬ

    public ShipmentsController(AppDbContext context, AuditService auditService) // ← ДОБАВИТЬ
    {
        _context = context;
        _auditService = auditService; // ← ДОБАВИТЬ
    }

    [HttpGet("my")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> My()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);

        var shipments = await _context.Shipments
            .Include(x => x.ServiceType)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.CurrentStatus)
            .Where(x => x.ClientId == userId)
            .Include(x => x.CurrentOffice)
            .Include(x => x.DestinationOffice)
            .ToListAsync();

        var result = shipments.Select(x => new ShipmentDto
        {
            Id = x.Id,
            TrackingNumber = x.TrackingNumber,
            RecipientName = x.RecipientName,
            DestinationAddress = x.DestinationAddress,
            ServiceType = x.ServiceType.Name,
            DeliveryMethod = x.DeliveryMethod.Name,
            Status = x.CurrentStatus.Name,
            WeightKg = x.WeightKg,
            TotalPrice = x.TotalPrice,
            AcceptedAt = x.AcceptedAt,
            RecipientPhone = x.RecipientPhone,
            CurrentOfficeName = x.CurrentOffice?.Name,
            DestinationOfficeName = x.DestinationOffice?.Name,
        });

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operator,DepartmentHead")]
    public async Task<IActionResult> GetAll()
    {
        var shipments = await _context.Shipments
            .Include(x => x.ServiceType)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.CurrentStatus)
            .Include(x => x.CurrentOffice)
            .Include(x => x.DestinationOffice)
            .ToListAsync();

        var result = shipments.Select(x => new ShipmentDto
        {
            Id = x.Id,
            TrackingNumber = x.TrackingNumber,
            RecipientName = x.RecipientName,
            DestinationAddress = x.DestinationAddress,
            ServiceType = x.ServiceType.Name,
            DeliveryMethod = x.DeliveryMethod.Name,
            Status = x.CurrentStatus.Name,
            WeightKg = x.WeightKg,
            TotalPrice = x.TotalPrice,
            AcceptedAt = x.AcceptedAt,
            RecipientPhone = x.RecipientPhone,
            CurrentOfficeName = x.CurrentOffice?.Name,
            DestinationOfficeName = x.DestinationOffice?.Name,
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Operator,DepartmentHead,Client")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        var shipment = await _context.Shipments
            .Include(x => x.ServiceType)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.CurrentStatus)
            .Include(x => x.CurrentOffice)
            .Include(x => x.DestinationOffice)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (shipment == null)
            return NotFound();

        if (role == "Client" && shipment.ClientId != userId)
        {
            return Forbid();
        }

        var result = new ShipmentDto
        {
            Id = shipment.Id,
            TrackingNumber = shipment.TrackingNumber,
            RecipientName = shipment.RecipientName,
            DestinationAddress = shipment.DestinationAddress,
            ServiceType = shipment.ServiceType.Name,
            DeliveryMethod = shipment.DeliveryMethod.Name,
            Status = shipment.CurrentStatus.Name,
            WeightKg = shipment.WeightKg,
            TotalPrice = shipment.TotalPrice,
            AcceptedAt = shipment.AcceptedAt,
            RecipientPhone = shipment.RecipientPhone,
            CurrentOfficeName = shipment.CurrentOffice?.Name,
            DestinationOfficeName = shipment.DestinationOffice?.Name,
        };

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Client,Operator,Admin")]
    public async Task<IActionResult> Create(CreateShipmentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Валидация
        if (string.IsNullOrWhiteSpace(request.RecipientName))
            return BadRequest("Укажите получателя");

        if (string.IsNullOrWhiteSpace(request.RecipientPhone))
            return BadRequest("Укажите телефон получателя");

        if (request.CurrentOfficeId == 0)
            return BadRequest("Выберите отделение отправления");

        if (request.DestinationOfficeId == 0)
            return BadRequest("Выберите отделение назначения");

        if (request.ServiceTypeId == 0)
            return BadRequest("Выберите тип услуги");

        if (request.DeliveryMethodId == 0)
            return BadRequest("Выберите способ доставки");

        if (request.WeightKg <= 0)
            return BadRequest("Укажите вес");

        var serviceType = await _context.ServiceTypes
            .FirstOrDefaultAsync(x => x.Id == request.ServiceTypeId && x.IsActive != false);

        var deliveryMethod = await _context.DeliveryMethods
            .FirstOrDefaultAsync(x => x.Id == request.DeliveryMethodId && x.IsActive != false);

        var acceptedStatus = await _context.ShipmentStatuses
            .FirstAsync(x => x.Name == "Принято");

        if (serviceType == null || deliveryMethod == null)
            return BadRequest("Неверные данные");

        var totalPrice = serviceType.BasePrice +
                         deliveryMethod.AdditionalPrice +
                         (request.WeightKg * 100);

        var code = Random.Shared.Next(100000, 999999).ToString();

        int clientId;
        int? senderEmployeeId = null;
        int currentOfficeId;

        if (role == "Client")
        {
            clientId = userId;
            currentOfficeId = request.CurrentOfficeId;
        }
        else
        {
            if (!request.ClientId.HasValue || request.ClientId == 0)
                return BadRequest("Выберите клиента");

            clientId = request.ClientId.Value;
            senderEmployeeId = userId;

            var employee = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive != false);
            currentOfficeId = employee?.PostOfficeId ?? request.CurrentOfficeId;
        }

        var shipment = new Shipment
        {
            TrackingNumber = Guid.NewGuid()
                .ToString()
                .Replace("-", "")
                .Substring(0, 12)
                .ToUpper(),
            ClientId = clientId,
            SenderEmployeeId = senderEmployeeId,
            CurrentOfficeId = currentOfficeId,
            DestinationOfficeId = request.DestinationOfficeId,
            RecipientName = request.RecipientName,
            DestinationAddress = request.DestinationAddress,
            RecipientPhone = request.RecipientPhone,
            ServiceTypeId = request.ServiceTypeId,
            DeliveryMethodId = request.DeliveryMethodId,
            CurrentStatusId = acceptedStatus.Id,
            WeightKg = request.WeightKg,
            DeclaredValue = request.DeclaredValue,
            TotalPrice = totalPrice,
            AcceptedAt = DateTime.UtcNow,
            Notes = request.Notes,
            ConfirmationCode = code,
            IsConfirmed = false,
            IsDeleted = false
        };

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        // Логируем создание отправления
        await _auditService.LogAsync(
            actionType: "Create",
            tableName: "Shipments",
            recordId: shipment.Id.ToString(),
            newValues: AuditService.SerializeObject(new
            {
                shipment.TrackingNumber,
                request.RecipientName,
                request.RecipientPhone,
                request.WeightKg,
                request.ServiceTypeId,
                request.DeliveryMethodId,
                ClientId = clientId
            })
        );

        return Ok();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateShipmentStatusRequest request)
    {
        var shipment = await _context.Shipments
            .Include(x => x.CurrentStatus)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (shipment == null)
            return NotFound();

        var oldStatusName = shipment.CurrentStatus?.Name;
        var newStatus = await _context.ShipmentStatuses
            .FirstOrDefaultAsync(x => x.Id == request.StatusId);
        var newStatusName = newStatus?.Name;

        shipment.CurrentStatusId = request.StatusId;
        await _context.SaveChangesAsync();

        // Логируем изменение статуса
        await _auditService.LogAsync(
            actionType: "UpdateStatus",
            tableName: "Shipments",
            recordId: id.ToString(),
            oldValues: $"Статус: {oldStatusName}",
            newValues: $"Статус: {newStatusName}"
        );

        return Ok();
    }

    [HttpGet("track/{trackingNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> Track(string trackingNumber)
    {
        var shipment = await _context.Shipments
            .Include(x => x.CurrentStatus)
            .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

        if (shipment == null)
            return NotFound();

        return Ok(new
        {
            shipment.TrackingNumber,
            Status = shipment.CurrentStatus.Name,
            shipment.AcceptedAt
        });
    }
}