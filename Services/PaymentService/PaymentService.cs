using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public PaymentService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<GetPrivatePaymentDto>> AddPayment(AddPrivatePaymentDto newPayment)
        {
            var response = new ServiceResponse<GetPrivatePaymentDto>();
            try
            {
                var payment = _mapper.Map<Payment>(newPayment);

                var requestId = newPayment.privateRegReqId;
                var request = await _context.PrivateRegistrationRequests
                    .FirstOrDefaultAsync(r => r.id == requestId);
                if (request is null)
                    throw new Exception($"Request with ID {requestId} not found.");

                if (newPayment.payment != null)
                {
                    payment.payment = new List<PaymentFile>();
                    foreach (var file in newPayment.payment)
                    {
                        var _file = _mapper.Map<PaymentFile>(file);
                        payment.payment.Add(_file);
                    }
                }
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetPrivatePaymentDto>(payment);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<List<GetPrivatePaymentDto>>> DeletePayment(int fileId)
        {
            var response = new ServiceResponse<List<GetPrivatePaymentDto>>();
            try
            {
                var dbPayment = await _context.PaymentFiles.FirstAsync(p => p.id == fileId);
                if (dbPayment is null)
                    throw new Exception($"Payment with ID {fileId} is not found!");
                _context.PaymentFiles.Remove(dbPayment);

                await _context.SaveChangesAsync();

                var _dbPayment = await _context.Payments.Where(p => p.id == dbPayment.id).ToListAsync();
                response.Data = _dbPayment.Select(p => _mapper.Map<GetPrivatePaymentDto>(p)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // public async Task<ServiceResponse<List<GetPaymentDto>>> GetPaymentByGroupReqId(int reqId)
        // {
        //     throw new NotImplementedException();
        // }
        public async Task<ServiceResponse<List<GetPrivatePaymentDto>>> GetPaymentByPrivateReqId(int reqId)
        {
            var response = new ServiceResponse<List<GetPrivatePaymentDto>>();
            try
            {
                var dbPayment = await _context.Payments
                    .Where(p => p.privateRegReqId == reqId)
                    .Include(p => p.payment)
                    .ToListAsync();
                if (!dbPayment.Any())
                    throw new Exception($"Payment with Request ID {reqId} is not found");

                response.Data = dbPayment.Select(p => _mapper.Map<GetPrivatePaymentDto>(p)).ToList();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetPrivatePaymentDto>> UpdatePayment(UpdatePrivatePaymentDto updatedPayment)
        {
            var response = new ServiceResponse<GetPrivatePaymentDto>();
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.payment)
                    .FirstOrDefaultAsync(p => p.privateRegReqId == updatedPayment.privateRegReqId);
                if (payment is null)
                    throw new Exception($"Payment with Request ID {updatedPayment.privateRegReqId} not found.");

                _mapper.Map(updatedPayment, payment);

                payment.paymentType = updatedPayment.paymentType;

                if (updatedPayment.payment != null)
                {
                    var existingFileIds = payment.payment.Select(f => f.id).ToList();
                    var updatedFileIds = updatedPayment.payment.Select(f => f.id).ToList();

                    var filesToRemove = payment.payment.Where(f => !updatedFileIds.Contains(f.id)).ToList();
                    if (filesToRemove != null)
                    {
                        foreach (var file in filesToRemove)
                        {
                            payment.payment.Remove(file);
                        }
                        foreach (var updatedFile in updatedPayment.payment)
                        {
                            var existingFile = payment.payment.FirstOrDefault(f => f.id == updatedFile.id);
                            if (existingFile is null)
                            {
                                existingFile = new PaymentFile();
                                payment.payment.Add(existingFile);
                            }
                            _mapper.Map(updatedFile, existingFile);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetPrivatePaymentDto>(payment);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}