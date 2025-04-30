using CourtRoom.Data;
using CourtRoom.Dtos.CourtOrderDtos;
using CourtRoom.Dtos.PaymentDetailDtos;
using CourtRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourtRoom.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentDetailController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaymentDetails()
    {
        try
        {
            var paymentDetails = await _context.PaymentDetails.Include(pd => pd.CourtOrder).ToListAsync();

            var paymentDetailDtos = paymentDetails.Select(pd => new PaymentDetailDtoWithCourtOrder
            (
                Id: pd.Id,
                CostDeposited: pd.CostDeposited,
                PaymentMode: pd.PaymentMode,
                PaymentRefNo: pd.PaymentRefNo,
                DateOfDeposit: pd.DateOfDeposit,
                ReceiptNo: pd.ReceiptNo,
                DateOfCatBarLibFund: pd.DateOfCatBarLibFund,
                Remarks: pd.Remarks,
                CourtOrder: new CourtOrderDto
                (
                    Id: pd.CourtOrder.Id,
                    CourtNo: pd.CourtOrder.CourtNo,
                    CaseNo: pd.CourtOrder.CaseNo,
                    CauseListSlNo: pd.CourtOrder.CauseListSlNo,
                    Date: pd.CourtOrder.Date,
                    CourtDirection: pd.CourtOrder.CourtDirection,
                    CostImposed: pd.CourtOrder.CostImposed,
                    ImposedOn: pd.CourtOrder.ImposedOn,
                    CashWhereToBeDeposited: pd.CourtOrder.CashWhereToBeDeposited
                )
            ));

            return Ok(new
            {
                success = true,
                message = "Payment details retrieved successfully!",
                data = paymentDetailDtos
            });
        }
        catch (Exception e)
        {
            return BadRequest(new {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentDetails(int id)
    {
        try
        {
            var paymentDetails = await _context.PaymentDetails.Include(p => p.CourtOrder)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paymentDetails == null) return NotFound(new
            {
                success = false,
                message = "Payment details not found!"
            });

            var paymentDetailsDto = new PaymentDetailDtoWithCourtOrder(
                Id: paymentDetails.Id,
                CostDeposited: paymentDetails.CostDeposited,
                PaymentMode: paymentDetails.PaymentMode,
                PaymentRefNo: paymentDetails.PaymentRefNo,
                DateOfDeposit: paymentDetails.DateOfDeposit,
                ReceiptNo: paymentDetails.ReceiptNo,
                DateOfCatBarLibFund: paymentDetails.DateOfCatBarLibFund,
                Remarks: paymentDetails.Remarks,
                CourtOrder: new CourtOrderDto(
                    Id: paymentDetails.CourtOrder.Id,
                    CourtNo: paymentDetails.CourtOrder.CourtNo,
                    CaseNo: paymentDetails.CourtOrder.CaseNo,
                    CauseListSlNo: paymentDetails.CourtOrder.CauseListSlNo,
                    Date: paymentDetails.CourtOrder.Date,
                    CourtDirection: paymentDetails.CourtOrder.CourtDirection,
                    CostImposed: paymentDetails.CourtOrder.CostImposed,
                    ImposedOn: paymentDetails.CourtOrder.ImposedOn,
                    CashWhereToBeDeposited: paymentDetails.CourtOrder.CashWhereToBeDeposited
                )
            );

            return Ok(new
            {
                success = true,
                message = "Payment details retrieved successfully!",
                data = paymentDetailsDto
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Cashier")]
    public async Task<IActionResult> CreatePaymentDetail(AddPaymentDetailDto dto)
    {
        try
        {
            PaymentDetail paymentDetail = new()
            {
                CostDeposited = dto.CostDeposited,
                PaymentMode = dto.PaymentMode,
                PaymentRefNo = dto.PaymentRefNo,
                DateOfDeposit = dto.DateOfDeposit,
                ReceiptNo = dto.ReceiptNo,
                DateOfCatBarLibFund = dto.DateOfCatBarLibFund,
                Remarks = dto.Remarks,
                CourtOrderId = dto.CourtOrderId
            };


            paymentDetail.DateOfDeposit = DateTime.SpecifyKind(paymentDetail.DateOfDeposit, DateTimeKind.Utc);
            paymentDetail.DateOfCatBarLibFund =
                DateTime.SpecifyKind(paymentDetail.DateOfCatBarLibFund, DateTimeKind.Utc);

            _context.PaymentDetails.Add(paymentDetail);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Payment detail created successfully!",
                data = paymentDetail
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePaymentDetail(int id)
    {
        try
        {
            var paymentDetail = await _context.PaymentDetails.FindAsync(id);
            if (paymentDetail == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Payment detail not found!"
                });
            }
            
            _context.PaymentDetails.Remove(paymentDetail);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Payment detail deleted successfully!",
                data = paymentDetail
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePaymentDetail(int id, UpdatePaymentDetailDto dto)
    {
        try
        {
            var paymentDetails = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.Id == id);

            if (paymentDetails == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Payment detail not found!"
                });
            }
        
            paymentDetails.CostDeposited = dto.CostDeposited;
            paymentDetails.PaymentMode = dto.PaymentMode;
            paymentDetails.PaymentRefNo = dto.PaymentRefNo;
            paymentDetails.DateOfDeposit = DateTime.SpecifyKind(dto.DateOfDeposit, DateTimeKind.Utc);
            paymentDetails.ReceiptNo = dto.ReceiptNo;
            paymentDetails.DateOfCatBarLibFund = DateTime.SpecifyKind(dto.DateOfCatBarLibFund, DateTimeKind.Utc);
            paymentDetails.Remarks = dto.Remarks;
        
            await _context.SaveChangesAsync();
        
            return Ok(new
            {
                success = true,
                message = "Payment detail updated successfully!",
                data = paymentDetails
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }
}