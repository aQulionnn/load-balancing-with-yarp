using System.Globalization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using webapi.Dtos;

namespace webapi.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    [HttpGet]
    public IActionResult DownloadOrderReceipt()
    {
        var pdfStream = GeneratePdf(new OrderDto());
        Response.Headers.Add("Content-Disposition", "attachment; filename=OrderReceipt.pdf");
        pdfStream.Position = 0;
        
        return File(pdfStream, "application/pdf");
    }
    
    private MemoryStream GeneratePdf(OrderDto order)
    {
        Settings.License = LicenseType.Community;
        var pdfStream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.PageColor(Colors.White);
                
                page.Content()
                    .PaddingHorizontal(20)
                    .PaddingVertical(20)
                    .DefaultTextStyle(x => x.FontSize(14))
                    .Column(main =>
                    {
                        BuildDetails(main, order);
                    });
            });
        }).GeneratePdf(pdfStream);
        
        return pdfStream;
    }

    private void BuildDetails(ColumnDescriptor descriptor, OrderDto order)
    {
        descriptor.Item()
            .Container()
            .Text("Receipt Details")
            .Bold();
        
        descriptor.Item().Container().Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(60);
                c.RelativeColumn(3);
                c.RelativeColumn(3);
                c.RelativeColumn(3);
            });

            table.Cell().Element(c =>
            {
                c.Container().Text(order.Id.ToString());
            });
            
            table.Cell().Element(c =>
            {
                c.Container().Text(order.CustomerName);
            });
            
            table.Cell().Element(c =>
            {
                c.Container().Text(order.OrderDate.ToString("dd/MM/yyyy"));
            });
            
            table.Cell().Element(c =>
            {
                c.Container().Text(order.Sum.ToString(CultureInfo.CurrentCulture));
            });
        });
    }
}