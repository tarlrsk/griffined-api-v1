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

            // Staff
            CreateMap<Staff, GetStaffDto>();
            CreateMap<AddStaffDto, Staff>();
            CreateMap<UpdateStaffDto, Staff>();

            // Additional Files
            CreateMap<StudentAdditionalFile, GetStudentAdditionalFilesDto>();
            CreateMap<AddStudentAdditionalFilesDto, StudentAdditionalFile>();
            CreateMap<UpdateStudentAdditionalFilesDto, StudentAdditionalFile>();
        }
    }
}

