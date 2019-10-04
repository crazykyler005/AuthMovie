using System.ComponentModel.DataAnnotations;

namespace AuthMovie{

    public class User {

        public int Id {get; set;}

        [Required]
        public string UserName {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        public int PlaylistId {get;set;}
    }
}