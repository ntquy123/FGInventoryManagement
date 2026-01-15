using System;
using AutoMapper;
using Oracle.ManagedDataAccess.Client;
using entities.Common;
using entities.Setting;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.api.Attribute;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Collections.Generic;
using System.Data;
namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class FGInventoryController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;

        public FGInventoryController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
        IApiExecutionLockService ApiExcLockService,

        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _ApiExcLockService = ApiExcLockService;


            var option = (IOptions<AppSettings>)_serviceProvider.GetService(typeof(IOptions<AppSettings>));
            if (option != null)
                _appSettings = option.Value;
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetHeaderSubWhReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> GetHeaderSubWhReceipt(string whCode, string toSubwh)
        {
            try
            {
                var data = await _service.GetHeaderSubWhReceipt(whCode, toSubwh);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt");
                return new HandleState(false, message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetDetailSubWhReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh)
        {
            try
            {
                var data = await _service.GetDetailSubWhReceipt(reqNo, whCode, toSubwh);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt");
                return new HandleState(false, message);
            }
        }
        /// <param name="pData">
        /// Data input for QR code scanning
        /// Example:
        /// ```json
        /// {
        ///   "whCode": "920",
        ///   "subwhCode": "PGW2",
        ///   "locCode": "A01-0001", 
        ///   "referInfo": "I-GA2-250730-0003",
        ///   "cartonId": "MKS1854RGL001202507080004",
        ///   "userId": "2021017",
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQrtoSubWHReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQrtoSubWHReceipt([FromBody] ParamScanQR pData)
        {
            try
            {
                var data = await _service.ScanQrtoSubWHReceipt(pData);
                var isSuccess = !string.Equals(data.rtnCode, "E", StringComparison.OrdinalIgnoreCase);
                return new HandleState(isSuccess, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt", pData);
                return new HandleState(false, message);
            }
        }
        /// <param name="cartonId">MKS1854RGL001202507080004</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetUccByCartonAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetUccByCartonAsync(string cartonId)
        {
            try
            {
                var data = await _service.GetUccByCartonAsync(cartonId);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Location", new { cartonId });
                return new HandleState(false, message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQRtoTransferLocation))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoTransferLocation([FromBody] LocTransferRequest pData)
        {
            try
            {
                var result = await _service.ScanQRtoTransferLocation(pData);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);

                if (!isSuccess)
                {
                    var message = await LogErrorAsync(new Exception(result.rtnMsg ?? "Transfer Location error"), "Transfer Location", pData);
                    result.rtnMsg = message;
                }

                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Location");
                return new HandleState(false, message);
            }
        }

        /// <summary>
        /// Check whether a location code exists for the given warehouse and sub warehouse.
        /// </summary>
        /// <param name="locCode">A01-0001</param>
        /// <param name="subwhCode">PGW2</param>
        /// <param name="whCode">920</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(CheckRackExistsAsync))]
        [AllowAnonymous]
        public async Task<HandleState> CheckRackExistsAsync(string locCode, string subwhCode, string whCode)
        {
            try
            {
                const string sql = @"SELECT SLT.LOC_CODE
                                  FROM ST_SUBWH_TBL SST
                                  JOIN ST_LOCATION_TBL SLT ON SST.SUBWH_CODE = SLT.SUBWH_CODE
                                  WHERE SST.USED_FLAG = 'Y'
                                    AND SLT.USED_FLAG = 'Y'
                                    AND SLT.LOC_TYPE IN ('C','A')
                                    AND SST.WH_CODE = :whCode
                                    AND SST.SUBWH_CODE = :subwhCode
                                    AND SLT.LOC_CODE = :locCode";

                var parameters = new List<OracleParameter>
                {
                    new OracleParameter("whCode", OracleDbType.Varchar2, whCode, ParameterDirection.Input),
                    new OracleParameter("subwhCode", OracleDbType.Varchar2, subwhCode, ParameterDirection.Input),
                    new OracleParameter("locCode", OracleDbType.Varchar2, locCode, ParameterDirection.Input),
                };

                var result = _context.ExcuteDataSet(sql, CommandType.Text, parameters);
                var exists = result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;

                if (exists)
                {
                    return new HandleState(true, locCode, locCode);
                }

                return new HandleState(false, "Rack not exits");
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Check Rack", new { locCode, subwhCode, whCode });
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
            return "Error ID:" + log.LogId + ": " + ex.Message;
        }
    }
}
