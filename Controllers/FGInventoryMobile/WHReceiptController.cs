using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class WHReceiptController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;
        public WHReceiptController(
            IFGInventoryService service,
            IServiceProvider serviceProvider,
            AmtContext context,
             IApiExecutionLockService ApiExcLockService,
            ICurrentUser currentUser)
            : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _ApiExcLockService = ApiExcLockService;
        }

        /// <param name="whCode">920</param>
        /// <param name="subwhCode">PGW2</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetReceiptHeadersAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetReceiptHeadersAsync(string whCode, string subwhCode)
        {
            try
            {
                var data = await _service.GetReceiptHeadersAsync(whCode, subwhCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="invoiceNo">20251107-00001</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetReceiptLinesAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetReceiptLinesAsync(string invoiceNo)
        {
            try
            {
                var data = await _service.GetReceiptLinesAsync(invoiceNo);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="request">
        /// Data input for WH receipt Scan
        /// Example:
        /// ```json
        /// {
        ///   "WhCode": "920",
        ///   "SubwhCode": "PGW2",
        ///   "LocCode": "A01-0001",
        ///   "InvNo": "20251107-00001",
        ///   "CartonId": "MKS1927RGL003001",
        ///   "UserId": "2021017"
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanReceiptAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ScanReceiptAsync([FromBody] WHReceiptScanRequest request)
        {
            try
            {
                var result = await _service.ScanReceiptAsync(request);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);

                if (!isSuccess)
                {
                    var message = await LogErrorAsync(new Exception(result.rtnMsg ?? "Receipt error"), "Receipt", request);
                    result.rtnMsg = message;
                }

                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Receipt");
                return new HandleState(false, message);
            }
        }

        private async Task<string> LogErrorAsync(Exception ex, string menuName, object vm = null)
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            string jsonData = JsonSerializer.Serialize(vm);
            var modelAdd = new ApiLogs
            {
                Method = HttpContext.Request.Method,
                ApiName = currentUrl,
                RequestJson = jsonData,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            string mess = "Error ID:" + log.LogId + ": " + ex.Message;
            return mess;
        }
    }
}
