using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using griffined_api.Dtos.RegistrationRequestDto;

namespace griffined_api.Services.RegistrationRequestService
{
    public interface IRegistrationRequestService
    {
        Task<ServiceResponse<string>> AddNewRequestedCourses(NewCoursesRequestDto newCourses);
        Task<ServiceResponse<string>> AddStudentAddingRequest(StudentAddingRequestDto newRequest, List<IFormFile> filesToUpload);
        Task<ServiceResponse<List<RegistrationRequestResponseDto>>> ListRegistrationRequests();
        Task<ServiceResponse<RegistrationRequestPendingEADetailResponseDto>> GetPendingEADetail(int requestId);
        Task<ServiceResponse<string>> DeclineSchedule(int requestId);
        Task<ServiceResponse<RegistrationRequestPendingEADetail2ResponseDto>> GetPendingEADetail2(int requestId);
        Task<ServiceResponse<RegistrationRequestPendingECResponseDto>> GetPendingECDetail(int requestId);
        Task<ServiceResponse<string>> SubmitPayment(int requestId, SubmitPaymentRequestDto paymentRequest, List<IFormFile> newPaymentFiles);
        Task<ServiceResponse<RegistrationRequestPendingOAResponseDto>> GetPendingOADetail(int requestId);
        Task<ServiceResponse<string>> ApprovePayment(int requestId, PaymentStatus paymentStatus);
        Task<ServiceResponse<string>> DeclinePayment(int requestId);
        Task<ServiceResponse<string>> CancelRequest(int requestId);
        Task<ServiceResponse<string>> UpdatePayment(int requestId, UpdatePaymentRequestDto updatePayment);
    }
}