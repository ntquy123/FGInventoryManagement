namespace FGInventoryManagement.Controllers.FGInventoryMobile
{
    public class FGInventoryController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
        private readonly IPkTbMasDeviceService _deviceService;
        public FGInventoryController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
        IPkTbMasDeviceService deviceService,
        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _deviceService = deviceService;

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
                return new HandleState(false, ex.Message);
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
                return new HandleState(false, ex.Message);
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
                return new HandleState(true, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
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
                return new HandleState(false, ex.Message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQRtoTransferLocation))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoTransferLocation([FromBody] LocTransferRequest pData)
        {
            try
            {
                var data = await _service.ScanQRtoTransferLocation(pData);
                return new HandleState(true, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

    }
}
