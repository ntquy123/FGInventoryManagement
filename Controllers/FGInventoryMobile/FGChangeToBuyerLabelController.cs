using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using entities.Common;
using entities.Setting;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities.Common;
using FGInventoryManagement.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;
using service.Common.Base.Interface;
using service.Interface;

namespace FGInventoryManagement.Controllers.FGInventoryMobile
{
    public class FGChangeToBuyerLabelController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
 
        public FGChangeToBuyerLabelController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
 
        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
 

            var option = (IOptions<AppSettings>)_serviceProvider.GetService(typeof(IOptions<AppSettings>));
            if (option != null)
                _appSettings = option.Value;
        }
        /// <param name="cartonId">MKS1854RGL001202507080004</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(ScanQRtoChangeLabelForCarton))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoChangeLabelForCarton(string cartonId)
        {
            try
            {
                var data = await _service.ScanQRtoChangeLabelForCarton(cartonId);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }
        /// <param name="cartonId">MKS1854RGL001202507080004</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(ScanQRtoChangeLabelForBuyer))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoChangeLabelForBuyer(string cartonId)
        {
            try
            {
                var data = await _service.ScanQRtoChangeLabelForBuyer(cartonId);
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
        ///   "FromCartonId": "MKS1854RGL001202507080004",
        ///   "ToCartonId": "CARTONTEST123",
        ///   "userId": "2021017",
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQRtoLabelChange))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoLabelChange([FromBody] LabelChangeRequest pData)
        {
            try
            {
                var data = await _service.ScanQRtoLabelChange(pData);
                return new HandleState(true, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

    }
}
