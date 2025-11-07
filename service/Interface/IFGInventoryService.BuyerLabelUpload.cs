using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using service.Common.Base.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<StByrmstTblView>> GetComBoBoxForBuyer();
        Task<List<MtUccListUpload>> SaveBuyerLabelUpload(DataSaveLableUpload Data);
        Task<string> GetScanIdAsync(bool isExcel, DateTime? date = null, string module = "UCC_UPLOAD", string separator = "@", int width = 6);
    }
}
