using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
            AvailableTags = new List<TagViewModel>();
            SelectedTags = new List<int>();
        }

        public int ArticleId { get; set; }

        public int ViewCount { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public List<TagViewModel> AvailableTags { get; set; }

        public List<int> SelectedTags { get; set; }
    }

    public class TagViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
