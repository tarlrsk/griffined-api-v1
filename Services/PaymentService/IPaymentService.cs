using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ServiceResponse<List<GetPrivatePaymentDto>>> GetPaymentByPrivateReqId(int reqId);
        // Task<ServiceResponse<List<GetPaymentDto>>> GetPaymentByGroupReqId(int reqId);
        Task<ServiceResponse<GetPrivatePaymentDto>> AddPayment(AddPrivatePaymentDto newPayment);
        Task<ServiceResponse<List<GetPrivatePaymentDto>>> DeletePayment(int id);
        Task<ServiceResponse<GetPrivatePaymentDto>> UpdatePayment(UpdatePrivatePaymentDto updatedPayment);
    }
}