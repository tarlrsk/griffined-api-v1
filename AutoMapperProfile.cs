using griffined_api.Dtos.StudentReportDtos;

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
            CreateMap<Parent, ParentResponseDto>();
            CreateMap<ParentRequestDto, Parent>();

            // Address
            CreateMap<Address, AddressResponseDto>();
            CreateMap<AddressRequestDto, Address>();

            // Teacher
            CreateMap<Teacher, GetTeacherDto>()
                .ForMember(dest => dest.Mandays, opt => opt.MapFrom(src => src.Mandays));
            CreateMap<AddTeacherDto, Teacher>();
            CreateMap<Teacher, AvailableTeacherResponseDto>();
            CreateMap<UpdateTeacherDto, Teacher>();

            // WorkTime
            CreateMap<Manday, MandayResponseDto>()
                .ForMember(dest => dest.WorkDays, opt => opt.MapFrom(src => src.WorkTimes));
            CreateMap<MandayRequestDto, Manday>();
            CreateMap<WorkTime, WorkTimeResponseDto>();
            CreateMap<WorkTimeRequestDto, WorkTime>();

            // Staff
            CreateMap<Staff, StaffResponseDto>();
            CreateMap<AddStaffRequestDto, Staff>();
            CreateMap<UpdateStaffRequestDto, Staff>();

            // Additional Files
            CreateMap<StudentAdditionalFile, FilesResponseDto>();
            CreateMap<AddStudentAdditionalFilesRequestDto, StudentAdditionalFile>();

            // Profile Picture
            CreateMap<ProfilePicture, FilesResponseDto>();
            CreateMap<AddProfilePictureRequestDto, ProfilePicture>();

            // Student Report
            CreateMap<AddStudentReportRequestDto, StudentReport>();
        }
    }
}

