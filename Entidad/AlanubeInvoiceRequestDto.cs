using System;
using System.Collections.Generic;

namespace Andloe.Data.Fiscal
{
    public sealed class AlanubeInvoiceRequestDto
    {
        public AlanubeIdDocDto? IdDoc { get; set; }
        public AlanubePartyDto? Sender { get; set; }
        public AlanubePartyDto? Buyer { get; set; }
        public AlanubeTotalsDto? Totals { get; set; }
        public List<AlanubeItemDetailDto> ItemDetails { get; set; } = new();
    }

    public sealed class AlanubeIdDocDto
    {
        public string? Encf { get; set; }
        public int DocumentType { get; set; }
        public int IncomeType { get; set; }
        public int PaymentType { get; set; }
        public string? SequenceDueDate { get; set; }


        public string? PaymentDeadline { get; set; }

        public List<AlanubePaymentFormDto> PaymentFormsTable { get; set; } = new();
    }

    public sealed class AlanubePaymentFormDto
    {
        public int PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public sealed class AlanubePartyDto
    {
        public string? Rnc { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? StampDate { get; set; }
    }

    public sealed class AlanubeTotalsDto
    {
        public decimal TotalTaxedAmount { get; set; }
        public decimal TotalExemptAmount { get; set; }
        public decimal TotalItbis { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ItbisS1 { get; set; }
        public decimal Itbis1Total { get; set; }
    }

    public sealed class AlanubeItemDetailDto
    {
        public int LineNumber { get; set; }
        public int BillingIndicator { get; set; }
        public string? ItemName { get; set; }
        public int GoodServiceIndicator { get; set; }
        public string? ItemDescription { get; set; }
        public decimal QuantityItem { get; set; }
        public int UnitMeasure { get; set; }
        public decimal UnitPriceItem { get; set; }
        public decimal ItemAmount { get; set; }
    }

    public sealed class AlanubeEmitResponseDto
    {
        public string? Id { get; set; }
        public string? TrackId { get; set; }
        public string? Status { get; set; }
        public string? LegalStatus { get; set; }
        public string? Message { get; set; }
        public string? RawJson { get; set; }

        public string? GetTrackOrId()
        {
            return !string.IsNullOrWhiteSpace(TrackId) ? TrackId : Id;
        }
    }

    public sealed class AlanubeStatusResponseDto
    {
        public string? Id { get; set; }
        public string? TrackId { get; set; }
        public string? Status { get; set; }
        public string? LegalStatus { get; set; }
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? RawJson { get; set; }
    }

    public sealed class AlanubeConfigDto
    {
        public string BaseUrl { get; set; } = "";
        public string Token { get; set; } = "";
        public string Ambiente { get; set; } = "sandbox";
        public int TimeoutSegundos { get; set; } = 60;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(BaseUrl) &&
            !string.IsNullOrWhiteSpace(Token);
    }
}