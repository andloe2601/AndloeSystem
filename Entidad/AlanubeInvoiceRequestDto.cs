using System;
using System.Collections.Generic;

namespace Andloe.Entidad
{
    public sealed class AlanubeInvoiceRequestDto
    {
        public string DocumentType { get; set; } = "";
        public string? ENcf { get; set; }
        public DateTime IssueDate { get; set; }

        public AlanubePartyDto Issuer { get; set; } = new();
        public AlanubePartyDto Buyer { get; set; } = new();

        public string? IncomeType { get; set; }
        public string? PaymentType { get; set; }
        public string? Notes { get; set; }

        public decimal TaxedAmount { get; set; }
        public decimal ExemptAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<AlanubeInvoiceItemDto> Items { get; set; } = new();
    }

    public sealed class AlanubePartyDto
    {
        public string? TaxId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public sealed class AlanubeInvoiceItemDto
    {
        public int LineNumber { get; set; }
        public string? Code { get; set; }
        public string Description { get; set; } = "";
        public string? UnitCode { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxedAmount { get; set; }
        public decimal ExemptAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}