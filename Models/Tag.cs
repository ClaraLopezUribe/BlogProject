using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class Tag
    {
        //PRIMARY KEY for Tag model
        public int Id { get; set; }


        //FOREIGN KEYS (Primary keys of other classes/models):
        public int? PostId { get; set; }
        public string? BlogUserId { get; set; }


        //DESCRIPTION PROPERTIES:
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        public string Text { get; set; }


        //NAVIGATION PROPERTIES:

            // Child of:
        public virtual Post Post { get; set; }
        public virtual BlogUser BlogUser { get; set; }



       
    }
}
