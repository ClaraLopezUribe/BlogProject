using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlogProject.Models
{
    public class Blog
    {

        //PRIMARY KEY for the Blog model:
        public int Id { get; set; }


        //FOREIGN KEYS (Primary Keys of other classes/model): 
        public string? BlogUserId { get; set; }


        //DESCRIPTION PROPERTIES:
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long", MinimumLength = 2)]
        public string Description { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime? Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }


        //IMAGE PROPERTIES:
        [Display(Name = "Blog Image")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Image Type")]
        public string? ContentType { get; set; }

        //Represents the physical file the user selects when creating new blog and selecting image; will not be saved in the data
        [NotMapped]
        public IFormFile? Image { get; set; }


        //NAVIGATION PROPERTIES:

        //Child of:

        [Display (Name = "Author")]
        public virtual BlogUser? BlogUser { get; set; }

            // Parent of:
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
