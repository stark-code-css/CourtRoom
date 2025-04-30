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
public class CourtOrderController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // Return all court orders with their respective payment details
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCourtOrders()
    {
        try
        {
            var courtOrders = await _context.CourtOrders.Include(co => co.PaymentDetail).ToListAsync();

            var courtOrderDtos = courtOrders.Select(co => new CourtOrderDtoWithPaymentDetails
            (
                Id: co.Id,
                CourtNo: co.CourtNo,
                CaseNo: co.CaseNo,
                CauseListSlNo: co.CauseListSlNo,
                Date: co.Date,
                CourtDirection: co.CourtDirection,
                CostImposed: co.CostImposed,
                ImposedOn: co.ImposedOn,
                CashWhereToBeDeposited: co.CashWhereToBeDeposited,
                PaymentDetail: co.PaymentDetail != null
                    ? new PaymentDetailDto
                    (
                        Id: co.PaymentDetail.Id,
                        CostDeposited: co.PaymentDetail.CostDeposited,
                        PaymentMode: co.PaymentDetail.PaymentMode,
                        PaymentRefNo: co.PaymentDetail.PaymentRefNo,
                        DateOfDeposit: co.PaymentDetail.DateOfDeposit,
                        ReceiptNo: co.PaymentDetail.ReceiptNo,
                        DateOfCatBarLibFund: co.PaymentDetail.DateOfCatBarLibFund,
                        Remarks: co.PaymentDetail.Remarks
                    )
                    : null
            ));

            return Ok(new
            {
                success = true,
                message = "Court Orders retrieved successfully.",
                data = courtOrderDtos
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


    // Return the court order with respective payment details of provided id 
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetCourtOrder(int id)
    {
        try
        {
            var courtOrder = await _context.CourtOrders.Include(c => c.PaymentDetail)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (courtOrder == null) return NotFound(new
            {
                success = false,
                message = "Court Order not found."
            });

            var courtOrderDtos = new CourtOrderDtoWithPaymentDetails
            (
                Id: courtOrder.Id,
                CourtNo: courtOrder.CourtNo,
                CaseNo: courtOrder.CaseNo,
                CauseListSlNo: courtOrder.CauseListSlNo,
                Date: courtOrder.Date,
                CourtDirection: courtOrder.CourtDirection,
                CostImposed: courtOrder.CostImposed,
                ImposedOn: courtOrder.ImposedOn,
                CashWhereToBeDeposited: courtOrder.CashWhereToBeDeposited,
                PaymentDetail: courtOrder.PaymentDetail != null
                    ? new PaymentDetailDto
                    (
                        Id: courtOrder.PaymentDetail.Id,
                        CostDeposited: courtOrder.PaymentDetail.CostDeposited,
                        PaymentMode: courtOrder.PaymentDetail.PaymentMode,
                        PaymentRefNo: courtOrder.PaymentDetail.PaymentRefNo,
                        DateOfDeposit: courtOrder.PaymentDetail.DateOfDeposit,
                        ReceiptNo: courtOrder.PaymentDetail.ReceiptNo,
                        DateOfCatBarLibFund: courtOrder.PaymentDetail.DateOfCatBarLibFund,
                        Remarks: courtOrder.PaymentDetail.Remarks
                    )
                    : null
            );
            return Ok(new
            {
                success = true,
                message = "Court Orders retrieved successfully.",
                data = courtOrderDtos
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

    // Creates new court order
    [HttpPost]
    [Authorize(Roles = "Admin, CourtMaster")]
    public async Task<IActionResult> CreateCourtOrder(AddCourtOrderDto dto)
    {
        try
        {
            CourtOrder courtOrder = new()
            {
                CourtNo = dto.CourtNo,
                CaseNo = dto.CaseNo,
                CauseListSlNo = dto.CauseListSlNo,
                Date = dto.Date,
                CourtDirection = dto.CourtDirection,
                CostImposed = dto.CostImposed,
                ImposedOn = dto.ImposedOn,
                CashWhereToBeDeposited = dto.CashWhereToBeDeposited,
            };

            courtOrder.Date = DateTime.SpecifyKind(courtOrder.Date, DateTimeKind.Utc);

            _context.CourtOrders.Add(courtOrder);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Court Order created Successfully!",
                data = courtOrder
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
    
    // Delete court order
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourtOrder(int id)
    {
        try
        {
            var courtOrder = await _context.CourtOrders.FirstOrDefaultAsync(c => c.Id == id);
            if (courtOrder == null) return NotFound();
            _context.CourtOrders.Remove(courtOrder);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Court order deleted successfully.",
                data = courtOrder
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCourtOrder(int id, UpdateCourtOrderDto dto)
    {
        try
        {
            var courtOrder = await _context.CourtOrders.FirstOrDefaultAsync(c => c.Id == id);
            if (courtOrder == null) 
            {
                return NotFound(new
                {
                    success = false,
                    message = "Court Order not found."
                });
            }
            
            courtOrder.CourtNo = dto.CourtNo;
            courtOrder.CaseNo = dto.CaseNo;
            courtOrder.CauseListSlNo = dto.CauseListSlNo;
            courtOrder.Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            courtOrder.CourtDirection = dto.CourtDirection;
            courtOrder.CostImposed = dto.CostImposed;
            courtOrder.ImposedOn = dto.ImposedOn;
            courtOrder.CashWhereToBeDeposited = dto.CashWhereToBeDeposited;

            _context.CourtOrders.Update(courtOrder);
            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                success = true,
                message = "Court Order updated successfully.",
                data = courtOrder
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