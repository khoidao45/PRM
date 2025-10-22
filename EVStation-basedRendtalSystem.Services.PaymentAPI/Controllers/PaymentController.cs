using EVStation_basedRentalSystem.Services.PaymentAPI.Data;
using EVStation_basedRentalSystem.Services.PaymentAPI.Models;
using EVStation_basedRendtalSystem.Services.PaymentAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly PayOSService _payOSService;
    private readonly IBookingService _bookingService;

    public PaymentController(PaymentDbContext context, PayOSService payOSService, IBookingService bookingService)
    {
        _context = context;
        _payOSService = payOSService;
        _bookingService = bookingService;
    }

    // 1️⃣ Create a new payment
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
        if (booking == null)
            return NotFound("Booking not found");

        var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == booking.Id);
        if (existingPayment != null)
            return BadRequest("Payment already created for this booking");

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = booking.TotalPrice,
            Status = "Pending"
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Generate PayOS payment link
        var (checkoutUrl, qrCode, orderCode) = await _payOSService.GeneratePaymentQR(payment);
        payment.OrderCode = orderCode;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.Status,
            CheckoutUrl = checkoutUrl,
            QrCode = qrCode,
            OrderCode = orderCode
        });
    }

    [HttpPost("sync/{bookingId}")]
    public async Task<IActionResult> SyncPaymentStatus(int bookingId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
        if (payment == null) return NotFound("Payment not found");

        // Lấy trạng thái thanh toán từ PayOS
        var info = await _payOSService.GetPaymentLinkInformation(payment.OrderCode);

        if (info.status == "PAID" && payment.Status != "Success")
        {
            payment.Status = "Success";
            payment.TransactionId = info.transactions.FirstOrDefault()?.reference ?? "";
            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 1. Update booking
            var bookingUpdated = await _bookingService.UpdateBookingStatusAsync(payment.BookingId, "Confirmed");

            // 2. Tạo hợp đồng (HopDong)
           
        }

        return Ok(new
        {
            payment.BookingId,
            payment.Status,
            payment.TransactionId,
            payment.UpdatedAt,
            infoStatus = info.status
        });
    }


    // 3️⃣ Check local payment status
    [HttpGet("{bookingId}/status")]
    public async Task<IActionResult> GetPaymentStatus(int bookingId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
        if (payment == null) return NotFound();

        return Ok(new
        {
            payment.BookingId,
            payment.Status,
            payment.TransactionId,
            payment.UpdatedAt
        });
    }

}

// Request DTO
public class PaymentRequest
{
    public int BookingId { get; set; }
}
