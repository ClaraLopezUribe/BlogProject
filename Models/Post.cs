using BlogProject.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Models
{
    public class Post
    {
        //PRIMARY KEY for Post model:
        public int Id { get; set; }


        //FOREIGN KEYS (Primary keys of other classes/models):

        [Display(Name = "Blog Name")]
        public int? BlogId { get; set; }
        public string? BlogUserId { get; set; }


        //DESCRIPTION PROPERTIES:
        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }


        //Enumerable to list available status options            
        public ReadyStatus ReadyStatus { get; set; }


        //Programatically created property derived from the title properties. Used for SEO
        public string? Slug { get; set; }


        //IMAGE PROPERTIES:
        public byte[]? ImageData { get; set; }
        public string? ContentType { get; set; }

        [NotMapped]
        public IFormFile? Image {  get; set; }


        //NAVIGATION PROPERTIES:
            
            // Child of:
        public virtual Blog? Blog { get; set; }
        public virtual BlogUser? BlogUser { get; set; }

            // Parent of:
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
