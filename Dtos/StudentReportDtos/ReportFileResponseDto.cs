namespace griffined_api.Dtos.StudentReportDtos
{
    public class ReportFileResponseDto
    {
        public int UploadedBy { get; set; }
        public Progression Progression { get; set; }
        public FilesResponseDto File { get; set; } = new FilesResponseDto();
    }
}