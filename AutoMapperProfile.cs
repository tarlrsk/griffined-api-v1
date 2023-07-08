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
            CreateMap<Student, StudentResponseDto>();
            CreateMap<AddStudentRequestDto, Student>()
                .ForMember(dest => dest.AdditionalFiles, opt => opt.MapFrom(src => src.AdditionalFiles));
            CreateMap<UpdateStudentRequestDto, Student>();

            // Parent
            CreateMap<Parent, ParentDtoResponse>();
            CreateMap<ParentRequestDto, Parent>();

            // Address
            CreateMap<Address, AddressResponseDto>();
            CreateMap<AddressRequestDto, Address>();

            // Teacher
            CreateMap<Teacher, GetTeacherDto>()
                .ForMember(dest => dest.WorkTimes, opt => opt.MapFrom(src => src.WorkTimes));
            CreateMap<AddTeacherDto, Teacher>();
            CreateMap<Teacher, AvailableTeacherResponseDto>();
            CreateMap<UpdateTeacherDto, Teacher>();

            // WorkTime
            CreateMap<WorkTime, WorkTimeResponseDto>();
            CreateMap<WorkTimeRequestDto, WorkTime>();

            // Staff
            CreateMap<Staff, StaffResponseDto>();
            CreateMap<AddStaffRequestDto, Staff>();
            CreateMap<UpdateStaffRequestDto, Staff>();

            // Additional Files
            CreateMap<StudentAdditionalFile, StudentAdditionalFilesResponseDto>();
            CreateMap<AddStudentAdditionalFilesRequestDto, StudentAdditionalFile>();
            CreateMap<UpdateStudentAdditionalFilesRequestDto, StudentAdditionalFile>();
        }
    }
}

