using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Student
            CreateMap<Student, GetStudentDto>();
            CreateMap<AddStudentDto, Student>()
                .ForMember(dest => dest.additionalFiles, opt => opt.MapFrom(src => src.additionalFiles));
            CreateMap<UpdateStudentDto, Student>();

            // Parent
            CreateMap<Parent, GetParentDto>();
            CreateMap<AddParentDto, Parent>();
            CreateMap<UpdateParentDto, Parent>();

            // Address
            CreateMap<Address, GetAddressDto>();
            CreateMap<AddAddressDto, Address>();
            CreateMap<UpdateAddressDto, Address>();

            // Teacher
            CreateMap<Teacher, GetTeacherDto>()
                .ForMember(dest => dest.workTimes, opt => opt.MapFrom(src => src.workTimes));
            CreateMap<AddTeacherDto, Teacher>();
            CreateMap<Teacher, GetAvailableTeacherDto>();
            CreateMap<UpdateTeacherDto, Teacher>();

            // WorkTime
            CreateMap<WorkTime, GetWorkTimeDto>();
            CreateMap<AddWorkTimeDto, WorkTime>();
            CreateMap<UpdateWorkTimeDto, WorkTime>();

            // EP
            CreateMap<EP, GetEPDto>();
            CreateMap<AddEPDto, EP>();
            CreateMap<UpdateEPDto, EP>();

            // EA
            CreateMap<EA, GetEADto>();
            CreateMap<AddEADto, EA>();
            CreateMap<UpdateEADto, EA>();

            // Staff
            CreateMap<Staff, GetStaffDto>();
            CreateMap<AddStaffDto, Staff>();
            CreateMap<UpdateStaffDto, Staff>();

            // OA
            CreateMap<OA, GetOADto>();
            CreateMap<AddOADto, OA>();
            CreateMap<UpdateOADto, OA>();

            // Additional Files
            CreateMap<StudentAdditionalFiles, GetStudentAdditionalFilesDto>();
            CreateMap<AddStudentAdditionalFilesDto, StudentAdditionalFiles>();
            CreateMap<UpdateStudentAdditionalFilesDto, StudentAdditionalFiles>();

            // Payment
            CreateMap<Payment, GetPrivatePaymentDto>();
            CreateMap<AddPrivatePaymentDto, Payment>()
                .ForMember(dest => dest.payment, opt => opt.MapFrom(src => src.payment));
            CreateMap<UpdatePrivatePaymentDto, Payment>()
                .ForMember(dest => dest.payment, opt => opt.MapFrom(src => src.payment));
            CreateMap<PaymentFile, GetPaymentFileDto>();
            CreateMap<AddPaymentFileDto, PaymentFile>();
            CreateMap<UpdatePaymentFileDto, PaymentFile>();

            // Private Registration Request
            CreateMap<AddPrivateRegistrationRequestDto, PrivateRegistrationRequest>();
            CreateMap<AddPrivateRegReqInformationDto, PrivateRegistrationRequestInfo>();
            CreateMap<AddPreferredDayDto, PreferredDay>();
            CreateMap<PrivateRegistrationRequest, GetPrivateRegReqWithInfoDto>()
                .ForMember(dest => dest.request, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.information, opt => opt.MapFrom(src => src.privateRegistrationRequestInfos))
                .ForMember(dest => dest.students, opt => opt.MapFrom(src =>
                    src.students.Select(s => new GetStudentNameDto
                    {
                        id = s.id,
                        fullName = s.fullName,
                        nickname = s.nickname
                    })
                ));
            CreateMap<PrivateRegistrationRequest, GetPrivateRegistrationRequestDto>();
            CreateMap<PrivateRegistrationRequestInfo, GetPrivateRegReqInformationDto>();
            CreateMap<PreferredDay, GetPreferredDayDto>();
            CreateMap<UpdatePrivateRegistrationRequestDto, PrivateRegistrationRequest>();

            // Schedules
            CreateMap<AddPrivateCourseDto, PrivateCourse>();
            CreateMap<AddPrivateClassDto, PrivateClass>();
            CreateMap<AddStudentPrivateClassDto, StudentPrivateClass>();
            CreateMap<AddTeacherPrivateClassDto, TeacherPrivateClass>();
            CreateMap<PrivateCourse, GetPrivateCourseDto>();
            CreateMap<PrivateClass, GetPrivateClassDto>()
                .ForMember(dest => dest.studentPrivateClasses, opt => opt.MapFrom(src => src.studentPrivateClasses))
                .ForMember(dest => dest.teacherPrivateClass, opt => opt.MapFrom(src => src.teacherPrivateClass));
            CreateMap<PrivateClass, GetPrivateCourseDto>();
            CreateMap<PrivateClass, GetAvailableTimeDto>();
            CreateMap<StudentPrivateClass, GetStudentPrivateClassDto>();
            CreateMap<TeacherPrivateClass, GetTeacherPrivateClassDto>();
            CreateMap<UpdatePrivateClassDto, PrivateClass>();
            CreateMap<UpdateStudentPrivateClassDto, StudentPrivateClass>();
            CreateMap<UpdateTeacherPrivateClassDto, TeacherPrivateClass>();

            CreateMap<GetScheduleDto, PrivateCourse>();

            CreateMap<TeacherLeavingRequest, GetTeacherLeavingRequestDto>();
            CreateMap<AddTeacherLeavingRequestDto, TeacherLeavingRequest>();
            CreateMap<UpdateTeacherLeavingRequestDto, TeacherLeavingRequest>();
        }
    }
}

