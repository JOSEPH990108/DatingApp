
namespace API.Entities
{
    public class UserLike
    {
        public ApplicationUser SourceUser { get; set; }

         public int SourceUserId { get; set; }

          public ApplicationUser TargetUser { get; set; }

           public int TargetUserId { get; set; }
    }
}