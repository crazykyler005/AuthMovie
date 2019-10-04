using System.ComponentModel.DataAnnotations;

namespace AuthMovie {

    public class Movie {

        public int Id { get; set; }

        [Required]
        public string Name { get; set;}

        public string Director { get; set;}

        public int YearReleased {get;set;}
    }
}