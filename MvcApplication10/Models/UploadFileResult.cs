namespace MvcApplication10.Models
{
    public class UploadFileResult
    {
        //带后缀的名称，比如xxx.jpg
        public string FileName { get; set; }

        //图片的字节数
        public int Length { get; set; }

        //图片的类型：image/jpeg
        public string Type { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }

        //图片的完整路径：~/AjaxUpload/20141112_large.jpg
        public string FilePath { get; set; }
    }
}