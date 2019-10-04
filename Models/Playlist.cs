using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthMovie {

    public class Playlist {
        private static readonly char delimiter = ';';
        private string _MovieNames;

        public int Id { get; set; }
        
        [Required]
        public string Name{get;set;}

        [NotMapped]
        public string[] MovieNames
        {
            get { return _MovieNames.Split(delimiter); }
            set
            {
                _MovieNames = string.Join($"{delimiter}", value);
            }
        }
        
        public int UserId {get;set;}
    }
}