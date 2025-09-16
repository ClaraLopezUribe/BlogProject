using BlogProject.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class Comment
    {
        //PRIMARY KEY for Comment model
        public int Id { get; set; }


        //FOREIGN KEYS (Primary keys of other classes/models):
        public int PostId { get; set; }
        public string? BlogUserId { get; set; }
        public string? ModeratorId { get; set; }


        //DESCRIPTION PROPERTIES:
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long",  MinimumLength = 2)]
        [Display(Name = "Comment")]
        public required string Body { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created {  get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Moderated Date")]
        public DateTime? Moderated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Deleted Date")]
        public DateTime? Deleted { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        [Display(Name = "Moderated Comment")]
        public string? ModeratedBody { get; set; }


        //Enumerable to list moderation reason types
        public ModerationType? ModerationType { get; set; }


        //NAVIGATION PROPERTIES:

        public virtual Post? Post { get; set; }
        public virtual BlogUser? BlogUser { get; set; }
        public virtual BlogUser? Moderator { get; set; }
    }
}
