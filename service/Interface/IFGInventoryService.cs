using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace service.Interface
{
    public interface IFGInventoryService
    {
        new string PrimaryKey { get; }
        Task<(IReadOnlyList<FgReceiptResultRow> rows, string rtnCode, string rtnMsg)> ScanQrtoSubWHReceipt(ParamScanQR param);
        Task<List<FgRequestRow>> GetHeaderSubWhReceipt(string whCode, string toSubwh);
        Task<List<FgRequestDetailRow>> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh);

        Task<List<UccStockRow>> GetUccByCartonAsync(string cartonId);
        Task<(string rtnCode, string rtnMsg)> ScanQRtoTransferLocation(LocTransferRequest param);

        Task<(string rtnCode, string rtnMsg)> ScanQRtoLabelChange(LabelChangeRequest param);
        Task<List<UccListDetailDto>> ScanQRtoChangeLabelForCarton(string cartonId);
        Task<List<UccListBoxDto>> ScanQRtoChangeLabelForBuyer(string cartonId);
        Task<List<StByrmstTbl>> GetComBoBoxForBuyer();
        Task<List<MtUccListUpload>> SaveBuyerLabelUpload(DataSaveLableUpload Data);
        Task<List<Pcinput>> ExecutePcInputAndQueryAsync(Pcinputrequest req);
        Task<List<Pccount>> GetPcCountAsync(string whCode, string subwhCode);
    }
}
