using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.FGInventoryMobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Common.Base.Interface;
namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class PhysicalCountingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        public IServiceProvider _serviceProvider;
 
        public PhysicalCountingController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetPcCountAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetPcCountAsync(string whCode, string subwhCode)
        {
            try
            {
                var data = await _service.GetPcCountAsync(whCode, subwhCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }
        /// <param name="req">
        /// Data input for QR code scanning
        /// Example:
        /// ```json
        /// {
        ///   "whCode": "920",
        ///   "subwhCode": "PGW2",
        ///   "pcName": "920PGW1_TEST", 
        ///   "locCode": "A01-0002",
        ///    "trAction": 1, // Note: 1 (Input), -1 (Cancel)
        ///   "cartonId": "MKS1854RGL001202507080004",
        ///   "userId": "2021017",
        /// }
        /// ```
        /// </param>

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ExecutePcInputAndQueryAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ExecutePcInputAndQueryAsync([FromBody] Pcinputrequest req)
        {
            try
            {
                var data = await _service.ExecutePcInputAndQueryAsync(req);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

    }
}
