using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Application.Features.Installments.Commands;
using BudgetTracker.Application.Features.Installments.DTOs;
using BudgetTracker.Application.Features.Installments.Queries;

namespace BudgetTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InstallmentsController : ControllerBase
{
    private readonly ISender _sender;

    public InstallmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InstallmentPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InstallmentPlanDto>>> GetPlans(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInstallmentPlansQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InstallmentPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstallmentPlanDto>> GetPlanById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInstallmentPlanByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(InstallmentPlanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InstallmentPlanDto>> CreatePlan(
        [FromBody] CreateInstallmentPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateInstallmentPlanCommand(
                request.CardId, request.Name, request.Category,
                request.TotalAmount, request.MonthlyPayment, request.NumberOfMonths, request.StartDate),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{planId:guid}/payments/{paymentId:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkPaymentPaid(Guid planId, Guid paymentId, CancellationToken cancellationToken)
    {
        await _sender.Send(new MarkPaymentPaidCommand(planId, paymentId), cancellationToken);
        return NoContent();
    }
}
