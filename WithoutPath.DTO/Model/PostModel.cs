using System.ComponentModel.DataAnnotations;

namespace WithoutPath.DTO
{
    public class PostModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите заголовок!")]
        public string Header { get; set; }
        [Required(ErrorMessage = "Введите текст!")]
        public string Content { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public int Width { get; set; }
        public bool IsVerified { get; set; }
        [Required]
        public bool IsFixed { get; set; }
        [Required]
        public bool IsInternal { get; set; }
        public string Picture { get; set; }
    }
}
