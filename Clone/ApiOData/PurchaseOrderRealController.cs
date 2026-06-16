//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System.Globalization;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;

//namespace Indotalent.Controllers
//{
//    [Route("[controller]")]
//    public class PurchaseOrderRealController : Controller
//    {
//        private readonly IWebHostEnvironment _webHostEnvironment;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private static List<PurchaseOrderImportDto> _tempImportData = new();

//        public PurchaseOrderRealController(
//            IWebHostEnvironment webHostEnvironment,
//            IHttpClientFactory httpClientFactory)
//        {
//            _webHostEnvironment = webHostEnvironment;
//            _httpClientFactory = httpClientFactory;
//        }

//        [HttpPost("Save")]
//        public async Task<IActionResult> Save(IFormFile UploadFiles)
//        {
//            try
//            {
//                Console.WriteLine("Save method called");

//                if (UploadFiles != null && UploadFiles.Length > 0)
//                {
//                    Console.WriteLine($"File received: {UploadFiles.FileName}, Size: {UploadFiles.Length}");

//                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
//                    if (!Directory.Exists(uploadsFolder))
//                    {
//                        Directory.CreateDirectory(uploadsFolder);
//                    }

//                    var filePath = Path.Combine(uploadsFolder, UploadFiles.FileName);

//                    using (var stream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await UploadFiles.CopyToAsync(stream);
//                    }

//                    Console.WriteLine($"File saved to: {filePath}");

//                    _tempImportData = ReadExcelFile(filePath);

//                    Console.WriteLine($"Read {_tempImportData.Count} rows from Excel");

//                    return Json(new { success = true, message = $"Loaded {_tempImportData.Count} rows" });
//                }

//                Console.WriteLine("No file received");
//                return Json(new { success = false, message = "No file uploaded" });
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error in Save: {ex.Message}");
//                Console.WriteLine($"Stack trace: {ex.StackTrace}");
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet("GetData")]
//        public IActionResult GetData()
//        {
//            Console.WriteLine($"GetData called, returning {_tempImportData.Count} rows");
//            return Json(_tempImportData);
//        }

//        [HttpPost("Remove")]
//        public IActionResult Remove()
//        {
//            _tempImportData.Clear();
//            return Json(new { success = true });
//        }

//        [HttpPost("UpdateData")]
//        public async Task<IActionResult> UpdateData()
//        {
//            try
//            {
//                if (_tempImportData.Any())
//                {
//                    var result = await CreatePurchaseOrdersInBC(_tempImportData);
//                    _tempImportData.Clear();

//                    if (result.success)
//                    {
//                        return Json(new { success = true, message = $"Successfully created {result.count} purchase orders" });
//                    }
//                    else
//                    {
//                        return Json(new { success = false, message = result.message });
//                    }
//                }
//                return Json(new { success = false, message = "No data to import" });
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error in UpdateData: {ex.Message}");
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        private List<PurchaseOrderImportDto> ReadExcelFile(string filePath)
//        {
//            var orders = new List<PurchaseOrderImportDto>();
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            using (var package = new ExcelPackage(new FileInfo(filePath)))
//            {
//                var worksheet = package.Workbook.Worksheets[0];
//                var rowCount = worksheet.Dimension.Rows;

//                Console.WriteLine($"Excel has {rowCount} rows");

//                for (int row = 2; row <= rowCount; row++)
//                {
//                    try
//                    {
//                        var vendorNo = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? "";
//                        if (string.IsNullOrEmpty(vendorNo))
//                            continue;

//                        var order = new PurchaseOrderImportDto
//                        {
//                            VendorNo = vendorNo,
//                            LocationCode = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "HCM",
//                            DocumentDate = worksheet.Cells[row, 3].Value != null
//                                ? DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString(), out DateTime parsed)
//                                    ? parsed.ToString("yyyy-MM-dd")
//                                    : DateTime.Now.ToString("yyyy-MM-dd")
//                                : DateTime.Now.ToString("yyyy-MM-dd"),
//                            ItemNo = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? "",
//                            Quantity = decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var qty) ? qty : 0,
//                        };

//                        Console.WriteLine($"Row {row}: Vendor={order.VendorNo}, Item={order.ItemNo}, Qty={order.Quantity}");
//                        orders.Add(order);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error reading row {row}: {ex.Message}");
//                    }
//                }
//            }

//            return orders;
//        }

//        public async Task<(bool success, int count, string message)> CreatePurchaseOrdersInBC(List<PurchaseOrderImportDto> orders)
//        {
//            var groupedOrders = orders
//                .GroupBy(o => new { o.VendorNo, o.LocationCode, o.DocumentDate })
//                .ToList();

//            Console.WriteLine($"Creating {groupedOrders.Count} purchase orders from {orders.Count} lines");

//            string soapEnvelope = @"<Envelope xmlns=""http://schemas.xmlsoap.org/soap/envelope/"">
//  <Body>
//    <CreateMultiple xmlns=""urn:microsoft-dynamics-schemas/page/purchaseorderapi"">
//      <PurchaseOrderAPI_List>";

//            foreach (var group in groupedOrders)
//            {
//                var firstOrder = group.First();
//                string vendorNo = System.Security.SecurityElement.Escape(firstOrder.VendorNo ?? "");
//                string location = System.Security.SecurityElement.Escape(firstOrder.LocationCode ?? "");
//                string docDate = firstOrder.DocumentDate;

//                Console.WriteLine($"Building PO for Vendor: {vendorNo}, Location: {location}, Date: {docDate}");

//                soapEnvelope += $@"
//        <PurchaseOrderAPI>
//          <Buy_from_Vendor_No>{vendorNo}</Buy_from_Vendor_No>
//          <Document_Date>{docDate}</Document_Date>
//          <Posting_Date>{docDate}</Posting_Date>
//          <Location_Code>{location}</Location_Code>
//          <PurchLines>";

//                foreach (var order in group)
//                {
//                    string itemNo = System.Security.SecurityElement.Escape(order.ItemNo ?? "");
//                    string qty = order.Quantity.ToString("0.##", CultureInfo.InvariantCulture);

//                    Console.WriteLine($"  Adding line: Item={itemNo}, Qty={qty}");

//                    soapEnvelope += $@"
//            <Purchase_Order_Line>
//              <Type>Item</Type>
//              <No>{itemNo}</No>
//              <Quantity>{qty}</Quantity>
//            </Purchase_Order_Line>";
//                }

//                soapEnvelope += @"
//          </PurchLines>
//        </PurchaseOrderAPI>";
//            }

//            soapEnvelope += @"
//      </PurchaseOrderAPI_List>
//    </CreateMultiple>
//  </Body>
//</Envelope>";

//            Console.WriteLine("===== SOAP REQUEST =====");
//            Console.WriteLine(soapEnvelope);
//            Console.WriteLine("========================");

//            using (var httpClient = new HttpClient())
//            {
//                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
//                content.Headers.Clear();
//                content.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/page/purchaseorderapi:CreateMultiple");
//                content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

//                string username = "tkv01";
//                string password = "Taka@2024";
//                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
//                httpClient.DefaultRequestHeaders.Authorization =
//                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

//                string serviceUrl = "http://192.168.80.59:7247/TKV/WS/TKV/Page/PurchaseOrderAPI";

//                try
//                {
//                    var response = await httpClient.PostAsync(serviceUrl, content);
//                    string responseContent = await response.Content.ReadAsStringAsync();

//                    Console.WriteLine("===== SOAP RESPONSE =====");
//                    Console.WriteLine(responseContent);
//                    Console.WriteLine("=========================");

//                    if (response.IsSuccessStatusCode)
//                    {
//                        if (responseContent.Contains("<PurchLines/>") || responseContent.Contains("<PurchLines />"))
//                        {
//                            return (false, 0, "Purchase Orders created but without lines. Check item numbers and permissions.");
//                        }

//                        return (true, groupedOrders.Count, $"Successfully created {groupedOrders.Count} purchase orders");
//                    }
//                    else
//                    {
//                        return (false, 0, $"API Error: {response.StatusCode} - {responseContent}");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Exception calling BC API: {ex.Message}");
//                    return (false, 0, $"Exception: {ex.Message}");
//                }
//            }
//        }
//    }

//    public class PurchaseOrderImportDto
//    {
//        public string VendorNo { get; set; }
//        public string LocationCode { get; set; }
//        public string DocumentDate { get; set; }
//        public string ItemNo { get; set; }
//        public decimal Quantity { get; set; }
//    }
//}