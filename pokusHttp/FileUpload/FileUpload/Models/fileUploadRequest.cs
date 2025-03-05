namespace FileUpload.Models
{
    public class fileUploadRequest
    {
        public string userName { get; set; }
        public string uploadFormat { get; set; }
        public string wantedFormat { get; set; }

        public IFormFile file { get; set; }
    }
}
