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
    public class ExFactoryPickingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;
        public ExFactoryPickingController(
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
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetExFactoryPickingHeadersAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetExFactoryPickingHeadersAsync(string whCode)
        {
            try
            {
                var data = await _service.GetExFactoryPickingHeadersAsync(whCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="shpPkg">MKS-25W47-0001</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetExFactoryPickingLinesAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetExFactoryPickingLinesAsync(string shpPkg)
        {
            try
            {
                var data = await _service.GetExFactoryPickingLinesAsync(shpPkg);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="request">
        /// Example:
        /// ```json
        /// {
        ///   "WhCode": "920",
        ///   "SubwhCode": "PGW2",
        ///   "LocCode": "A01-0001",
        ///   "TrAction": -1,
        ///   "TrInfo": "MKS-25W47-0001",
        ///   "CartonId": "MKS1854RGL0012511240001",
        ///   "ContainerNo": "CTN001004",
        ///   "UserId": "2021017"
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanExFactoryPickingAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ScanExFactoryPickingAsync([FromBody] ExFactoryPickingScanRequest request)
        {
            try
            {
                var result = await _service.ScanExFactoryPickingAsync(request);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);

                if (!isSuccess)
                {
                    var message = await LogErrorAsync(new Exception(result.rtnMsg ?? "Ex-Factory picking error"), "ExFactoryPicking", request);
                    result.rtnMsg = message;
                }

                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "ExFactoryPicking");
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
